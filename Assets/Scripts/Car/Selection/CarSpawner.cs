using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public Transform spawnPoint;

    void Start()
    {
        int index = PlayerPrefs.GetInt("SelectedCar", 0);

        Instantiate(
            carPrefabs[index],
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint
        );
    }
}
