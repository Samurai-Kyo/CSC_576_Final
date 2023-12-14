using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class endMenu : MonoBehaviour
{

    public Button restartButton, startMenuButton;

    public Canvas mainCanvas;


    void RestartGame() {
        SceneManager.LoadScene("SampleScene");
    }

    void GoToMainMenu() {
        SceneManager.LoadScene("Mainmenu");
    }


    void Start() {
        restartButton.onClick.AddListener(RestartGame);
        startMenuButton.onClick.AddListener(GoToMainMenu);
    }

}
