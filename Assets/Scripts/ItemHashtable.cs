using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* Unity does not serialize scriptable objects to JSON when we tell it to. If we peek at the JSON it actually stores a reference ID.
 * This *is* in fact what we want, but the problem is that this ID can change between sessions. We therefore have to setup our own
 * references to these objects. In essence, we are just casting an ID variable to a reference to the ScriptableObject. 
 * Notice that we're using the [ExecuteAlways] property. We do this so that the reference list is updated automatically in the editor
 * without having to manually think about it. But as a result, we must be careful to check if we are in play or edit more before executing any code.
 *
 * Note that we are serializing the NAMES of the objects we want to retrive. In a real world situation this can be bad, because if you change the name
 * of an item, all saved instances of that item will break. Instead, you could give each item an unique ID that you never change, but this project
 * was already getting kind of complicated and opaque. */

[ExecuteAlways]
public class ItemHashtable : MonoBehaviour
{
    //public Dictionary<string, InventoryItem> hashTable;
    [SerializeField]
    private List<HashtablePair> hashtable;

    void Update()
    {
        //This code is only executed in Edit mode. In here we update our list of references
        if (!Application.IsPlaying(gameObject)) 
        {
            //We start by loading in all the ScriptableObjects we can find of the correct type and putting them in an array
            Resources.LoadAll("", typeof(InventoryItem));
            InventoryItem[] availableItemsHelper = Resources.FindObjectsOfTypeAll<InventoryItem>();

            //Using the HashtablePair struct (declared at the bottom of this class) we create a list of ScriptableObjects and an identifier (key) we can search for that object with
            hashtable = new List<HashtablePair>();

            foreach (InventoryItem item in availableItemsHelper)
            {
                HashtablePair nextPair = new HashtablePair
                {
                    value = item,
                    key = item.name
                };

                hashtable.Add(nextPair);
            }
        }
    }

    /* This function takes in a list of strings (stored in the Inventory helper class) and returns a list
     * of InventoryItems. We need to do this because Unity doesn't serialize ScriptableObjects with JSON */
    public List<InventoryItem> ParseInventoryFromReference(Inventory inventory)
    {
        List<InventoryItem> parsedInventory = new List<InventoryItem>();

        foreach (string item in inventory.savedInventory)
        {
            /* This code is very complex for beginners to wrap their head around, but suffice it to say that we are using the
             * System.Linq include which lets us sort and manage lists in a very shorthand form. What we are actually doing
             * is querying a list of the custom struct HashtablePair (Declared at the bottom of the script) for the string/ID
             * we are looking for, and if we find it we return the associated InventoryItem */

            if (hashtable.Any(toCheck => toCheck.key == item))
            {
                HashtablePair toAdd = hashtable.SingleOrDefault(toCheck => toCheck.key == item);
                parsedInventory.Add(toAdd.value);
            }
            else
            {
                Debug.LogWarning("Could not parse item with key: '" + item + "'");
            }
        }

        return parsedInventory;
    }

    /* This function simply takes in a list of InventoryItems and returns a list of
     * the names of all the objects. We use it to be able to turn a list of InventoryItems
     * into their respective keys for when it comes time to save them. In principle we could do this too
     * with LINQ using much less verbose code, but it would also be harder to read */
    public List<string> ParseInventoryToReference(List<InventoryItem> currentInventory)
    {
        List<string> parsedInventory = new List<string>();

        foreach (InventoryItem item in currentInventory)
        {
            parsedInventory.Add(item.name);
        }

        return parsedInventory;
    }
}

/* This struct is just a data container that lets us adress a pair (or more, if we wished) of variables.
 * We can then use LINQ on a list of these structs to get back a certain item based on some criteria.
 * In this example, we use it to get a list of ScriptableObjects from a list of strings (keys) */

[System.Serializable]
public struct HashtablePair
{
    public string key;
    public InventoryItem value;
}