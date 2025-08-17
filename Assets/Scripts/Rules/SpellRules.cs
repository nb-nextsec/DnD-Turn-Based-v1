using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CasterProgression { None, Third, Half, Full }

public static class SpellRules
{
    // Map classes to caster progression
    public static CasterProgression ProgressionFor(ClassId cls) => cls switch
    {
        ClassId.Barbarian => CasterProgression.None,
        ClassId.Fighter => CasterProgression.Third,   // Eldritch Knight
        ClassId.Rogue => CasterProgression.Third,   // Arcane Trickster
        ClassId.Paladin => CasterProgression.Half,
        ClassId.Ranger => CasterProgression.Half,
        ClassId.Bard => CasterProgression.Full,
        ClassId.Cleric => CasterProgression.Full,
        ClassId.Druid => CasterProgression.Full,
        ClassId.Sorcerer => CasterProgression.Full,
        ClassId.Warlock => CasterProgression.Full,    // (pact slots ignored here)
        ClassId.Wizard => CasterProgression.Full,
        ClassId.Artificer => CasterProgression.Half,
        ClassId.Monk => CasterProgression.None,
        _ => CasterProgression.None
    };

    // Very small, playable caps (not exhaustive PHB): level → (cantrips, L1, L2, L3)
    static readonly Dictionary<CasterProgression, (int can, int l1, int l2, int l3)[]> Caps =
        new Dictionary<CasterProgression, (int, int, int, int)[]>
        {
            // index by character level (1..20) – we only populate 1..10 here
            [CasterProgression.Full] = new (int, int, int, int)[]
        {
            (0,0,0,0), // 0 (unused)
            (3, 2, 0, 0), // 1
            (3, 3, 0, 0), // 2
            (3, 4, 2, 0), // 3
            (4, 4, 3, 0), // 4
            (4, 4, 3, 2), // 5
            (4, 4, 3, 3), // 6
            (4, 4, 3, 3), // 7
            (4, 4, 3, 3), // 8
            (4, 4, 3, 3), // 9
            (5, 4, 3, 3), // 10
        },
            [CasterProgression.Half] = new (int, int, int, int)[]
        {
            (0,0,0,0),
            (0, 0, 0, 0), // 1
            (0, 2, 0, 0), // 2
            (0, 3, 0, 0), // 3
            (0, 3, 2, 0), // 4
            (0, 4, 2, 0), // 5
            (0, 4, 3, 0), // 6
            (0, 4, 3, 0), // 7
            (0, 4, 3, 2), // 8
            (0, 4, 3, 2), // 9
            (0, 4, 3, 2), // 10
        },
            [CasterProgression.Third] = new (int, int, int, int)[]
        {
            (0,0,0,0),
            (0, 0, 0, 0), // 1
            (0, 0, 0, 0), // 2
            (0, 2, 0, 0), // 3
            (0, 2, 0, 0), // 4
            (0, 3, 0, 0), // 5
            (0, 3, 2, 0), // 6
            (0, 3, 2, 0), // 7
            (0, 4, 2, 0), // 8
            (0, 4, 2, 0), // 9
            (0, 4, 3, 0), // 10
        },
            [CasterProgression.None] = new (int, int, int, int)[]
        {
            (0,0,0,0),
            (0, 0, 0, 0), // 1..10 all zero
            (0, 0, 0, 0),
            (0, 0, 0, 0),
            (0, 0, 0, 0),
            (0, 0, 0, 0),
            (0, 0, 0, 0),
            (0, 0, 0, 0),
            (0, 0, 0, 0),
            (0, 0, 0, 0),
            (0, 0, 0, 0),
        },
        };

    public static (int can, int l1, int l2, int l3) AllowedAtLevel(ClassId cls, int level)
    {
        level = Mathf.Clamp(level, 1, 10);
        var prog = ProgressionFor(cls);
        return Caps[prog][level];
    }

    // Filter a class’ base list down to level-based caps
    public static List<KnownSpell> TrimToCaps(ClassId cls, int level, IEnumerable<KnownSpell> baseList)
    {
        var caps = AllowedAtLevel(cls, level);
        var can = new List<KnownSpell>();
        var l1 = new List<KnownSpell>();
        var l2 = new List<KnownSpell>();
        var l3 = new List<KnownSpell>();

        foreach (var ks in baseList)
        {
            if (!SpellDB.All.TryGetValue(ks.id, out var def)) continue;
            switch (def.level)
            {
                case 0: if (can.Count < caps.can) can.Add(new KnownSpell(ks.id, 0)); break;
                case 1: if (l1.Count < caps.l1) l1.Add(new KnownSpell(ks.id, 1)); break;
                case 2: if (l2.Count < caps.l2) l2.Add(new KnownSpell(ks.id, 2)); break;
                case 3: if (l3.Count < caps.l3) l3.Add(new KnownSpell(ks.id, 3)); break;
            }
        }

        var result = new List<KnownSpell>();
        result.AddRange(can); result.AddRange(l1); result.AddRange(l2); result.AddRange(l3);
        return result;
    }

    // 5e proficiency bonus by character level
    public static int ProficiencyForLevel(int level) =>
        Mathf.Clamp(2 + (Mathf.Max(level, 1) - 1) / 4, 2, 6);
}

public static class SpellRulesExtensions
{
    // 5e cantrip tiers: levels 1–4 -> x1, 5–10 -> x2, 11–16 -> x3, 17+ -> x4
    public static int CantripTier(int characterLevel)
    {
        if (characterLevel >= 17) return 4;
        if (characterLevel >= 11) return 3;
        if (characterLevel >= 5) return 2;
        return 1;
    }

    // Rough “effective slot” for full-casters by level; half/third casters trail.
    // This is a simple mapping to give your AI stronger casts at higher levels.
    public static int EstimatedSlotLevel(ClassId cls, int characterLevel)
    {
        var prog = SpellRules.ProgressionFor(cls);
        int baseSlot =
            characterLevel switch
            {
                >= 17 => 9,
                >= 15 => 8,
                >= 13 => 7,
                >= 11 => 6,
                >= 9 => 5,
                >= 7 => 4,
                >= 5 => 3,
                >= 3 => 2,
                _ => 1
            };

        // Half casters lag ~2 levels of slot power; third casters lag ~3
        return prog switch
        {
            CasterProgression.Full => baseSlot,
            CasterProgression.Half => Mathf.Clamp(baseSlot - 2, 1, 9),
            CasterProgression.Third => Mathf.Clamp(baseSlot - 3, 1, 9),
            _ => 1
        };
    }
}
