using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sensitivity : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    private float sensitivityValue = 5f;
    public TextMeshProUGUI sensitivityText;
    public float CurrentSensitivity => sensitivityValue;


    void Start()
    {
        // Ambil nilai tersimpan dari PlayerPrefs, jika belum ada gunakan default
        float savedValue = PlayerPrefs.GetFloat("Sensitivity", sensitivityValue);
        sensitivitySlider.value = savedValue;

        // Update teks awal sesuai nilai slider
        UpdateSensitivity(savedValue);

        // Pasang listener untuk update saat slider digeser
        sensitivitySlider.onValueChanged.AddListener(UpdateSensitivity);
    }

    void UpdateSensitivity(float value)
    {
        sensitivityValue = value;
        sensitivityText.text = sensitivityValue.ToString("0.##");
        PlayerPrefs.SetFloat("Sensitivity", sensitivityValue);
        PlayerPrefs.Save();
        Debug.Log("Sensitivity diset ke: " + sensitivityValue);
    }
}
