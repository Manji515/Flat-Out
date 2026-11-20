using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(CarMovement))]
public class carSound : MonoBehaviour
{
    [Header("Mixers (Opsional)")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource driftAudio;
    public AudioSource engineAudio;
    public AudioSource hitAudio;

    private CarMovement car;
    private float targetEnginePitch;
    private float targetEngineVolume;
    void Awake()
    {
        car = GetComponentInParent<CarMovement>();
    }

    void Start()
    {
        // Pastikan semua AudioSource aktif
        if (engineAudio && !engineAudio.isPlaying) engineAudio.Play();
        if (driftAudio && !driftAudio.isPlaying) driftAudio.Play();
    }
    void Update()
    {
        if (car == null) return;

        UpdateEngineSFX();
        UpdateDriftSFX();
        if (Time.timeScale == 0f)
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
        if (engineAudio.isPlaying) engineAudio.Pause();
        if (driftAudio.isPlaying) driftAudio.Pause();
    }

    void ResumeAudio()
    {
        if (!engineAudio.isPlaying) engineAudio.UnPause();
        if (!driftAudio.isPlaying) driftAudio.UnPause();
    }

    // === Suara Mesin ===
    void UpdateEngineSFX()
    {
        if (engineAudio == null) return;

        float normalizedSpeed = Mathf.Clamp01(car.speed / car.topSpeed);

        // Volume naik non-linear agar idle tetap terdengar
        targetEngineVolume = Mathf.Lerp(0.3f, 1f, Mathf.Pow(normalizedSpeed, 0.7f));
        engineAudio.volume = Mathf.Lerp(engineAudio.volume, targetEngineVolume, Time.deltaTime * 5f);

        // Pitch idle -> tinggi saat top speed
        targetEnginePitch = Mathf.Lerp(0.8f, 2.0f, normalizedSpeed);
        engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, targetEnginePitch, Time.deltaTime * 3f);
    }

    // === Suara Drift / Decitan Ban ===
    void UpdateDriftSFX()
{
    if (driftAudio == null) return;

    float driftAmount = Mathf.Abs(car.Drifting());

    if (car.isDrifting && driftAmount > 0.1f)
    {
        // Volume naik sesuai kekuatan drift
        float targetVolume = Mathf.Clamp(driftAmount * 0.08f, 0.1f, 1f);
        driftAudio.volume = Mathf.Lerp(driftAudio.volume, targetVolume, Time.deltaTime * 5f);

        // Pitch tetap (tidak naik walau drift makin kuat)
        driftAudio.pitch = Mathf.Lerp(driftAudio.pitch, 1.0f, Time.deltaTime * 3f);
    }
    else
    {
        // Fade out suara drift
        driftAudio.volume = Mathf.Lerp(driftAudio.volume, 0f, Time.deltaTime * 6f);
    }
}
void OnCollisionEnter2D(Collision2D collision2D) 
    {
        //Get the relative velocity of the collision
        float relativeVecolity = collision2D.relativeVelocity.magnitude;

        hitAudio.pitch = Random.Range(0.95f, 1.05f);
        hitAudio.volume = relativeVecolity;

        if (!hitAudio.isPlaying)
        hitAudio.Play();
    }
}
