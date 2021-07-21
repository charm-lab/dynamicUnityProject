using UnityEngine;
using UnityEditor;

public static class MyTools
{
    [MenuItem("My Tools/Add To Report %F1")]
    static void DEV_appendToReport()
    {
        Debug.Log("PRESSED SAVE!");
    }

    [MenuItem("My Tools/Start Game %F2")]
    static void DEV_startGame()
    {
        Debug.Log("<color=green>Game started!</color>");
    }
}
