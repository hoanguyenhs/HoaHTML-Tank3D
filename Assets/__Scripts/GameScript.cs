using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class GameScript : NetworkBehaviour
{
    [SyncVar]
    public bool isEnd;
    [SyncVar]
    public float blueScore;
    [SyncVar]
    public float redScore;
    private bool showUI;

    // Use this for initialization
    void Start()
    {
        showUI = false;
        isEnd = false;
        blueScore = 0f;
        redScore = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        GameCondition();
    }

    private void GameCondition()
    {
        if (!isEnd)
        {
            if (redScore > 99)
            {
                isEnd = true;
                showUI = true;
            }
            else if (blueScore > 99)
            {
                isEnd = true;
                showUI = true;
            }
        }
    }

    void OnGUI()
    {
        if (!showUI)
        {
            return;
        }

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
        Texture texture = Resources.Load("Win_Blue") as Texture;
        if (redScore > blueScore)
        {
            texture = Resources.Load("Win_Red") as Texture;
        }
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);

        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 175, 100, 30), "Back"))
        {
            showUI = false;
            GameObject.Find("Manager_Network").GetComponent<CustomNetworkManager>().StopHost();
        }
    }
}
