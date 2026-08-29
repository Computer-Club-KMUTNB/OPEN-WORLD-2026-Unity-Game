using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class SliderValueDisplay : MonoBehaviour
{
    public enum ValueFormat
    {
        Percentage,   // 0.0 - 1.0 -> 0% - 100%
        Multiplier,   // 1.0 -> 1.0x
        Decimal,      // 0.85
        Integer       // 85
    }

    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private ValueFormat format = ValueFormat.Percentage;
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider != null)
        {
            slider.onValueChanged.AddListener(UpdateDisplay);
            UpdateDisplay(slider.value);
        }
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(UpdateDisplay);
        }
    }

    public void UpdateDisplay(float value)
    {
        if (targetText == null) return;

        string formattedVal = "";
        switch (format)
        {
            case ValueFormat.Percentage:
                formattedVal = Mathf.RoundToInt(value * 100f) + "%";
                break;
            case ValueFormat.Multiplier:
                formattedVal = value.ToString("F1") + "x";
                break;
            case ValueFormat.Decimal:
                formattedVal = value.ToString("F2");
                break;
            case ValueFormat.Integer:
                formattedVal = Mathf.RoundToInt(value).ToString();
                break;
        }

        targetText.text = prefix + formattedVal + suffix;
    }
}
