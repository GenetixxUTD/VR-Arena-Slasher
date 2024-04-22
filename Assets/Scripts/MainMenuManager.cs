using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public float rotationRate;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationRate, 0);
    }

    public void toGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void toQuitGame()
    {
        Application.Quit();
    }
}
