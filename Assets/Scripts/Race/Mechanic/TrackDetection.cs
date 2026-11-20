using UnityEngine;

public class TrackDetection : MonoBehaviour
{
    [Header("Surface Detection Settings")]
    public LayerMask trackLayer;
    public LayerMask carLayer;
    private int trackContacts = 0;
    private int carContacts = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        int otherLayer = other.gameObject.layer;

        if (((1 << otherLayer) & trackLayer) != 0)
            trackContacts++;

        if (((1 << otherLayer) & carLayer) != 0)
            carContacts++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        int otherLayer = other.gameObject.layer;

        if (((1 << otherLayer) & trackLayer) != 0)
            trackContacts = Mathf.Max(0, trackContacts - 1);

        if (((1 << otherLayer) & carLayer) != 0)
            carContacts = Mathf.Max(0, carContacts - 1);
    }

    public bool IsOnTrack() => trackContacts > 0;
    public bool IsTouchingCar() => carContacts > 0;
}
