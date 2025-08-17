using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitUI : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] Vector3 worldOffset = new Vector3(0, 1.70f, 0);
    [SerializeField] float canvasScale = 0.015f;        // overall size in world units
    [SerializeField] Vector2 barSize = new Vector2(160, 18); // width x height of bar
    [SerializeField] int fontSize = 22;                 // TMP font size

    [Header("Runtime refs (auto)")]
    public Canvas canvas;
    public Slider hpSlider;
    public TextMeshProUGUI labelTMP;

    Unit unit;
    Camera cam;

    void Awake()
    {
        unit = GetComponentInParent<Unit>();
        cam = Camera.main;

        // World-space canvas
        var goCanvas = new GameObject("UnitUI_Canvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        goCanvas.transform.SetParent(transform, false);

        canvas = goCanvas.GetComponent<Canvas>();
        var scaler = goCanvas.GetComponent<CanvasScaler>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = cam;
        scaler.dynamicPixelsPerUnit = 10;

        var rtCanvas = goCanvas.GetComponent<RectTransform>();
        rtCanvas.sizeDelta = new Vector2(barSize.x, Mathf.Max(barSize.y + 6f, 22f));
        rtCanvas.localPosition = worldOffset;
        rtCanvas.localScale = Vector3.one * canvasScale;

        // Slider background
        var goSlider = new GameObject("HP_Slider",
            typeof(RectTransform), typeof(Slider), typeof(Image));
        goSlider.transform.SetParent(goCanvas.transform, false);

        var rtSlider = goSlider.GetComponent<RectTransform>();
        rtSlider.sizeDelta = barSize;

        var bg = goSlider.GetComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.6f);
        bg.raycastTarget = false;

        // Fill
        var fillGO = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fillGO.transform.SetParent(goSlider.transform, false);

        var rtFill = fillGO.GetComponent<RectTransform>();
        rtFill.anchorMin = Vector2.zero;
        rtFill.anchorMax = Vector2.one;
        rtFill.offsetMin = new Vector2(2, 2);
        rtFill.offsetMax = new Vector2(-2, -2);

        var fillImg = fillGO.GetComponent<Image>();
        fillImg.color = Color.green;
        fillImg.raycastTarget = false;

        hpSlider = goSlider.GetComponent<Slider>();
        hpSlider.fillRect = rtFill;
        hpSlider.targetGraphic = fillImg;
        hpSlider.minValue = 0f;
        hpSlider.maxValue = 1f;
        hpSlider.value = 1f;

        // TMP label on top of bar
        var goText = new GameObject("LabelTMP",
            typeof(RectTransform), typeof(TextMeshProUGUI));
        goText.transform.SetParent(goSlider.transform, false);

        var rtText = goText.GetComponent<RectTransform>();
        rtText.anchorMin = Vector2.zero;
        rtText.anchorMax = Vector2.one;
        rtText.offsetMin = Vector2.zero;
        rtText.offsetMax = Vector2.zero;

        labelTMP = goText.GetComponent<TextMeshProUGUI>();
        labelTMP.alignment = TextAlignmentOptions.Midline;
        labelTMP.fontSize = fontSize;
        labelTMP.enableAutoSizing = false;
        labelTMP.color = Color.white;
        labelTMP.raycastTarget = false;
        labelTMP.margin = Vector4.zero;

        // Outline for readability
        labelTMP.enableWordWrapping = false;
        labelTMP.fontMaterial.EnableKeyword("OUTLINE_ON");
        labelTMP.outlineWidth = 0.2f;
        labelTMP.outlineColor = new Color32(0, 0, 0, 180);
    }

    void Start() { Refresh(); }

    void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        if (cam != null)
            canvas.transform.rotation =
                Quaternion.LookRotation(canvas.transform.position - cam.transform.position);

        canvas.transform.localPosition = worldOffset;

        // Green → Red as HP drops
        if (hpSlider != null && hpSlider.fillRect != null)
        {
            var fill = hpSlider.fillRect.GetComponent<Image>();
            fill.color = Color.Lerp(Color.red, Color.green, hpSlider.value);
        }
    }

    public void SetHP(float pct)
    {
        if (hpSlider == null) return;
        hpSlider.value = Mathf.Clamp01(pct);
        RefreshLabelOnly();
    }

    public void Refresh()
    {
        if (unit == null) unit = GetComponentInParent<Unit>();
        if (hpSlider != null)
            hpSlider.value = (unit != null) ? unit.HealthPct() : 1f;
        RefreshLabelOnly();
    }

    void RefreshLabelOnly()
    {
        if (labelTMP == null || unit == null) return;
        labelTMP.text = $"{unit.baseStats.displayName}  {unit.currentHP}/{unit.baseStats.maxHP}";
    }
}
