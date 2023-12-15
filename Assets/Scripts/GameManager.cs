using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.AI;

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
    // --------------------------

    // Location of camp
    public Vector3 campsite_center;

    // Prefabs for spawning
    public GameObject sightRobot_prefab;
    public GameObject soundRobot_prefab;

    public GameObject tower_prefab;
    public GameObject rocks_prefab;
    public GameObject item_prefab;
    public GameObject tarp_prefab;
    public GameObject barrel_prefab;
    public GameObject log_prefab;
    public GameObject pot_prefab;
    // --------------------------------

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
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + 1f;

            // Regenerate random point if it is too close to the camp
            while (RobotBase.distance_2d(campsite_center, new Vector3(x, y, z)) < 20)
            {
                x = Random.Range(xMin, xMax);
                z = Random.Range(zMin, zMax);
                y = terrain.SampleHeight(new Vector3(x, 0, z)) + 1f;
            }

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

            GameObject thing = Instantiate(prefab, position, rotation);

            // If it's not a robot prefab, it must be an item prefab, so spawn item marker as a child of prefab
            if (prefab != sightRobot_prefab && prefab != soundRobot_prefab)
            {
                GameObject item = Instantiate(item_prefab, position, rotation);
                item.transform.SetParent(thing.transform);
                item.name = "Item Light";
            }
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

    void SpawnRobots()
    {
        if (difficulty == 0)
        {
            SpawnNPrefabs(sightRobot_prefab, 5);
            SpawnNPrefabs(soundRobot_prefab, 5);
        }
        else if (difficulty == 1)
        {
            SpawnNPrefabs(sightRobot_prefab, 8);
            SpawnNPrefabs(soundRobot_prefab, 8);
        }
        else
        {
            SpawnNPrefabs(sightRobot_prefab, 10);
            SpawnNPrefabs(soundRobot_prefab, 10);
        }
    }

    // Creates an explosion at the specified location
    // Need to do this here rather than in robot scrip[t since destroying the robot
    // cuts the animation and sound off early
    public void Explosion(Vector3 position)
    {
        src.PlayOneShot(explosion_clip, 2f);
        Instantiate(explosion_fx, position, Quaternion.identity);
    }

    public void loadSettings()
    {
        string filepath = Path.Combine(Application.persistentDataPath, "settings.txt");

        // Read and parse settings
        if (!File.Exists(filepath))
        { // if file doesn't exist, create it with default values
            File.WriteAllText(filepath, "0.5,0");
        }
        string[] settings = File.ReadAllText(filepath).Replace("\n", "").Split(',');
        float.TryParse(settings[0], out volume);
        int.TryParse(settings[1], out difficulty);
    }

    // Returns array of 4 items containing the number to spawn depending on the difficulty level
    public int[] GetItemCounts()
    {
        int[] counts = new int[4];

        // Minimum number of each resource required to win
        int minTarps = 3;
        int minLogs = 7;
        int minBarrels = 1;
        int minPots = 1;

        counts[0] = minTarps + (3 - difficulty);
        counts[1] = minLogs + (3 - difficulty);
        counts[2] = minBarrels + (3 - difficulty);
        counts[3] = minPots + (3 - difficulty);

        return counts;
    }



    // Start is called before the first frame update
    void Start()
    {
        tarp_prefab.name = "TARP";
        barrel_prefab.name = "BARREL";
        log_prefab.name = "PLANK";
        pot_prefab.name = "POT";

        // Seed RNG
        Random.InitState((int)System.DateTime.Now.Ticks);

        // Load settings
        loadSettings();

        // Generate terrain
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        TerrainGen generator = terrain.GetComponent<TerrainGen>();
        generator.Generate();

        campsite_center = GameObject.Find("campsite_center").transform.position;

        // ---- Spawning ----
        int[] itemCounts = GetItemCounts();
        SpawnItems(itemCounts[0], itemCounts[1], itemCounts[2], itemCounts[3]);
        SpawnRobots();
        // ------------------


        // Audio setup
        src = GetComponent<AudioSource>();
        src.clip = storm;
        src.loop = true;
        src.volume = volume;
        src.Play();
    }

    void Update()
    {
        src.volume = volume;
    }


}
