using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Sprite untuk skor 0-3")]
    public GameObject levelLine;
    public GameObject sprite1;
    public GameObject sprite2;
    public GameObject sprite3;

    [Header("Nama Level (harus sama dengan nama Scene)")]
    public string levelName;
    public int requiredScore = 0;     // minimal score untuk unlock
    private Button button;
    private int score = 0;

    void Awake()
    {
        levelLine.SetActive(false);
        button = GetComponent<Button>();
    }

    void Start()
    {
        // cek apakah level sudah terbuka
        if (GameData.totalScore >= requiredScore)
        {
            levelLine.SetActive(true);
            UpdateScoreUI();
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    public void UpdateScoreUI()
    {
        // Ambil skor terbaik untuk level ini
        if (GameData.bestScorePerLevel.ContainsKey(levelName))
            score = GameData.bestScorePerLevel[levelName];

        // Matikan semua dulu
        sprite1.SetActive(false);
        sprite2.SetActive(false);
        sprite3.SetActive(false);

        // Aktifkan sesuai score
        switch (score)
        {
            case 1: sprite1.SetActive(true); break;
            case 2: sprite2.SetActive(true); break;
            case 3: sprite3.SetActive(true); break;
        }
    }
}
