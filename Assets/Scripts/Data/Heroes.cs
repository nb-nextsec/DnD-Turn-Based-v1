using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class HeroDef
{
    public string name;
    public ClassId cls;
    public int level;

    public int maxHP;
    public int armourClass;

    public int str, dex, con, intel, wis, cha;

    public string weaponId = "longsword";
    public bool twoHands = false;
    public bool shield = false;
    public bool isRanged = false;

    public Vector3 spawnOffset;

    public HeroDef(string name, ClassId cls, int level,
                   int hp, int ac,
                   (int str, int dex, int con, int intel, int wis, int cha) ab,
                   string weaponId, bool twoHands, bool shield, bool isRanged,
                   Vector3 spawnOffset)
    {
        this.name = name; this.cls = cls; this.level = level;
        this.maxHP = hp; this.armourClass = ac;
        this.str = ab.str; this.dex = ab.dex; this.con = ab.con;
        this.intel = ab.intel; this.wis = ab.wis; this.cha = ab.cha;
        this.weaponId = weaponId; this.twoHands = twoHands; this.shield = shield; this.isRanged = isRanged;
        this.spawnOffset = spawnOffset;
    }

    // Convenience: clone with different level/HP/AC if you want variants later
    public HeroDef WithLevel(int lvl, int hp, int ac) =>
        new HeroDef(name, cls, lvl, hp, ac,
            (str, dex, con, intel, wis, cha), weaponId, twoHands, shield, isRanged, spawnOffset);
}

public static class HeroesDB
{
    // --- Base lineup at ~level 5 (your previous defaults) ---
    public static readonly HeroDef WarriorL5 = new HeroDef(
        name: "Warrior", cls: ClassId.Fighter, level: 5,
        hp: 46, ac: 18,
        ab: (18, 12, 14, 8, 10, 10),
        weaponId: "longsword", twoHands: false, shield: true, isRanged: false,
        spawnOffset: new Vector3(-3, 0, 0)
    );

    public static readonly HeroDef ClericL5 = new HeroDef(
        name: "Cleric", cls: ClassId.Cleric, level: 5,
        hp: 36, ac: 17,
        ab: (14, 10, 14, 10, 16, 12),
        weaponId: "warhammer", twoHands: false, shield: true, isRanged: false,
        spawnOffset: new Vector3(-3, 0, 2)
    );

    public static readonly HeroDef RogueL5 = new HeroDef(
        name: "Rogue", cls: ClassId.Rogue, level: 5,
        hp: 34, ac: 15,
        ab: (10, 18, 12, 12, 10, 14),
        weaponId: "rapier", twoHands: false, shield: false, isRanged: false,
        spawnOffset: new Vector3(-3, 0, -2)
    );

    public static readonly HeroDef SorcererL5 = new HeroDef(
        name: "Sorcerer", cls: ClassId.Sorcerer, level: 5,
        hp: 30, ac: 13,
        ab: (8, 14, 14, 10, 10, 18),
        weaponId: "dagger", twoHands: false, shield: false, isRanged: true,
        spawnOffset: new Vector3(-3, 0, 4)
    );

    public static List<HeroDef> DefaultParty => new List<HeroDef>
    {
        WarriorL5, ClericL5, RogueL5, SorcererL5
    };

    // --- Strong parties (average levels 8, 12, 15, 18) ---

    public static List<HeroDef> StrongL8 => new List<HeroDef>
    {
        new HeroDef("Champion", ClassId.Fighter, 8,
            hp: 82, ac: 19, ab:(20,14,16,8,10,10),
            weaponId:"longsword", twoHands:false, shield:true, isRanged:false,
            spawnOffset:new Vector3(-3,0,0)),
        new HeroDef("Tempest Cleric", ClassId.Cleric, 8,
            hp: 64, ac: 18, ab:(14,10,16,10,18,12),
            weaponId:"warhammer", twoHands:false, shield:true, isRanged:false,
            spawnOffset:new Vector3(-3,0,2)),
        new HeroDef("Assassin", ClassId.Rogue, 8,
            hp: 58, ac: 17, ab:(10,20,14,12,12,14),
            weaponId:"rapier", twoHands:false, shield:false, isRanged:false,
            spawnOffset:new Vector3(-3,0,-2)),
        new HeroDef("Draconic Sorcerer", ClassId.Sorcerer, 8,
            hp: 56, ac: 14, ab:(8,14,16,10,10,20),
            weaponId:"dagger", twoHands:false, shield:false, isRanged:true,
            spawnOffset:new Vector3(-3,0,4)),
    };

    public static List<HeroDef> StrongL12 => new List<HeroDef>
    {
        new HeroDef("Eldritch Knight", ClassId.Fighter, 12,
            hp: 112, ac: 20, ab:(20,16,18,14,10,10),
            weaponId:"longsword", twoHands:false, shield:true, isRanged:false,
            spawnOffset:new Vector3(-3,0,0)),
        new HeroDef("Life Cleric", ClassId.Cleric, 12,
            hp: 92, ac: 19, ab:(14,10,18,10,20,12),
            weaponId:"warhammer", twoHands:false, shield:true, isRanged:false,
            spawnOffset:new Vector3(-3,0,2)),
        new HeroDef("Arcane Trickster", ClassId.Rogue, 12
            , hp: 84, ac: 18, ab:(10,20,16,16,12,14),
            weaponId:"rapier", twoHands:false, shield:false, isRanged:false,
            spawnOffset:new Vector3(-3,0,-2)),
        new HeroDef("Storm Sorcerer", ClassId.Sorcerer, 12,
            hp: 84, ac: 15, ab:(8,16,18,12,10,20),
            weaponId:"dagger", twoHands:false, shield:false, isRanged:true,
            spawnOffset:new Vector3(-3,0,4)),
    };

    public static List<HeroDef> StrongL15 => new List<HeroDef>
    {
        new HeroDef("Battle Master", ClassId.Fighter, 15,
            hp: 136, ac: 20, ab:(20,18,18,14,12,12),
            weaponId:"longsword", twoHands:false, shield:true, isRanged:false,
            spawnOffset:new Vector3(-3,0,0)),
        new HeroDef("War Cleric", ClassId.Cleric, 15,
            hp: 120, ac: 20, ab:(16,12,18,10,20,14),
            weaponId:"warhammer", twoHands:false, shield:true, isRanged:false,
            spawnOffset:new Vector3(-3,0,2)),
        new HeroDef("Swashbuckler", ClassId.Rogue, 15,
            hp: 110, ac: 19, ab:(12,20,18,14,14,16),
            weaponId:"rapier", twoHands:false, shield:false, isRanged:false,
            spawnOffset:new Vector3(-3,0,-2)),
        new HeroDef("Wild Mage", ClassId.Sorcerer, 15,
            hp: 112, ac: 16, ab:(8,18,18,14,12,20),
            weaponId:"dagger", twoHands:false, shield:false, isRanged:true,
            spawnOffset:new Vector3(-3,0,4)),
    };

    public static List<HeroDef> StrongL18 => new List<HeroDef>
    {
        new HeroDef("Swordmaster", ClassId.Fighter, 18,
            hp: 168, ac: 21, ab:(20,20,18,14,14,12),
            weaponId:"longsword", twoHands:false, shield:true, isRanged:false,
            spawnOffset:new Vector3(-3,0,0)),
        new HeroDef("Sun Cleric", ClassId.Cleric, 18,
            hp: 150, ac: 21, ab:(16,12,20,12,22,14),
            weaponId:"warhammer", twoHands:false, shield:true, isRanged:false,
            spawnOffset:new Vector3(-3,0,2)),
        new HeroDef("Master Rogue", ClassId.Rogue, 18,
            hp: 148, ac: 20, ab:(12,22,20,16,16,16),
            weaponId:"rapier", twoHands:false, shield:false, isRanged:false,
            spawnOffset:new Vector3(-3,0,-2)),
        new HeroDef("Arch Sorcerer", ClassId.Sorcerer, 18,
            hp: 142, ac: 17, ab:(8,20,20,16,14,22),
            weaponId:"dagger", twoHands:false, shield:false, isRanged:true,
            spawnOffset:new Vector3(-3,0,4)),
    };

    // Parties registry
    public static readonly Dictionary<string, List<HeroDef>> Parties =
        new Dictionary<string, List<HeroDef>>
        {
            ["default-party"] = DefaultParty,
            ["strong-l8"] = StrongL8,
            ["strong-l12"] = StrongL12,
            ["strong-l15"] = StrongL15,
            ["strong-l18"] = StrongL18,
        };

    public static List<HeroDef> GetParty(string id) =>
        Parties.TryGetValue(id, out var list) ? list : null;
}
