using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toss : MonoBehaviour {


    public GameObject grenadePrefab;
    public Transform cameraTrans;
    public GameObject flareFX_prefab;
    public GameObject smoke_prefab;
    public GameObject light_prefab;

    public int numFlares = 3;
    public float delay = 5f;
    public float force = 15.0f;

    // Sounds
    public AudioClip throw_clip, explosion_clip;

    

    // Start is called before the first frame update
    void Start() {
        cameraTrans = GameObject.Find("Camera").transform;
    }

    IEnumerator Toss() {
        // Calculate spawn location
        Vector3 spawn = cameraTrans.position + cameraTrans.forward; // spawn a bit in front of camera

        // Spawn object and add force to it
        GameObject flare = Instantiate(grenadePrefab, spawn, Quaternion.identity);
        Rigidbody rb = flare.GetComponent<Rigidbody>();
        rb.AddForce(cameraTrans.forward * force, ForceMode.Impulse);

        // Add smoke effects before it ignites
        GameObject smoke = Instantiate(smoke_prefab, flare.transform.position, Quaternion.identity);
        smoke.transform.parent = flare.transform;

        // Create audio for object
        AudioSource src = flare.GetComponent<AudioSource>();
        src.loop = true;
        src.PlayOneShot(throw_clip);

        // Delay before igniting
        yield return new WaitForSeconds(delay);
        Destroy(smoke);

        // Spawn particle system and light
        GameObject particles = Instantiate(flareFX_prefab, flare.transform.position, Quaternion.identity);
        GameObject light = Instantiate(light_prefab, flare.transform.position, Quaternion.identity);
        
        // Ensure that the particle system and light follow the flare as it rolls
        particles.transform.parent = flare.transform;
        light.transform.parent = flare.transform;

        // Play audio
        src.pitch *= Random.Range(0.85f, 1.15f); // randomize pitch a little
        src.volume *= Random.Range(0.8f, 0.95f);   // Randomize volume as well
        src.clip = explosion_clip;
        src.spatialBlend = 1.0f; // 
        src.Play();

        // Fizzle out
        yield return new WaitForSeconds(2 * delay);
        Destroy(flare);
        src.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        bool paused = GetComponent<player>().paused;

        // If player presses E, throw flare
        if (!paused && Input.GetKeyDown(KeyCode.F) && numFlares > 0) {
            numFlares--;
            StartCoroutine(Toss());
        }
        
    }
}
