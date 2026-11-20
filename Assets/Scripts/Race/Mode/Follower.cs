using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;   // Objek yang dikejar
    public Vector3 position;

    void LateUpdate()
    {
        transform.position = target.position + position;
    }
}
