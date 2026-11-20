using System.Collections;
using UnityEngine;
using TMPro;

public class SurvivorMode : MonoBehaviour
{
    private RaceManager raceManager;
    private Finishing finishCinematic;
    private bool gameRunning = false;
    private float timer;
    private int rank;

    [Header("UI")]
    public TextMeshProUGUI[] eliminationTimerText;
    public TextMeshProUGUI positionText;
    public Color position;
    public Color lastPosition;

    [Header("Settings")]
    public float eliminationInterval = 10f;

    void Start()
    {
        raceManager = GetComponent<RaceManager>();
        finishCinematic = GetComponent<Finishing>();
        StartCoroutine(WaitForCountdownThenStart());
    }

    IEnumerator WaitForCountdownThenStart()
    {
        while (raceManager.countdown > 0) yield return null;
        yield return new WaitForSeconds(1f);

        timer = eliminationInterval;
        gameRunning = true;
        StartCoroutine(EliminationLoop());
    }

    void Update()
    {
        SortCars();
        UpdateRankUI();

        if (!gameRunning) return;

        timer -= Time.deltaTime;
        UpdateTimerUI();

        // 2 mobil tersisa = mulai cinematic
        if (raceManager.carList.Count == 2)
            finishCinematic.NearFinish();

        // 1 mobil tersisa = selesai
        if (raceManager.carList.Count <= 1)
            EndGame();
    }
    void SortCars()
    {
        raceManager.carList.Sort((a, b) =>
            b.transform.position.x.CompareTo(a.transform.position.x));

        rank = raceManager.carList.IndexOf(raceManager.playerCar) + 1;
    }
    void UpdateRankUI()
    {
        positionText.text =
            $"{rank}<voffset=0.4em><size=40%>/{raceManager.carList.Count}</voffset></size>";
    }

    void UpdateTimerUI()
    {
        bool playerLast = raceManager.carList[^1] == raceManager.playerCar;
        Color targetColor = playerLast ? lastPosition : position;

        foreach (var t in eliminationTimerText)
        {
            t.color = targetColor;
            t.text = timer <= 0 ? "" : Mathf.Ceil(timer).ToString();
        }
    }
    IEnumerator EliminationLoop()
    {
        while (gameRunning)
        {
            yield return new WaitForSeconds(eliminationInterval);

            GameObject lastCar = raceManager.carList[^1]; // elemen terakhir

            bool eliminatedPlayer = lastCar == raceManager.playerCar;
            int forcedRank = eliminatedPlayer ? rank : -1;

            lastCar.gameObject.SetActive(false);
            raceManager.carList.Remove(lastCar);

            Debug.Log(eliminatedPlayer ?
                "Player Eliminated!" :
                "AI Eliminated: " + lastCar.name);

            if (eliminatedPlayer)
            {
                EndGame(forcedRank);
                yield break;
            }

            timer = eliminationInterval;
        }
    }
    void EndGame(int forcedRank = -1)
    {
        gameRunning = false;

        finishCinematic.NearFinish();
        finishCinematic.Finished();

        if (forcedRank > 0)
            rank = forcedRank;

        int score = rank switch
        {
            1 => 3,
            2 => 2,
            3 => 1,
            _ => 0
        };

        string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        finishCinematic.ShowFinishUI(rank);

        GameData.SaveLevelScore(levelName, score);

        Debug.Log($"Player Rank: {rank} Score: {score}");
    }
}
