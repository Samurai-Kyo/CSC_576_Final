using System;
using UnityEngine;
using UnityEngine.UI;

// This script can just be attached to the Player GameObject.
// As far as I'm aware, the trigger events shouldn't interfere.
// Of note: the items you wish to pick up must have "Is Trigger" set to "true".
public class Inventory : MonoBehaviour
{

    // The game objects that you want to pick up MUST match these names here.
    // (This can change in the future if inconvenient, could maybe use tags or something)
    enum Item {
        PLANK,
        TINDER,
        FLINT,
        TARP,
        BARREL
    }

    public Text pickup_text; // Not required, used to display a prompt to the user, something like (Press [E] to pick up)
    public int[] item_counts; // The counts of the items in the inventory
    private int[] max; // An array of the max values for the inventory space for an Item at index Item.<>
    private bool just_picked_up; // Used to prevent frame quick successive pick ups
    private readonly int num_item_types = 5; // Number of item types
    private readonly KeyCode pick_up_key = KeyCode.E; // Used to (maybe) customize the key used to pick up the items

    void Start() {
        pickup_text.gameObject.SetActive(false);
        item_counts = new int[num_item_types];
        max = new int[num_item_types];
        max[(int) Item.PLANK] = 3;
        max[(int) Item.TINDER] = 2;
        max[(int) Item.FLINT] = 1;
        max[(int) Item.TARP] = 1;
        max[(int) Item.BARREL] = 1;
        just_picked_up = false;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            string result = "";
            for (int i = 0; i < num_item_types; ++i) {
                string name = ((Item) i).ToString();
                result += "(" + name + ", " + item_counts[i] + "), ";
            }
            Debug.Log(result);
        }
    }

    void OnTriggerStay(Collider other) {
        // "Input validation"
        if (!Enum.TryParse(other.name, out Item type)) return;
        pickup_text.gameObject.SetActive(true); // Display prompt to pick up item
        if (Input.GetKey(pick_up_key) && !just_picked_up) {
            PickUpItem(other.gameObject, (int)type);
            just_picked_up = true;
            pickup_text.gameObject.SetActive(false);
        } else if (!Input.GetKey(pick_up_key) && just_picked_up) {
            // Released key, now can pick up again
            just_picked_up = false;
        }
    }

    private void OnTriggerExit(Collider other) {
        pickup_text.gameObject.SetActive(false);
    }

    private bool PickUpItem(GameObject item, int type_index) {
        // Check if we have room
        if (item_counts[type_index] >= max[type_index]) {
            item_counts[type_index] = max[type_index];
            return false;
        }
        // We have room, try to "collect" the item
        Destroy(item);
        // if (!item.IsDestroyed()) {
        //     return false;
        // }
        // "Collected" the item, update inventory
        item_counts[type_index] += 1;
        return true;
    }

    public int[] GetItemCounts() {
        int[] copy = new int[item_counts.Length];
        item_counts.CopyTo(copy, 0);
        return copy;
    }

    public void ReduceItemByNumber(int index, int amount) {
        item_counts[index] -= amount;
    }

    public override string ToString() {
        string result = "";
        for (int i = 0; i < item_counts.Length - 1; ++i) {
            string name = ((Item) i).ToString();
            result += "(" + name + ", " + item_counts[i] + "), ";
        }
        result += "(" + name + ", " + item_counts[^1] + ")";
        return result;
    }

    // Add display function for the pause menu

    // Maybe add other things I can't think of right now

}
