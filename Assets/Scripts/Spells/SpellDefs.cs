using UnityEngine;
using System;
using System.Collections.Generic;

public enum SaveType { None, STR, DEX, CON, INT, WIS, CHA }

[Serializable]
public struct SpellRange
{
    public float metres;       // simple metres range
    public bool requiresLoS;   // reserved for LoS later
}

[Serializable]
public class SpellDef
{
    public string id;          // unique id, e.g., "fire-bolt"
    public string name;        // display name, e.g., "Fire Bolt"

    // New: spell level (0 = cantrip, 1–9 for leveled spells)
    public int level = 0;

    // Base damage/heal: XdY (+ flatBonus per dart if multi-dart)
    public int diceCount = 1;
    public int diceSides = 6;
    public int flatBonus = 0;
    public int darts = 1;            // Magic Missile = 3

    // Behaviour
    public bool isHeal = false;      // positive effect (healing)
    public bool autoHit = false;     // e.g., Magic Missile
    public bool usesAttackRoll = false;  // e.g., Fire Bolt

    // Save-based
    public SaveType save = SaveType.None;   // e.g., DEX save
    public bool halfOnSave = false;         // e.g., Fireball half on save

    // Typing (kept as string, flexible vs weapons' enum)
    public string damageType = "force";     // fire, radiant, necrotic, healing, thunder, etc.

    // Add casting mod to damage/heal (e.g., Cure Wounds heal)
    public bool addAbilityToDmg = false;

    public SpellRange range = new SpellRange { metres = 18f, requiresLoS = true };

    public override string ToString() => name;
}

public static class SpellDB
{
    // Canon: one definition per spell id.
    public static readonly Dictionary<string, SpellDef> All =
        new Dictionary<string, SpellDef>(StringComparer.OrdinalIgnoreCase)
        {
            // --- Cantrips (0) ---
            ["fire-bolt"] = new SpellDef { id = "fire-bolt", name = "Fire Bolt", diceCount = 1, diceSides = 10, usesAttackRoll = true, damageType = "fire", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["ray-of-frost"] = new SpellDef { id = "ray-of-frost", name = "Ray of Frost", diceCount = 1, diceSides = 8, usesAttackRoll = true, damageType = "cold", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["acid-splash"] = new SpellDef { id = "acid-splash", name = "Acid Splash", diceCount = 1, diceSides = 6, save = SaveType.DEX, damageType = "acid", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["shocking-grasp"] = new SpellDef { id = "shocking-grasp", name = "Shocking Grasp", diceCount = 1, diceSides = 8, usesAttackRoll = true, damageType = "lightning", range = new SpellRange { metres = 2f, requiresLoS = true } },
            ["eldritch-blast"] = new SpellDef { id = "eldritch-blast", name = "Eldritch Blast", diceCount = 1, diceSides = 10, usesAttackRoll = true, damageType = "force", range = new SpellRange { metres = 36f, requiresLoS = true } },
            ["vicious-mockery"] = new SpellDef { id = "vicious-mockery", name = "Vicious Mockery", diceCount = 1, diceSides = 4, save = SaveType.WIS, damageType = "psychic", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["sacred-flame"] = new SpellDef { id = "sacred-flame", name = "Sacred Flame", diceCount = 1, diceSides = 8, save = SaveType.DEX, damageType = "radiant", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["thaumaturgy"] = new SpellDef { id = "thaumaturgy", name = "Thaumaturgy" }, // flavour
            ["guidance"] = new SpellDef { id = "guidance", name = "Guidance" },       // flavour/buff placeholder

            // --- Level 1 ---
            ["magic-missile"] = new SpellDef { id = "magic-missile", name = "Magic Missile", diceCount = 1, diceSides = 4, flatBonus = 1, darts = 3, autoHit = true, damageType = "force", range = new SpellRange { metres = 36f, requiresLoS = true } },
            ["shield"] = new SpellDef { id = "shield", name = "Shield" }, // reaction buff placeholder
            ["burning-hands"] = new SpellDef { id = "burning-hands", name = "Burning Hands", diceCount = 3, diceSides = 6, save = SaveType.DEX, halfOnSave = true, damageType = "fire", range = new SpellRange { metres = 3f, requiresLoS = true } },
            ["chromatic-orb"] = new SpellDef { id = "chromatic-orb", name = "Chromatic Orb", diceCount = 3, diceSides = 8, usesAttackRoll = true, damageType = "elemental", range = new SpellRange { metres = 27f, requiresLoS = true } },
            ["witch-bolt"] = new SpellDef { id = "witch-bolt", name = "Witch Bolt", diceCount = 1, diceSides = 12, usesAttackRoll = true, damageType = "lightning", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["cure-wounds"] = new SpellDef { id = "cure-wounds", name = "Cure Wounds", diceCount = 1, diceSides = 8, isHeal = true, autoHit = true, addAbilityToDmg = true, damageType = "healing", range = new SpellRange { metres = 2f, requiresLoS = true } },
            ["healing-word"] = new SpellDef { id = "healing-word", name = "Healing Word", diceCount = 1, diceSides = 4, isHeal = true, autoHit = true, addAbilityToDmg = true, damageType = "healing", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["bless"] = new SpellDef { id = "bless", name = "Bless" }, // buff placeholder
            ["bane"] = new SpellDef { id = "bane", name = "Bane" },   // debuff placeholder
            ["guiding-bolt"] = new SpellDef { id = "guiding-bolt", name = "Guiding Bolt", diceCount = 4, diceSides = 6, usesAttackRoll = true, damageType = "radiant", range = new SpellRange { metres = 36f, requiresLoS = true } },
            ["thunderwave"] = new SpellDef { id = "thunderwave", name = "Thunderwave", diceCount = 2, diceSides = 8, save = SaveType.CON, halfOnSave = true, damageType = "thunder", range = new SpellRange { metres = 3f, requiresLoS = true } },
            ["hail-of-thorns"] = new SpellDef { id = "hail-of-thorns", name = "Hail of Thorns", diceCount = 1, diceSides = 10, save = SaveType.DEX, halfOnSave = true, damageType = "piercing", range = new SpellRange { metres = 18f, requiresLoS = true } },

            // --- Level 2 ---
            ["scorching-ray"] = new SpellDef { id = "scorching-ray", name = "Scorching Ray", diceCount = 2, diceSides = 6, darts = 3, usesAttackRoll = true, damageType = "fire", range = new SpellRange { metres = 36f, requiresLoS = true } },
            ["shatter"] = new SpellDef { id = "shatter", name = "Shatter", diceCount = 3, diceSides = 8, save = SaveType.CON, halfOnSave = true, damageType = "thunder", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["spiritual-weapon"] = new SpellDef { id = "spiritual-weapon", name = "Spiritual Weapon", diceCount = 1, diceSides = 8, flatBonus = SpellScaling.AbilityBonus, usesAttackRoll = true, damageType = "force", range = new SpellRange { metres = 18f, requiresLoS = true } },
            ["prayer-of-healing"] = new SpellDef { id = "prayer-of-healing", name = "Prayer of Healing", diceCount = 2, diceSides = 8, isHeal = true, autoHit = true, addAbilityToDmg = true, damageType = "healing", range = new SpellRange { metres = 18f, requiresLoS = false } },

            // --- Level 3 ---
            ["fireball"] = new SpellDef { id = "fireball", name = "Fireball", diceCount = 8, diceSides = 6, save = SaveType.DEX, halfOnSave = true, damageType = "fire", range = new SpellRange { metres = 45f, requiresLoS = true } },
            ["lightning-bolt"] = new SpellDef { id = "lightning-bolt", name = "Lightning Bolt", diceCount = 8, diceSides = 6, save = SaveType.DEX, halfOnSave = true, damageType = "lightning", range = new SpellRange { metres = 30f, requiresLoS = true } },
            ["counterspell"] = new SpellDef { id = "counterspell", name = "Counterspell" }, // reaction placeholder
            ["revivify"] = new SpellDef { id = "revivify", name = "Revivify", isHeal = true, autoHit = true, damageType = "healing", range = new SpellRange { metres = 2f, requiresLoS = true } },
        };
}

// Small extension trick: lets us optionally refer to "add ability mod" without hard-wiring a number
public static class SpellScaling { public const int AbilityBonus = 9999; }
