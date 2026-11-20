using UnityEngine;

public class Sequence : MonoBehaviour
{
    public GameObject[] objects;
    public int currentIndex = 0;

    [HideInInspector] public SequenceManager manager; // otomatis diisi manager

    void Start()
    {
        for (int i = 0; i < objects.Length; i++)
            objects[i].SetActive(i == 0);
    }

    public void OnSequence()
    {
        if (manager != null)
            manager.NotifySequenceProgress(this);

        objects[currentIndex].SetActive(false);

        currentIndex++;

        if (currentIndex < objects.Length)
            objects[currentIndex].SetActive(true);
        else
            Debug.Log("Sequence selesai");
    }

    public void ResetSequence()
    {
        foreach (var obj in objects)
            obj.SetActive(false);

        currentIndex = 0;

        if (objects.Length > 0)
            objects[0].SetActive(true);
    }
}
