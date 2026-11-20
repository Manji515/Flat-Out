using System.Linq;
using UnityEngine;
using TMPro;

public class ClassicMode : MonoBehaviour
{
    [Header("References")]
    public RaceManager raceManager;
    public TextMeshProUGUI[] positionText;

    private Finishing finishing;
    private int playerRank;
    private bool resultSaved = false;

    void Start()
    {
        finishing = GetComponent<Finishing>();
    }

    void Update()
    {
        // Jika cinematic selesai → simpan posisi player sekali saja
        if (finishing.finished)
        {
            SaveFinalPlayerRank();
            return;
        }

        if (raceManager.playerCar != null)
            UpdateRacePosition();
    }

    void UpdateRacePosition()
    {
        // Ambil semua mobil dan urutkan berdasarkan X terbesar → terkecil
        var sortedCars = raceManager.carList
            .OrderByDescending(c => c.transform.position.x)
            .ToList();

        // Ranking player = index dalam list +1
        playerRank = sortedCars.FindIndex(c => c == raceManager.playerCar) + 1;

        foreach (var t in positionText)
            t.text = $"{playerRank}<voffset=0.4em><size=40%>/{raceManager.carList.Count}</voffset></size>";
    }

    void SaveFinalPlayerRank()
    {
        if (resultSaved) return; // Pastikan hanya tersimpan sekali
        resultSaved = true;

        // Hitung skor berdasar posisi
        int point = (playerRank == 1) ? 3 :
                    (playerRank == 2) ? 2 :
                    (playerRank == 3) ? 1 : 0;

        string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GameData.SaveLevelScore(levelName, point);

        Debug.Log($"Player finish {playerRank}, point = {point}");

        // Tampilkan UI finish
        finishing.ShowFinishUI(playerRank);
    }
}
