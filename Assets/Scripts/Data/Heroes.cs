using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class HeroDef
{
    public string name;
    public ClassId cls;
    public int level;

    // Core stat block
    public int maxHP;
    public int armourClass;

    // Abilities
    public int str, dex, con, intel, wis, cha;

    // Equipment
    public string weaponId = "longsword";
    public bool twoHands = false;
    public bool shield = false;
    public bool isRanged = false;

    // Spawn offset (relative placement for your party line)
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
}

public static class HeroesDB
{
    // Define your reusable heroes here
    public static readonly HeroDef WarriorL5 = new HeroDef(
        name: "Warrior", cls: ClassId.Fighter, level: 5,
        hp: 46, ac: 18,
        ab: (str: 18, dex: 12, con: 14, intel: 8, wis: 10, cha: 10),
        weaponId: "longsword", twoHands: false, shield: true, isRanged: false,
        spawnOffset: new Vector3(-3, 0, 0)
    );

    public static readonly HeroDef ClericL5 = new HeroDef(
        name: "Cleric", cls: ClassId.Cleric, level: 5,
        hp: 36, ac: 17,
        ab: (str: 14, dex: 10, con: 14, intel: 10, wis: 16, cha: 12),
        weaponId: "warhammer", twoHands: false, shield: true, isRanged: false,
        spawnOffset: new Vector3(-3, 0, 2)
    );

    public static readonly HeroDef RogueL5 = new HeroDef(
        name: "Rogue", cls: ClassId.Rogue, level: 5,
        hp: 34, ac: 15,
        ab: (str: 10, dex: 18, con: 12, intel: 12, wis: 10, cha: 14),
        weaponId: "rapier", twoHands: false, shield: false, isRanged: false,
        spawnOffset: new Vector3(-3, 0, -2)
    );

    public static readonly HeroDef SorcererL5 = new HeroDef(
        name: "Sorcerer", cls: ClassId.Sorcerer, level: 5,
        hp: 30, ac: 13,
        ab: (str: 8, dex: 14, con: 14, intel: 10, wis: 10, cha: 18),
        weaponId: "dagger", twoHands: false, shield: false, isRanged: true,
        spawnOffset: new Vector3(-3, 0, 4)
    );

    public static readonly HeroDef WarlockL5 = new HeroDef(
    name: "Warlock", cls: ClassId.Warlock, level: 5,
    hp: 30, ac: 13,
    ab: (str: 8, dex: 16, con: 15, intel: 18, wis: 10, cha: 18),
    weaponId: "dagger", twoHands: false, shield: false, isRanged: true,
    spawnOffset: new Vector3(-3, 0, -4)
);

    // Choose your party loadout here
    public static List<HeroDef> DefaultParty => new List<HeroDef>
    {
        WarriorL5, ClericL5, RogueL5, SorcererL5, WarlockL5
    };
}
