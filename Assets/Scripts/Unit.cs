using UnityEngine;

public class Unit : MonoBehaviour
{
    public Stats baseStats;
    public int currentHP;
    public int remainingActions;
    public bool isPlayerControlled = false;
    public bool isAlive => currentHP > 0;

    UnitUI ui;

    public void Init()
    {
        // Clamp ability scores appropriately before combat starts
        baseStats?.ClampFor(isPlayerControlled);

        currentHP = baseStats.maxHP;
        remainingActions = baseStats.actionsPerTurn;
        name = baseStats.displayName;

        ui = GetComponentInChildren<UnitUI>();
        ui?.Refresh();
    }

    public void BeginTurn() => remainingActions = baseStats.actionsPerTurn;

    public void ConsumeAction() => remainingActions = Mathf.Max(0, remainingActions - 1);

    public float HealthPct() => Mathf.Clamp01((float)currentHP / baseStats.maxHP);

    public void ReceiveDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);

        Debug.Log($"{name} takes {amount} dmg ({currentHP}/{baseStats.maxHP})");
        CombatLog.Print($"{name} takes {amount} dmg ({currentHP}/{baseStats.maxHP})");

        if (ui == null) ui = GetComponentInChildren<UnitUI>();
        if (ui != null)
        {
            ui.SetHP(HealthPct());
            ui.SpawnFloatingText($"-{amount}", Color.red);
        }

        if (currentHP == 0)
        {
            CombatLog.Print($"*** {name} has been slain! ***");
            gameObject.SetActive(false);
        }
    }

    public void ReceiveHeal(int amount)
    {
        currentHP = Mathf.Min(baseStats.maxHP, currentHP + amount);

        Debug.Log($"{name} heals {amount} ({currentHP}/{baseStats.maxHP})");
        CombatLog.Print($"{name} heals {amount} ({currentHP}/{baseStats.maxHP})");

        if (ui == null) ui = GetComponentInChildren<UnitUI>();
        if (ui != null)
        {
            ui.SetHP(HealthPct());
            ui.SpawnFloatingText($"+{amount}", new Color(0.1f, 1f, 0.1f));
        }
    }

    public void MoveTowards(Vector3 destination, float maxDistance)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, maxDistance);
        Debug.Log($"{name} moves {maxDistance:0.0}m towards target.");
        CombatLog.Print($"{name} moves {maxDistance:0.0}m towards target.");
    }
}
