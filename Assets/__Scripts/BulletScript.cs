using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletScript : NetworkBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Red") || collision.gameObject.name.Contains("Blue"))
        {
            CmdCollisionTank(collision.gameObject);
        }
        else
        {
            CmdCollisionObject();
        }
        Destroy(gameObject);
    }

    [Command]
    private void CmdCollisionTank(GameObject tank)
    {
        if (tank.name.Contains("Tank"))
        {
            GameObject flash = Instantiate(Resources.Load("Collision_Tank")) as GameObject;
            flash.transform.position = new Vector3(tank.transform.position.x, tank.transform.position.y + 2, tank.transform.position.z);
            NetworkServer.Spawn(flash);
            Destroy(flash, 2f);
        }
        else
        {
            GameObject flash = Instantiate(Resources.Load("Collision_Tank")) as GameObject;
            flash.transform.position = transform.position;
            NetworkServer.Spawn(flash);
            Destroy(flash, 2f);
        }
    }

    [Command]
    private void CmdCollisionObject()
    {
        GameObject smallExplosion = Instantiate(Resources.Load("Collision_Object")) as GameObject;
        smallExplosion.transform.position = transform.position;
        NetworkServer.Spawn(smallExplosion);
        Destroy(smallExplosion, 2f);
    }
}
