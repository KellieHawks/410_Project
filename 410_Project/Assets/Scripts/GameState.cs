using UnityEngine;
using System.Collections;

public static class GameState {
    // Use this for initialization
    public static int m_levelNumber = 0;
    public static int levelinc = 0;
    public static int m_NumRoundsToWin = 5;
    public static float m_StartDelay = 3f;
    public static float m_EndDelay = 3f;
    public static bool replay = false;
    public static bool winner = false;
    //public static bool gamewinner = false;
}
