using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private Finishing finishing;
    // Start is called before the first frame update
    void Start()
    {
        finishing = FindAnyObjectByType<Finishing>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finishing"))
        {
            finishing.NearFinish();
        }
        if (other.CompareTag("Finish"))
        {
            finishing.Finished();
        }
    }
}
