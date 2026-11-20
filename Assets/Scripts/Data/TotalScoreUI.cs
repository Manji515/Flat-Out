using UnityEngine;
using TMPro;

public class TotalScore : MonoBehaviour
{
    [Header("UI Text")]
    public TextMeshProUGUI totalScoreText;   // tampilkan total score
    public TextMeshProUGUI requiredScoreText; // tampilkan score yang dibutuhkan
    public GameObject requiredScoreBG;

    private int lastScore = -1;

    void Update()
    {
        if (GameData.totalScore != lastScore)
        {
            lastScore = GameData.totalScore;
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        int total = GameData.totalScore;

        // Update total score
        if (totalScoreText != null)
            totalScoreText.text = GameData.totalScore.ToString();

        // Cari level terkunci terdekat
        LevelManager[] levels = FindObjectsOfType<LevelManager>();
        int nextRequired = int.MaxValue;

        foreach (LevelManager lvl in levels)
        {
            if (lvl.requiredScore > total && lvl.requiredScore < nextRequired)
            {
                nextRequired = lvl.requiredScore;
            }
        }

        // Kalau masih ada level yang terkunci
        if (nextRequired != int.MaxValue)
        {
            if (requiredScoreText != null)
            {
                requiredScoreText.gameObject.SetActive(true);
                requiredScoreBG.gameObject.SetActive(true);
                requiredScoreText.text = nextRequired.ToString();
            }
        }
        else
        {
            // Semua level sudah terbuka → sembunyikan teks requirement
            if (requiredScoreText != null)
                requiredScoreText.gameObject.SetActive(false);
                requiredScoreBG.gameObject.SetActive(false);
        }
    }
}
