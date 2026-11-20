using UnityEngine;

public class raceBGM : MonoBehaviour
{
     public AudioSource bgm;
    void Start()
    {
        // Pastikan semua AudioSource aktif
        if (bgm && !bgm.isPlaying) bgm.Play();
    }
    
}
