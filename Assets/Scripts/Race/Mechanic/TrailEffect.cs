using UnityEngine;

public class TrailEffect : MonoBehaviour
{
    public TrailRenderer skidOnTrack;
    public TrailRenderer skidOffTrack;
    public TrailRenderer nitroTrail;
    private TrackDetection surfaceHandler;
    public MovementAdjusment movementAdjusment;
    public CarMovement carMovement;

    [Header("Trail Settings")]
    [Tooltip("Waktu toleransi sebelum trail mati setelah drift berhenti")]
    public float driftHoldTime = 0.2f; // tahan 0.2 detik sebelum mematikan trail
    private float driftTimer = 0f;

    void Awake()
    {
        surfaceHandler = GetComponent<TrackDetection>();
        if (skidOffTrack != null) skidOffTrack.emitting = false;
        if (skidOnTrack != null) skidOnTrack.emitting = false;
        if (nitroTrail != null) nitroTrail.emitting = false;
    }

    void Update()
    {
        bool onTrack = surfaceHandler.IsOnTrack();
        bool drifting = carMovement.isDrifting;

        if (drifting)
        {
            driftTimer = driftHoldTime;
        }
        else if (driftTimer > 0)
        {
            driftTimer -= Time.deltaTime;
            drifting = true;
        }

        if (drifting && onTrack && movementAdjusment.isNitroActive == false && movementAdjusment.sliped == false)
        {
            skidOnTrack.emitting = true;
        }
        else
        {
            skidOnTrack.emitting = false;
        } 

        if (movementAdjusment.isNitroActive == true && onTrack && movementAdjusment.sliped == false)
            nitroTrail.emitting = true;
        else
            nitroTrail.emitting = false;
        
        if (movementAdjusment.sliped == true)skidOffTrack.emitting = true;
        else skidOffTrack.emitting = false;
    }
}
