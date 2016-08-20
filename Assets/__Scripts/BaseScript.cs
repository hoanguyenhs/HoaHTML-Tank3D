using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BaseScript : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeHealth")]
    public float HP;
    [SyncVar(hook = "OnChangeIsAlive")]
    public bool isAlive;
    private GameObject firePoint;
    private GameObject gameManager;

    // Use this for initialization
    void Start()
    {
        HP = 10f;
        isAlive = true;
        firePoint = transform.Find("FirePoint").gameObject;
        gameManager = GameObject.Find("Manager_Game");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Bullet_Red") && gameObject.name.Contains("Blue"))
        {
            BeDestroyed();
        }
        else if (collision.gameObject.name.Contains("Bullet_Blue") && gameObject.name.Contains("Red"))
        {
            BeDestroyed();
        }
    }

    private void BeDestroyed()
    {
        HP--;
        if (HP <= 0 && isAlive)
        {
            isAlive = false;
            if (gameObject.name.Contains("Blue"))
            {
                gameManager.GetComponent<GameScript>().redScore += 5f;
            }
            else if (gameObject.name.Contains("Red"))
            {
                gameManager.GetComponent<GameScript>().blueScore += 5f;
            }
            CmdBeDestroyed();
        }
    }

    [Command]
    private void CmdBeDestroyed()
    {
        //for (int i = 0; i < gameObject.GetComponent<Renderer>().materials.GetLength(0); i++)
        //{
        //    gameObject.GetComponent<Renderer>().materials[i].color = Color.black;
        //}
        GameObject fire1 = Instantiate(Resources.Load("Fire")) as GameObject;
        GameObject fire2 = Instantiate(Resources.Load("Fire")) as GameObject;
        GameObject fire3 = Instantiate(Resources.Load("Fire")) as GameObject;
        GameObject fire4 = Instantiate(Resources.Load("Fire")) as GameObject;
        GameObject fire5 = Instantiate(Resources.Load("Fire")) as GameObject;
        fire1.transform.position = new Vector3(firePoint.transform.position.x + 4f, firePoint.transform.position.y, firePoint.transform.position.z + 4f);
        fire2.transform.position = new Vector3(firePoint.transform.position.x - 4f, firePoint.transform.position.y, firePoint.transform.position.z + 4f);
        fire3.transform.position = new Vector3(firePoint.transform.position.x, firePoint.transform.position.y, firePoint.transform.position.z);
        fire4.transform.position = new Vector3(firePoint.transform.position.x + 4f, firePoint.transform.position.y, firePoint.transform.position.z - 4f);
        fire5.transform.position = new Vector3(firePoint.transform.position.x - 4f, firePoint.transform.position.y, firePoint.transform.position.z - 4f);
        NetworkServer.Spawn(fire1);
        NetworkServer.Spawn(fire2);
        NetworkServer.Spawn(fire3);
        NetworkServer.Spawn(fire4);
        NetworkServer.Spawn(fire5);
    }

    private void OnChangeHealth(float newHP)
    {
        if (isAlive)
        {
            HP = newHP;
        }
    }

    private void OnChangeIsAlive(bool newValue)
    {
        isAlive = newValue;
    }
}
