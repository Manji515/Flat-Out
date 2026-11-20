using UnityEngine;

public class OilSpill : MonoBehaviour
{
    private SliperryMode sliperryMode;
    private void Start()
    {
        sliperryMode = FindAnyObjectByType<SliperryMode>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        MovementAdjusment adjust = other.GetComponent<MovementAdjusment>();
        if (adjust != null)
        {
            adjust.StartCoroutine(adjust.ApplyOilEffect(sliperryMode.driftIncrase, sliperryMode.recoverySpeed));
        }
        Debug.Log("Mobil Tergelincir!");
    }
}
