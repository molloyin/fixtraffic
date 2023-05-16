using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    /**<summary>
     * Is used to quit the game.
     * </summary>
     */
    public void exitGame()
    {
        Application.Quit(); //For if game is built
        UnityEditor.EditorApplication.isPlaying = false; //For in editor
    }

}
