using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceManager : MonoBehaviour
{
    [Header("Car References")]
    public List<GameObject> carList = new List<GameObject>();
    public int countdown = 3;
    public GameObject pausePanel;
    public GameObject controlPanel;
    public GameObject playerCar;

    [Header("Race References")]
    public TextMeshProUGUI countdownText;
    public bool starting = true;

    [Header("Audio")]
    public AudioSource bgm;   // <-- tambahkan ini
    private Pause pauseManager;
    private Finishing finishing;
    void Start()
    {
        pauseManager = GetComponent<Pause>();
        finishing = GetComponent<Finishing>();
    }
    void Awake()
    {
        Time.timeScale = 0f;
        countdownText.gameObject.SetActive(true);
        controlPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void BeginRace()
    {
        StartCoroutine(StartCountdown());
        bgm.Play();
    }

    IEnumerator StartCountdown()
    {
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }

        Time.timeScale = 1f;
        controlPanel.SetActive(true);
        countdownText.text = "GO!";
        starting = false;

        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        pausePanel.SetActive(true);
    }
    void Update()
    {
        if (pauseManager.isPaused == true || finishing.finished == true)
        {
            PauseAudio();
        }
        else
        {
            ResumeAudio();
        }
    }

    void PauseAudio()
    {
        if (bgm.isPlaying) bgm.Pause();
    }

    void ResumeAudio()
    {
        if (!bgm.isPlaying) bgm.UnPause();
    }

}
