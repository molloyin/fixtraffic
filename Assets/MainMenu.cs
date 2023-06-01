using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private const string gameScene = "building_map"; //Variable for game scene

    /**<summary>
     * Is used to quit the game.
     * </summary>
     */
    public void exitGame()
    {
        Application.Quit(); //For if game is built
        UnityEditor.EditorApplication.isPlaying = false; //For in editor
    }


    /**<summary>
     * Loads in the game scene and unloads the main menu scene
     * </summary>
     */
    public void startGame()
    {
        //Unloading main menu scene
        SceneManager.UnloadSceneAsync("Main_Menu");

        //Loading in game scene
        SceneManager.LoadSceneAsync(gameScene);

    }



    /**<summary>
     * Opens up the options  page in the main menu
     * </summary>
     */
    public void openOptions()
    {
        Debug.Log("Opening main menu");
    }

    /**<summary>
     * Closes the main menu
     * </summary>
     */
    public void closeOptions()
    {
        Debug.Log("Closing main menu");
    }
}
