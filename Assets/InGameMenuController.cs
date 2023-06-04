using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class InGameMenuController : MonoBehaviour
{
    private bool gamePaused;
    private float savedVolume;
    [SerializeField] public AudioMixer mixer;

    // Start is called before the first frame update
    void Start()
    {
        this.gamePaused = false; //By default game is started
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gamePaused)
            {
                resumeGame();
                gamePaused = false;
            } else
            {
                pauseGame();
                gamePaused = true;
            }
        }
        
    }

    //Pauses the game
    private void pauseGame()
    {
        //Saving the game audio level
        mixer.GetFloat("MasterVolume", out this.savedVolume);

        //Muting sound effects
        mixer.SetFloat("MasterVolume", -80f);

        Time.timeScale = 0;


    }

    //Resumes the game
    private void resumeGame()
    {
        Time.timeScale = 1;

        //Unmute sound effects
        mixer.SetFloat("MasterVolume", this.savedVolume);
    }
}
