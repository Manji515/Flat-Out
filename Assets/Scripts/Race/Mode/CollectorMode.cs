using UnityEngine;
using TMPro;

public class CollectorMode : MonoBehaviour
{
    private RaceManager raceManager;
    private Finishing finishCinematic;

    public TMP_Text[] timerText;
    public TMP_Text[] coinsText;

    public int goldPoint = 20;
    public int silverPoint = 15;
    public int bronzePoint = 10;

    public Color goldColor;
    public Color silverColor;
    public Color bronzeColor;
    public Color failColor;

    public float timeLimit = 60f;

    private float timer;
    private int collectedCoins = 0;
    private bool isFinished = false;

    void Start()
    {
        timer = timeLimit;
        raceManager = GetComponent<RaceManager>();
        finishCinematic = GetComponent<Finishing>();
    }

    void Update()
    {
        if (isFinished) return;

        UpdateTimerDisplay();
        UpdateCoinDisplay();

        if (raceManager.countdown <= 0f)
            timer -= Time.deltaTime;

        if (timer <= 10f)
            finishCinematic.NearFinish();

        if (timer <= 0f)
            FinishRace();
    }

    void UpdateCoinDisplay()
    {
        // Tentukan warna berdasarkan jumlah coin
        Color c =
            collectedCoins >= goldPoint ? goldColor :
            collectedCoins >= silverPoint ? silverColor :
            collectedCoins >= bronzePoint ? bronzeColor :
            failColor;

        foreach (var t in coinsText)
        {
            t.text = collectedCoins.ToString();
            t.color = c;
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        int ms = Mathf.FloorToInt((timer * 100f) % 100);

        string formatted = $"{minutes:00}:{seconds:00}:{ms:00}";

        foreach (var t in timerText)
            t.text = formatted;
    }

    public void AddCoin()
    {
        if (!isFinished)
            collectedCoins++;
    }

    void FinishRace()
    {
        isFinished = true;

        int rank;
        int earnedPoint;

        if (collectedCoins >= goldPoint)
        {
            earnedPoint = 3; rank = 1;
        }
        else if (collectedCoins >= silverPoint)
        {
            earnedPoint = 2; rank = 2;
        }
        else if (collectedCoins >= bronzePoint)
        {
            earnedPoint = 1; rank = 3;
        }
        else
        {
            earnedPoint = 0; rank = 4;
        }

        string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GameData.SaveLevelScore(levelName, earnedPoint);

        finishCinematic.ShowFinishUI(rank);
        finishCinematic.Finished();

        Debug.Log($"Collector Finish! Coins: {collectedCoins}, Earned: {earnedPoint} pts");
    }
}
