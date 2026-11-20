using UnityEngine;
using TMPro;

public class DeadlineMode : MonoBehaviour
{
    [Header("UI Timer")]
    public TMP_Text[] timerText;

    [Header("Deadline Settings")]
    public float goldTime = 10f;
    public float silverTime = 15f;
    public float bronzeTime = 20f;
    private RaceManager raceManager;
    private Finishing finishing;

    [Header("UI Target Time")]
    public TMP_Text goldTimeUI;
    public TMP_Text silverTimeUI;
    public TMP_Text bronzeTimeUI;

    [Header("Timer Colors")]
    public Color goldColor;
    public Color silverColor;
    public Color bronzeColor;
    public Color failColor;

    private float timer = 0f;
    private bool isRunning = false;
    private bool finished = false;

    void Start()
    {
        finishing = GetComponent<Finishing>();
        raceManager = GetComponent<RaceManager>();

        goldTimeUI.text = FormatTime(goldTime);
        silverTimeUI.text = FormatTime(silverTime);
        bronzeTimeUI.text = FormatTime(bronzeTime);

        SetTierUI(false, false, false);
    }

    void Update()
    {
        if (finished) return;

        if (raceManager.countdown <= 0f)
            isRunning = true;

        if (isRunning)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
            UpdateTierUI();
        }

        if (finishing.finished == true)
        {
            FinishRun();
        }
    }

    string FormatTime(float t)
    {
        int m = Mathf.FloorToInt(t / 60f);
        int s = Mathf.FloorToInt(t % 60f);
        int ms = Mathf.FloorToInt((t * 100f) % 100);
        return $"{m:00}:{s:00}:{ms:00}";
    }

    void UpdateTimerDisplay()
    {
        string formatted = FormatTime(timer);
        foreach (var t in timerText) t.text = formatted;
    }

    void UpdateTierUI()
    {
        Color c;
        bool g, s, b;

        if (timer <= goldTime)
        { c = goldColor; g = true; s = false; b = false; }
        else if (timer <= silverTime)
        { c = silverColor; g = false; s = true; b = false; }
        else if (timer <= bronzeTime)
        { c = bronzeColor; g = false; s = false; b = true; }
        else
        { c = failColor; g = s = b = false; }

        foreach (var t in timerText) t.color = c;
        SetTierUI(g, s, b);
    }

    void SetTierUI(bool g, bool s, bool b)
    {
        goldTimeUI.gameObject.SetActive(g);
        silverTimeUI.gameObject.SetActive(s);
        bronzeTimeUI.gameObject.SetActive(b);
    }

    void FinishRun()
    {
        finished = true;
        isRunning = false;

        SetTierUI(false, false, false);
        EvaluateScore();
    }

    void EvaluateScore()
    {
        int score, rank;

        if (timer <= goldTime) { score = 3; rank = 1; }
        else if (timer <= silverTime) { score = 2; rank = 2; }
        else if (timer <= bronzeTime) { score = 1; rank = 3; }
        else { score = 0; rank = 4; }

        string level = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GameData.SaveLevelScore(level, score);

        finishing?.ShowFinishUI(rank);
    }
}
