#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(HeroPicker))]
public class HeroPickerEditor : Editor
{
    string[] ids;
    int currentIndex;

    void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        ids = HeroesDB.Parties.Keys.ToArray();
        var picker = (HeroPicker)target;
        currentIndex = Mathf.Max(0, System.Array.IndexOf(ids, picker.selectedPartyId));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var picker = (HeroPicker)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("battleGame"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("applyOnPlay"));

        if (ids == null || ids.Length == 0)
        {
            EditorGUILayout.HelpBox("No parties found in HeroesDB.Parties.", MessageType.Warning);
            if (GUILayout.Button("Refresh")) Refresh();
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Party", EditorStyles.boldLabel);

        int newIndex = EditorGUILayout.Popup("Party", currentIndex, ids);
        if (newIndex != currentIndex)
        {
            currentIndex = newIndex;
            picker.selectedPartyId = ids[currentIndex];
            EditorUtility.SetDirty(picker);
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Details", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("ID", picker.selectedPartyId);
        var list = HeroesDB.GetParty(picker.selectedPartyId);
        EditorGUILayout.LabelField("Members", (list != null ? list.Count : 0).ToString());

        if (list != null)
        {
            foreach (var h in list)
                EditorGUILayout.LabelField($"• {h.name} (Lvl {h.level} {h.cls}) AC {h.armourClass}, HP {h.maxHP}");
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Apply to BattleGame now"))
        {
            if (picker.battleGame != null)
            {
                var field = typeof(BattleGame).GetField("partyId",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(picker.battleGame, picker.selectedPartyId);
                    EditorUtility.SetDirty(picker.battleGame);
                    Debug.Log($"HeroPicker: set BattleGame.partyId = {picker.selectedPartyId}");
                }
                else
                {
                    Debug.LogWarning("HeroPicker: couldn't find BattleGame.partyId.");
                }
            }
            else
            {
                Debug.LogWarning("Assign a BattleGame reference first.");
            }
        }

        if (GUILayout.Button("Refresh Parties")) Refresh();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
