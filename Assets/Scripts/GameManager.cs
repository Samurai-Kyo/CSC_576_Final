using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using Palmmedia.ReportGenerator.Core;

public class GameManager : MonoBehaviour {

    // Audio
    public AudioSource src;
    public AudioClip storm;

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


    // TODO: spawn robots


    // Spawns the given prefab n times randomly throughout the world
    void SpawnNPrefabs(GameObject prefab, int n) {
        for (int i = 0; i < n; i++) {
            float x = Random.Range(xMin, xMax);
            float z = Random.Range(zMin, zMax);
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + 0.1f;

            // Adjust position and rotation based on which prefab it is
            Vector3 position = new Vector3(x, y, z);
            Quaternion rotation = Quaternion.identity;
            if (prefab == tarp_prefab) {
                rotation = Quaternion.Euler(90f, 0, 0);
            }
            else if (prefab == barrel_prefab) {
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
    void SpawnItems(int numTarps, int numLogs, int numBarrels, int numPots) { 
        SpawnNPrefabs(tarp_prefab, numTarps);
        SpawnNPrefabs(log_prefab, numLogs);
        SpawnNPrefabs(barrel_prefab, numBarrels);
        SpawnNPrefabs(pot_prefab, numPots);
    }




    // Start is called before the first frame update
    void Start() {
        // Seed RNG
        Random.InitState((int)System.DateTime.Now.Ticks);

        // Generate terrain
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        TerrainGen generator = terrain.GetComponent<TerrainGen>();
        generator.Generate();

        // ---- Spawning ----
        SpawnItems(1, 1, 1, 1);
        // ------------------


        // Audio setup
        src = GetComponent<AudioSource>();
        src.clip = storm;
        src.loop = true;
        src.Play();
    }


}
