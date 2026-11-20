using UnityEngine;
using UnityEngine.UI; // supaya bisa akses Button

public class Instruction : MonoBehaviour
{
    private RaceManager raceManager;
    public Button startButton; // tombol di panel instruksi

    void Start()
    {
        Time.timeScale = 0f;
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonPressed);
        }
    }

    void OnStartButtonPressed()
    {
        // Mulai countdown race
        if (raceManager != null)
        {
            raceManager.BeginRace();
            Time.timeScale = 1f;
        }
    }
}
