using UnityEngine;
using UnityEngine.UI;

public static class UnitUIExtensions
{
    public static void SpawnFloatingText(this UnitUI ui, string msg, Color colour)
    {
        if (ui == null || ui.canvas == null) return;

        // tiny overlay canvas for each floating text (so it doesn't clip inside slider)
        var c = new GameObject("FloatCanvas", typeof(RectTransform), typeof(Canvas));
        c.transform.SetParent(ui.canvas.transform, false);
        var canvas = c.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        var rtC = c.GetComponent<RectTransform>();
        rtC.sizeDelta = new Vector2(60, 20);
        rtC.localScale = Vector3.one * 0.01f;
        rtC.localPosition = new Vector3(Random.Range(-0.05f, 0.05f), 0.02f, 0);

        var go = new GameObject("FloatText", typeof(RectTransform), typeof(Text), typeof(FloatingText));
        go.transform.SetParent(c.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(60, 20);

        var t = go.GetComponent<Text>();
        t.text = msg;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = 22;
        t.color = colour;
    }
}
