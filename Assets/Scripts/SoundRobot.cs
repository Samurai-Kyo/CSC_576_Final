using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class SoundRobot : RobotBase
{
    // Distances at which robot can hear player based on player speed
    public float hear_distance_crouched = 3f;
    public float hear_distance = 10f;
    public float hear_distance_running = 30f;

    public float flare_distance = 60f;


    void Start()
    {
        Init();
        StartCoroutine(Sense());
    }

    // Returns whether the player can be heard by the robot
    protected override bool canSensePlayer()
    {
        float playerSpeed = player.GetComponent<CharacterController>().velocity.magnitude;

        // Running
        if (player.GetComponent<player>().isRunning && distance_2d(player.transform.position, transform.position) < hear_distance_running)
        {
            return playerSpeed > 5f;
        }
        // Crouching
        else if (player.GetComponent<player>().isCrouching && distance_2d(player.transform.position, transform.position) < hear_distance_crouched)
        {
            return playerSpeed > 1f;
        }
        // Walking
        else if (distance_2d(player.transform.position, transform.position) < hear_distance)
        {
            return playerSpeed > 2f;
        }
        return false;
    }

    // Returns true if the robot can hear a flare
    protected override bool canSenseFlare()
    {
        GameObject[] flare = GameObject.FindGameObjectsWithTag("flare");
        foreach (GameObject f in flare)
        {
            if (distance_2d(f.transform.position, transform.position) < flare_distance)
            {
                flare_to_follow = f;
                return true;
            }
        }
        return false;
    }


    // Robot will collect information about it's surroundings
    // Make it a corotuine to improve performance - run a few times a second instead of 30-60 times a second
    protected override IEnumerator Sense()
    {
        WaitForSeconds wait = new WaitForSeconds(senseDelay);
        Vector3 campsite = GameObject.Find("campsite_center").transform.position;

        while (true)
        {

            // If robot is close to player, explode
            if (distance_2d(player.transform.position, transform.position) < 2 * target_error)
            {
                Explode();
            }


            // If too much time has passed, set heard_player_recently to false
            if (Time.time - sensed_timestamp > sensedSeconds)
            {
                sensed_player_recently = false;
            }

            // Choose new random location to wander to by setting is_wandering to false
            // when robot gets close to its target
            if (is_wandering && distance_2d(target, transform.position) < target_error)
            {
                animator.SetBool("walk", false);
                animator.SetBool("jog", false);
                animator.SetBool("run", false);
                is_wandering = false;
                speed = 0;
                yield return new WaitForSeconds(1);
            }

            // Pursue flare
            if (canSenseFlare())
            {
                speed = runSpeed;
                target = flare_to_follow.transform.position;

                animator.SetBool("walk", false);
                animator.SetBool("jog", false);
                animator.SetBool("run", true);
            }

            // Pursue player
            else if (canSensePlayer())
            {

                // Limit playing sound to at most once every 5 seconds
                if (Time.time - sensed_timestamp > 5)
                {
                    other_src.PlayOneShot(seen_clip);
                }

                sensed_player_recently = true;
                sensed_timestamp = Time.time;

                speed = runSpeed;
                target = player.transform.position;
                animator.SetBool("walk", false);
                animator.SetBool("jog", false);
                animator.SetBool("run", true);
            }

            // Go to if we have seen the player recently and they are close enough
            else if (sensed_player_recently && distance_2d(transform.position, player.transform.position) < hear_distance)
            {
                speed = jogSpeed;
                target = player.transform.position;
                animator.SetBool("walk", false);
                animator.SetBool("jog", true);
                animator.SetBool("run", false);
            }

            // Wander
            else if (!is_wandering)
            {

                is_wandering = true;

                // Move to random location
                float x = UnityEngine.Random.Range(-wanderDistance, wanderDistance);
                float z = UnityEngine.Random.Range(-wanderDistance, wanderDistance);
                Vector3 offset = new Vector3(x, 0, z);

                speed = walkSpeed;
                target = transform.position + offset;
                animator.SetBool("walk", true);
                animator.SetBool("jog", false);
                animator.SetBool("run", false);
            }

            // If robot is close to non-player target, idle
            if (!canSensePlayer() && distance_2d(target, transform.position) < target_error)
            {
                speed = 0;
                animator.SetBool("walk", false);
                animator.SetBool("jog", false);
                animator.SetBool("run", false);
            }

            // If target is in campsite (near radio tower), run in opposite direction
            // since robots don't like the radio tower
            if (distance_2d(target, campsite) < 12)
            {
                Vector3 direction = campsite - transform.position;
                target = -10 * direction.normalized + transform.position;
            }

            yield return wait;
        }
    }
}
