using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
    public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
    public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
    public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
    public GameObject m_FollowerPrefab;             // Reference to the prefab the players will control.
    public CharacterManager[] m_characters;               // A collection of managers for enabling and disabling different aspects of the tanks.


    private int m_levelNumber = 0;                  // Which round the game is currently on.
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
    private CharacterManager m_LevelWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private CharacterManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.

    /*
    General Notes on Set up: Kellie
    m_characters[0] - follower
    m_characters[1] - runner
    */

    private void Start()
    {
        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllCharacters();
        SetCameraTargets();

        // Once the games assets have been created and the camera is following the cat, start the gameloop
        StartCoroutine(GameLoop());
    }


    private void SpawnAllCharacters() //loops through and instantiates the two characters, follower and runner
    {
        for (int i = 0; i < m_characters.Length; i++)
        {
            // ... create them, set their player number and references needed for control.
            m_characters[i].m_Instance =
                Instantiate(m_FollowerPrefab, m_characters[i].m_SpawnPoint.position, m_characters[i].m_SpawnPoint.rotation) as GameObject;
            m_characters[i].m_PlayerNumber = i + 1;
            m_characters[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        // Create a collection of transforms the same size as the number of characters.
        // m_CameraControl.player = m_CameraControl.player;
    }


    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop()
    {
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundStarting());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine(RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
        if (m_GameWinner != null)
        {
            // If there is a game winner, restart the level.
            SceneManager.LoadScene(0); //We will eventually change this to load the different scenes
        }
        else
        {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        // As soon as the round starts reset the tanks and make sure they can't move.
        ResetAllCharacters();
        DisableCharacterControl();

        // Snap the camera's zoom and position to something appropriate for the reset tanks.
        //m_CameraControl.SetStartPositionAndSize();

        // Increment the round number and display text showing the players what round it is.
        m_levelNumber++;
        m_MessageText.text = "Level " + m_levelNumber;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }

    //NEED TO CHANGE SO IT TAKES INTO ACCOUNT IF THE CHARACTER HAS BEEN OUTSIDE OF CAMERA
    private IEnumerator RoundPlaying()
    {
        // As soon as the round begins playing let the players control the tanks.
        EnableCharacterControl();

        // Clear the text from the screen.
        m_MessageText.text = string.Empty;

        // While there is not one tank left...
        while (!CheckLost())
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        // Stop tanks from moving.
        DisableCharacterControl();

        // Clear the winner from the previous round.
        m_LevelWinner = null;

        // See if there is a winner now the round is over.
        m_LevelWinner = GetRoundWinner();

        // If there is a winner, increment their score.
        if (m_LevelWinner != null)
            m_LevelWinner.m_Wins++;

        // Now the winner's score has been incremented, see if someone has one the game.
        m_GameWinner = GetGameWinner();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage();
        m_MessageText.text = message;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }


    private bool CheckLost() //this function checks if the user has lost the game
    {
        // ... and if they are active, increment the counter.
        if (m_characters[0].m_Instance.activeSelf) {
            return false;
        }
                
        return true;
    }

    private CharacterManager GetRoundWinner() //this function checks if the character still exists, if so returns it as winner
    {
        // ... and if one of them is active, it is the winner so return it.
        if (m_characters[0].m_Instance.activeSelf) { 
            return m_characters[0];
        }

        // If none of the tanks are active it is a draw so return null.
        return null;
    }

    // This function is to find out if there is a winner of the game.
    private CharacterManager GetGameWinner() //returns the overall game winner
    {
        // Go through all the tanks...

        // ... and if one of them has enough rounds to win the game, return it.
        if (m_characters[0].m_Wins == m_NumRoundsToWin)
        {
            return m_characters[0];
        }
      
        // if user has not beat the game, return null
        return null;
    }


    private string EndMessage() // Returns a string message to display at the end of each round.
    {
        string message = "";

        if (m_LevelWinner != null)
        {
            message = "Level complete!" + m_levelNumber + "/5";

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";
        }

        if (m_GameWinner != null)
        { // If there is a winner then change the message to reflect that.
            message = "You Caught the Runner!!";
        }
        return message;
    }

    private void ResetAllCharacters()   //reset characters
    {
        for (int i = 0; i < m_characters.Length; i++)
        {
            m_characters[i].Reset();
        }
    }


    private void EnableCharacterControl() //enables control of character
    {
        m_characters[0].EnableControl();
    }


    private void DisableCharacterControl() //diables control of follower
    {
        m_characters[0].DisableControl();
    }
}