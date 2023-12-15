using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class footsteps : MonoBehaviour
{

    public CharacterController controller;

    private GameManager gm;
    public AudioSource walk_src, run_src, crouch_src;
    public AudioClip footstep, run, crouch;

    [Range(0f, 1f)]
    public float volume = 0.75f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.loadSettings();

        // Walking sound
        walk_src = this.AddComponent<AudioSource>();
        walk_src.clip = footstep;
        walk_src.loop = true;
        walk_src.volume = gm.volume;
        walk_src.Play();

        // Running sound
        run_src = this.AddComponent<AudioSource>();
        run_src.clip = run;
        run_src.loop = true;
        run_src.volume = gm.volume;
        run_src.Play();

        // Crouch sound
        crouch_src = this.AddComponent<AudioSource>();
        crouch_src.clip = crouch;
        crouch_src.loop = true;
        crouch_src.volume = gm.volume;
        crouch_src.Play();
    }

    void Update()
    {

        walk_src.volume = 0;
        run_src.volume = 0;
        crouch_src.volume = 0;


        // If on the ground and moving
        if (controller.isGrounded && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            AudioSource src = Input.GetKey(KeyCode.LeftShift) ? run_src : Input.GetKey(KeyCode.C) ? crouch_src : walk_src;
            src.volume = gm.volume;
        }


    }
}
