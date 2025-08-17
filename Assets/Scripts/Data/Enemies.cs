using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyDef
{
    public string name;
    public ClassId cls;
    public int level;

    public int maxHP;
    public int armourClass;

    public int str, dex, con, intel, wis, cha;

    public string weaponId = "shortsword";
    public bool twoHands = false;
    public bool shield = false;
    public bool isRanged = false;

    public bool canHeal = false;              // for your non-spell heal system
    public (int x, int y) healDice = (1, 6);
    public float healThreshold = 0.4f;

    public Vector3 spawnOffset;

    public EnemyDef(string name, ClassId cls, int level,
                    int hp, int ac,
                    (int str, int dex, int con, int intel, int wis, int cha) ab,
                    string weaponId, bool twoHands, bool shield, bool isRanged,
                    Vector3 spawnOffset,
                    bool canHeal = false, (int, int)? healDice = null, float healThresh = 0.4f)
    {
        this.name = name; this.cls = cls; this.level = level;
        this.maxHP = hp; this.armourClass = ac;
        this.str = ab.str; this.dex = ab.dex; this.con = ab.con;
        this.intel = ab.intel; this.wis = ab.wis; this.cha = ab.cha;
        this.weaponId = weaponId; this.twoHands = twoHands; this.shield = shield; this.isRanged = isRanged;
        this.spawnOffset = spawnOffset;
        this.canHeal = canHeal;
        this.healDice = healDice ?? (1, 6);
        this.healThreshold = healThresh;
    }
}

[System.Serializable]
public class EncounterDef
{
    public string id;     // e.g. "undead-army-1"
    public string title;  // "Encounter: Undead Army #1"
    public List<EnemyDef> enemies = new List<EnemyDef>();
}

// --- Add these new encounters into EncounterDB ---

public static class EncounterDB
{
    public static readonly EncounterDef UndeadArmy1 = new EncounterDef
    {
        id = "undead-army-1",
        title = "Encounter: Undead Army #1",
        enemies = new List<EnemyDef>
        {
            new EnemyDef(
                name:"Skeleton A", cls: ClassId.Fighter, level:3,
                hp: 18, ac: 12,
                ab:(12,12,10,6,8,5),
                weaponId:"shortsword", twoHands:false, shield:false, isRanged:false,
                spawnOffset: new Vector3(3,0,0)
            ),
            new EnemyDef(
                name:"Skeleton B", cls: ClassId.Fighter, level:3,
                hp: 18, ac: 12,
                ab:(12,12,10,6,8,5),
                weaponId:"shortsword", twoHands:false, shield:false, isRanged:false,
                spawnOffset: new Vector3(3,0,2)
            ),
            new EnemyDef(
                name:"Necromancer", cls: ClassId.Wizard, level:5,
                hp: 22, ac: 12,
                ab:(8,14,10,16,12,12),
                weaponId:"heavy-xbow", twoHands:true, shield:false, isRanged:true,
                spawnOffset: new Vector3(3,0,-2),
                canHeal:false
            ),
        }
    };

    // === New: Drow strike team (mixed melee + cleric + wizard) ===
    public static readonly EncounterDef DrowStrikeTeam1 = new EncounterDef
    {
        id = "drow-strike-team-1",
        title = "Encounter: Drow Strike Team #1",
        enemies = new List<EnemyDef>
        {
            // Frontline fighters (nimble, higher DEX, better AC)
            new EnemyDef(
                name:"Drow Fighter A", cls: ClassId.Fighter, level:6,
                hp: 45, ac: 16,
                ab:(14,18,12,12,12,12),
                weaponId:"rapier", twoHands:false, shield:false, isRanged:false,
                spawnOffset: new Vector3(3.5f,0, 1.5f)
            ),
            new EnemyDef(
                name:"Drow Fighter B", cls: ClassId.Fighter, level:6,
                hp: 45, ac: 16,
                ab:(14,18,12,12,12,12),
                weaponId:"scimitar", twoHands:false, shield:false, isRanged:false,
                spawnOffset: new Vector3(3.5f,0,-1.5f)
            ),

            // Drow Cleric (WIS caster, can heal allies)
            new EnemyDef(
                name:"Drow Cleric", cls: ClassId.Cleric, level:6,
                hp: 42, ac: 17,
                ab:(12,14,14,12,18,14),
                weaponId:"warhammer", twoHands:false, shield:true, isRanged:false,
                spawnOffset: new Vector3(5.0f,0, 0f),
                canHeal:true, healDice:(1,8), healThresh:0.50f
            ),

            // Drow Wizard (INT caster, fragile but dangerous at range)
            new EnemyDef(
                name:"Drow Wizard", cls: ClassId.Wizard, level:7,
                hp: 36, ac: 14,
                ab:(8,16,12,18,12,14),
                weaponId:"dagger", twoHands:false, shield:false, isRanged:true,
                spawnOffset: new Vector3(6.0f,0, 2.5f)
            ),

            // Drow Marksman (ranged pressure)
            new EnemyDef(
                name:"Drow Marksman", cls: ClassId.Ranger, level:6,
                hp: 44, ac: 16,
                ab:(12,18,12,12,14,12),
                weaponId:"shortbow", twoHands:true, shield:false, isRanged:true,
                spawnOffset: new Vector3(6.0f,0,-2.5f)
            ),
        }
    };

    // === New: Orc warband (brutal melee with a chieftain & shaman) ===
    public static readonly EncounterDef OrcWarband1 = new EncounterDef
    {
        id = "orc-warband-1",
        title = "Encounter: Orc Warband #1",
        enemies = new List<EnemyDef>
        {
            // Chieftain (greataxe, hits hard)
            new EnemyDef(
                name:"Orc Chieftain", cls: ClassId.Barbarian, level:7,
                hp: 72, ac: 15,
                ab:(20,14,16,8,10,12),
                weaponId:"greataxe", twoHands:true, shield:false, isRanged:false,
                spawnOffset: new Vector3(3.0f,0,0f)
            ),
            // Two brutes
            new EnemyDef(
                name:"Orc Brute A", cls: ClassId.Fighter, level:5,
                hp: 52, ac: 14,
                ab:(18,12,16,8,10,8),
                weaponId:"greataxe", twoHands:true, shield:false, isRanged:false,
                spawnOffset: new Vector3(4.5f,0,1.5f)
            ),
            new EnemyDef(
                name:"Orc Brute B", cls: ClassId.Fighter, level:5,
                hp: 52, ac: 14,
                ab:(18,12,16,8,10,8),
                weaponId:"greataxe", twoHands:true, shield:false, isRanged:false,
                spawnOffset: new Vector3(4.5f,0,-1.5f)
            ),
            // Shaman (support caster, minor heal, ranged cantrip pressure)
            new EnemyDef(
                name:"Orc Shaman", cls: ClassId.Warlock, level:6,
                hp: 40, ac: 13,
                ab:(10,12,14,12,14,16),
                weaponId:"dagger", twoHands:false, shield:false, isRanged:true,
                spawnOffset: new Vector3(6.0f,0,0f),
                canHeal:true, healDice:(1,6), healThresh:0.45f
            ),
            // Skirmisher (bow)
            new EnemyDef(
                name:"Orc Skirmisher", cls: ClassId.Ranger, level:5,
                hp: 42, ac: 14,
                ab:(14,16,12,10,12,8),
                weaponId:"shortbow", twoHands:true, shield:false, isRanged:true,
                spawnOffset: new Vector3(6.5f,0,-3.0f)
            ),
        }
    };

    // === New: Hill Giant ambush (one big threat + 5 goblins) ===
    public static readonly EncounterDef HillGiantAmbush1 = new EncounterDef
    {
        id = "hill-giant-ambush-1",
        title = "Encounter: Hill Giant Ambush #1",
        enemies = new List<EnemyDef>
        {
            // Hill Giant (massive HP, huge club)
            new EnemyDef(
                name:"Hill Giant", cls: ClassId.Barbarian, level:9,
                hp: 138, ac: 14,
                ab:(21,8,19,5,9,6),
                weaponId:"greatclub", twoHands:true, shield:false, isRanged:false,
                spawnOffset: new Vector3(4.0f,0,0f)
            ),
            // Goblins (nimble ranged harassers & one melee)
            new EnemyDef(
                name:"Goblin A", cls: ClassId.Rogue, level:3,
                hp: 18, ac: 14,
                ab:(10,16,12,10,8,8),
                weaponId:"shortbow", twoHands:true, shield:false, isRanged:true,
                spawnOffset: new Vector3(6.0f,0, 3.0f)
            ),
            new EnemyDef(
                name:"Goblin B", cls: ClassId.Rogue, level:3,
                hp: 18, ac: 14,
                ab:(10,16,12,10,8,8),
                weaponId:"shortbow", twoHands:true, shield:false, isRanged:true,
                spawnOffset: new Vector3(6.5f,0, 1.5f)
            ),
            new EnemyDef(
                name:"Goblin C", cls: ClassId.Rogue, level:3,
                hp: 18, ac: 14,
                ab:(10,16,12,10,8,8),
                weaponId:"shortbow", twoHands:true, shield:false, isRanged:true,
                spawnOffset: new Vector3(6.5f,0,-1.5f)
            ),
            new EnemyDef(
                name:"Goblin D", cls: ClassId.Rogue, level:3,
                hp: 18, ac: 14,
                ab:(10,16,12,10,8,8),
                weaponId:"shortbow", twoHands:true, shield:false, isRanged:true,
                spawnOffset: new Vector3(6.0f,0,-3.0f)
            ),
            new EnemyDef(
                name:"Goblin E", cls: ClassId.Rogue, level:3,
                hp: 20, ac: 15,
                ab:(12,16,12,10,8,8),
                weaponId:"scimitar", twoHands:false, shield:false, isRanged:false,
                spawnOffset: new Vector3(5.5f,0, 0.0f)
            ),
        }
    };

    public static readonly Dictionary<string, EncounterDef> All =
        new Dictionary<string, EncounterDef>
        {
            [UndeadArmy1.id] = UndeadArmy1,
            [DrowStrikeTeam1.id] = DrowStrikeTeam1,
            [OrcWarband1.id] = OrcWarband1,
            [HillGiantAmbush1.id] = HillGiantAmbush1,
        };
}

