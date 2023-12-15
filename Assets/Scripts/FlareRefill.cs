using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlareRefill : MonoBehaviour
{
    public GameObject flare_area;
    private readonly KeyCode pickup_flare_button = KeyCode.E;
    private toss flare_manager;

    void Start()
    {
        flare_manager = GameObject.Find("Player").GetComponent<toss>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Flare Area")
        {
            if (Input.GetKeyDown(pickup_flare_button))
            {
                flare_manager.numFlares = 3;
            }
        }
    }

}
