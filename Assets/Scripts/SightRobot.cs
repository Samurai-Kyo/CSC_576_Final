using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightRobot : RobotBase
{

    public float light_distance;
    public float dark_distance = 8f;
    public float flare_distance = 60f; // distance at which robots can see flares
    public float fov = 90f;  // degrees for field of view of enemy
    public float flashlight_threshold_angle = 25f; // angle at which the robot detects the flashlight is pointed at it

    // Start is called before the first frame update
    void Start()
    {
        Init(); // inherited from RobotBase

        // Set light distance to whatever the range of the flashlight is
        light_distance = GameObject.Find("Spot Light").GetComponent<Light>().range;


        StartCoroutine(Sense());
    }


    // Returns whether the player can be seen by the robot
    protected override bool canSensePlayer() {
        // Determine view distance based on if flashlight is on or off
        Vector3 to_player = player.transform.position - transform.position;
        bool flashlight_is_on = player.GetComponentInChildren<Light>().enabled;
        float radius = flashlight_is_on ? light_distance : dark_distance;

        // Find angle between forward direction and direction to player
        // We use a 2D vector bc we only care about angle in the XZ plane
        Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 to_player2D = new Vector2(to_player.x, to_player.z);
        float angle = Vector3.Angle(forward.normalized, to_player2D.normalized);

        // Determine if flashlight is pointed in robot's direction
        Vector3 flashlight_direction = player.transform.Find("Camera").transform.forward; // get forward direction of camera
        float flashlight_angle = Vector3.Angle(flashlight_direction, -to_player);

        bool robot_can_see_flashlight = flashlight_is_on && to_player.magnitude < light_distance && flashlight_angle < flashlight_threshold_angle;
        bool robot_can_see_player = angle < fov / 2 && to_player.magnitude < radius;

        return robot_can_see_flashlight || robot_can_see_player;
    }

    // Returns true if the robot can see a flare
    protected override bool canSenseFlare() {
        GameObject[] flare = GameObject.FindGameObjectsWithTag("flare");
        foreach (GameObject f in flare) {
            if (distance_2d(f.transform.position, transform.position) < flare_distance) {
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

        while (true) {
            animator.SetBool("attack", false); // false by default

            // If too much time has passed, set seen_player_recently to false
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

                // If robot is close to player, attack
                if (distance_2d(player.transform.position, transform.position) < target_error)
                {
                    animator.SetBool("attack", true);
                    speed = 0;
                    yield return new WaitForSeconds(0.5f);
                }

            }

            // Follow if we have seen the player recently and they are close enough
            else if (sensed_player_recently && distance_2d(transform.position, player.transform.position) < 2 * dark_distance)
            {
                speed = jogSpeed;
                target = player.transform.position;
                animator.SetBool("walk", false);
                animator.SetBool("jog", true);
                animator.SetBool("run", false);
            }

            // Wander
            else if (!is_wandering) {
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

            // If robot is close to target, idle
            if (distance_2d(target, transform.position) < target_error) {
                speed = 0;
                animator.SetBool("walk", false);
                animator.SetBool("jog", false);
                animator.SetBool("run", false);
            }

            // If target is in campsite (near radio tower), run in opposite direction
            // since robots don't like the radio tower
            if (distance_2d(target, campsite) < 12) {
                Vector3 direction = campsite - transform.position;
                target = -10 * direction.normalized + transform.position;
            }

            yield return wait;
        }
    }

}
