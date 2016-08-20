using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class TankScript : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeHealth")]
    public float HP = 10f;
    [SyncVar(hook = "OnChangeIsAlive")]
    public bool isAlive = true;

    // For tank action
    private GameObject body;
    private GameObject bodyFront;
    private GameObject bodyBack;
    private GameObject turret;
    private GameObject cannon;
    private GameObject barrel;
    private GameObject barrelEffect;
    private GameObject camera;
    private float nextFire;
    private float reload;

    // For HUD
    private GameObject canvas;
    private GameObject hpText;
    private GameObject ammoText;
    private GameObject countdownText;
    private GameObject blueScoreText;
    private GameObject redScoreText;
    private GameObject gameManager;

    // For tank wheel
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

    // For Respawn
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private GameObject aliveCamPos;
    private GameObject deadCamPos;
    private GameObject aliveTurretPos;
    private float countdown;
    private float respawn;

    // Use this for initialization
    void Start()
    {
        // For tank action
        body = transform.Find("TankBody").gameObject;
        bodyFront = body.transform.Find("BodyFront").gameObject;
        bodyBack = body.transform.Find("BodyBack").gameObject;
        turret = transform.Find("TankTurret").gameObject;
        cannon = turret.transform.Find("TankCannon").gameObject;
        barrel = cannon.transform.Find("CannonBarrel").gameObject;
        barrelEffect = barrel.transform.Find("BarrelEffect").gameObject;
        camera = cannon.transform.Find("TankCamera").gameObject;
        nextFire = Time.time + reload;
        reload = 1.5f;

        // For HUD
        canvas = camera.transform.Find("Canvas").gameObject;
        hpText = canvas.transform.Find("HPText").gameObject;
        hpText.GetComponent<Text>().text = "+  " + 10 * 10f;
        ammoText = canvas.transform.Find("AmmoText").gameObject;
        ammoText.GetComponent<Text>().text = "I";
        countdownText = canvas.transform.Find("CountdownText").gameObject;
        countdownText.SetActive(false);
        blueScoreText = canvas.transform.Find("BlueScoreText").gameObject;
        redScoreText = canvas.transform.Find("RedScoreText").gameObject;
        gameManager = GameObject.Find("Manager_Game");

        // For tank wheels
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

        // For Respawn
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        aliveCamPos = cannon.transform.Find("AliveCamPos").gameObject;
        deadCamPos = transform.Find("DeadCamPos").gameObject;
        aliveTurretPos = transform.Find("AliveTurretPos").gameObject;
        respawn = 10f;

        if (!isLocalPlayer)
        {
            camera.SetActive(false);
            camera.GetComponent<AudioListener>().enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            camera.SetActive(false);
            camera.GetComponent<AudioListener>().enabled = false;
            return;
        }
        if (isAlive)
        {
            Move();
            Aim();
            Fire();
        }
        BeDestroyed();
        UpdateScore();
        FixRotation();
        Cheat();
    }

    private void Cheat()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (reload == 2f)
            {
                reload = 0.5f;
            }
            else
            {
                reload = 2f;
            }
        }
    }

    private void FixRotation()
    {
        Vector3 angle = transform.localEulerAngles;
        angle.x = ClampAngle(angle.x, -45f, 45f);
        angle.z = ClampAngle(angle.z, -45f, 45f);
        transform.localEulerAngles = angle;
    }

    private void UpdateScore()
    {
        blueScoreText.GetComponent<Text>().text = (gameManager.GetComponent<GameScript>().blueScore * 10f) + "";
        redScoreText.GetComponent<Text>().text = (gameManager.GetComponent<GameScript>().redScore * 10f) + "";
    }

    private void Fire()
    {
        if (Time.time > nextFire)
        {
            ammoText.GetComponent<Text>().text = "I";
            if (Input.GetKey(KeyCode.Space))
            {
                ammoText.GetComponent<Text>().text = " ";
                nextFire = Time.time + reload;
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
            redBullet.transform.GetComponent<Rigidbody>().velocity = -redBullet.transform.forward * 400;
            NetworkServer.Spawn(redBullet);
        }
        else if (gameObject.tag.Contains("Blue"))
        {
            GameObject blueBullet = Instantiate(Resources.Load("Bullet_Blue")) as GameObject;
            blueBullet.transform.position = barrelEffect.transform.position;
            blueBullet.transform.rotation = cannon.transform.rotation;
            blueBullet.transform.GetComponent<Rigidbody>().velocity = -blueBullet.transform.forward * 400;
            NetworkServer.Spawn(blueBullet);
        }
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

    private void Aim()
    {
        float rotationX = Input.GetAxis("Mouse X");
        float rotationZ = Input.GetAxis("Mouse Y");

        turret.transform.Rotate(Vector3.up, Mathf.Deg2Rad * rotationX * 100);
        cannon.transform.Rotate(Vector3.right, Mathf.Deg2Rad * rotationZ * 100);
        Vector3 angle = cannon.transform.localEulerAngles;
        angle.x = ClampAngle(angle.x, -15f, 20f);
        cannon.transform.localEulerAngles = angle;
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

    private void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += (bodyFront.transform.position - bodyBack.transform.position) * 2f * Time.deltaTime;
            RotateWheel(true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= (bodyFront.transform.position - bodyBack.transform.position) * 1.5f * Time.deltaTime;
            RotateWheel(false);
        }
        if (Input.GetKey(KeyCode.A))
        {
            body.transform.Rotate(new Vector3(0.0f, -0.5f * 2f, 0.0f));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            body.transform.Rotate(new Vector3(0.0f, 0.5f * 2f, 0.0f));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Bullet_Red") && gameObject.name.Contains("Blue"))
        {
            if (HP > 0)
            {
                HP -= 2f;
            }
            else if (isAlive && HP <= 0)
            {
                HP = 8f;
            }
        }
        else if (collision.gameObject.name.Contains("Bullet_Blue") && gameObject.name.Contains("Red"))
        {
            if (HP > 0)
            {
                HP -= 2f;
            }
            else if (isAlive && HP <= 0)
            {
                HP = 8f;
            }
        }
    }

    private void BeDestroyed()
    {
        if (HP <= 0 && isAlive)
        {
            if (gameObject.name.Contains("Blue"))
            {
                gameManager.GetComponent<GameScript>().redScore += 2f;
            }
            else if (gameObject.name.Contains("Red"))
            {
                gameManager.GetComponent<GameScript>().blueScore += 2f;
            }
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
            isAlive = false;
            countdown = Time.time + 5f;
        }
        else if (HP <= 0 && !isAlive)
        {
            respawn -= Time.deltaTime;
            countdownText.SetActive(true);
            countdownText.GetComponent<Text>().text = (int)respawn + "";
            if (Time.time > countdown)
            {
                camera.transform.position = deadCamPos.transform.position;
                camera.transform.rotation = deadCamPos.transform.rotation;
            }
            if (respawn <= 0)
            {
                HP = 10f;
                hpText.GetComponent<Text>().text = "+  " + HP * 10f;
                countdown = Time.time + 5f;
                respawn = 10f;
                Destroy(turret.GetComponent<Rigidbody>());
                transform.position = originalPosition;
                transform.rotation = originalRotation;
                body.transform.rotation = originalRotation;
                turret.transform.position = aliveTurretPos.transform.position;
                turret.transform.rotation = aliveTurretPos.transform.rotation;
                camera.transform.position = aliveCamPos.transform.position;
                camera.transform.rotation = aliveCamPos.transform.rotation;
                countdownText.SetActive(false);
                countdownText.GetComponent<Text>().text = "9";
                isAlive = true;
            }
        }
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
        if (newHP >= 0 && isAlive)
        {
            HP = newHP;
            hpText.GetComponent<Text>().text = "+  " + HP * 10f;
        }
    }

    private void OnChangeIsAlive(bool newValue)
    {
        isAlive = newValue;
    }
}
