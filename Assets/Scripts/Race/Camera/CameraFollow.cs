using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 cameraPosition;

    void LateUpdate()
    {
        if (!target) return;

        // Gunakan rotasi kamera saat ini untuk offset posisi
        Vector3 rotatedOffset = transform.rotation * cameraPosition;
        transform.position = target.position + rotatedOffset;
    }
}
