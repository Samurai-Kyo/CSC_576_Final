using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampSpawner : MonoBehaviour
{
    // Master objects of the camp
    public GameObject tent_area;
    public GameObject fire_area;
    public GameObject water_catcher_area;
    public GameObject fence_area;

    // Child of master objects
    private GameObject tent;
    private GameObject fire_pit;
    private GameObject water_catcher;
    private GameObject fence;

    void Start()
    {
        // Initialize child objects
        tent                    = tent_area.transform.GetChild(0).gameObject;
        fire_pit                = fire_area.transform.GetChild(0).gameObject;
        water_catcher           = water_catcher_area.transform.GetChild(0).gameObject;
        fence                   = fence_area.transform.GetChild(0).gameObject;
       

        // Hide objects you need to build
        tent.SetActive(false);
        water_catcher.SetActive(false);
        fire_pit.SetActive(false);
        fence.SetActive(false);
    }
}

