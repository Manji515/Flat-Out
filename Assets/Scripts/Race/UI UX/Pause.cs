using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Pause : MonoBehaviour
{
    [Header("UI References")]
    private Finishing finishing;
    public GameObject pausePanel;
    public GameObject gamePanel;
    public GameObject finishPanel;
    public GameObject controlPanel;
    public Button resumeButton;
    public Button restartButton; // Tambahkan referensi tombol restart
    public Button pauseButtton;
    public TextMeshProUGUI countdownText;

    public bool isPaused = false;

    void Start()
    {
        finishing = GetComponent<Finishing>();
        pausePanel.SetActive(false);
        pauseButtton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame); // Hubungkan tombol restart
    }
    public void FinishingGame()
    {
        if (isPaused == false)
        {
            finishPanel.SetActive(true);
            gamePanel.SetActive(false);
        }
    }
    public void FinishGame()
    {
        controlPanel.SetActive(false);
    }
    void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        gamePanel.SetActive(false);
        controlPanel.SetActive(false);
        finishPanel.SetActive(false);
    }

    void ResumeGame()
    {
        if (!isPaused) return;

        Time.timeScale = 1f;
        if (finishing.isFinishing == false)
        {
            gamePanel.SetActive(true);
            controlPanel.SetActive(true);
            pausePanel.SetActive(false);
        }
        else
        {
            gamePanel.SetActive(false);
            finishPanel.SetActive(true);
            controlPanel.SetActive(true);
            pausePanel.SetActive(false);
        }
        StartCoroutine(ResumeCountdown());
    }

    IEnumerator ResumeCountdown()
    {
        Time.timeScale = 0f;
        int count = 2;
        countdownText.gameObject.SetActive(true);

        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSecondsRealtime(1f); // pakai realtime
            count--;
        }
        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1f; // lanjutkan game
        isPaused = false;
    }

    void RestartGame()
    {
        // Pastikan game jalan lagi sebelum restart
        Time.timeScale = 1f;
        isPaused = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
