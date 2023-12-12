using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RobotBase : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    protected GameObject player;
    protected GameObject flare_to_follow;

    // Movement variables
    public Vector3 target = Vector3.zero;   // target location to move to
    public float speed;
    public float rotationSpeed = 15f;
    public float walkSpeed = 2f;
    public float jogSpeed = 4f;
    public float runSpeed = 6f;
    public float jumpSpeed = 6f;    // How fast the player moves vertically when they start jumping
    public float y_velocity = 0f;   // keep track of how fast player is moving vertically
    public float gravity = 9.81f;   // acceleration due to gravity


    // delay in seconds between robot sensing its surroundings
    public float senseDelay = 0.2f;

    public bool is_wandering = true;
    public float target_error = 1f;  // how many meters the robot can be off from its target before it stops moving
    public float wanderDistance = 20f; // how far robot wander between stops

    public bool sensed_player_recently = false;
    public float sensed_timestamp = -1;
    public float sensedSeconds = 10; // number of seconds sensed_player_recently will be true for after sensing the player


    // Audio stuff
    protected AudioSource walk_src, run_src, other_src;
    public AudioClip walk_clip, run_clip, wander_clip, seen_clip;


    protected void Init()
    {
        // Movement setup
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player");


        // Audio setup
        walk_src = this.AddComponent<AudioSource>();
        walk_src.clip = walk_clip;
        walk_src.loop = true;
        walk_src.spatialBlend = 1.0f;
        walk_src.rolloffMode = AudioRolloffMode.Linear;
        walk_src.maxDistance = 50f;
        walk_src.Play();

        run_src = this.AddComponent<AudioSource>();
        run_src.clip = run_clip;
        run_src.loop = true;
        run_src.spatialBlend = 1.0f;
        run_src.rolloffMode = AudioRolloffMode.Linear;
        run_src.maxDistance = 50f;
        run_src.Play();

        // This source is for playing non-footstep sounds
        other_src = this.AddComponent<AudioSource>();
        other_src.clip = wander_clip;
        other_src.loop = true;
        other_src.spatialBlend = 1.0f;
        other_src.rolloffMode = AudioRolloffMode.Linear;
        other_src.maxDistance = 50f;
        other_src.Play();
    }

    // Returns whether the robot can sense the player
    protected abstract bool canSensePlayer();

    // Coroutine that contains logic for what the robots can perceive
    protected abstract IEnumerator Sense();

    // Returns true if the robot can sense a flare
    protected abstract bool canSenseFlare();


    // Returns the distance between vectors A and B but only considers the X and Z components
    public static float distance_2d(Vector3 A, Vector3 B)
    {
        float square = (A.x - B.x) * (A.x - B.x) + (A.z - B.z) * (A.z - B.z);
        return Mathf.Sqrt(square);
    }

    protected void Explode()
    {
        player.GetComponent<player>().health -= 1;
        GameObject.Find("GameManager").GetComponent<GameManager>().Explosion(transform.position);
        Destroy(this.gameObject);
    }


    protected void Update()
    {
        // Rotate robot to face target
        Vector3 to_target = target - transform.position;
        to_target.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(to_target);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Move the character (also add gravity)
        Vector3 move = (target - transform.position).normalized * speed;
        y_velocity -= controller.isGrounded ? 0 : gravity * Time.deltaTime;
        move.y = y_velocity;
        controller.Move(move * Time.deltaTime);

        // Hacky way to have the sounds play only when the robot is moving
        if (speed > 0.1f && speed < jogSpeed - 1)
        {
            walk_src.volume = 1;
            run_src.volume = 0;
        }
        else if (speed >= jogSpeed - 1)
        {
            walk_src.volume = 0;
            run_src.volume = 1;
        }
        else
        {
            walk_src.volume = 0;
            run_src.volume = 0;
        }

    }
}
