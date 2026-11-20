using UnityEngine;
using TMPro;

public class HuntedMode : MonoBehaviour
{
    private bool resultSaved = false;
    [Header("Refs")]
    private RaceManager raceManager;
    private Finishing finishing;
    public Drone drone;
    public GameObject car;

    [Header("UI")]
    public TMP_Text[] penaltyText;

    [Header("Points")]
    public int penaltyPoints = 0;
    public int goldPoint = 5;
    public int silverPoint = 10;
    public int bronzePoint = 15;

    [Header("Colors")]
    public Color goldColor;
    public Color silverColor;
    public Color bronzeColor;
    public Color failColor;
    private int penaltyLimit;

    void Start()
    {
        raceManager = GetComponent<RaceManager>();
        finishing = GetComponent<Finishing>();
        drone.enabled = false;
    }

    void Update()
    {
        if (raceManager.countdown <= 0)
            drone.enabled = true;

        UpdatePenaltyVisual();

        if (finishing.finished == true) 
        {
            FinishRace();
            return;
        }
    }

    void UpdatePenaltyVisual()
    {
        (int limit, Color col) = penaltyPoints <= goldPoint ? (goldPoint, goldColor)
                                : penaltyPoints <= silverPoint ? (silverPoint, silverColor)
                                : penaltyPoints <= bronzePoint ? (bronzePoint, bronzeColor)
                                : (penaltyLimit, failColor);

        penaltyLimit = limit;

        foreach (var p in penaltyText)
        {
            p.color = col;
            p.text = $"{penaltyPoints}<voffset=0.4em><size=40%>/{penaltyLimit}</voffset></size>";
        }
    }

    void FinishRace()
    {
        if (resultSaved) return; // Pastikan hanya tersimpan sekali
        resultSaved = true;
        if (car != null) car.SetActive(false);

        int earned = penaltyPoints <= goldPoint ? 3
                 : penaltyPoints <= silverPoint ? 2
                 : penaltyPoints <= bronzePoint ? 1
                 : 0;

        int rank = earned == 3 ? 1
                : earned == 2 ? 2
                : earned == 1 ? 3
                : 4;

        string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GameData.SaveLevelScore(levelName, earned);

        finishing.ShowFinishUI(rank);
        drone.enabled = false;
        Debug.Log($"Collector Finish! Penalty: {penaltyPoints}, Earned: {earned} pts");
    }
}
