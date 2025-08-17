using System.Linq;
using UnityEngine;

public static class SimpleAI
{
    // Decide action each time the AI has an action to spend.
    public static AIActionType TakeAction(Unit self, Unit[] allies, Unit[] enemies)
    {
        if (!self.isAlive || self.remainingActions <= 0) return AIActionType.None;

        var viable = enemies.Where(u => u.isAlive).ToList();
        if (viable.Count == 0) { self.ConsumeAction(); return AIActionType.Wait; }

        var target = viable.OrderBy(u => CombatResolver.Distance(self, u)).First();

        // --- Spellcasting (per-class known list) ---
        if (self.baseStats.isCaster && self.baseStats.knownSpells != null && self.baseStats.knownSpells.Count > 0)
        {
            // Emergency heal if ally is low and a heal is known
            var lowAlly = allies.Where(a => a.isAlive).OrderBy(a => a.HealthPct()).FirstOrDefault();
            if (lowAlly != null && lowAlly.HealthPct() <= 0.45f)
            {
                var healSpell = self.baseStats.knownSpells
                    .Where(s => SpellDB.All.TryGetValue(s.id, out var sd) && sd.isHeal)
                    .Select(s => SpellDB.All[s.id])
                    .OrderBy(sd => sd.diceCount * sd.diceSides)
                    .FirstOrDefault();

                if (healSpell != null && CombatResolver.Distance(self, lowAlly) <= healSpell.range.metres)
                {
                    int heal = CombatResolver.RollDice(new Vector2Int(healSpell.diceCount, healSpell.diceSides));
                    if (healSpell.addAbilityToDmg)
                    {
                        int modOnly = self.baseStats.SpellAttackBonus - self.baseStats.proficiencyBonus;
                        heal += modOnly;
                    }
                    lowAlly.ReceiveHeal(heal);
                    CombatLog.Print($"{self.name} casts {healSpell.name} on {lowAlly.name} for {heal} healing.");
                    self.ConsumeAction();
                    return AIActionType.Heal;
                }
            }

            // Auto-hit first (e.g. Magic Missile)
            var autoHit = self.baseStats.knownSpells
                .Where(s => SpellDB.All.TryGetValue(s.id, out var sd) && sd.autoHit && !sd.isHeal)
                .Select(s => SpellDB.All[s.id])
                .OrderBy(s => s.level)
                .FirstOrDefault();

            if (autoHit != null && CombatResolver.Distance(self, target) <= autoHit.range.metres)
            {
                int dmg = CombatResolver.ResolveMagicMissile(self, target, autoHit);
                target.ReceiveDamage(dmg);
                self.ConsumeAction();
                return AIActionType.Cast;
            }

            // Spell attack next (e.g. Fire Bolt / Guiding Bolt)
            var spellAtk = self.baseStats.knownSpells
                .Where(s => SpellDB.All.TryGetValue(s.id, out var sd) && sd.usesAttackRoll && !sd.isHeal)
                .Select(s => SpellDB.All[s.id])
                .OrderBy(s => s.level)
                .FirstOrDefault();

            if (spellAtk != null && CombatResolver.Distance(self, target) <= spellAtk.range.metres)
            {
                bool crit; int dmg;
                if (CombatResolver.TrySpellAttack(self, target, spellAtk, out crit, out dmg))
                    target.ReceiveDamage(dmg);
                self.ConsumeAction();
                return AIActionType.Cast;
            }

            // Save-based last (e.g. Sacred Flame / Shatter)
            var saveSpell = self.baseStats.knownSpells
                .Where(s => SpellDB.All.TryGetValue(s.id, out var sd) && sd.save != SaveType.None && !sd.isHeal)
                .Select(s => SpellDB.All[s.id])
                .OrderBy(s => s.level)
                .FirstOrDefault();

            if (saveSpell != null && CombatResolver.Distance(self, target) <= saveSpell.range.metres)
            {
                bool saved; int dmg = CombatResolver.ResolveSaveSpell(self, target, saveSpell, out saved);
                if (dmg > 0) target.ReceiveDamage(dmg);
                self.ConsumeAction();
                return AIActionType.Cast;
            }
        }

        // Non-spell healing fallback
        if (self.baseStats.canHeal)
        {
            var healTarget = allies.Where(u => u.isAlive).OrderBy(u => u.HealthPct()).FirstOrDefault();
            if (healTarget != null && healTarget.HealthPct() <= self.baseStats.healThreshold)
            {
                int heal = CombatResolver.RollHeal(self);
                healTarget.ReceiveHeal(heal);
                CombatLog.Print($"{self.name} heals {healTarget.name} for {heal}");
                self.ConsumeAction();
                return AIActionType.Heal;
            }
        }

        // Weapon attack if in range
        var inRange = viable
            .Where(e => CombatResolver.Distance(self, e) <= self.baseStats.attackRange)
            .OrderBy(e => (float)e.currentHP / Mathf.Max(1, e.baseStats.maxHP))
            .ThenByDescending(e => e.baseStats.damage.x * e.baseStats.damage.y + e.baseStats.damageBonus)
            .ToList();

        if (inRange.Count > 0)
        {
            var t = inRange.First();
            bool crit; int dmg;
            if (CombatResolver.TryAttack(self, t, out crit, out dmg))
                t.ReceiveDamage(dmg);
            self.ConsumeAction();
            return AIActionType.Attack;
        }

        // Move towards closest enemy
        {
            float stopAt = Mathf.Max(0.1f, self.baseStats.desiredRange);
            Vector3 dir = (target.transform.position - self.transform.position).normalized;
            Vector3 desiredPos = target.transform.position - dir * stopAt;

            self.MoveTowards(desiredPos, self.baseStats.movePerAction);
            self.ConsumeAction();
            return AIActionType.Move;
        }
    }
}
