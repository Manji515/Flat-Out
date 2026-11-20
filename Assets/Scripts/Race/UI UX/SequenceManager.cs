using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    public Sequence[] sequences;

    void Awake()
    {
        // Cari semua Sequence di child (termasuk nested children)
        sequences = GetComponentsInChildren<Sequence>(true);

        // Beri reference manager ke setiap Sequence
        foreach (var seq in sequences)
        {
            seq.manager = this;
        }
    }

    public void NotifySequenceProgress(Sequence sender)
    {
        foreach (var seq in sequences)
        {
            if (seq != sender)
                seq.ResetSequence();
        }
    }
}
