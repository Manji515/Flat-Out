using UnityEngine;
using System.Collections;

public class Drone : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 position;

    [Header("AI Setting")]
    public float distanceToPlayer;
    public float currentSteerInput;
    public float steerSmoother = 5f;
    public float minSteeringSensitivity = 0.3f;
    public float maxSteeringSensitivity = 1.2f;
    public float steeringAngleLimit = 25f; 

    [Header("Dynamic Speed Multiplier")]
    public float baseTopSpeed;
    public float baseAcceleration;
    public float engineInput;
    public float minMultiplier = 1f;       // multiplier saat dekat
    public float maxMultiplier = 3f;       // multiplier saat jauh
    public float maxDistance = 30f;        // jarak maksimum dianggap penuh
    private CarMovement carMovement;
    private HuntedMode huntedMode;

    private void Start()
    {
        carMovement = GetComponent<CarMovement>();
        huntedMode = FindAnyObjectByType<HuntedMode>();
        baseTopSpeed = carMovement.topSpeed;
        baseAcceleration = carMovement.acceleration;
        transform.position = target.position + position;
    }

    void FixedUpdate()
    {
        if (target == null || carMovement == null) return;
        maxMultiplier += 0.015f * Time.fixedDeltaTime;

        float aiX = transform.position.x;
        float playerX = target.transform.position.x;
        distanceToPlayer = Mathf.Abs(playerX - aiX);

        // Hitung speedMultiplier dinamis
        float multiplier = Mathf.Lerp(minMultiplier, maxMultiplier, distanceToPlayer / maxDistance);
        multiplier = Mathf.Clamp(multiplier, minMultiplier, maxMultiplier);


        Vector2 dirToTarget = (target.position - transform.position).normalized;
        float steerDir = Vector2.SignedAngle(transform.right, dirToTarget);

        // Hitung t berdasarkan besar sudut
        float t = Mathf.Clamp01(Mathf.Abs(steerDir) / steeringAngleLimit);
        float mappedSharpness = Mathf.Lerp(minSteeringSensitivity, maxSteeringSensitivity, t);

        float targetSteerInput = Mathf.Clamp(-steerDir / mappedSharpness, -1f, 1f);
        currentSteerInput = Mathf.Lerp(currentSteerInput, targetSteerInput, Time.fixedDeltaTime * steerSmoother);

        // Set input
        carMovement.SetInputVector(new Vector2(engineInput, currentSteerInput));
        carMovement.topSpeed = baseTopSpeed * multiplier;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            transform.position = target.position + position;
            huntedMode.penaltyPoints++;
        }
    }

}
