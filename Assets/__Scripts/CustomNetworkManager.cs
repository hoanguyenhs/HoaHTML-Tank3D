using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public int teamID = 0;
    private bool showUI;
    private int status = 0;
    private GameObject camera;
    private GameObject canvas;
    private GameObject blueBase = null;
    private GameObject blueSpawningPoint = null;
    private GameObject redBase = null;
    private GameObject redSpawningPoint = null;
    private GameObject gameManager;

    void Start()
    {
        showUI = false;
        status = 0;
        //if (!PlayerPrefs.HasKey("NetworkMessage"))
        //{
        //    PlayerPrefs.SetString("NetworkMessage", "");
        //}
        camera = transform.Find("Camera").gameObject;
        canvas = camera.transform.Find("Canvas").gameObject;
        //canvas.transform.Find("Status").GetComponent<Text>().text = PlayerPrefs.GetString("NetworkMessage");
        blueBase = GameObject.Find("Base_BlueTeam");
        blueSpawningPoint = blueBase.transform.Find("Base_Spawn").gameObject;
        redBase = GameObject.Find("Base_RedTeam");
        redSpawningPoint = redBase.transform.Find("Base_Spawn").gameObject;
        gameManager = GameObject.Find("Manager_Game");
    }

    void Update()
    {
        ShowUI();
        if (status == 1)
        {
            Invoke("CheckErrorHost", 2f);
        }
        else if (status == 2)
        {
            Invoke("CheckErrorClient", 2f);
        }
    }

    void OnApplicationQuit()
    {
        //PlayerPrefs.SetString("NetworkMessage", "");
    }

    private void CheckErrorHost()
    {
        if (ClientScene.localPlayers.Count == 0)
        {
            StopHost();
            SceneManager.LoadScene("_Scene_Play");
        }
    }

    private void CheckErrorClient()
    {
        if (ClientScene.localPlayers.Count == 0)
        {
            StopClient();
            SceneManager.LoadScene("_Scene_Play");
        }
    }

    public class TeamInfo : MessageBase
    {
        public int teamID;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        TeamInfo message = extraMessageReader.ReadMessage<TeamInfo>();
        int selectedTeam = message.teamID;
        //PlayerPrefs.SetString("NetworkMessage", "");
        if (selectedTeam == 0)
        {
            GameObject player = Instantiate(Resources.Load("Player_BlueTank", typeof(GameObject))) as GameObject;
            player.transform.position = new Vector3(
                blueSpawningPoint.transform.position.x,
                blueSpawningPoint.transform.position.y + 4f,
                blueSpawningPoint.transform.position.z);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
        if (selectedTeam == 1)
        {
            GameObject player = Instantiate(Resources.Load("Player_RedTank", typeof(GameObject))) as GameObject;
            player.transform.position = new Vector3(
                redSpawningPoint.transform.position.x,
                redSpawningPoint.transform.position.y + 4f,
                redSpawningPoint.transform.position.z);
            player.transform.Rotate(new Vector3(0, 180, 0));
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        TeamInfo test = new TeamInfo();
        test.teamID = teamID;
        ClientScene.AddPlayer(conn, 0, test);
    }

    public void ChangeColor()
    {
        Slider teamPicker = canvas.transform.Find("TeamPicker").GetComponent<Slider>();
        if (teamPicker.value == 0)
        {
            teamPicker.handleRect.GetComponent<Image>().color = Color.blue;
        }
        else
        {
            teamPicker.handleRect.GetComponent<Image>().color = Color.red;
        }
    }

    public void Host()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            status = 1;
            //PlayerPrefs.SetString("NetworkMessage", "Cannot create host on this IP Address");
            teamID = (int)canvas.transform.Find("TeamPicker").GetComponent<Slider>().value;
            camera.GetComponent<AudioListener>().enabled = false;
            camera.SetActive(false);
            StartHost();
        }
    }

    public void Join()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            status = 2;
            //PlayerPrefs.SetString("NetworkMessage", "Cannot connect to this IP Address");
            teamID = (int)canvas.transform.Find("TeamPicker").GetComponent<Slider>().value;
            camera.GetComponent<AudioListener>().enabled = false;
            camera.SetActive(false);
            networkAddress = canvas.transform.Find("IPAddress").GetComponent<InputField>().text;
            StartClient();
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("_Scene_Main");
    }

    private void ShowUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (camera.activeSelf)
            {
                return;
            }
            showUI = true;
        }
    }

    void OnGUI()
    {
        if (!showUI)
        {
            return;
        }

        GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100), "Confirmation");
        GUI.Label(new Rect(Screen.width / 2 - 110, Screen.height / 2 - 30, 220, 30), "Are you sure to aboddan this match?");
        if (GUI.Button(new Rect(Screen.width / 2 - 125, Screen.height / 2, 100, 30), "Yes"))
        {
            if (NetworkServer.active)
            {
                StopHost();
            }
            if (NetworkClient.active)
            {
                StopClient();
            }
            showUI = false;
        }
        if (GUI.Button(new Rect(Screen.width / 2 + 25, Screen.height / 2, 100, 30), "No"))
        {
            showUI = false;
        }
    }
}
