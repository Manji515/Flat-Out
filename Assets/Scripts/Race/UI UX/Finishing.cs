using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro; // pastikan pakai TMP

public class Finishing : MonoBehaviour
{
    public bool finished = false;
    public bool isFinishing = false;
    private Pause pause;
    [Header("UI Settings")]  
    [SerializeField] private RectTransform topBar;    
    [SerializeField] private RectTransform bottomBar; 
    [SerializeField] private RectTransform finishBar; 
    [SerializeField] private SpriteRenderer overlayImage;      
    [SerializeField] private TextMeshProUGUI tapToContinueText;
    public TextMeshProUGUI finishText;
    public TextMeshProUGUI []scoreText;
    public float animDuration = 1f;
    public float finishedPending;
    public Vector2 offsetStart = new Vector2(-100f, 0f);

    [Header("Letterbox Settings")]
    [SerializeField] private float cinematicAnimationTime = 0.5f; 
    [SerializeField] private float finishAnimationTime = 0.25f;   
    [SerializeField] private float LetterBoxWide = 1f; 
    [SerializeField] private float FinishBoxWide = 1f;
    [SerializeField] private float Alpha = 0.5f; 

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "MainMenu"; 
    private bool canTapToContinue = false;

    private void Start()
    {
        pause = GetComponent<Pause>();
        if (finishText != null) finishText.gameObject.SetActive(false);
        if (tapToContinueText != null) tapToContinueText.gameObject.SetActive(false);
        if (topBar != null) topBar.localScale = new Vector3(topBar.localScale.x, 0, 1);
        if (bottomBar != null) bottomBar.localScale = new Vector3(bottomBar.localScale.x, 0, 1);
        if (finishBar != null) finishBar.localScale = new Vector3(finishBar.localScale.x, 0, 1);

        foreach (var scoreText in scoreText)
        if (scoreText != null) scoreText.gameObject.SetActive(false);

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            overlayImage.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    private void Update()
    {
        // jika sudah boleh tap -> cek input
        if (canTapToContinue && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
    public void NearFinish()
    {
        pause.FinishingGame();
        isFinishing = true;

        foreach (var scoreText in scoreText)
        scoreText.gameObject.SetActive(true);

        ShowLetterbox();
        FadeOverlay();
    }
    public void Finished()
    {
        pause.FinishGame();
        finished = true;
        ShowFinishbox();
        StartCoroutine(EnableTapAfterDelay(3f));
    }
    private void ShowLetterbox()
    {
        if (topBar != null) StartCoroutine(AnimateScale(topBar, LetterBoxWide, cinematicAnimationTime));
        if (bottomBar != null) StartCoroutine(AnimateScale(bottomBar, LetterBoxWide, cinematicAnimationTime));
    }

    private void FadeOverlay()
    {
        if (overlayImage != null) StartCoroutine(AnimateOverlay(Alpha, cinematicAnimationTime));
    }

    private void ShowFinishbox()
    {
        if (finishBar != null) StartCoroutine(AnimateScale(finishBar, FinishBoxWide, finishAnimationTime));
    }

    private IEnumerator AnimateScale(RectTransform bar, float targetY, float duration)
    {
        float startY = bar.localScale.y;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newY = Mathf.Lerp(startY, targetY, elapsed / duration);
            bar.localScale = new Vector3(bar.localScale.x, newY, 1);
            yield return null;
        }

        bar.localScale = new Vector3(bar.localScale.x, targetY, 1);
    }

    private IEnumerator AnimateOverlay(float targetAlpha, float duration)
    {
        Color startColor = overlayImage.color;
        float startAlpha = startColor.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            overlayImage.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        overlayImage.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }

    private IEnumerator EnableTapAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canTapToContinue = true;

        if (tapToContinueText != null) 
            tapToContinueText.gameObject.SetActive(true); // tampilkan teks

        Debug.Log("Player bisa tap untuk lanjut!");
        Time.timeScale = 0f;
    }
    public void ShowFinishUI(int rank)
    {
        foreach (var scoreText in scoreText)
        scoreText.gameObject.SetActive(false);
        if (finishText == null) return;

        string suffix = "th";
        if (rank == 1) suffix = "st";
        else if (rank == 2) suffix = "nd";
        else if (rank == 3) suffix = "rd";

        // Gunakan rich text TMP untuk warna
        finishText.text = $"<color=#FFFFFF>FINISHED</color> <color=#FFA500>{rank}{suffix}</color>";
        finishText.gameObject.SetActive(true);

        // Set awal transparan & geser
        finishText.color = new Color(finishText.color.r, finishText.color.g, finishText.color.b, 0f);
        RectTransform rect = finishText.rectTransform;
        Vector2 startPos = rect.anchoredPosition + offsetStart;
        rect.anchoredPosition = startPos;

        // mulai coroutine dengan delay
        StartCoroutine(AnimateFinishText(rect, startPos));
    }

    private IEnumerator AnimateFinishText(RectTransform rect, Vector2 startPos)
    {
        Vector2 endPos = startPos - offsetStart; // target posisi
        Color startColor = finishText.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        // Tunggu sebentar sebelum animasi dimulai
        yield return new WaitForSeconds(finishedPending); // bisa atur di inspector juga kalau mau fleksibel

        float t = 0;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / animDuration);

            // easing (EaseOutCubic)
            float eased = 1f - Mathf.Pow(1f - normalized, 3f);

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            finishText.color = Color.Lerp(startColor, endColor, eased);

            yield return null;
        }
    }
}
