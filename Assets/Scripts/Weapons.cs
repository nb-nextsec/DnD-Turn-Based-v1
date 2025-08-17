// using UnityEngine;
using UnityEngine;
using System;
using System.Collections.Generic;

public enum DamageType { Slashing, Piercing, Bludgeoning }

[Serializable]
public struct RangeBand
{
    public int normal;     // e.g. 80
    public int longRange;  // e.g. 320
}

[Serializable]
public struct VersatileDamage
{
    public int diceCount;  // e.g. 1
    public int diceSides;  // e.g. 10 (for 1d10)
}

[Serializable]
public class WeaponDef
{
    public string id;                // "longsword"
    public string name;              // "Longsword"
    public int diceCount;            // default one-handed dice
    public int diceSides;
    public DamageType damageType;

    public bool isRanged;
    public bool finesse;             // melee finesse: can use STR or DEX (higher)
    public bool twoHanded;
    public bool heavy;
    public bool reach;               // 10 ft reach style (we'll map to a longer melee range)

    public bool hasRange;
    public RangeBand range;

    public bool hasVersatile;
    public VersatileDamage versatile;
}

public static class WeaponDB
{
    // D&D 5e SRD-style weapons (simplified)
    public static readonly Dictionary<string, WeaponDef> All =
        new Dictionary<string, WeaponDef>(StringComparer.OrdinalIgnoreCase)
        {
            // --- Simple melee ---
            ["club"] = new WeaponDef { id = "club", name = "Club", diceCount = 1, diceSides = 4, damageType = DamageType.Bludgeoning, isRanged = false },
            ["dagger"] = new WeaponDef { id = "dagger", name = "Dagger", diceCount = 1, diceSides = 4, damageType = DamageType.Piercing, isRanged = false, finesse = true },
            ["greatclub"] = new WeaponDef { id = "greatclub", name = "Greatclub", diceCount = 1, diceSides = 8, damageType = DamageType.Bludgeoning, isRanged = false, twoHanded = true },
            ["handaxe"] = new WeaponDef { id = "handaxe", name = "Handaxe", diceCount = 1, diceSides = 6, damageType = DamageType.Slashing, isRanged = false },
            ["mace"] = new WeaponDef { id = "mace", name = "Mace", diceCount = 1, diceSides = 6, damageType = DamageType.Bludgeoning, isRanged = false },
            ["quarterstaff"] = new WeaponDef { id = "quarterstaff", name = "Quarterstaff", diceCount = 1, diceSides = 6, damageType = DamageType.Bludgeoning, isRanged = false, hasVersatile = true, versatile = new VersatileDamage { diceCount = 1, diceSides = 8 } },
            ["spear"] = new WeaponDef { id = "spear", name = "Spear", diceCount = 1, diceSides = 6, damageType = DamageType.Piercing, isRanged = false, hasVersatile = true, versatile = new VersatileDamage { diceCount = 1, diceSides = 8 } },
            ["light-hammer"] = new WeaponDef { id = "light-hammer", name = "Light Hammer", diceCount = 1, diceSides = 4, damageType = DamageType.Bludgeoning, isRanged = false },

            // --- Martial melee ---
            ["longsword"] = new WeaponDef { id = "longsword", name = "Longsword", diceCount = 1, diceSides = 8, damageType = DamageType.Slashing, isRanged = false, hasVersatile = true, versatile = new VersatileDamage { diceCount = 1, diceSides = 10 } },
            ["shortsword"] = new WeaponDef { id = "shortsword", name = "Shortsword", diceCount = 1, diceSides = 6, damageType = DamageType.Piercing, isRanged = false, finesse = true },
            ["rapier"] = new WeaponDef { id = "rapier", name = "Rapier", diceCount = 1, diceSides = 8, damageType = DamageType.Piercing, isRanged = false, finesse = true },
            ["scimitar"] = new WeaponDef { id = "scimitar", name = "Scimitar", diceCount = 1, diceSides = 6, damageType = DamageType.Slashing, isRanged = false, finesse = true },
            ["battleaxe"] = new WeaponDef { id = "battleaxe", name = "Battleaxe", diceCount = 1, diceSides = 8, damageType = DamageType.Slashing, isRanged = false, hasVersatile = true, versatile = new VersatileDamage { diceCount = 1, diceSides = 10 } },
            ["warhammer"] = new WeaponDef { id = "warhammer", name = "Warhammer", diceCount = 1, diceSides = 8, damageType = DamageType.Bludgeoning, isRanged = false, hasVersatile = true, versatile = new VersatileDamage { diceCount = 1, diceSides = 10 } },
            ["flail"] = new WeaponDef { id = "flail", name = "Flail", diceCount = 1, diceSides = 8, damageType = DamageType.Bludgeoning, isRanged = false },
            ["morningstar"] = new WeaponDef { id = "morningstar", name = "Morningstar", diceCount = 1, diceSides = 8, damageType = DamageType.Piercing, isRanged = false },
            ["war-pick"] = new WeaponDef { id = "war-pick", name = "War Pick", diceCount = 1, diceSides = 8, damageType = DamageType.Piercing, isRanged = false },
            ["greataxe"] = new WeaponDef { id = "greataxe", name = "Greataxe", diceCount = 1, diceSides = 12, damageType = DamageType.Slashing, isRanged = false, heavy = true, twoHanded = true },
            ["greatsword"] = new WeaponDef { id = "greatsword", name = "Greatsword", diceCount = 2, diceSides = 6, damageType = DamageType.Slashing, isRanged = false, heavy = true, twoHanded = true },
            ["halberd"] = new WeaponDef { id = "halberd", name = "Halberd", diceCount = 1, diceSides = 10, damageType = DamageType.Slashing, isRanged = false, heavy = true, twoHanded = true, reach = true },
            ["glaive"] = new WeaponDef { id = "glaive", name = "Glaive", diceCount = 1, diceSides = 10, damageType = DamageType.Slashing, isRanged = false, heavy = true, twoHanded = true, reach = true },
            ["pike"] = new WeaponDef { id = "pike", name = "Pike", diceCount = 1, diceSides = 10, damageType = DamageType.Piercing, isRanged = false, heavy = true, twoHanded = true, reach = true },
            ["maul"] = new WeaponDef { id = "maul", name = "Maul", diceCount = 2, diceSides = 6, damageType = DamageType.Bludgeoning, isRanged = false, heavy = true, twoHanded = true },
            ["lance"] = new WeaponDef { id = "lance", name = "Lance", diceCount = 1, diceSides = 12, damageType = DamageType.Piercing, isRanged = false, reach = true },

            // --- Ranged ---
            ["shortbow"] = new WeaponDef { id = "shortbow", name = "Shortbow", diceCount = 1, diceSides = 6, damageType = DamageType.Piercing, isRanged = true, twoHanded = true, hasRange = true, range = new RangeBand { normal = 80, longRange = 320 } },
            ["longbow"] = new WeaponDef { id = "longbow", name = "Longbow", diceCount = 1, diceSides = 8, damageType = DamageType.Piercing, isRanged = true, twoHanded = true, heavy = true, hasRange = true, range = new RangeBand { normal = 150, longRange = 600 } },
            ["heavy-xbow"] = new WeaponDef { id = "heavy-xbow", name = "Heavy Crossbow", diceCount = 1, diceSides = 10, damageType = DamageType.Piercing, isRanged = true, twoHanded = true, heavy = true, hasRange = true, range = new RangeBand { normal = 100, longRange = 400 } },
        };

    /// <summary>
    /// Apply a weapon's properties into an existing Stats block.
    /// If twoHands=true and the weapon is versatile, use its versatile dice.
    /// </summary>
    public static void Equip(Stats s, string weaponId, bool twoHands = false)
    {
        if (!All.TryGetValue(weaponId, out var w))
        {
            Debug.LogWarning($"Weapon '{weaponId}' not found. Keeping existing stats.");
            return;
        }

        // Damage dice
        if (w.hasVersatile && twoHands)
            s.damage = new Vector2Int(w.versatile.diceCount, w.versatile.diceSides);
        else
            s.damage = new Vector2Int(w.diceCount, w.diceSides);

        // Melee/ranged & reach mapping
        s.isRanged = w.isRanged;
        s.attackRange = w.isRanged ? 7.0f : (w.reach ? 3.5f : 2.2f);

        // Finesse flag
        s.finesse = w.finesse;

        // Track which weapon was equipped
        s.weaponId = w.id;
        s.weaponName = w.name;

        // ✅ Log for debugging
        Debug.Log($"{s.displayName} equips {w.name} {(twoHands && w.hasVersatile ? "(two-handed)" : "")} | Damage: {s.damage.x}d{s.damage.y}");
    }
}
