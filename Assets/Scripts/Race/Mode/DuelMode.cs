using UnityEngine;
using TMPro;

public class DuelMode : MonoBehaviour
{
    [Header("References")]
    private RaceManager raceManager;
    public Transform playerCar;
    public Transform enemyCar;
    public TextMeshProUGUI[] positionText;

    private Finishing finishCinematic;
    private bool resultSaved = false;
    private int playerRank = 1; // default

    void Start()
    {
        finishCinematic = GetComponent<Finishing>();
        raceManager = GetComponent<RaceManager>();
    }

    void Update()
    {
        if (finishCinematic.finished)
        {
            SaveFinalPlayerRank();
            return;
        }

        if (playerCar != null && enemyCar != null)
            UpdateRacePosition();
    }

    void UpdateRacePosition()
    {
        // Player lebih maju jika X lebih besar
        bool playerAhead = playerCar.position.x > enemyCar.position.x;

        playerRank = playerAhead ? 1 : 2;

        // Tampilkan "1/2" atau "2/2"
        foreach (var t in positionText)
        t.text = $"{playerRank}<voffset=0.4em><size=40%>/2</voffset></size>";
    }

    void SaveFinalPlayerRank()
    {
        if (resultSaved) return;
        resultSaved = true;

        // Menang = 3, Kalah = 0
        int point = (playerRank == 1) ? 3 : 0;

        string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GameData.SaveLevelScore(levelName, point);

        Debug.Log($"Duel result: player rank {playerRank}, point = {point}");

        finishCinematic.ShowFinishUI(playerRank);
    }
}
