using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image nitroBar;

    void Awake()
    {
        Instance = this;
    }
}

