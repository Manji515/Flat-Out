using UnityEngine;
using System.Collections.Generic;

public static class GameData
{
    public static int totalScore = 0;
    public static bool isLoaded = false;
    public static Dictionary<string, int> bestScorePerLevel = new Dictionary<string, int>();
    public static List<string> allLevels = new List<string>();
    public static void InitLevels(List<string> levelNames)
    {
        allLevels = new List<string>(levelNames);
    }

    public static void LoadFromPrefs()
    {
        bestScorePerLevel.Clear();
        totalScore = 0;

        foreach (string levelName in allLevels)
        {
            int score = PlayerPrefs.GetInt("LevelScore_" + levelName, 0);
            if (score > 0)
            {
                bestScorePerLevel[levelName] = score;
                totalScore += score;
            }
        }

        PlayerPrefs.SetInt("TotalScore", totalScore);
    }

    public static void SaveLevelScore(string levelName, int newScore)
    {
        if (bestScorePerLevel.TryGetValue(levelName, out int oldScore))
        {
            if (newScore > oldScore)
            {
                totalScore -= oldScore;
                totalScore += newScore;
                bestScorePerLevel[levelName] = newScore;
            }
        }
        else
        {
            bestScorePerLevel[levelName] = newScore;
            totalScore += newScore;
        }

        SaveToPrefs();
    }

    // Simpan semua data ke PlayerPrefs
    public static void SaveToPrefs()
    {
        PlayerPrefs.SetInt("TotalScore", totalScore);

        // Simpan setiap skor level ke PlayerPrefs
        foreach (var kvp in bestScorePerLevel)
        {
            PlayerPrefs.SetInt("LevelScore_" + kvp.Key, kvp.Value);
        }

        PlayerPrefs.Save();
    }
        public static void AddOneToTotalScore(int amount)
    {
        totalScore += amount;
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();
    }

    public static void ResetAllScore(List<string> levelNames)
    {
        totalScore = 0;
        bestScorePerLevel.Clear();

        // Hapus semua level score di PlayerPrefs
        foreach (string levelName in levelNames)
        {
            PlayerPrefs.DeleteKey("LevelScore_" + levelName);
        }

        PlayerPrefs.DeleteKey("TotalScore");
        PlayerPrefs.Save();
    }

}