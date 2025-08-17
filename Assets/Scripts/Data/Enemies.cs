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

    public static readonly Dictionary<string, EncounterDef> All =
        new Dictionary<string, EncounterDef>
        {
            [UndeadArmy1.id] = UndeadArmy1
        };
}
