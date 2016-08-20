using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AITankScript : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeHealth")]
    public float HP = 1f;
    [SyncVar(hook = "OnChangeIsAlive")]
    public bool isAlive;
    private bool isReady;
    private float think;
    private float delay;
    private float randomX;
    private float randomZ;
    private GameObject body;
    private GameObject bodyFront;
    private GameObject bodyBack;
    private GameObject turret;
    private GameObject cannon;
    private GameObject cannonTop;
    private GameObject cannonBack;
    private GameObject barrel;
    private GameObject barrelEffect;
    private float thinkMove;
    private float delayMove;
    private float thinkAim;
    private float delayAim;
    private float thinkFire;
    private float delayFire;

    private GameObject wheel_L1;
    private GameObject wheel_L2;
    private GameObject wheel_L3;
    private GameObject wheel_L4;
    private GameObject wheel_L5;
    private GameObject wheel_L6;
    private GameObject wheel_L7;
    private GameObject wheel_L8;
    private GameObject wheel_R1;
    private GameObject wheel_R2;
    private GameObject wheel_R3;
    private GameObject wheel_R4;
    private GameObject wheel_R5;
    private GameObject wheel_R6;
    private GameObject wheel_R7;
    private GameObject wheel_R8;
    private GameObject gameManager;

    // Use this for initialization
    void Start()
    {
        body = transform.Find("TankBody").gameObject;
        bodyFront = body.transform.Find("BodyFront").gameObject;
        bodyBack = body.transform.Find("BodyBack").gameObject;
        turret = body.transform.Find("TankTurret").gameObject;
        cannon = turret.transform.Find("TankCannon").gameObject;
        cannonTop = cannon.transform.Find("CannonTop").gameObject;
        barrel = cannon.transform.Find("CannonBarrel").gameObject;
        barrelEffect = barrel.transform.Find("BarrelEffect").gameObject;

        wheel_L1 = body.transform.Find("wheel_L1").gameObject;
        wheel_L2 = body.transform.Find("wheel_L2").gameObject;
        wheel_L3 = body.transform.Find("wheel_L3").gameObject;
        wheel_L4 = body.transform.Find("wheel_L4").gameObject;
        wheel_L5 = body.transform.Find("wheel_L5").gameObject;
        wheel_L6 = body.transform.Find("wheel_L6").gameObject;
        wheel_L7 = body.transform.Find("wheel_L7").gameObject;
        wheel_L8 = body.transform.Find("wheel_L8").gameObject;
        wheel_R1 = body.transform.Find("wheel_R1").gameObject;
        wheel_R2 = body.transform.Find("wheel_R2").gameObject;
        wheel_R3 = body.transform.Find("wheel_R3").gameObject;
        wheel_R4 = body.transform.Find("wheel_R4").gameObject;
        wheel_R5 = body.transform.Find("wheel_R5").gameObject;
        wheel_R6 = body.transform.Find("wheel_R6").gameObject;
        wheel_R7 = body.transform.Find("wheel_R7").gameObject;
        wheel_R8 = body.transform.Find("wheel_R8").gameObject;
        gameManager = GameObject.Find("Manager_Game");

        isAlive = true;
        isReady = false;
        thinkMove = Time.time + delayMove;
        delayMove = 30f;
        thinkAim = Time.time + delayAim;
        delayAim = 3f;
        thinkFire = Time.time + delayFire;
        delayFire = 1.5f;
        if (gameObject.name.Contains("Blue"))
        {
            randomX = transform.position.x + 15f;
            randomZ = transform.position.z + 40f;
        }
        else if (gameObject.name.Contains("Red"))
        {
            randomX = transform.position.x - 15f;
            randomZ = transform.position.z - 40f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAlive)
        {
            Think();
        }
        FixRotation();
    }
    private void FixRotation()
    {
        Vector3 angle = transform.localEulerAngles;
        angle.x = ClampAngle(angle.x, -45f, 45f);
        angle.z = ClampAngle(angle.z, -45f, 45f);
        transform.localEulerAngles = angle;
    }
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < 90 || angle > 270)
        {       // if angle in the critic region...
            if (angle > 180) angle -= 360;  // convert all angles to -180..+180
            if (max > 180) max -= 360;
            if (min > 180) min -= 360;
        }
        angle = Mathf.Clamp(angle, min, max);
        if (angle < 0) angle += 360;  // if angle negative, convert to 0..360
        return angle;
    }

    private void Think()
    {
        if (isReady)
        {
            // Think to Move
            if (Time.time > thinkMove)
            {
                thinkMove = Time.time + delayMove;
                randomX = UnityEngine.Random.Range(140f, 860f);
                randomZ = UnityEngine.Random.Range(50f, 1950f);
            }
            Vector3 destination = new Vector3(randomX, transform.position.y, randomZ);
            Move(destination);
            // Think to Aim
            Vector3 target = getTarget();
            if (Time.time > thinkAim)
            {
                thinkAim = Time.time + delayAim;
                target = getTarget();
            }
            Aim(target);
            // Think to Fire
            FireCannon(target);
        }
        else
        {
            Vector3 destination = new Vector3(randomX, transform.position.y, randomZ);
            Move(destination);
            float distance = Vector3.Distance(transform.position, destination);
            if (distance < 5f)
            {
                isReady = true;
            }
        }

    }

    private void FireCannon(Vector3 target)
    {
        float distance = Vector3.Distance(body.transform.position, target);
        if (distance <= 400)
        {
            if (Time.time > thinkFire)
            {
                thinkFire = Time.time + delayFire;
                CmdFire();
            }
        }
    }

    [Command]
    private void CmdFire()
    {
        GameObject cannonFlash = Instantiate(Resources.Load("Flash")) as GameObject;
        cannonFlash.transform.position = barrelEffect.transform.position;
        NetworkServer.Spawn(cannonFlash);
        Destroy(cannonFlash, 1f);

        if (gameObject.name.Contains("Red"))
        {
            GameObject redBullet = Instantiate(Resources.Load("Bullet_Red")) as GameObject;
            redBullet.transform.position = barrelEffect.transform.position;
            redBullet.transform.rotation = cannon.transform.rotation;
            redBullet.transform.GetComponent<Rigidbody>().useGravity = false;
            redBullet.transform.GetComponent<Rigidbody>().velocity = -redBullet.transform.forward * 400;
            NetworkServer.Spawn(redBullet);
        }
        else if (gameObject.tag.Contains("Blue"))
        {
            GameObject blueBullet = Instantiate(Resources.Load("Bullet_Blue")) as GameObject;
            blueBullet.transform.position = barrelEffect.transform.position;
            blueBullet.transform.rotation = cannon.transform.rotation;
            blueBullet.transform.GetComponent<Rigidbody>().useGravity = false;
            blueBullet.transform.GetComponent<Rigidbody>().velocity = -blueBullet.transform.forward * 400;
            NetworkServer.Spawn(blueBullet);
        }
    }

    private void Aim(Vector3 target)
    {
        // Aim the turret
        Quaternion targetRotation = Quaternion.LookRotation(turret.transform.position - target, Vector3.up);
        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * 500f);
        turret.transform.localEulerAngles = new Vector3(0, turret.transform.localEulerAngles.y, 0);
        // Aim the cannon
        if (cannonTop.transform.position.y - body.transform.position.y <= 3 || cannonTop.transform.position.y - body.transform.position.y >= 1.2)
        {
            targetRotation.x += 0f;
            cannon.transform.rotation = Quaternion.Slerp(cannon.transform.rotation, targetRotation, Time.deltaTime * 500f);
            cannon.transform.localEulerAngles = new Vector3(cannon.transform.localEulerAngles.x, 0, 0);
        }
    }

    private Vector3 getTarget()
    {
        GameObject target = null;
        GameObject[] enermies = new GameObject[10];
        if (gameObject.name.Contains("Blue"))
        {
            enermies = GameObject.FindGameObjectsWithTag("RedTeam");
        }
        else if (gameObject.name.Contains("Red"))
        {
            enermies = GameObject.FindGameObjectsWithTag("BlueTeam");
        }
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < enermies.GetLength(0); i++)
        {
            if (enermies[i].name.Contains("Base"))
            {
                if (enermies[i] != gameObject && enermies[i].GetComponent<BaseScript>().isAlive)
                {
                    float distance = Vector3.Distance(enermies[i].transform.position, transform.position);
                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        target = enermies[i];
                    }
                }
                else
                {
                    continue;
                }
            }
            else if (enermies[i].name.Contains("AI"))
            {
                if (enermies[i] != gameObject && enermies[i].GetComponent<AITankScript>().isAlive)
                {
                    float distance = Vector3.Distance(enermies[i].transform.position, transform.position);
                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        target = enermies[i];
                    }
                }
                else
                {
                    continue;
                }
            }
            else if (enermies[i].name.Contains("Player"))
            {
                if (enermies[i] != gameObject && enermies[i].GetComponent<TankScript>().isAlive)
                {
                    float distance = Vector3.Distance(enermies[i].transform.position, transform.position);
                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        target = enermies[i];
                    }
                }
                else
                {
                    continue;
                }
            }
        }
        return target.transform.position;
    }

    private void Move(Vector3 destination)
    {
        // Rotate the body 
        Quaternion destinationRotation = Quaternion.LookRotation(body.transform.position - destination, Vector3.up);
        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, destinationRotation, Time.deltaTime);
        // Move the body
        transform.position += (bodyFront.transform.position - bodyBack.transform.position) * 2f * Time.deltaTime;
        RotateWheel(true);
    }

    private void RotateWheel(bool forward)
    {
        if (forward)
        {
            wheel_L1.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.left);
            wheel_L2.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.left);
            wheel_L3.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.left);
            wheel_L4.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.left);
            wheel_L5.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.left);
            wheel_L6.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.left);
            wheel_L7.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.left);
            wheel_L8.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.left);
            wheel_R1.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.right);
            wheel_R2.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.right);
            wheel_R3.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.right);
            wheel_R4.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.right);
            wheel_R5.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.right);
            wheel_R6.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.right);
            wheel_R7.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.right);
            wheel_R8.transform.rotation *= Quaternion.AngleAxis(400 * Time.deltaTime, Vector3.right);
        }
        else
        {
            wheel_L1.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.right);
            wheel_L2.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.right);
            wheel_L3.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.right);
            wheel_L4.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.right);
            wheel_L5.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.right);
            wheel_L6.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.right);
            wheel_L7.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.right);
            wheel_L8.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.right);
            wheel_R1.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.left);
            wheel_R2.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.left);
            wheel_R3.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.left);
            wheel_R4.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.left);
            wheel_R5.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.left);
            wheel_R6.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.left);
            wheel_R7.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.left);
            wheel_R8.transform.rotation *= Quaternion.AngleAxis(200 * Time.deltaTime, Vector3.left);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Bullet_Red") && gameObject.name.Contains("Blue"))
        {
            HP--;
            if (HP <= 0 && isAlive)
            {
                gameManager.GetComponent<GameScript>().redScore += 1f;
                BeDestroyed();
            }
        }
        else if (collision.gameObject.name.Contains("Bullet_Blue") && gameObject.name.Contains("Red"))
        {
            HP--;
            if (HP <= 0 && isAlive)
            {
                gameManager.GetComponent<GameScript>().blueScore += 1f;
                BeDestroyed();
            }
        }
    }

    private void BeDestroyed()
    {
        isAlive = false;
        CmdBeDestroyed();
        turret.AddComponent<Rigidbody>();
        int num = UnityEngine.Random.Range(1, 4);
        switch (num)
        {
            case 1:
                turret.GetComponent<Rigidbody>().AddForce(Vector3.up + Vector3.forward * 1000f);
                break;
            case 2:
                turret.GetComponent<Rigidbody>().AddForce(Vector3.up + Vector3.back * 1000f);
                break;
            case 3:
                turret.GetComponent<Rigidbody>().AddForce(Vector3.up + Vector3.left * 1000f);
                break;
            case 4:
                turret.GetComponent<Rigidbody>().AddForce(Vector3.up + Vector3.right * 1000f);
                break;
        }
        turret.transform.Rotate(new Vector3(0, UnityEngine.Random.Range(90f, 180f), UnityEngine.Random.Range(90f, 180f)));
    }

    [Command]
    private void CmdBeDestroyed()
    {
        GameObject explosion = Instantiate(Resources.Load("Explosion_Tank")) as GameObject;
        explosion.transform.position = transform.position;
        NetworkServer.Spawn(explosion);
        Destroy(explosion, 2f);

        GameObject fire = Instantiate(Resources.Load("Fire")) as GameObject;
        fire.transform.position = new Vector3(body.transform.position.x, body.transform.position.y + 1.5f, body.transform.position.z);
        NetworkServer.Spawn(fire);
        Destroy(fire, 10f);
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
