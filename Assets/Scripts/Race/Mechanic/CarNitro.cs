using UnityEngine;
using UnityEngine.UI;

public class CarNitro : MonoBehaviour
{
    [Header("Nitro Settings")]
    public float TopSpeedBonus = 1.5f;
    public float AccelerationBonus = 3.0f;
    public float ConsumptionRate = 20f;
    public float maxCapacity = 100f;
    public float currentCapacity = 0f;
    public float RegenSpeed = 5.0f;
    public float DrainSpeed = 10f;
    public Image nitroBar;
    public bool findBar = false;
    private CarMovement carMovement;
    private MovementAdjusment movementAdjusment;
    private TrackDetection surfaceHandler;
    public Color ActiavatedNitro;
    public Color NitroRegen;
    public Color NitroDrain;
    public Color NitroNeutral;
    public Color NitroFull;
    public bool nitroActivated = false;

    void Start()
    {
        carMovement = GetComponent<CarMovement>();
        surfaceHandler = GetComponent<TrackDetection>();
        movementAdjusment = GetComponent<MovementAdjusment>();
        if (findBar == true) nitroBar = UIManager.Instance.nitroBar;
    }

    void Update()
    {

        bool onTrack = surfaceHandler != null && surfaceHandler.IsOnTrack();
        bool isDrifting = carMovement != null && carMovement.isDrifting;

        if (isDrifting && onTrack && !nitroActivated)
        {
            currentCapacity = Mathf.MoveTowards(currentCapacity, maxCapacity, RegenSpeed * Time.deltaTime);
        }

        if (nitroActivated)
        {
            currentCapacity = Mathf.MoveTowards(currentCapacity, 0f, ConsumptionRate * Time.deltaTime);
            if (currentCapacity <= 0f)
            {
                nitroActivated = false;
            }
        }
        else if (!onTrack)
        {
            currentCapacity = Mathf.MoveTowards(currentCapacity, 0f, DrainSpeed * Time.deltaTime);
        }

        if (movementAdjusment != null)
        {
            movementAdjusment.isNitroActive = nitroActivated && currentCapacity > 0f;
        }

        if (nitroBar != null)
        {
            float fillPercent = currentCapacity / maxCapacity;
            nitroBar.fillAmount = fillPercent;

            if (nitroActivated && currentCapacity > 0f)
                nitroBar.color = ActiavatedNitro;
            else if (!onTrack)
                nitroBar.color = NitroDrain;
            else if (Mathf.Approximately(currentCapacity, maxCapacity))
                nitroBar.color = NitroFull;
            else if (isDrifting && onTrack)
                nitroBar.color = NitroRegen;
            else
                nitroBar.color = NitroNeutral;
        }
    }
}
