using System;
using Unity.VisualScripting;
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
        POT,
        TARP,
        BARREL
    }

    public Text pickup_text; // Not required, used to display a prompt to the user, something like (Press [E] to pick up)
    public Text inventory_ui; // The text for the inventory in the pause menu
    public bool show_prompts = true; // Displays button prompts for picking up items
    public int[] item_counts; // The counts of the items in the inventory
    private int[] max; // An array of the max values for the inventory space for an Item at index Item.<>
    private bool just_picked_up; // Used to prevent frame quick successive pick ups
    private readonly int num_item_types = 4; // Number of item types
    private readonly KeyCode pick_up_key = KeyCode.E; // Used to (maybe) customize the key used to pick up the items

    void Start() {
        if (pickup_text == null) {
            Debug.Log("Be sure to include pick up text with the inventory!");
        }
        if (inventory_ui == null) {
            Debug.Log("Be sure to include inventory ui text with the inventory!");
        }
        pickup_text.gameObject.SetActive(false);
        item_counts = new int[num_item_types];
        max = new int[num_item_types];
        max[(int) Item.PLANK] = 4;
        max[(int) Item.POT] = 2;
        max[(int) Item.TARP] = 2;
        max[(int) Item.BARREL] = 2;
        just_picked_up = false;

        UpdateInvUI();
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

        // delete (clone) pare of name
        string item_name = other.name[..^7];
        // "Input validation"
        if (!Enum.TryParse(item_name, out Item type)) return;
        pickup_text.gameObject.SetActive(show_prompts); // Display prompt to pick up item
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
        UpdateInvUI();
        return true;
    }

    public int[] GetItemCounts() {
        int[] copy = new int[item_counts.Length];
        item_counts.CopyTo(copy, 0);
        return copy;
    }

    public void ReduceItemByNumber(int index, int amount) {
        item_counts[index] -= amount;
        UpdateInvUI();
    }

    public override string ToString() {
        string result = "";
        for (int i = 0; i < item_counts.Length; ++i) {
            string name = ((Item) i).ToString();
            result += "(" + name + ", " + item_counts[i] + "), ";
        }
        result += "(" + name + ", " + item_counts[^1] + ")";
        return result;
    }

    // Called whenever the state of the inventory changes
    private void UpdateInvUI() {
        string inv = "";
        for (int i = 0; i < item_counts.Length; ++i) {
            //if (item_counts[i] <= 0) continue; // Skip empty items
            string name = ((Item) i).ToString().ToLower().FirstCharacterToUpper();
            inv += name + 's' + " x" + item_counts[i] + "\n";
        }
        inventory_ui.text = inv;
    }

    // Called by player while pausing and unpausing the game to display the UI
    public void ShowInventory(bool show) {
        inventory_ui.gameObject.SetActive(show);
    }

    // Maybe add other things I can't think of right now

}
