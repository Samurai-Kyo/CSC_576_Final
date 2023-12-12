using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class mainMenu : MonoBehaviour
{

    public Button startButton, instructionsButton, controlsButton, settingsButton;
    public Button backButtonInstructions, backButtonControls, backButtonSettings;
    public Button confirmButton;
    public Canvas mainCanvas, instructionsCanvas, controlsCanvas, settingsCanvas;

    public Slider volume, difficulty;

    public string filepath;


    void startGame() {
        SceneManager.LoadScene("SampleScene");
    }

    void showStart() {
        mainCanvas.gameObject.SetActive(true);
        instructionsCanvas.gameObject.SetActive(false);
        controlsCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(false);
    }

    void showInstructions() {
        mainCanvas.gameObject.SetActive(false);
        instructionsCanvas.gameObject.SetActive(true);
        controlsCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(false);
    }

    void showControls() {
        mainCanvas.gameObject.SetActive(false);
        instructionsCanvas.gameObject.SetActive(false);
        controlsCanvas.gameObject.SetActive(true);
        settingsCanvas.gameObject.SetActive(false);
    }

    void showSettings()
    {
        mainCanvas.gameObject.SetActive(false);
        instructionsCanvas.gameObject.SetActive(false);
        controlsCanvas.gameObject.SetActive(false);
        settingsCanvas.gameObject.SetActive(true);
    }

    // Write values to file that will be parsed by GameManager during setup for the main scene
    public void confirmSettings() {
        float vol = volume.value;
        float diff = difficulty.value;

        File.WriteAllText(filepath, string.Format("{0},{1}", vol, diff));
    }


    void Start() {
        // Setup filepath where we will write the settings values to
        filepath = Path.Combine(Application.persistentDataPath, "settings.txt");

        startButton.onClick.AddListener(startGame);
        instructionsButton.onClick.AddListener(showInstructions);
        controlsButton.onClick.AddListener(showControls);
        settingsButton.onClick.AddListener(showSettings);

        confirmButton.onClick.AddListener(confirmSettings);

        backButtonInstructions.onClick.AddListener(showStart);
        backButtonControls.onClick.AddListener(showStart);
        backButtonSettings.onClick.AddListener(showStart);

    }


}
