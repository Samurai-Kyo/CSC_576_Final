using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GameManager : MonoBehaviour
{
    // Audio
    public AudioSource src;
    public AudioClip storm;
    public AudioClip explosion_clip;

    private Terrain terrain;
    public int xMin = 1;
    public int xMax = 480;
    public int zMin = 1;
    public int zMax = 480;

    // Prefabs for spawning
    public GameObject tower_prefab;
    public GameObject rocks_prefab;
    public GameObject item_prefab;
    public GameObject tarp_prefab;
    public GameObject barrel_prefab;
    public GameObject log_prefab;
    public GameObject pot_prefab;

    // Explosion particle system
    public GameObject explosion_fx;

    // Global volume and difficulty settings
    public float volume;
    public int difficulty;



    // Spawns the given prefab n times randomly throughout the world
    void SpawnNPrefabs(GameObject prefab, int n)
    {
        for (int i = 0; i < n; i++)
        {
            float x = Random.Range(xMin, xMax);
            float z = Random.Range(zMin, zMax);
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + 0.1f;

            // Adjust position and rotation based on which prefab it is
            Vector3 position = new Vector3(x, y, z);
            Quaternion rotation = Quaternion.identity;
            if (prefab == tarp_prefab)
            {
                rotation = Quaternion.Euler(90f, 0, 0);
            }
            else if (prefab == barrel_prefab)
            {
                position.y += 0.5f;
            }
            else if (prefab == pot_prefab)
            {
                position.y += 1;
            }

            Instantiate(prefab, position, rotation);
            Instantiate(item_prefab, position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }


    // Spawns a given number of each item throughout the world randomly
    void SpawnItems(int numTarps, int numLogs, int numBarrels, int numPots)
    {
        SpawnNPrefabs(tarp_prefab, numTarps);
        SpawnNPrefabs(log_prefab, numLogs);
        SpawnNPrefabs(barrel_prefab, numBarrels);
        SpawnNPrefabs(pot_prefab, numPots);
    }

    // Creates an explosion at the specified location
    // Need to do this here rather than in robot scrip[t since destroying the robot
    // cuts the animation and sound off early
    public void Explosion(Vector3 position) {
        src.PlayOneShot(explosion_clip, 2f);
        Instantiate(explosion_fx, position, Quaternion.identity);
    }

    public void loadSettings() {
        string filepath = Path.Combine(Application.persistentDataPath, "settings.txt");
        
        // Read and parse settings
        string[] settings = File.ReadAllText(filepath).Replace("\n", "").Split(',');
        float.TryParse(settings[0], out volume);
        int.TryParse(settings[1], out difficulty);

        // TODO remove
        Debug.Log(volume.ToString() + " " + difficulty.ToString());
    }

    // Returns array of 4 items containg the number to spawn depending on the difficulty level
    public int[] getItemCounts() {
        int[] counts = new int[4];

        if (difficulty == 0) { // easy 
            counts[0] = 10;
            counts[1] = 10;
            counts[2] = 10;
            counts[3] = 10;
        }
        else if (difficulty == 1) { // medium
            counts[0] = 5;
            counts[1] = 5;
            counts[2] = 5;
            counts[3] = 5;
        }
        else { // hard
            counts[0] = 3;
            counts[1] = 3;
            counts[2] = 3;
            counts[3] = 3;
        }
        return counts;
    }



    // Start is called before the first frame update
    void Start()
    {
        // Seed RNG
        Random.InitState((int)System.DateTime.Now.Ticks);

        // Load settings
        loadSettings();

        // Generate terrain
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        TerrainGen generator = terrain.GetComponent<TerrainGen>();
        generator.Generate();

        // ---- Spawning ----
        int[] itemCounts = getItemCounts();
        SpawnItems(itemCounts[0], itemCounts[1], itemCounts[2], itemCounts[3]);
        // TODO spawn robots
        // ------------------


        // Audio setup
        src = GetComponent<AudioSource>();
        src.clip = storm;
        src.loop = true;
        src.Play();
    }


}
