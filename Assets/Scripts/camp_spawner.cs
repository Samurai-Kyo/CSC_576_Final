using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camp_spawner : MonoBehaviour
{

    public GameObject tent_area;
    public GameObject fire_area;
    public GameObject water_catcher_area;
    public GameObject fence_area;

    private GameObject tent_spawner;
    private GameObject tent;
    private GameObject fire_spawner;
    private GameObject fire;
    private GameObject water_catcher_spawner;
    private GameObject water_catcher;
    private GameObject fence_spawner;
    private GameObject fence;


    // Start is called before the first frame update
    void Start()
    {
        tent = tent_area.transform.GetChild(0).gameObject;
        tent_spawner = tent_area.transform.GetChild(1).gameObject;

        fire_spawner = fire_area.transform.GetChild(5).gameObject;
        fire = fire_area.transform.GetChild(4).gameObject;
        water_catcher_spawner = water_catcher_area.transform.GetChild(0).gameObject;
        water_catcher = water_catcher_area.transform.GetChild(1).gameObject;
        fence_spawner = fence_area.transform.GetChild(0).gameObject;
        fence = fence_area.transform.GetChild(1).gameObject;

        tent.SetActive(false);
        water_catcher.SetActive(false);
        fire.SetActive(false);
        foreach (Transform child in fence.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         tent_spawner.SetActive(true);
    //         water_catcher_spawner.SetActive(true);
    //         fire_spawner.SetActive(true);
    //         fence_spawner.SetActive(true);
    //     }
    // }
}

