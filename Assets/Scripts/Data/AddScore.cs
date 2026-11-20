using UnityEngine;
using UnityEngine.SceneManagement;

public class AddScore : MonoBehaviour
{
    public void AddScores()
    {
        GameData.AddOneToTotalScore(1);
        Debug.Log("Total Score: " + GameData.totalScore);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
