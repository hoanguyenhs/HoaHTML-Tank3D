using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BotScript : NetworkBehaviour
{
    private GameObject gameManager;
    private GameObject blueBase;
    private GameObject blueAISpawningPoint;
    private GameObject redBase;
    private GameObject redAISpawningPoint;
    private float respawn;
    private float delay;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Manager_Game");
        blueBase = GameObject.Find("Base_BlueTeam");
        blueAISpawningPoint = blueBase.transform.Find("Base_SpawnAI").gameObject;
        redBase = GameObject.Find("Base_RedTeam");
        redAISpawningPoint = redBase.transform.Find("Base_SpawnAI").gameObject;
        respawn = Time.time + delay;
        delay = 7.5f;
    }

    // Update is called once per frame
    void Update()
    {
        SpawningTanks();
    }

    private void SpawningTanks()
    {
        if (Time.time > respawn && !gameManager.GetComponent<GameScript>().isEnd)
        {
            respawn = Time.time + delay;
            GameObject aiBlueTank = Instantiate(Resources.Load("AI_BlueTank")) as GameObject;
            aiBlueTank.transform.position = new Vector3(
                blueAISpawningPoint.transform.position.x,
                blueAISpawningPoint.transform.position.y + 4f,
                blueAISpawningPoint.transform.position.z); ;
            aiBlueTank.transform.Rotate(new Vector3(0f, 180f, 0.0f));
            NetworkServer.Spawn(aiBlueTank);

            GameObject aiRedTank = Instantiate(Resources.Load("AI_RedTank")) as GameObject;
            aiRedTank.transform.position = new Vector3(
                redAISpawningPoint.transform.position.x,
                redAISpawningPoint.transform.position.y + 4f,
                redAISpawningPoint.transform.position.z); ;
            NetworkServer.Spawn(aiRedTank);
        }
    }
}
