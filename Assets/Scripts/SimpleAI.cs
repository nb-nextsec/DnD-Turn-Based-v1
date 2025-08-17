using System.Linq;
using UnityEngine;

public static class SimpleAI
{
    // Decide action each time the AI has an action to spend.
    public static void TakeAction(Unit self, Unit[] allies, Unit[] enemies)
    {
        if (!self.isAlive || self.remainingActions <= 0) return;

        // 1) Caster logic (very simple policy)
        if (self.baseStats.isCaster)
        {
            // pick nearest living enemy
            var target = enemies.Where(u => u.isAlive)
                                .OrderBy(u => CombatResolver.Distance(self, u))
                                .FirstOrDefault();

            if (target != null)
            {
                // prefer Magic Missile if available and in range (auto-hit)
                if (SpellDB.All.TryGetValue("magic-missile", out var mm) &&
                    CombatResolver.Distance(self, target) <= mm.range.metres)
                {
                    int dmg = CombatResolver.ResolveMagicMissile(self, target, mm);
                    target.ReceiveDamage(dmg);
                    self.ConsumeAction();
                    return;
                }

                // else try Fire Bolt (spell attack) if in range
                if (SpellDB.All.TryGetValue("fire-bolt", out var fb) &&
                    CombatResolver.Distance(self, target) <= fb.range.metres)
                {
                    bool crit; int dmg;
                    if (CombatResolver.TrySpellAttack(self, target, fb, out crit, out dmg))
                        target.ReceiveDamage(dmg);
                    self.ConsumeAction();
                    return;
                }

                // Cleric-esque: Sacred Flame (DEX save) if in range
                if (SpellDB.All.TryGetValue("sacred-flame", out var sf) &&
                    CombatResolver.Distance(self, target) <= sf.range.metres)
                {
                    bool saved;
                    int dmg = CombatResolver.ResolveSaveSpell(self, target, sf, out saved);
                    if (dmg > 0) target.ReceiveDamage(dmg);
                    self.ConsumeAction();
                    return;
                }
            }
        }

        // 2) Healing logic (non-spell or cleric heal)
        if (self.baseStats.canHeal)
        {
            var healTarget = allies
                .Where(u => u.isAlive)
                .OrderBy(u => u.HealthPct())
                .FirstOrDefault();

            if (healTarget != null && healTarget.HealthPct() <= self.baseStats.healThreshold)
            {
                int heal = CombatResolver.RollHeal(self);
                healTarget.ReceiveHeal(heal);
                Debug.Log($"{self.name} heals {healTarget.name} for {heal}");
                CombatLog.Print($"{self.name} heals {healTarget.name} for {heal}");
                self.ConsumeAction();
                return;
            }
        }

        // 3) Martial attack if in range
        var viable = enemies.Where(u => u.isAlive).ToList();
        if (viable.Count == 0) { self.ConsumeAction(); return; }

        var inRange = viable
            .Where(e => CombatResolver.Distance(self, e) <= self.baseStats.attackRange)
            .OrderBy(e => (float)e.currentHP / Mathf.Max(1, e.baseStats.maxHP))
            .ThenByDescending(e => e.baseStats.damage.x * e.baseStats.damage.y + e.baseStats.damageBonus)
            .ToList();

        if (inRange.Count > 0)
        {
            var target = inRange.First();
            bool crit; int dmg;
            if (CombatResolver.TryAttack(self, target, out crit, out dmg))
            {
                target.ReceiveDamage(dmg);
            }
            self.ConsumeAction();
            return;
        }

        // 4) Otherwise, move towards closest enemy to desired range
        var closest = viable
            .OrderBy(e => CombatResolver.Distance(self, e))
            .FirstOrDefault();

        if (closest != null)
        {
            float stopAt = Mathf.Max(0.1f, self.baseStats.desiredRange);
            Vector3 dir = (closest.transform.position - self.transform.position).normalized;
            Vector3 desiredPos = closest.transform.position - dir * stopAt;

            self.MoveTowards(desiredPos, self.baseStats.movePerAction);
            self.ConsumeAction();
            return;
        }

        Debug.Log($"{self.name} waits (no targets in range).");
        CombatLog.Print($"{self.name} waits (no targets in range).");
        self.ConsumeAction();
    }
}
