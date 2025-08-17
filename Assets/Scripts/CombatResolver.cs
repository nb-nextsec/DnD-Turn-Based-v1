using UnityEngine;
using System.Text;

public static class CombatResolver
{
    static System.Random rng = new System.Random();

    // ---------- Core dice ----------
    public static int RollD(int sides) => rng.Next(1, sides + 1);

    public static int RollDice(Vector2Int dice)
    {
        int total = 0;
        for (int i = 0; i < dice.x; i++) total += RollD(dice.y);
        return total;
    }

    public static int RollInitiative(int bonus) => RollD(20) + bonus;

    static int Mod(int score) => Mathf.FloorToInt((score - 10) / 2f);

    // (atkBonus, abilityMod, profBonus, abilityName)
    static (int atkBonus, int abilityMod, int profBonus, string abilityName) AttackBonusParts(Unit u)
    {
        var s = u.baseStats;

        int abilityScore;
        string abilityName;

        if (s.isRanged)
        {
            abilityScore = s.dex; abilityName = "DEX";
        }
        else if (s.finesse)
        {
            int strMod = Mod(s.str);
            int dexMod = Mod(s.dex);
            if (dexMod >= strMod) { abilityScore = s.dex; abilityName = "DEX"; }
            else { abilityScore = s.str; abilityName = "STR"; }
        }
        else
        {
            abilityScore = s.str; abilityName = "STR";
        }

        int abilityMod = Mod(abilityScore);
        int profBonus = s.proficiencyBonus;
        int atkBonus = abilityMod + profBonus;

        return (atkBonus, abilityMod, profBonus, abilityName);
    }

    // ---------- Weapon attack ----------
    public static bool TryAttack(Unit attacker, Unit defender, out bool crit, out int damage)
    {
        crit = false;
        damage = 0;

        int d20 = RollD(20);

        var (atkBonus, abilityMod, profBonus, abilityName) = AttackBonusParts(attacker);
        int totalAtk = d20 + atkBonus;

        if (d20 == 20) crit = true; // nat 20
        bool hit = crit || totalAtk >= defender.baseStats.armourClass;

        // Weapon/dmg type
        string weap = !string.IsNullOrEmpty(attacker.baseStats.weaponName)
                        ? attacker.baseStats.weaponName
                        : (!string.IsNullOrEmpty(attacker.baseStats.weaponId) ? attacker.baseStats.weaponId : "unarmed");

        string dmgType = "damage";
        if (!string.IsNullOrEmpty(attacker.baseStats.weaponId) &&
            WeaponDB.All.TryGetValue(attacker.baseStats.weaponId, out var wDef))
        {
            dmgType = wDef.damageType.ToString().ToLower(); // slashing/piercing/bludgeoning
        }

        string breakdown =
            $"{attacker.name} attacks {defender.name} with {weap}: " +
            $"d20={d20} + ({abilityName} mod {abilityMod:+#;-#;0} + prof {profBonus}) = total {totalAtk} " +
            $"vs AC {defender.baseStats.armourClass} => {(hit ? "HIT" : "MISS")}{(crit ? " (CRIT!)" : "")}";

        Debug.Log(breakdown);
        CombatLog.Print(breakdown);

        if (!hit) return false;

        // Damage (crit doubles dice only)
        var s = attacker.baseStats;
        int dieCount = s.damage.x;
        int dieSides = s.damage.y;
        int modBonus = s.damageBonus;

        int sum = 0;
        var rolls = new StringBuilder();
        for (int i = 0; i < dieCount; i++)
        {
            int r = RollD(dieSides);
            sum += r;
            if (i > 0) rolls.Append(", ");
            rolls.Append(r);
        }

        int critSum = 0;
        if (crit)
        {
            var critRolls = new StringBuilder();
            for (int i = 0; i < dieCount; i++)
            {
                int r = RollD(dieSides);
                critSum += r;
                if (i > 0) critRolls.Append(", ");
                critRolls.Append(r);
            }
            CombatLog.Print($"Critical! Extra {dieCount}d{dieSides} dice: [{critRolls}] = {critSum}");
            Debug.Log($"Critical! Extra {dieCount}d{dieSides} dice: [{critRolls}] = {critSum}");
        }

        int diceTotal = sum + critSum;
        damage = Mathf.Max(1, diceTotal + modBonus);

        string notation = $"{dieCount}d{dieSides}" + (modBonus != 0 ? $"{(modBonus > 0 ? "+" : "")}{modBonus}" : "");

        string dmgLine =
            $"{attacker.name} hits {defender.name} with {weap} for {damage} {dmgType} dmg " +
            $"({notation}; rolls [{rolls}]{(crit ? $" + crit [{critSum}]" : "")}).";

        CombatLog.Print(dmgLine);
        Debug.Log(dmgLine);

        return true;
    }

    // ---------- Spells ----------
    // Utility: roll XdY and return (sum, "a, b, c")
    static (int sum, string rollsCsv) RollXdY(int count, int sides)
    {
        int sum = 0;
        var sb = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            int r = RollD(sides);
            sum += r;
            if (i > 0) sb.Append(", ");
            sb.Append(r);
        }
        return (sum, sb.ToString());
    }

    /// Spells that use a spell attack roll (e.g. Fire Bolt, Guiding Bolt)
    public static bool TrySpellAttack(Unit caster, Unit target, SpellDef spell, out bool crit, out int damage)
    {
        crit = false; damage = 0;

        int d20 = RollD(20);
        int atkBonus = caster.baseStats.SpellAttackBonus;
        int total = d20 + atkBonus;

        if (d20 == 20) crit = true;
        bool hit = crit || total >= target.baseStats.armourClass;

        CombatLog.Print($"{caster.name} casts {spell.name} at {target.name}: " +
                        $"d20={d20} + spell atk {atkBonus} = {total} vs AC {target.baseStats.armourClass} => {(hit ? "HIT" : "MISS")}{(crit ? " (CRIT!)" : "")}");
        Debug.Log($"{caster.name} casts {spell.name} -> {(hit ? "HIT" : "MISS")}");

        if (!hit) return false;

        var (sum, rolls) = RollXdY(spell.diceCount, spell.diceSides);
        int extra = 0;
        if (crit)
        {
            var (cSum, cRolls) = RollXdY(spell.diceCount, spell.diceSides);
            extra = cSum;
            CombatLog.Print($"Critical! Extra {spell.diceCount}d{spell.diceSides}: [{cRolls}] = {cSum}");
        }

        int abilityBonus = 0;
        if (spell.addAbilityToDmg && caster.baseStats.isCaster)
            abilityBonus = caster.baseStats.SpellAttackBonus - caster.baseStats.proficiencyBonus; // just the casting mod

        damage = Mathf.Max(1, sum + extra + abilityBonus + spell.flatBonus * Mathf.Max(1, spell.darts));

        string notation = $"{spell.diceCount}d{spell.diceSides}"
                          + (abilityBonus != 0 ? $"{(abilityBonus > 0 ? "+" : "")}{abilityBonus}" : "")
                          + (spell.flatBonus != 0 ? $"+{spell.flatBonus}" : "")
                          + (spell.darts > 1 ? $" ×{spell.darts}" : "");

        CombatLog.Print($"{caster.name} hits {target.name} with {spell.name} for {damage} {spell.damageType} dmg " +
                        $"({notation}; rolls [{rolls}]{(crit ? $" + crit [{extra}]" : "")}).");
        return true;
    }

    /// Auto-hit multi-dart spell like Magic Missile (3 × 1d4+1)
    public static int ResolveMagicMissile(Unit caster, Unit target, SpellDef spell)
    {
        int total = 0;
        var sb = new StringBuilder();
        int darts = Mathf.Max(1, spell.darts);
        for (int i = 0; i < darts; i++)
        {
            int r = RollD(spell.diceSides) + spell.flatBonus;
            total += r;
            if (i > 0) sb.Append(", ");
            sb.Append(r);
        }
        string line = $"{caster.name} casts {spell.name} on {target.name}: {darts} × (1d{spell.diceSides}+{spell.flatBonus}) -> [{sb}] = {total} {spell.damageType} dmg.";
        CombatLog.Print(line);
        Debug.Log(line);
        return total;
    }

    /// Save-based spell (e.g., Sacred Flame: DEX save, no half)
    public static int ResolveSaveSpell(Unit caster, Unit target, SpellDef spell, out bool saved)
    {
        saved = false;
        int dc = caster.baseStats.SpellSaveDC;

        int saveMod = spell.save switch
        {
            SaveType.STR => Stats.Mod(target.baseStats.str),
            SaveType.DEX => Stats.Mod(target.baseStats.dex),
            SaveType.CON => Stats.Mod(target.baseStats.con),
            SaveType.INT => Stats.Mod(target.baseStats.@int),
            SaveType.WIS => Stats.Mod(target.baseStats.wis),
            SaveType.CHA => Stats.Mod(target.baseStats.cha),
            _ => 0
        };

        int d20 = RollD(20);
        int total = d20 + saveMod;
        saved = total >= dc;

        CombatLog.Print($"{target.name} saves vs {spell.name}: d20={d20} + mod {saveMod:+#;-#;0} = {total} vs DC {dc} => {(saved ? "SUCCESS" : "FAIL")}");
        Debug.Log($"{target.name} {(saved ? "succeeds" : "fails")} {spell.name} save");

        if (saved && !spell.halfOnSave) return 0;

        var (sum, rolls) = RollXdY(spell.diceCount, spell.diceSides);
        if (saved && spell.halfOnSave) sum = Mathf.Max(1, sum / 2);

        CombatLog.Print($"{spell.name} deals {sum} {spell.damageType} dmg to {target.name} (rolls [{rolls}]{(saved ? ", half on save" : "")}).");
        return sum;
    }

    // ---------- Healing ----------
    public static int RollHeal(Unit caster)
    {
        int rolled = 0; string rolls = "";
        for (int i = 0; i < caster.baseStats.healAmount.x; i++)
        {
            int r = RollD(caster.baseStats.healAmount.y);
            rolled += r;
            rolls += r + (i < caster.baseStats.healAmount.x - 1 ? ", " : "");
        }
        string breakdown = $"{caster.name} rolls heal ({caster.baseStats.healAmount.x}d{caster.baseStats.healAmount.y}): [{rolls}] => {rolled}";
        Debug.Log(breakdown);
        CombatLog.Print(breakdown);
        return Mathf.Max(1, rolled);
    }

    // ---------- Dist ----------
    public static float Distance(Unit a, Unit b) => Vector3.Distance(a.transform.position, b.transform.position);


    static void GetScaledParams(Unit caster, SpellDef def,
                            out int diceCount, out int diceSides, out int flat, out int darts)
    {
        diceCount = def.diceCount;
        diceSides = def.diceSides;
        flat = def.flatBonus;
        darts = def.darts;

        // Cantrip scaling
        if (def.level == 0 && def.cantripScalesDice)
        {
            int tier = SpellRulesExtensions.CantripTier(caster.baseStats.level);
            diceCount *= Mathf.Max(1, tier);
        }

        // Slot scaling (leveled spells)
        if (def.level >= 1 && def.scalesWithSlot)
        {
            int slot = SpellRulesExtensions.EstimatedSlotLevel(caster.baseStats.classId, caster.baseStats.level);
            int above = Mathf.Max(0, slot - 1);

            if (def.extraDartsPerSlot != 0) darts += def.extraDartsPerSlot * above;
            if (def.extraDicePerSlot != 0) diceCount += def.extraDicePerSlot * above;
            if (def.extraFlatPerSlot != 0) flat += def.extraFlatPerSlot * above;
        }
    }

}
