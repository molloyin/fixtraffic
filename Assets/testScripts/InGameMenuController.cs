using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class InGameMenuController : MonoBehaviour
{
    public bool gamePaused;
    public float savedVolume;
    [SerializeField] public AudioMixer mixer;
    [SerializeField] public GameObject gameMenuCanvas;
    [SerializeField] public GameObject menuPage;
    [SerializeField] public GameObject optionsPage;

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
            Debug.Log("INPUT");
            if (gamePaused)
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
    public void pauseGame()
    {
        //Setting menu as active
        this.gameMenuCanvas.SetActive(true);
        this.menuPage.SetActive(true);
        this.optionsPage.SetActive(false);

        if(mixer != null)
        {
            mixer.GetFloat("MasterVolume", out this.savedVolume);    //Saving the game audio level
            mixer.SetFloat("MasterVolume", -80f);   //Muting sound effects
        }

        this.gamePaused = true;

        Time.timeScale = 0;


    }

    //Resumes the game
    public void resumeGame()
    {
        Time.timeScale = 1;
        this.gamePaused = false;

        //Setting menu as unactive
        this.gameMenuCanvas.SetActive(false);
        this.menuPage.SetActive(true);
        this.optionsPage.SetActive(false);

        if(mixer != null)
        {
            //Unmute sound effects
            mixer.SetFloat("MasterVolume", this.savedVolume);
        }
    }

    public void backToMainMenu()
    {
        //Unloading main menu scene
        SceneManager.UnloadSceneAsync("building_map");

        //Loading in game scene
        SceneManager.LoadSceneAsync("Main_menu");
    }
}
