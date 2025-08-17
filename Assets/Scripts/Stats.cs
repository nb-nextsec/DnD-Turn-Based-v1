using UnityEngine;
using System.Collections.Generic;

public enum CastingAbility { None, STR, DEX, CON, INT, WIS, CHA }

// Optional: your class enum lives in Classes/ClassId.cs
// public enum ClassId { Barbarian, Bard, Cleric, Druid, Fighter, Monk, Paladin, Ranger, Rogue, Sorcerer, Warlock, Wizard, Artificer }

// Optional: KnownSpell struct lives in Classes/SpellList.cs
// public struct KnownSpell { public string id; public int level; public KnownSpell(string id,int level){ this.id=id; this.level=level; } }

[System.Serializable]
public class Stats
{
    // === Core combat ===
    public string displayName = "Unit";
    public int maxHP = 10;
    public int armourClass = 12;

    // damage as XdY + bonus
    public Vector2Int damage = new Vector2Int(1, 6);
    public int damageBonus = 0;

    public float attackRange = 2.2f;
    public int actionsPerTurn = 1;
    public int initiativeBonus = 2;

    // Healing (optional)
    public bool canHeal = false;
    public Vector2Int healAmount = new Vector2Int(1, 6);
    public float healThreshold = 0.4f;

    // Movement (optional)
    public float movePerAction = 2.5f;
    public float desiredRange = 2.0f;

    // === D&D 5e-style ability scores ===
    [Header("5e Ability Scores")]
    [Range(1, 30)] public int str = 10;
    [Range(1, 30)] public int dex = 10;
    [Range(1, 30)] public int con = 10;
    [Range(1, 30)] public int @int = 10;
    [Range(1, 30)] public int wis = 10;
    [Range(1, 30)] public int cha = 10;

    [Header("Weapon (auto-filled)")]
    public string weaponId = "";     // e.g. "longsword"
    public string weaponName = "";   // e.g. "Longsword"

    [Header("Attack Style")]
    public bool isRanged = false;      // false = melee, true = ranged
    public bool finesse = false;       // melee may use the better of STR/DEX
    public bool usingTwoHands = false; // if true and weapon hasVersatile, use bigger die
    [Range(1, 6)] public int proficiencyBonus = 2; // 5e levels 1..17

    [Header("Casting")]
    public bool isCaster = false;
    public CastingAbility castingAbility = CastingAbility.None;

    [Header("Class & Spellbook")]
    public ClassId classId = ClassId.Fighter;     // set when spawning the unit
    [System.NonSerialized] public List<KnownSpell> knownSpells; // filled from class spell list at spawn

    // ---------- Helpers ----------
    public static int Mod(int score) => Mathf.FloorToInt((score - 10) / 2f);

    /// Martial attack bonus: Ranged → DEX; Finesse melee → max(STR, DEX); Melee → STR.
    public int AttackBonus
    {
        get
        {
            if (isRanged) return Mod(dex) + proficiencyBonus;
            if (finesse) return Mathf.Max(Mod(str), Mod(dex)) + proficiencyBonus;
            return Mod(str) + proficiencyBonus;
        }
    }

    /// Spell attack bonus and save DC (8 + prof + casting mod)
    public int SpellAttackBonus
    {
        get
        {
            if (!isCaster || castingAbility == CastingAbility.None) return 0;
            return CastingMod() + proficiencyBonus;
        }
    }

    public int SpellSaveDC
    {
        get
        {
            if (!isCaster || castingAbility == CastingAbility.None) return 10;
            return 8 + proficiencyBonus + CastingMod();
        }
    }

    int CastingMod()
    {
        return castingAbility switch
        {
            CastingAbility.STR => Mod(str),
            CastingAbility.DEX => Mod(dex),
            CastingAbility.CON => Mod(con),
            CastingAbility.INT => Mod(@int),
            CastingAbility.WIS => Mod(wis),
            CastingAbility.CHA => Mod(cha),
            _ => 0
        };
    }

    /// Clamp ability scores by unit type: PCs 8..20, NPCs/monsters 1..30.
    public void ClampFor(bool isPlayerControlled)
    {
        int min = isPlayerControlled ? 8 : 1;
        int max = isPlayerControlled ? 20 : 30;

        str = Mathf.Clamp(str, min, max);
        dex = Mathf.Clamp(dex, min, max);
        con = Mathf.Clamp(con, min, max);
        @int = Mathf.Clamp(@int, min, max);
        wis = Mathf.Clamp(wis, min, max);
        cha = Mathf.Clamp(cha, min, max);

        proficiencyBonus = Mathf.Clamp(proficiencyBonus, 1, 6);
    }
}
