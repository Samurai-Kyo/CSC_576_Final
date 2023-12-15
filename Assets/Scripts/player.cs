using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    // Camera
    public Camera view;
    private Vector3 originalCameraPosition;

    public GameObject flashlight;
    public CharacterController controller;
    private GameManager gm;

    // Keep track of if game is paused
    public bool paused = false;
    public Canvas pauseMenu;
    public Slider volumeSlider;

    // Stats
    public int health;
    public float stamina = 100;


    // Audio
    public AudioSource src;
    public AudioClip click;

    public float crouchSpeed = 2f;
    public bool isCrouching = false;
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public bool isRunning = false;
    public float stamina_cost = 15f; // rate at which stamina drains/recovers
    public float jumpSpeed = 6f;    // How fast the player moves vertically when they start jumping
    public float y_velocity = 0f;   // keep track of how fast player is moving vertically
    public float gravity = 9.81f;   // acceleration due to gravity

    public float lookSensitivity = 2f;  // controls speed of camera rotation

    public Vector3 movement_direction = Vector3.zero; // Unit vector that points in direction player is moving
    public Vector3 facingDirection = Vector3.zero;    // Unit vector in XZ plane that keeps track of direction the player is facing


    void Start()
    {
        controller = GetComponent<CharacterController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.loadSettings();

        // Audio
        src = this.AddComponent<AudioSource>();

        // Get camera positions
        originalCameraPosition = view.transform.localPosition;

        // Hide cursor when playing
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set player health according to difficulty;
        health = 3 - gm.difficulty;
    }

    void handleCamera()
    {

        // Get direction player is facing
        float xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        float zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        facingDirection = new Vector3(xdirection, 0, zdirection);

        // Camera rotation
        float x_rotation = Input.GetAxis("Mouse X") * lookSensitivity;
        float y_rotation = Input.GetAxis("Mouse Y") * lookSensitivity;
        y_rotation = Mathf.Clamp(y_rotation, -90, 90);

        // We want to rotate player left/right and camera up/down
        transform.Rotate(Vector3.up, x_rotation);
        view.transform.Rotate(Vector3.left, y_rotation);

        // Lower camera if crouching
        if (isCrouching)
        {
            Vector3 nextPos = view.transform.localPosition;
            nextPos.y -= Time.deltaTime * 3f;
            nextPos.y = Mathf.Max(nextPos.y, originalCameraPosition.y - 0.4f);
            view.transform.localPosition = nextPos;
        }
        else
        {
            Vector3 nextPos = view.transform.localPosition;
            nextPos.y += Time.deltaTime * 3f;
            nextPos.y = Mathf.Min(nextPos.y, originalCameraPosition.y);
            view.transform.localPosition = nextPos;
        }
    }

    void handleMovement()
    {

        // X-Z directions control horizontal movement
        // Y direction controls vertical movement

        float speed = 0f;
        Vector3 move = Vector3.zero;

        // toggle flashlight
        if (Input.GetMouseButtonDown(0))
        {
            flashlight.GetComponentInChildren<Light>().enabled = !flashlight.GetComponentInChildren<Light>().enabled;
            src.PlayOneShot(click, gm.volume);
        }

        isRunning = Input.GetKey(KeyCode.LeftShift);
        isCrouching = isRunning ? false : Input.GetKey(KeyCode.C); // cant be running and crouching at same time

        if (Input.GetKey(KeyCode.W)) // forward movement
        {
            speed = walkSpeed;
            if (isRunning && stamina > 0)
            {
                speed = runSpeed;
                stamina -= stamina_cost * Time.deltaTime;
            }
            move = facingDirection;
        }
        if (Input.GetKey(KeyCode.S)) // backward movement
        {
            speed = walkSpeed;
            move = -facingDirection;
        }
        if (Input.GetKey(KeyCode.A)) // strafe left
        {
            speed = walkSpeed;
            move += Quaternion.Euler(0, -90, 0) * facingDirection; // multiplying by the quaternion performs a rotation
        }
        else if (Input.GetKey(KeyCode.D)) // strafe right
        {
            speed = walkSpeed;
            move += Quaternion.Euler(0, 90, 0) * facingDirection;
        }

        // Crouching
        if (isCrouching)
        {
            speed = crouchSpeed;
            // Camera change done in handleCamera()
        }


        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            y_velocity = jumpSpeed;
        }

        // Apply horizontal speed
        move = move.normalized * speed;

        // Add gravity if not grounded
        y_velocity -= controller.isGrounded ? 0 : gravity * Time.deltaTime;
        move.y = y_velocity;

        movement_direction = move;
        controller.Move(move * Time.deltaTime);

        // Recover stamina when not running
        if (!isRunning)
        {
            stamina += stamina_cost * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 100);
        }

    }

    void TogglePause()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!paused)
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // UI
            pauseMenu.gameObject.SetActive(true);
            volumeSlider.value = gm.volume;
            paused = true;
        }
        else
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // UI
            gm.volume = volumeSlider.value;
            pauseMenu.gameObject.SetActive(false);
            paused = false;
        }
    }


    void Update()
    {

        // Pause game
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        if (!paused && health >= 1)
        {
            handleMovement();
            handleCamera();
        }
    }
}
