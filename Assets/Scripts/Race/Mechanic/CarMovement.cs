using UnityEngine;
using System.Collections;

public class CarMovement : MonoBehaviour
{
    private Rigidbody2D body; // Fisik Mobil
    private TrackDetection surfaceHandler;
    private Collider2D col;
    private SpriteRenderer sr;

    [Header("Car Setting")]
    public float acceleration = 10f;     // Akselerasi
    public float topSpeed = 15f;         // Batas Kecepatan (legacy, jangan dipakai langsung)
    public float steerPower = 3.5f;      // Kekuatan Kemudi
    public float brakePower = 2.0f;      // Kekuatan Pengereman
    public float reverseGear = 0.5f;     // Gigi Mundur
    public float driftFactor = 0.95f;    // Kekuatan Drift
   
    [Header("Car Body")]
    public float collisionHoldTime = 1f;   // butuh kontak selama 1 detik
    public float ghostDuration = 1f;       // berapa lama jadi ghost
    [Range(0f, 1f)] public float ghostAlpha = 0.5f; // transparansi saat ghost
    private float collisionTimer = 0f;
    private bool isGhost = false;
    public float ghostTimer = 0f;
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    public float maxWheelSteerAngle = 30f;

    [Header("Car Condition")]
    public float speed;
    public float engineInput; // Input Mesin
    public float steerInput;  // Input Kemudi
    public bool isDrifting = false;
    private float steerAngle; 
    [HideInInspector] public float speedRatio;

    void Awake()
    {
        body = GetComponentInParent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        surfaceHandler = GetComponent<TrackDetection>();
        steerAngle = body.rotation;
    }


    public void SetInputVector(Vector2 inputVector)
    {
        engineInput = inputVector.x;
        steerInput = inputVector.y;
    }

    void Update()
    {
        isDrifting = Mathf.Abs(Drifting()) > 0.5f;
        RotateWheels();
    }

    void FixedUpdate() 
    {
        Engine();
        Steer();
    }

    public float GetVelocityMagnitude()
    {
        return body.velocity.magnitude;
    }

    void Engine() // Mengatur laju mobil
    {
        float forwardGear = Vector2.Dot(transform.right, body.velocity);
        speed = body.velocity.magnitude;
        speedRatio = Mathf.Clamp01(speed / topSpeed);

        // Pengereman 
        if (engineInput < 0 && forwardGear > 0) 
            body.drag = Mathf.Lerp(body.drag, brakePower, Time.fixedDeltaTime * 1);
        else
            body.drag = 0f;

        float effectiveAcceleration = acceleration * (1 - speedRatio);

        float backward = (engineInput < 0 && forwardGear < 0) 
            ? effectiveAcceleration * reverseGear 
            : effectiveAcceleration;

        Vector2 engineForceVector = transform.right * engineInput * backward;
        body.AddForce(engineForceVector, ForceMode2D.Force);
        // Batasi kecepatan agar tidak melebihi CurrentTopSpeed
        if (body.velocity.magnitude > topSpeed)
        {
            Vector2 targetVelocity = body.velocity.normalized * topSpeed;
            body.velocity = Vector2.Lerp(body.velocity, targetVelocity, Time.fixedDeltaTime * 0.5f);
        }

    }

    void Steer() 
    {
        float minSpeedForTurning = body.velocity.magnitude / 8f;
        minSpeedForTurning = Mathf.Clamp01(minSpeedForTurning);

        steerAngle -= steerInput * steerPower * minSpeedForTurning;
        body.MoveRotation(steerAngle);

        Vector2 forwardVelocity = transform.up * Vector2.Dot(body.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(body.velocity, transform.right);
        body.velocity = rightVelocity + forwardVelocity * driftFactor;
    }
    
    void RotateWheels()
    {
        float wheelAngle = -steerInput * maxWheelSteerAngle;
        if (frontLeftWheel != null)
            frontLeftWheel.localRotation = Quaternion.Euler(0, 0, wheelAngle);
        if (frontRightWheel != null)
            frontRightWheel.localRotation = Quaternion.Euler(0, 0, wheelAngle);
    }

    public float Drifting()
    {
        return Vector2.Dot(transform.up, body.velocity);
    }
    // ===== AntiCollision logic =====
    void OnCollisionStay2D(Collision2D collision)
    {
        if (isGhost) return;

        collisionTimer += Time.deltaTime;
        if (collisionTimer >= collisionHoldTime)
        {
            StartCoroutine(ActivateGhostMode());
            collisionTimer = 0f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        collisionTimer = 0f;
    }

    private IEnumerator ActivateGhostMode()
    {
        isGhost = true;
        col.isTrigger = true;

        // ubah transparansi sprite
        Color c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, ghostAlpha);

        // Selama waktu ghost belum habis atau masih tumpang tindih dengan mobil lain
        while (ghostTimer < ghostDuration || surfaceHandler.IsTouchingCar())
        {
            // Jika sudah lewat durasi tapi masih overlap, reset timer supaya menunggu overlap selesai
            if (surfaceHandler.IsTouchingCar())
                ghostTimer = 0f; 
            else
                ghostTimer += Time.deltaTime;

            yield return null;
        }

        // Kembali normal
        ghostTimer = 0f;
        col.isTrigger = false;
        sr.color = new Color(c.r, c.g, c.b, 1f);
        isGhost = false;
    }


}


