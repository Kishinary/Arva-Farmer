using UnityEngine;
using UnityEngine.Audio;

public class audioController : MonoBehaviour
{
    [Header("References")]
    public AudioMixer mainMixer; 

  
    public void SetMusicVolume(float sliderValue)
    {

        float volumeInDb = Mathf.Log10(sliderValue) * 20f;

        mainMixer.SetFloat("MusicVolume", volumeInDb);
    }
}
