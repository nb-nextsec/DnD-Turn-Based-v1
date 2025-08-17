#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(EncounterPicker))]
public class EncounterPickerEditor : Editor
{
    string[] ids;
    string[] titles;
    int currentIndex;

    void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        // Pull ids / titles from your EncounterDB
        var all = EncounterDB.All;
        ids = all.Keys.ToArray();
        titles = ids.Select(id => EncounterDB.All[id].title).ToArray();

        var picker = (EncounterPicker)target;
        currentIndex = Mathf.Max(0, System.Array.IndexOf(ids, picker.selectedEncounterId));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var picker = (EncounterPicker)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("battleGame"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("applyOnPlay"));

        if (ids == null || ids.Length == 0)
        {
            EditorGUILayout.HelpBox("No encounters found in EncounterDB.All.", MessageType.Warning);
            if (GUILayout.Button("Refresh")) Refresh();
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Select Encounter", EditorStyles.boldLabel);

        // Nice popup showing titles, but stores the id
        int newIndex = EditorGUILayout.Popup("Encounter", currentIndex, titles);
        if (newIndex != currentIndex)
        {
            currentIndex = newIndex;
            picker.selectedEncounterId = ids[currentIndex];
            EditorUtility.SetDirty(picker);
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Details", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("ID", picker.selectedEncounterId);
        EditorGUILayout.LabelField("Title", EncounterDB.All[picker.selectedEncounterId].title);
        EditorGUILayout.LabelField("Enemies", EncounterDB.All[picker.selectedEncounterId].enemies.Count.ToString());

        EditorGUILayout.Space();
        if (GUILayout.Button("Apply to BattleGame now"))
        {
            if (picker.battleGame != null)
            {
                var field = typeof(BattleGame).GetField("encounterId",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(picker.battleGame, picker.selectedEncounterId);
                    EditorUtility.SetDirty(picker.battleGame);
                    Debug.Log($"EncounterPicker: set BattleGame.encounterId = {picker.selectedEncounterId}");
                }
                else
                {
                    Debug.LogWarning("EncounterPicker: couldn't find BattleGame.encounterId (did you rename it?).");
                }
            }
            else
            {
                Debug.LogWarning("Assign a BattleGame reference first.");
            }
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh Encounters"))
        {
            Refresh();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
