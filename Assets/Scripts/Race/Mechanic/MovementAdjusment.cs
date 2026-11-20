using UnityEngine;
using System.Collections;

public class MovementAdjusment : MonoBehaviour
{
    [HideInInspector] public bool isNitroActive = false;
    [HideInInspector] public bool isAiOnFront;

    private TrackDetection trackDetection;
    private CarMovement carMovement;
    private AiMovement aiMovement;
    private CarNitro carNitro;
    public bool sliped;

    [Header("Base Value")]
    public float baseTopSpeed;
    public float baseAcceleration;
    public float baseDriftFactor;

    [Header("Runtime Modifier")]
    [HideInInspector] public float oilDriftBoost = 0f;

    void Start()
    {
        trackDetection = GetComponent<TrackDetection>();
        carMovement = GetComponent<CarMovement>();
        aiMovement = GetComponent<AiMovement>();
        carNitro = GetComponent<CarNitro>();

        baseTopSpeed = carMovement.topSpeed;
        baseAcceleration = carMovement.acceleration;
        baseDriftFactor = carMovement.driftFactor;
    }

    void Update()
    {
        ApplyMovementState();
    }

    // ===== TRACK STATE =====
    void ApplyMovementState()
    {
        bool onTrack = trackDetection.IsOnTrack();

        // RESET ke base dulu
        float finalTopSpeed = baseTopSpeed;
        float finalAcceleration = baseAcceleration;
        float finalDrift = baseDriftFactor + oilDriftBoost;

        // ===== TRACK =====
        if (!onTrack)
        {
            finalTopSpeed *= 0.5f;
            finalAcceleration *= 0.5f;
        }
        else if (isAiOnFront)
        {
            float factor = aiMovement.distanceToPlayer >
                        aiMovement.distanceToSuperSlowedFactor
                ? aiMovement.SuperSlowedFactor
                : aiMovement.slowDownFactor;

            finalTopSpeed *= factor;
            finalAcceleration *= factor;
        }

        // ===== NITRO (PALING AKHIR) =====
        if (isNitroActive)
        {
            finalTopSpeed *= carNitro.TopSpeedBonus;
            finalAcceleration *= carNitro.AccelerationBonus;

            finalDrift =
                (baseDriftFactor + oilDriftBoost) -
                (baseDriftFactor - 0.9f);
        }

        // ===== APPLY =====
        carMovement.topSpeed = finalTopSpeed;
        carMovement.acceleration = finalAcceleration;
        carMovement.driftFactor = finalDrift;
    }

    // ===== OIL EFFECT =====

    public IEnumerator ApplyOilEffect(float boost, float recoverSpeed)
    {
        sliped = true;
        oilDriftBoost += boost;

        while (oilDriftBoost > 0.01f)
        {
            oilDriftBoost = Mathf.Lerp(
                oilDriftBoost,
                0f,
                Time.deltaTime * recoverSpeed
            );

            carMovement.driftFactor = baseDriftFactor + oilDriftBoost;
            yield return null;
        }
        sliped = false;
        oilDriftBoost = 0f;
        carMovement.driftFactor = baseDriftFactor;
    }
}
