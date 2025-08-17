using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class TurnManager : MonoBehaviour
{
    public List<Unit> allUnits = new List<Unit>();

    // ⏱ Adjustable delays in seconds
    [Header("Action Delays (seconds)")]
    public float moveDelay = 0.30f;
    public float attackDelay = 0.60f;
    public float spellDelay = 0.60f;   // separate in case you want it different to attack
    public float healDelay = 0.50f;
    public float waitDelay = 0.20f;

    private Queue<Unit> roundOrder = new Queue<Unit>();
    private bool roundActive = false;

    public void RegisterUnits(IEnumerable<Unit> units)
    {
        allUnits = units?.Where(u => u != null).ToList() ?? new List<Unit>();
    }

    public void StartBattle()
    {
        BuildInitiative();
        roundActive = true;
        StartCoroutine(RunLoop());
    }

    void BuildInitiative()
    {
        var rolled = allUnits
            .Select(u => new { unit = u, init = CombatResolver.RollInitiative(u.baseStats.initiativeBonus) })
            .OrderByDescending(x => x.init)
            .ToList();

        roundOrder.Clear();
        foreach (var r in rolled)
        {
            roundOrder.Enqueue(r.unit);
            Debug.Log($"Initiative: {r.unit.name} = {r.init}");
            CombatLog.Print($"Initiative: {r.unit.name} = {r.init}");
        }
    }

    System.Collections.IEnumerator RunLoop()
    {
        while (roundActive)
        {
            if (BattleOver()) yield break;

            // New round: refresh actions for everyone
            foreach (var u in allUnits) u.BeginTurn();

            int count = roundOrder.Count;
            for (int i = 0; i < count; i++)
            {
                var current = roundOrder.Dequeue();
                if (!current.isAlive)
                    continue;

                Debug.Log($"\n== {current.name}'s turn ==");
                CombatLog.Print($"== {current.name}'s turn ==");

                while (current.isAlive && current.remainingActions > 0)
                {
                    var actionType = DoUnitAction(current);
                    yield return new WaitForSeconds(GetDelay(actionType));

                    if (BattleOver()) yield break;
                }

                roundOrder.Enqueue(current);
                if (BattleOver()) yield break;
            }
        }
    }

    AIActionType DoUnitAction(Unit current)
    {
        // Split allies/enemies from perspective of current
        var allies = allUnits.Where(u => u.isPlayerControlled == current.isPlayerControlled && u.isAlive).ToArray();
        var enemies = allUnits.Where(u => u.isPlayerControlled != current.isPlayerControlled && u.isAlive).ToArray();

        // SimpleAI.TakeAction should both EXECUTE the action and RETURN which action type it took.
        if (current.isPlayerControlled)
            return SimpleAI.TakeAction(current, allies, enemies);
        else
            return SimpleAI.TakeAction(current, allies, enemies);
    }

    float GetDelay(AIActionType t)
    {
        switch (t)
        {
            case AIActionType.Move: return Mathf.Max(0f, moveDelay);
            case AIActionType.Attack: return Mathf.Max(0f, attackDelay);
            case AIActionType.CastSpell: return Mathf.Max(0f, spellDelay);
            case AIActionType.Heal: return Mathf.Max(0f, healDelay);
            case AIActionType.Wait:
            default: return Mathf.Max(0f, waitDelay);
        }
    }

    bool BattleOver()
    {
        bool playersAlive = allUnits.Any(u => u.isPlayerControlled && u.isAlive);
        bool enemiesAlive = allUnits.Any(u => !u.isPlayerControlled && u.isAlive);

        if (!playersAlive || !enemiesAlive)
        {
            roundActive = false;
            string winner = playersAlive ? "Players" : "Enemies";
            Debug.Log($"\n*** {winner} win! ***");
            CombatLog.Print($"*** {winner} win! ***");
            return true;
        }
        return false;
    }
}
