using UnityEngine;
using System.Collections.Generic;

public class GameLoader : MonoBehaviour
{
        void Awake()
    {
        if (GameData.isLoaded) return;

        LevelManager[] levelManagers = FindObjectsOfType<LevelManager>();
        List<string> levelNames = new List<string>();

        foreach (var lm in levelManagers)
        {
            if (!string.IsNullOrEmpty(lm.levelName) && !levelNames.Contains(lm.levelName))
            {
                levelNames.Add(lm.levelName);
            }
        }

        GameData.InitLevels(levelNames);
        GameData.LoadFromPrefs();

        GameData.isLoaded = true;
    }

}
