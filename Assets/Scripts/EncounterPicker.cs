using UnityEngine;

[DisallowMultipleComponent]
public class EncounterPicker : MonoBehaviour
{
    [Tooltip("BattleGame in the scene that actually spawns units.")]
    public BattleGame battleGame;

    [Tooltip("Encounter id chosen from the editor popup (see custom inspector).")]
    public string selectedEncounterId = "undead-army-1";

    [Tooltip("When checked, sets BattleGame.encounterId on Play.")]
    public bool applyOnPlay = true;

    void Reset()
    {
        if (battleGame == null) battleGame = FindFirstObjectByType<BattleGame>();
    }

    void Awake()
    {
        if (applyOnPlay && battleGame != null)
        {
            var field = typeof(BattleGame).GetField("encounterId",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(battleGame, selectedEncounterId);
            }
            else
            {
                Debug.LogWarning("EncounterPicker: couldn't find BattleGame.encounterId (did you rename it?).");
            }
        }
    }
}
