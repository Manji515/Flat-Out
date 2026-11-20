using UnityEngine;

public class CarSelection: MonoBehaviour
{
    public void SelectCar(int index)
    {
        PlayerPrefs.SetInt("SelectedCar", index);
    }
}
