using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.SearchService;
using UnityEngine;
using SceneLoader = UnityEngine.SceneManagement.SceneManager;

public class PlayerCampBuilder : MonoBehaviour
{
    // Master objects of the camp
    public GameObject tent_area;
    public GameObject fire_area;
    public GameObject water_catcher_area;
    public GameObject fence_area;

    // Child of master objects
    private GameObject tent_spawner;
    private GameObject tent;
    private GameObject fire_pit_spawner;
    private GameObject fire_pit;
    private GameObject water_catcher_spawner;
    private GameObject water_catcher;
    private GameObject fence_spawner;
    private GameObject fence;
    private readonly KeyCode build_camp_item_key = KeyCode.E;
    private readonly bool[] built = new bool[4];
    public Inventory inventory;

    enum Item
    {
        PLANK = 0,
        POT = 1,
        TARP = 2,
        BARREL = 3
    }


    void Start()
    {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();

        // Initialize camp objects
        tent = tent_area.transform.GetChild(0).gameObject;
        tent_spawner = tent_area.transform.GetChild(1).gameObject;
        fire_pit = fire_area.transform.GetChild(0).gameObject;
        fire_pit_spawner = fire_area.transform.GetChild(1).gameObject;
        water_catcher = water_catcher_area.transform.GetChild(0).gameObject;
        water_catcher_spawner = water_catcher_area.transform.GetChild(1).gameObject;
        fence = fence_area.transform.GetChild(0).gameObject;
        fence_spawner = fence_area.transform.GetChild(1).gameObject;

        // initialize built array to false
        for (int i = 0; i < built.Length; i++)
        {
            built[i] = false;
        }
    }
    // PLAYER INVENTORY (0: PLANK, 1: POT, 2: TARP, 3: BARREL)
    // Tent: 1 PLANK, 2 TARP
    // Fire: 2 PLANK, 1 POT
    // Water: 1 TARP, 1 BARREL
    // Fence: 4 PLANK
    // Player builds camp
    void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(build_camp_item_key))
        {
            if (other.gameObject.name == "Tent Area" && inventory.GetItemCounts()[(int)Item.PLANK] >= 1 &&
                inventory.GetItemCounts()[(int)Item.TARP] >= 2)
            {
                inventory.ReduceItemByNumber((int)Item.PLANK, 1);
                inventory.ReduceItemByNumber((int)Item.TARP, 2);
                tent_spawner.SetActive(false);
                tent.SetActive(true);
                built[0] = true;
            }
            else if (other.gameObject.name == "Fire Area" && inventory.GetItemCounts()[(int)Item.PLANK] >= 2 &&
                    inventory.GetItemCounts()[(int)Item.POT] >= 1)
            {
                inventory.ReduceItemByNumber((int)Item.PLANK, 2);
                inventory.ReduceItemByNumber((int)Item.POT, 1);
                fire_pit_spawner.SetActive(false);
                fire_pit.SetActive(true);
                built[1] = true;
            }
            else if (other.gameObject.name == "Water Area" && inventory.GetItemCounts()[(int)Item.TARP] >= 1 &&
                    inventory.GetItemCounts()[(int)Item.BARREL] >= 1)
            {
                inventory.ReduceItemByNumber((int)Item.TARP, 1);
                inventory.ReduceItemByNumber((int)Item.BARREL, 1);
                water_catcher_spawner.SetActive(false);
                water_catcher.SetActive(true);
                built[2] = true;
            }
            else if (other.gameObject.name == "Fence Area" && inventory.GetItemCounts()[(int)Item.PLANK] >= 4)
            {
                inventory.ReduceItemByNumber((int)Item.PLANK, 4);
                fence_spawner.SetActive(false);
                fence.SetActive(true);
                built[3] = true;
            }
            else if (other.gameObject.name == "Radio Area" && built[0] && built[1] && built[2] && built[3])
            {
                SceneLoader.LoadScene("WinScene");
            }

        }
    }

}
