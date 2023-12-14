using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerUI : MonoBehaviour
{

    // Player reference
    private GameObject player;

    // Health UI
    public Image[] hearts;

    // Flares UI
    public Image[] flares;

    // Stamina UI
    public GameObject stamina_bar;
    public float stamina_bar_initial_scale;

    // Coordinates UI
    public Text coordsText;

    // Death UI
    public Button playAgainButton, mainMenuButton;
    public GameObject deathMessage;

    void Start()
    {
        player = this.gameObject;
        stamina_bar_initial_scale = stamina_bar.transform.localScale.x;

        playAgainButton.onClick.AddListener(() => SceneManager.LoadScene("SampleScene"));
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        deathMessage.SetActive(false);

        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Display correct number of hearts based on health
    void HealthUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < GetComponent<player>().health)
            {
                hearts[i].gameObject.SetActive(true);
            }
            else
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
    }

    // Display correct number of flares in UI
    void FlareUI()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < GetComponent<toss>().numFlares)
            {
                flares[i].gameObject.SetActive(true);
            }
            else
            {
                flares[i].gameObject.SetActive(false);
            }
        }
    }

    // Handles the animation for the stamina bar
    void StaminaUI()
    {
        float stamina = player.GetComponent<player>().stamina;
        Vector3 scale = stamina_bar.transform.localScale;
        scale.x = Mathf.Lerp(0, stamina_bar_initial_scale, stamina / 100);
        stamina_bar.transform.localScale = scale;
    }

    void CoordsTextUI()
    {
        int x = (int)player.transform.position.x;
        int z = (int)player.transform.position.z;
        string pos = string.Format("({0}, {1})", x, z);
        coordsText.text = pos;
    }

    void DeathUI()
    {
        if (GetComponent<player>().health <= 0)
        {
            // Free cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // show death UI (restart button + main menu button)
            deathMessage.gameObject.SetActive(true);
        }
    }


    void Update()
    {
        HealthUI();
        FlareUI();
        StaminaUI();
        CoordsTextUI();
        DeathUI();
    }
}
