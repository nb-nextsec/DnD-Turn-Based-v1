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
    public string id;
    public string name;

    // 0 = cantrip, 1..9 = leveled spell
    public int level = 0;

    // Base damage/heal: XdY + flatBonus (� darts if multi-dart spell)
    public int diceCount = 1;
    public int diceSides = 6;
    public int flatBonus = 0;
    public int darts = 1;

    public bool isHeal = false;
    public bool autoHit = false;
    public bool usesAttackRoll = false;
    public SaveType save = SaveType.None;
    public bool halfOnSave = false;
    public string damageType = "force";
    public bool addAbilityToDmg = false;
    public SpellRange range = new SpellRange { metres = 18f, requiresLoS = true };

    // --- NEW: scaling descriptors ---
    // Cantrip scaling: multiply diceCount by cantrip tier (1..4). Works for 1d10 -> 2d10 -> 3d10 -> 4d10, etc.
    public bool cantripScalesDice = true;

    // Slot scaling (leveled spells). You can combine these per spell as needed.
    public bool scalesWithSlot = false;
    public int extraDartsPerSlot = 0;  // e.g. Magic Missile +1 dart/slot above 1st
    public int extraDicePerSlot = 0;  // e.g. Guiding Bolt +1d6/slot above 1st
    public int extraFlatPerSlot = 0;  // seldom used; available if needed

    public override string ToString() => name;
}

public static class SpellDB
{
    // Canon: one definition per spell id.
    public static readonly Dictionary<string, SpellDef> All =
        new Dictionary<string, SpellDef>(StringComparer.OrdinalIgnoreCase)
        {
            // ===== CANTRIPS (level = 0) =====
            ["fire-bolt"] = new SpellDef
            {
                id = "fire-bolt",
                name = "Fire Bolt",
                level = 0,
                diceCount = 1,
                diceSides = 10,
                usesAttackRoll = true,
                cantripScalesDice = true,
                damageType = "fire",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["ray-of-frost"] = new SpellDef
            {
                id = "ray-of-frost",
                name = "Ray of Frost",
                level = 0,
                diceCount = 1,
                diceSides = 8,
                usesAttackRoll = true,
                cantripScalesDice = true,
                damageType = "cold",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["acid-splash"] = new SpellDef
            {
                id = "acid-splash",
                name = "Acid Splash",
                level = 0,
                diceCount = 1,
                diceSides = 6,
                save = SaveType.DEX,
                cantripScalesDice = true,
                damageType = "acid",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["shocking-grasp"] = new SpellDef
            {
                id = "shocking-grasp",
                name = "Shocking Grasp",
                level = 0,
                diceCount = 1,
                diceSides = 8,
                usesAttackRoll = true,
                cantripScalesDice = true,
                damageType = "lightning",
                range = new SpellRange { metres = 2f, requiresLoS = true }
            },
            ["eldritch-blast"] = new SpellDef
            {
                id = "eldritch-blast",
                name = "Eldritch Blast",
                level = 0,
                diceCount = 1,
                diceSides = 10,
                usesAttackRoll = true,
                cantripScalesDice = true,   // simplified: extra dice, not extra beams
                damageType = "force",
                range = new SpellRange { metres = 36f, requiresLoS = true }
            },
            ["vicious-mockery"] = new SpellDef
            {
                id = "vicious-mockery",
                name = "Vicious Mockery",
                level = 0,
                diceCount = 1,
                diceSides = 4,
                save = SaveType.WIS,
                cantripScalesDice = true,
                damageType = "psychic",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["sacred-flame"] = new SpellDef
            {
                id = "sacred-flame",
                name = "Sacred Flame",
                level = 0,
                diceCount = 1,
                diceSides = 8,
                save = SaveType.DEX,
                cantripScalesDice = true,
                damageType = "radiant",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["guidance"] = new SpellDef { id = "guidance", name = "Guidance", level = 0 },
            ["thaumaturgy"] = new SpellDef { id = "thaumaturgy", name = "Thaumaturgy", level = 0 },

            // ===== LEVEL 1 =====
            ["magic-missile"] = new SpellDef
            {
                id = "magic-missile",
                name = "Magic Missile",
                level = 1,
                diceCount = 1,
                diceSides = 4,
                flatBonus = 1,
                darts = 3,
                autoHit = true,
                scalesWithSlot = true,
                extraDartsPerSlot = 1,   // +1 dart per slot
                damageType = "force",
                range = new SpellRange { metres = 36f, requiresLoS = true }
            },
            ["burning-hands"] = new SpellDef
            {
                id = "burning-hands",
                name = "Burning Hands",
                level = 1,
                diceCount = 3,
                diceSides = 6,
                save = SaveType.DEX,
                halfOnSave = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d6 per slot
                damageType = "fire",
                range = new SpellRange { metres = 3f, requiresLoS = true }
            },
            ["chromatic-orb"] = new SpellDef
            {
                id = "chromatic-orb",
                name = "Chromatic Orb",
                level = 1,
                diceCount = 3,
                diceSides = 8,
                usesAttackRoll = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d8 per slot
                damageType = "elemental",
                range = new SpellRange { metres = 27f, requiresLoS = true }
            },
            ["witch-bolt"] = new SpellDef
            {
                id = "witch-bolt",
                name = "Witch Bolt",
                level = 1,
                diceCount = 1,
                diceSides = 12,
                usesAttackRoll = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d12 per slot
                damageType = "lightning",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["guiding-bolt"] = new SpellDef
            {
                id = "guiding-bolt",
                name = "Guiding Bolt",
                level = 1,
                diceCount = 4,
                diceSides = 6,
                usesAttackRoll = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d6 per slot
                damageType = "radiant",
                range = new SpellRange { metres = 36f, requiresLoS = true }
            },
            ["thunderwave"] = new SpellDef
            {
                id = "thunderwave",
                name = "Thunderwave",
                level = 1,
                diceCount = 2,
                diceSides = 8,
                save = SaveType.CON,
                halfOnSave = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d8 per slot
                damageType = "thunder",
                range = new SpellRange { metres = 3f, requiresLoS = true }
            },
            ["hail-of-thorns"] = new SpellDef
            {
                id = "hail-of-thorns",
                name = "Hail of Thorns",
                level = 1,
                diceCount = 1,
                diceSides = 10,
                save = SaveType.DEX,
                halfOnSave = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d10 per slot
                damageType = "piercing",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["cure-wounds"] = new SpellDef
            {
                id = "cure-wounds",
                name = "Cure Wounds",
                level = 1,
                diceCount = 1,
                diceSides = 8,
                isHeal = true,
                autoHit = true,
                addAbilityToDmg = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d8 per slot
                damageType = "healing",
                range = new SpellRange { metres = 2f, requiresLoS = true }
            },
            ["healing-word"] = new SpellDef
            {
                id = "healing-word",
                name = "Healing Word",
                level = 1,
                diceCount = 1,
                diceSides = 4,
                isHeal = true,
                autoHit = true,
                addAbilityToDmg = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d4 per slot
                damageType = "healing",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["bless"] = new SpellDef { id = "bless", name = "Bless", level = 1 },
            ["bane"] = new SpellDef { id = "bane", name = "Bane", level = 1 },

            // ===== LEVEL 2 =====
            ["scorching-ray"] = new SpellDef
            {
                id = "scorching-ray",
                name = "Scorching Ray",
                level = 2,
                diceCount = 2,
                diceSides = 6,
                darts = 3,
                usesAttackRoll = true,
                scalesWithSlot = true,
                extraDartsPerSlot = 1,    // +1 ray per slot
                damageType = "fire",
                range = new SpellRange { metres = 36f, requiresLoS = true }
            },
            ["shatter"] = new SpellDef
            {
                id = "shatter",
                name = "Shatter",
                level = 2,
                diceCount = 3,
                diceSides = 8,
                save = SaveType.CON,
                halfOnSave = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d8 per slot
                damageType = "thunder",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["spiritual-weapon"] = new SpellDef
            {
                id = "spiritual-weapon",
                name = "Spiritual Weapon",
                level = 2,
                diceCount = 1,
                diceSides = 8,
                flatBonus = SpellScaling.AbilityBonus,
                usesAttackRoll = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d8 per slot
                damageType = "force",
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },
            ["prayer-of-healing"] = new SpellDef
            {
                id = "prayer-of-healing",
                name = "Prayer of Healing",
                level = 2,
                diceCount = 2,
                diceSides = 8,
                isHeal = true,
                autoHit = true,
                addAbilityToDmg = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d8 per slot
                damageType = "healing",
                range = new SpellRange { metres = 18f, requiresLoS = false }
            },

            // ===== LEVEL 3 =====
            ["fireball"] = new SpellDef
            {
                id = "fireball",
                name = "Fireball",
                level = 3,
                diceCount = 8,
                diceSides = 6,
                save = SaveType.DEX,
                halfOnSave = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d6 per slot
                damageType = "fire",
                range = new SpellRange { metres = 45f, requiresLoS = true }
            },
            ["lightning-bolt"] = new SpellDef
            {
                id = "lightning-bolt",
                name = "Lightning Bolt",
                level = 3,
                diceCount = 8,
                diceSides = 6,
                save = SaveType.DEX,
                halfOnSave = true,
                scalesWithSlot = true,
                extraDicePerSlot = 1,     // +1d6 per slot
                damageType = "lightning",
                range = new SpellRange { metres = 30f, requiresLoS = true }
            },
            ["counterspell"] = new SpellDef { id = "counterspell", name = "Counterspell", level = 3 },
            ["revivify"] = new SpellDef
            {
                id = "revivify",
                name = "Revivify",
                level = 3,
                isHeal = true,
                autoHit = true,
                damageType = "healing",
                range = new SpellRange { metres = 2f, requiresLoS = true }
            },
        };
}


    // Small extension trick: lets us optionally refer to "add ability mod" without hard-wiring a number
    public static class SpellScaling
    {
        public const int AbilityBonus = 9999;
    }