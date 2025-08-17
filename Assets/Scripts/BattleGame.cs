using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurnManager))]
public class BattleGame : MonoBehaviour
{
    [SerializeField] private TurnManager turnManager;


    [Header("Encounter Selection")]
    [SerializeField] private string encounterId = "undead-army-1";


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

        // Ensure a CombatLog exists (defensive)
        if (FindFirstObjectByType<CombatLog>() == null)
            new GameObject("CombatLog", typeof(CombatLog));

        var spawned = new List<Unit>();

        // 1) Spawn heroes from HeroesDB
        foreach (var hero in HeroesDB.DefaultParty)
        {
            var u = SpawnUnit(
                prefab: playerPrefab,
                pos: hero.spawnOffset,
                name: hero.name,
                hp: hero.maxHP,
                ac: hero.armourClass,
                dmg: (1, 6), dmgBonus: 0, // will be overridden by EquipWeapon
                canHeal: false, actions: 1,
                heal: null, healThresh: 0.4f,
                isRanged: hero.isRanged,
                abilities: (hero.str, hero.dex, hero.con, hero.intel, hero.wis, hero.cha),
                cls: hero.cls
            );
            // Level/proficiency
            u.baseStats.proficiencyBonus = SpellRules.ProficiencyForLevel(hero.level);
            // Load class spells then cap by level
            PostSpawnClassSetup(u);
            u.baseStats.knownSpells = SpellRules.TrimToCaps(u.baseStats.classId, hero.level, u.baseStats.knownSpells ?? new List<KnownSpell>());
            // Weapon/armour
            EquipWeapon(u, hero.weaponId, hero.twoHands, hero.shield);
            spawned.Add(u);
        }

        // 2) Spawn encounter by ID
        var encId = string.IsNullOrEmpty(encounterId) ? "undead-army-1" : encounterId;
        if (!EncounterDB.All.TryGetValue(encId, out var enc))
        {
            Debug.LogError($"Encounter '{encId}' not found.");
            return;
        }

        foreach (var e in enc.enemies)
        {
            var u = SpawnUnit(
                prefab: enemyPrefab,
                pos: e.spawnOffset,
                name: e.name,
                hp: e.maxHP,
                ac: e.armourClass,
                dmg: (1, 6), dmgBonus: 0,
                canHeal: e.canHeal, actions: 1,
                heal: e.healDice, healThresh: e.healThreshold,
                isRanged: e.isRanged,
                abilities: (e.str, e.dex, e.con, e.intel, e.wis, e.cha),
                cls: e.cls
            );
            u.baseStats.proficiencyBonus = SpellRules.ProficiencyForLevel(e.level);
            PostSpawnClassSetup(u);
            u.baseStats.knownSpells = SpellRules.TrimToCaps(u.baseStats.classId, e.level, u.baseStats.knownSpells ?? new List<KnownSpell>());
            EquipWeapon(u, e.weaponId, e.twoHands, e.shield);
            spawned.Add(u);
        }

        // 3) Init + start
        foreach (var u in spawned) u.Init();
        turnManager.RegisterUnits(spawned);
        CombatLog.Print($"Battle start: Loaded '{enc.title}'.");
        turnManager.StartBattle();
    }

    // ---------- Helpers ----------

    Unit SpawnUnit(
        GameObject prefab, Vector3 pos, string name, int hp, int ac,
        (int, int) dmg, int dmgBonus, bool canHeal, int actions,
        (int, int)? heal = null, float healThresh = 0.4f,
        bool isRanged = false,
        (int str, int dex, int con, int intel, int wis, int cha)? abilities = null,
        ClassId cls = ClassId.Fighter)
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
            usingTwoHands = false,

            classId = cls
        };

        unit.isPlayerControlled = (prefab == playerPrefab);

        // Create UnitUI BEFORE Init so labels/bars are ready
        var uiRoot = new GameObject("UI", typeof(UnitUI));
        uiRoot.transform.SetParent(unit.transform, false);

        return unit;
    }

    // Load class spellbook & casting ability after basic stats are set (and after weapon equip).
    void PostSpawnClassSetup(Unit unit)
    {
        // Load per-class spell list
        var list = Spellbooks.For(unit.baseStats.classId);
        unit.baseStats.knownSpells = new List<KnownSpell>(list);
        unit.baseStats.isCaster = unit.baseStats.knownSpells.Count > 0;

        // Choose casting ability by class
        switch (unit.baseStats.classId)
        {
            case ClassId.Cleric:
            case ClassId.Druid:
            case ClassId.Paladin:
                unit.baseStats.castingAbility = CastingAbility.WIS; break;
            case ClassId.Bard:
            case ClassId.Sorcerer:
            case ClassId.Warlock:
                unit.baseStats.castingAbility = CastingAbility.CHA; break;
            case ClassId.Wizard:
            case ClassId.Artificer:
            case ClassId.Rogue:   // Arcane Trickster
            case ClassId.Fighter: // Eldritch Knight
                unit.baseStats.castingAbility = CastingAbility.INT; break;
            default:
                unit.baseStats.castingAbility = CastingAbility.None; break;
        }
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

        // Store weapon name shown in logs/labels (WeaponDB already sets weaponId & name if you added that)
        s.weaponId = weaponId;
        if (WeaponDB.All.TryGetValue(weaponId, out var w)) s.weaponName = w.name;
    }
}
