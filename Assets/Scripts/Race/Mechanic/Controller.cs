using UnityEngine;

public class Controller : MonoBehaviour
{
    CarMovement carControl;
    AiMovement aiMovement;
    CarNitro carNitro;
    TrackDetection trackDetection;
    public Sensitivity sensitivity;
    float steerInput;
    float steerTarget;
    // Status tombol UI
    bool leftHeld = false;
    bool rightHeld = false;
    bool nitroHeld = false;

    void Awake()
    {
        carControl = GetComponent<CarMovement>();
        aiMovement = GetComponent<AiMovement>();
        carNitro = GetComponent<CarNitro>();
        trackDetection = GetComponent<TrackDetection>();
        if (aiMovement.enabled)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        bool onTrack = trackDetection.IsOnTrack();

        // --- STEER: UI buttons ---
        if (leftHeld) steerTarget = -1f;
        else if (rightHeld) steerTarget = 1f;
        else steerTarget = 0f;

        // --- STEER: Keyboard override (untuk testing PC) ---
        if (Input.GetKey(KeyCode.A))
            steerTarget = -1f;
        else if (Input.GetKey(KeyCode.D))
            steerTarget = 1f;
        else if (!leftHeld && !rightHeld)
            steerTarget = 0f;

        // --- Nitro: UI button ---
        bool nitroActive = nitroHeld;

        // --- Nitro: Keyboard ---
        if (Input.GetKey(KeyCode.Space))
            nitroActive = true;

        carNitro.nitroActivated = nitroActive && onTrack;

        // --- Steering smoothing ---
        
        float currentSensitivity = sensitivity.CurrentSensitivity;
        steerInput = Mathf.MoveTowards(steerInput, steerTarget, currentSensitivity * Time.deltaTime);


        Vector2 inputVector = new Vector2(1f, steerInput);
        carControl.SetInputVector(inputVector);
    }

    // ===== Button Event Functions =====
    public void OnLeftDown() => leftHeld = true;
    public void OnLeftUp() => leftHeld = false;

    public void OnRightDown() => rightHeld = true;
    public void OnRightUp() => rightHeld = false;

    public void OnNitroDown() => nitroHeld = true;
    public void OnNitroUp() => nitroHeld = false;
}
