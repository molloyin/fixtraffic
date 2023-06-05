using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] Slider soundSlider;
    [SerializeField] AudioMixer masterMixer;

    // Start is called before the first frame update
    void Start()
    {
        setVolume(100f);    
    }

    /**<summary>
     *  Called on slider change. Changes the volue according to the slider change
     * </summary>
     */
    public void refreshSlider(float value)
    {
        soundSlider.value = value;
    }

    public void setVolumeFromSlider()
    {
        setVolume(soundSlider.value);
    }

    public void setVolume(float value)
    {
        if(value < 1)
        {
            value = 0.001f;
        }

        refreshSlider(value);
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(value / 100) * 20f);
    }
}
