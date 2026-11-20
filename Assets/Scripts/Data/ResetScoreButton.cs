using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResetScoreButton : MonoBehaviour
{
    public Button resetButton;
    void Start()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetScores);
        }
    }

    void ResetScores()
    {
        GameData.ResetAllScore(GameData.allLevels);
        Debug.Log("Semua skor berhasil direset!");
    }
}
