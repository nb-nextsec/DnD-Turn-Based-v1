using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurnManager))]
public class BattleGame : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    void Awake()
    {
        if (turnManager == null) turnManager = GetComponent<TurnManager>();

        // Ensure a CombatLog exists
        if (FindFirstObjectByType<CombatLog>() == null)
            new GameObject("CombatLog", typeof(CombatLog));
    }

    void Start()
    {
        if (playerPrefab == null || enemyPrefab == null)
        {
            Debug.LogError("Assign Player Prefab and Enemy Prefab on BattleGame.");
            return;
        }

        var spawned = new List<Unit>();

        // ---------- PLAYERS ----------
        var warrior = SpawnUnit(
            prefab: playerPrefab, pos: new Vector3(-3, 0, 0), name: "Warrior",
            hp: 36, ac: 16, dmg: (1, 8), dmgBonus: 0,
            canHeal: false, actions: 1,
            isRanged: false,
            abilities: (str: 16, dex: 12, con: 14, intel: 8, wis: 10, cha: 10)
        );
        // Warrior: Longsword + Shield (melee)
        EquipWeapon(warrior, "longsword", twoHands: false, shield: true);
        spawned.Add(warrior);

        var cleric = SpawnUnit(
            prefab: playerPrefab, pos: new Vector3(-3, 0, 2), name: "Cleric",
            hp: 28, ac: 15, dmg: (1, 6), dmgBonus: 0,
            canHeal: true, actions: 1, heal: (1, 8), healThresh: 0.45f,
            isRanged: false,
            abilities: (str: 14, dex: 10, con: 14, intel: 10, wis: 14, cha: 12)
        );
        // Cleric is a caster (WIS). Weapon 50/50 Warhammer or Morningstar + Shield.
        cleric.baseStats.isCaster = true;
        cleric.baseStats.castingAbility = CastingAbility.WIS;
        if (Random.value < 0.5f) EquipWeapon(cleric, "warhammer", twoHands: false, shield: true);
        else EquipWeapon(cleric, "morningstar", twoHands: false, shield: true);
        spawned.Add(cleric);

        var rogue = SpawnUnit(
            prefab: playerPrefab, pos: new Vector3(-3, 0, -2), name: "Rogue",
            hp: 24, ac: 14, dmg: (1, 8), dmgBonus: 0,
            canHeal: false, actions: 1,
            isRanged: false,
            abilities: (str: 10, dex: 16, con: 12, intel: 12, wis: 10, cha: 14)
        );
        // Rogue: 50/50 Rapier (finesse melee) or Shortbow (ranged)
        if (Random.value < 0.5f) EquipWeapon(rogue, "rapier");
        else EquipWeapon(rogue, "shortbow");
        spawned.Add(rogue);

        // Sorcerer: CHA caster. Dagger as fallback weapon.
        var sorcerer = SpawnUnit(
            prefab: playerPrefab, pos: new Vector3(-3, 0, 4), name: "Sorcerer",
            hp: 22, ac: 13, dmg: (1, 4), dmgBonus: 0,
            canHeal: false, actions: 1,
            isRanged: true,
            abilities: (str: 8, dex: 14, con: 14, intel: 10, wis: 10, cha: 16)
        );
        sorcerer.baseStats.isCaster = true;
        sorcerer.baseStats.castingAbility = CastingAbility.CHA;
        EquipWeapon(sorcerer, "dagger"); // backup if engaged in melee
        spawned.Add(sorcerer);

        // ---------- ENEMIES ----------
        var skelA = SpawnUnit(
            prefab: enemyPrefab, pos: new Vector3(3, 0, 0), name: "Skeleton A",
            hp: 18, ac: 12, dmg: (1, 6), dmgBonus: 0,
            canHeal: false, actions: 1,
            isRanged: false,
            abilities: (str: 12, dex: 12, con: 10, intel: 6, wis: 8, cha: 5)
        );
        EquipWeapon(skelA, "shortsword");
        spawned.Add(skelA);

        var skelB = SpawnUnit(
            prefab: enemyPrefab, pos: new Vector3(3, 0, 2), name: "Skeleton B",
            hp: 18, ac: 12, dmg: (1, 6), dmgBonus: 0,
            canHeal: false, actions: 1,
            isRanged: false,
            abilities: (str: 12, dex: 12, con: 10, intel: 6, wis: 8, cha: 5)
        );
        EquipWeapon(skelB, "shortsword");
        spawned.Add(skelB);

        var necro = SpawnUnit(
            prefab: enemyPrefab, pos: new Vector3(3, 0, -2), name: "Necromancer",
            hp: 22, ac: 12, dmg: (1, 8), dmgBonus: 0,
            canHeal: true, actions: 1, heal: (1, 6), healThresh: 0.5f,
            isRanged: true,
            abilities: (str: 8, dex: 14, con: 10, intel: 16, wis: 12, cha: 12)
        );
        // Necromancer is a caster (INT). Weapon: 50/50 Dagger or Heavy Crossbow.
        necro.baseStats.isCaster = true;
        necro.baseStats.castingAbility = CastingAbility.INT;
        if (Random.value < 0.5f) EquipWeapon(necro, "dagger");
        else EquipWeapon(necro, "heavy-xbow");
        spawned.Add(necro);

        // ---------- Init + Start ----------
        foreach (var u in spawned) u.Init();

        turnManager.RegisterUnits(spawned);
        CombatLog.Print("Battle start.");
        turnManager.StartBattle();
    }

    // ---------- Helpers ----------

    Unit SpawnUnit(
        GameObject prefab, Vector3 pos, string name, int hp, int ac,
        (int, int) dmg, int dmgBonus, bool canHeal, int actions,
        (int, int)? heal = null, float healThresh = 0.4f,
        bool isRanged = false,
        (int str, int dex, int con, int intel, int wis, int cha)? abilities = null)
    {
        var go = Instantiate(prefab, pos, Quaternion.identity);
        var unit = go.GetComponent<Unit>() ?? go.AddComponent<Unit>();

        var a = abilities ?? (10, 10, 10, 10, 10, 10);

        unit.baseStats = new Stats
        {
            displayName = name,
            maxHP = hp,
            armourClass = ac,

            damage = new Vector2Int(dmg.Item1, dmg.Item2), // overridden by EquipWeapon
            damageBonus = dmgBonus,                        // set from ability mod below

            attackRange = isRanged ? 6.5f : 2.2f,
            actionsPerTurn = actions,
            initiativeBonus = 2,

            canHeal = canHeal,
            healAmount = new Vector2Int(heal?.Item1 ?? 1, heal?.Item2 ?? 6),
            healThreshold = healThresh,

            movePerAction = 2.5f,
            desiredRange = 2.0f,

            str = a.str,
            dex = a.dex,
            con = a.con,
            @int = a.intel,
            wis = a.wis,
            cha = a.cha,
            isRanged = isRanged,
            proficiencyBonus = 2,
            finesse = false,
            usingTwoHands = false
        };

        unit.isPlayerControlled = (prefab == playerPrefab);

        // Create UnitUI BEFORE Init so labels/bars are ready
        var uiRoot = new GameObject("UI", typeof(UnitUI));
        uiRoot.transform.SetParent(unit.transform, false);

        return unit;
    }

    // Equip a weapon and apply AC/damage bonus rules
    void EquipWeapon(Unit unit, string weaponId, bool twoHands = false, bool shield = false)
    {
        var s = unit.baseStats;

        // If caller asked for shield, ensure we're not two-handing
        if (shield) twoHands = false;

        // Apply weapon stats (sets damage dice, isRanged, finesse, reach→range)
        WeaponDB.Equip(s, weaponId, twoHands);

        // Damage bonus from ability mod:
        int StrMod() => Mathf.FloorToInt((s.str - 10) / 2f);
        int DexMod() => Mathf.FloorToInt((s.dex - 10) / 2f);

        if (s.isRanged)
            s.damageBonus = DexMod();
        else if (s.finesse)
            s.damageBonus = Mathf.Max(StrMod(), DexMod());
        else
            s.damageBonus = StrMod();

        // Shield: simple +2 AC if melee & using a shield
        if (shield && !s.isRanged)
            s.armourClass += 2;
    }
}
