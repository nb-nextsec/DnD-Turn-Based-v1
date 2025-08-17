using UnityEngine;
using System;
using System.Collections.Generic;

public enum SaveType { None, STR, DEX, CON, INT, WIS, CHA }

[Serializable]
public struct SpellRange
{
    public float metres;         // simple flat range in metres
    public bool requiresLoS;     // reserved for future line-of-sight checks
}

[Serializable]
public class SpellDef
{
    public string id;            // "magic-missile"
    public string name;          // "Magic Missile"

    // Base damage: XdY (+ flatBonus per dart if multi-dart spell)
    public int diceCount = 1;
    public int diceSides = 6;
    public int flatBonus = 0;
    public int darts = 1;              // e.g. Magic Missile = 3

    // Behaviour flags
    public bool isHeal = false;        // treat as healing (positive effect)
    public bool autoHit = false;       // Magic Missile-style
    public bool usesAttackRoll = false;// Fire Bolt-style (spell attack vs AC)

    // Save-based spells (Sacred Flame, Fireball, etc.)
    public SaveType save = SaveType.None; // which save the target rolls
    public bool halfOnSave = false;       // take half damage on a successful save?

    // Damage typing (plain string to keep it flexible with weapons’ DamageType)
    public string damageType = "force";   // "fire", "radiant", "necrotic", "healing", etc.

    public bool addAbilityToDmg = false;  // add casting ability mod to damage/heal (e.g., Cure Wounds heal)
    public SpellRange range = new SpellRange { metres = 18f, requiresLoS = true };

    public override string ToString() => name;
}

public static class SpellDB
{
    public static readonly Dictionary<string, SpellDef> All =
        new Dictionary<string, SpellDef>(StringComparer.OrdinalIgnoreCase)
        {
            // --- Arcane (Sorcerer/Wizard) ---
            ["fire-bolt"] = new SpellDef
            {
                id = "fire-bolt",
                name = "Fire Bolt",
                diceCount = 1,
                diceSides = 10,
                flatBonus = 0,
                darts = 1,
                damageType = "fire",
                autoHit = false,
                usesAttackRoll = true,
                save = SaveType.None,
                addAbilityToDmg = false,
                isHeal = false,
                range = new SpellRange { metres = 18f, requiresLoS = true }   // ~60 ft
            },

            // Magic Missile: 3 darts of 1d4+1 each (total 3d4+3 to one target here)
            ["magic-missile"] = new SpellDef
            {
                id = "magic-missile",
                name = "Magic Missile",
                diceCount = 1,
                diceSides = 4,
                flatBonus = 1,
                darts = 3,
                damageType = "force",
                autoHit = true,
                usesAttackRoll = false,
                save = SaveType.None,
                addAbilityToDmg = false,
                isHeal = false,
                range = new SpellRange { metres = 36f, requiresLoS = true }   // ~120 ft
            },

            // --- Divine (Cleric) ---
            ["sacred-flame"] = new SpellDef
            {
                id = "sacred-flame",
                name = "Sacred Flame",
                diceCount = 1,
                diceSides = 8,
                flatBonus = 0,
                darts = 1,
                damageType = "radiant",
                autoHit = false,
                usesAttackRoll = false,
                save = SaveType.DEX,
                halfOnSave = false,
                addAbilityToDmg = false,
                isHeal = false,
                range = new SpellRange { metres = 18f, requiresLoS = true }
            },

            ["guiding-bolt"] = new SpellDef
            {
                id = "guiding-bolt",
                name = "Guiding Bolt",
                diceCount = 4,
                diceSides = 6,
                flatBonus = 0,
                darts = 1,
                damageType = "radiant",
                autoHit = false,
                usesAttackRoll = true,
                save = SaveType.None,
                addAbilityToDmg = false,
                isHeal = false,
                range = new SpellRange { metres = 36f, requiresLoS = true }
            },

            ["cure-wounds"] = new SpellDef
            {
                id = "cure-wounds",
                name = "Cure Wounds",
                diceCount = 1,
                diceSides = 8,
                flatBonus = 0,
                darts = 1,
                damageType = "healing",
                autoHit = true,
                usesAttackRoll = false,
                save = SaveType.None,
                addAbilityToDmg = true,         // add casting ability mod to heal
                isHeal = true,
                range = new SpellRange { metres = 2f, requiresLoS = true }    // touch-ish
            },
        };
}
