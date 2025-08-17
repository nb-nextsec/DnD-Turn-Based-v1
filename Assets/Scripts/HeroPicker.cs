using UnityEngine;

[DisallowMultipleComponent]
public class HeroPicker : MonoBehaviour
{
    [Tooltip("BattleGame in the scene that spawns units.")]
    public BattleGame battleGame;

    [Tooltip("Party id chosen from the editor popup (see custom inspector).")]
    public string selectedPartyId = "default-party";

    [Tooltip("When checked, sets BattleGame.partyId on Play.")]
    public bool applyOnPlay = true;

    void Reset()
    {
        if (battleGame == null) battleGame = FindFirstObjectByType<BattleGame>();
    }

    void Awake()
    {
        if (applyOnPlay && battleGame != null)
        {
            var field = typeof(BattleGame).GetField("partyId",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(battleGame, selectedPartyId);
            }
            else
            {
                Debug.LogWarning("HeroPicker: couldn't find BattleGame.partyId.");
            }
        }
    }
}
