using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayGame()
    {
        SceneManager.LoadScene("_Scene_Play");
    }

    public void Options()
    {
        SceneManager.LoadScene("_Scene_Options");
    }

    public void Back()
    {
        SceneManager.LoadScene("_Scene_Main");
    }
}
