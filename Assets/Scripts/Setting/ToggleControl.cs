using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle), typeof(Image))]
public class CustomToggleHighlight : MonoBehaviour
{
    [Tooltip("Warna saat toggle aktif (ON)")]
    public Color selectedColor = new Color(0.6f, 0.9f, 1f);

    private Color normalColor;
    private Image targetImage;
    private Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        targetImage = GetComponent<Image>();

        // Ambil warna asli dari Image sebagai normalColor
        normalColor = targetImage.color;

        toggle.onValueChanged.AddListener(OnToggleChanged);

        // Set warna sesuai status awal
        UpdateColor(toggle.isOn);
    }

    void OnToggleChanged(bool isOn)
    {
        UpdateColor(isOn);
    }

    void UpdateColor(bool isOn)
    {
        targetImage.color = isOn ? selectedColor : normalColor;
    }
}
