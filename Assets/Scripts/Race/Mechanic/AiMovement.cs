using UnityEngine;
using System.Collections.Generic;

public class AiMovement : MonoBehaviour
{
    private CarMovement carMovement;
    private CarNitro carNitro;
    private bool aiInFront;
    public GameObject player;
    private MovementAdjusment movementAdjusment;
     [Header("AI Speed Control")]
    public bool slowedFactor;
    public float slowDownFactor = 0.7f;
    public float distanceToPlayer;
    public float distanceToSuperSlowedFactor;
    public float SuperSlowedFactor;
    [SerializeField] private List<EdgeCollider2D> edgeColliders;

    [Header("Aktifasi Gizmo (Editor Only)")]
    public bool gizmo;

    [Header("Steering Settings")]
    [Tooltip("Semakin kecil, semakin cepat pindah waypoint")]
    public float waypointReachThreshold = 1.0f; 
    [Tooltip("Sharpness minimum (belokan lebar)")]
    public float MinSteeringSharpness = 6f;
    [Tooltip("Sharpness maksimum (belokan tajam)")]
    public float MaxSteeringSharpness = 1f;
    [Tooltip("Sudut maksimal yang mempengaruhi sharpness")]
    public float limitToReachSteeringSharpness = 10f;
    [Tooltip("Semakin tinggi, belokan lebih smooth")]
    public float steerSmoother = 5f;

    [Header("Debug Info (ReadOnly)")]
    public float mappedSharpness;
    public float steerDir;
    public float currentSteerInput;

    private Transform tf; // cache transform
    private List<List<Vector2>> cachedWaypoints = new List<List<Vector2>>();
    private List<Vector2> waypoints;
    private int currentWaypointIndex = 0;
    private int currentEdgeColliderIndex = 0;

    void Awake()
    {
        tf = transform; // cache transform
        carMovement = GetComponent<CarMovement>();
        movementAdjusment = GetComponent<MovementAdjusment>();
        carNitro = GetComponent<CarNitro>();
    }

    void Start()
    {
        // Pre-cache semua waypoints per EdgeCollider
        foreach (var edge in edgeColliders)
        {
            List<Vector2> wpList = new List<Vector2>();
            foreach (Vector2 p in edge.points)
            {
                wpList.Add(edge.transform.TransformPoint(p));
            }
            cachedWaypoints.Add(wpList);
        }

        if (cachedWaypoints.Count > 0)
        {
            waypoints = cachedWaypoints[0];
        }
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Count == 0 || carMovement == null) return;

        Vector2 carPos = tf.position;
        Vector2 target = waypoints[currentWaypointIndex];
        Vector2 dirToTarget = (target - carPos).normalized;

        steerDir = Vector2.SignedAngle(tf.right, dirToTarget);

        float t = Mathf.Clamp01(Mathf.Abs(steerDir) / limitToReachSteeringSharpness);
        mappedSharpness = Mathf.Lerp(MinSteeringSharpness, MaxSteeringSharpness, t);

        float targetSteerInput = Mathf.Clamp(-steerDir / mappedSharpness, -1f, 1f);
        currentSteerInput = Mathf.Lerp(currentSteerInput, targetSteerInput, Time.fixedDeltaTime * steerSmoother);

        if (player != null && slowedFactor == true)
        {
            // Jarak X ke player
            float aiX = tf.position.x;
            float playerX = player.transform.position.x;

            // Selisih posisi
            distanceToPlayer = aiX - playerX;

            // AI di depan = nilai X lebih besar
            aiInFront = distanceToPlayer > 10f;

            movementAdjusment.isAiOnFront = aiInFront;
        }

        carMovement.SetInputVector(new Vector2(1f, currentSteerInput));

        float distance = Vector2.Distance(carPos, target);
        if (distance < waypointReachThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentEdgeColliderIndex = (currentEdgeColliderIndex + 1) % cachedWaypoints.Count;
                waypoints = cachedWaypoints[currentEdgeColliderIndex];
                currentWaypointIndex = 0;
            }
        }
    }
        void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TurboOn") && distanceToPlayer < 10f)
        {
            carNitro.nitroActivated = true;
        }
        if (other.CompareTag("TurboOff"))
        {
            carNitro.nitroActivated = false;
        }
    }


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (gizmo && waypoints != null)
        {
            Gizmos.color = Color.red;
            foreach (var wp in waypoints)
            {
                Gizmos.DrawSphere(wp, 0.2f);
            }

            if (currentWaypointIndex < waypoints.Count)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex]);
            }
        }
#endif
    }
}
