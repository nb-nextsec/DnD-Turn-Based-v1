using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatLog : MonoBehaviour
{
    public static CombatLog Instance;

    TextMeshProUGUI text;
    string buffer = "";
    const int MaxChars = 12000;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        Print("Combat log ready.");
    }

    void BuildUI()
    {
        // Canvas
        var canvasGO = new GameObject("CombatLogCanvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.transform.SetParent(transform, false);

        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5000; // ensure on top

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Panel
        var panelGO = new GameObject("Panel", typeof(Image));
        panelGO.transform.SetParent(canvasGO.transform, false);
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0f, 0f);
        panelRT.anchorMax = new Vector2(0f, 0f);
        panelRT.pivot = new Vector2(0f, 0f);
        panelRT.anchoredPosition = new Vector2(14f, 14f);
        panelRT.sizeDelta = new Vector2(520f, 280f);
        var panelImg = panelGO.GetComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.55f);

        // ScrollRect + Viewport
        var scrollGO = new GameObject("ScrollRect",
            typeof(RectTransform), typeof(ScrollRect), typeof(Image), typeof(Mask));
        scrollGO.transform.SetParent(panelGO.transform, false);
        var scrollRT = scrollGO.GetComponent<RectTransform>();
        scrollRT.anchorMin = new Vector2(0f, 0f);
        scrollRT.anchorMax = new Vector2(1f, 1f);
        scrollRT.offsetMin = new Vector2(8f, 8f);
        scrollRT.offsetMax = new Vector2(-8f, -8f);

        var scrollImg = scrollGO.GetComponent<Image>();
        scrollImg.color = new Color(0f, 0f, 0f, 0.25f);
        var mask = scrollGO.GetComponent<Mask>();
        mask.showMaskGraphic = false;

        // Content holder
        var contentGO = new GameObject("Content", typeof(RectTransform));
        contentGO.transform.SetParent(scrollGO.transform, false);
        var contentRT = contentGO.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0f, 0f);
        contentRT.anchorMax = new Vector2(1f, 1f);
        contentRT.pivot = new Vector2(0f, 1f);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0f, 0f);

        // TMP Text
        var textGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(contentGO.transform, false);
        var textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0f, 1f);
        textRT.anchorMax = new Vector2(1f, 1f);
        textRT.pivot = new Vector2(0f, 1f);
        textRT.anchoredPosition = new Vector2(0f, 0f);
        textRT.sizeDelta = new Vector2(0f, 0f);

        text = textGO.GetComponent<TextMeshProUGUI>();
        text.enableWordWrapping = true;
        text.fontSize = 22;
        text.color = Color.white;
        text.text = "";

        // Wire scrollrect
        var scroll = scrollGO.GetComponent<ScrollRect>();
        scroll.viewport = scrollRT;
        scroll.content = textRT;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
    }

    public static void Print(string msg)
    {
        if (Instance == null)
            new GameObject("CombatLog", typeof(CombatLog));
        Instance._Print(msg);
    }

    void _Print(string msg)
    {
        buffer += msg + "\n";
        if (buffer.Length > MaxChars)
            buffer = buffer.Substring(buffer.Length - MaxChars);

        if (text != null)
        {
            text.text = buffer;
            // Resize content to fit text height so ScrollRect knows how far to scroll
            text.rectTransform.sizeDelta = new Vector2(0f, text.preferredHeight);
            // Auto-scroll to bottom
            var sr = text.GetComponentInParent<ScrollRect>();
            if (sr != null) sr.verticalNormalizedPosition = 0f;
        }
    }

    // Optional toggle with 'L'
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var root = transform.GetChild(0).gameObject;
            root.SetActive(!root.activeSelf);
        }
    }
}
