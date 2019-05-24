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
    public GameObject m_RunnerPrefab;
    public CharacterManager[] m_characters;               // A collection of managers for enabling and disabling different aspects of the tanks.

    private int m_levelNumber = 0;                  // Which round the game is currently on.
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
    private CharacterManager m_LevelWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private CharacterManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.
    private bool winner = false;

    private bool replay = false;

    public float timeLeft = 10.0f;
    // here set up right? Bethany
    public WayPointMovement level;
    public GameObject m_WayPointPrefab;

    FollowerMovement fruitcounter;
    GameObject player;
    public Text m_message;

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

    void Update()
    {
        m_message.text = "Fruit: " + (m_characters[0].pumpkingetter()).ToString();

    }


    private void SpawnAllCharacters() //loops through and instantiates the two characters, follower and runner
    {
        // ... create them, set their player number and references needed for control.
        m_characters[0].m_Instance =
            Instantiate(m_FollowerPrefab, m_characters[0].m_SpawnPoint.position, m_characters[0].m_SpawnPoint.rotation) as GameObject;
        m_characters[1].m_Instance =
            Instantiate(m_RunnerPrefab, m_characters[1].m_SpawnPoint.position, m_characters[1].m_SpawnPoint.rotation) as GameObject;
        //m_characters[i].m_PlayerNumber = i + 1;
        m_characters[0].Setup();
        m_characters[1].Setup();
        // m_WayPointPrefab = Instantiate(m_WayPointPrefab, level.wayPointList[0].position, level.wayPointList[0].rotation) as GameObject;
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_characters.Length];
        // Create a collection of transforms the same size as the number of characters.

        for (int i = 0; i < targets.Length; i++)
        {
            // ... set it to the appropriate tank transform.
            targets[i] = m_characters[i].m_Instance.transform;
        }

        // These are the targets the camera should follow.
        m_CameraControl.m_Targets = targets;
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
        if (/*m_GameWinner*/ m_LevelWinner != null)
        {
            // If there is a game winner, restart the level.
            SceneManager.LoadScene("Level1"); //We will eventually change this to load the different scenes
        }
        else
        {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            SceneManager.LoadScene("Level1");
            StartCoroutine(GameLoop());
        }
    }

    private void ResetAllCharacters()   //reset characters
    {
        for (int i = 0; i < m_characters.Length; i++)
        {
            m_characters[i].Reset();
        }
    }


    private IEnumerator RoundStarting()
    {
        // As soon as the round starts reset the tanks and make sure they can't move.
        ResetAllCharacters();
        DisableCharacterControl();

        // Increment the round number and display text showing the players what round it is.
        if (replay == true)
        {
            m_MessageText.text = "Level " + m_levelNumber;
        }

        else
        {
            m_levelNumber++;
            m_MessageText.text = "Level " + m_levelNumber;
        }

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }

    private IEnumerator RoundPlaying()
    {
        // As soon as the round begins playing let the players control the tanks.
        EnableCharacterControl();

        // Clear the text from the screen.
        m_MessageText.text = string.Empty;

        // While there is not one tank left...
        while (!Playing())
        {
            yield return null;
        }
    }

    private CharacterManager GetRoundWinner() //this function checks if the character still exists, if so returns it as winner
    {
        //if the character reached the end in time, they won,
        if (winner == true)
        {
            return m_characters[0];
        }
        else return null; // return null if the winner did not win
    }


    private CharacterManager GetGameWinner() //returns the overall game winner
    {
        //if the number of wins is equal to the number of rounds needed to win...
        if (m_characters[0].m_Wins == m_NumRoundsToWin)
        {
            return m_characters[0]; //return that character as the game winner
        }

        return null;
    }

    private string EndMessage() // Returns a string message to display at the end of each round.
    {
        string message = "";

        if (m_LevelWinner != null)
        {
            message = "Level complete!" + m_levelNumber.ToString() + "/5";
            replay = false;

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";
        }
        else
        {
            replay = true;
            message = "Level Failed. Play Again?";
        }

        if (m_GameWinner != null)
        { // If there is a winner then change the message to reflect that.
            message = "You Caught the Runner!! You Won!";
        }

        return message;
    }

    private void EnableCharacterControl() //enables control of character
    {
        m_characters[0].EnableControl();
    }


    private void DisableCharacterControl() //diables control of follower
    {
        m_characters[0].DisableControl();
    }

    private IEnumerator RoundEnding()
    {
        timeLeft = 10f;

        // Stop tanks from moving.
        DisableCharacterControl();

        // Clear the winner from the previous round.
        m_LevelWinner = null;

        // See if there is a winner now the round is over.
        m_LevelWinner = GetRoundWinner();

        // If there is a winner, increment their score.
        if (m_LevelWinner != null)
        {
            m_LevelWinner.m_Wins++;
        }

        // Now the winner's score has been incremented, see if someone has one the game.
        m_GameWinner = GetGameWinner();

        if (m_GameWinner != null)
        {
            //Do something if game is completed
        }

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage();
        m_MessageText.text = message;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }

    private bool Playing() //this function checks if the user has lost the game
    {
        //once cat reaches trigger and dies start timer
        if (!m_characters[1].m_Instance.activeSelf)
        {
            StartCoroutine("LoseTime");
            if (!m_characters[0].m_Instance.activeSelf)
            {
                winner = true;
                return true;
            }
            if (timeLeft <= 0.00f)
            {
                return true;
            }
        }
        else
        { 
            return false;
        }
        return false;
    }

    IEnumerator LoseTime() // a coroutine that continues to run to decrement the countdown value
    {
        if (true)
        {
            timeLeft -= Time.deltaTime;
            m_MessageText.text = (timeLeft).ToString("0");
            if (timeLeft < 0)
            {
                m_MessageText.text = "";
                yield return m_EndWait;
            }
        }

    }
}