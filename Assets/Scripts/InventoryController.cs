using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

/* This script deals with the inventroy, its physical representation, and saving and loading its contents. 
 * Note that we use the TMPro namespace above for references to text, and the System.IO for dealing with the file system */

	//We define an enum for item rarity. We simply do this for our own convenience, so we can represent rarity with a word instead of a number.
	public enum Rarity {Common, Uncommon, Rare, Epic, Legendary };
public class InventoryController : MonoBehaviour
{
	#region Variable Declaration

	/*	We want the class to have a static reference to itself. This is what is called "Singleton logic" - the concept of a class that there should only ever exist one copy of
		This makes it so that we can reference this class without an object reference. (See the ItemInitializer class)	*/
	public static InventoryController control;

	//We need this helper array because when we call Unity to serialize the ScriptableObjects, they are put into an array by default
	private InventoryItem[] availableItemsHelper;

	//This is the actual inventory - a list containing all the item-ScriptableObjects
	private List<InventoryItem> currentInventory;
	//A list containing the item-gameobjects - that is the physical representation of the items that we actually see in the scene
	private List<GameObject> currentInventoryUI;

	//A string pointing to the path where our JSON save file resides, and a helper string that holds the contents of the JSON file we want to manipulate
	private string jsonPath, jsonContents;

	#region Editor Assigned Variables

	[SerializeField]
	private bool loadOnStart;

	[SerializeField]
	private ItemHashtable hashTableHolder;

	//We want references to the two UI panels, this is simply for visual purposes as we will see further in
	[SerializeField]
	private RectTransform allItemsPanel, inventoryPanel;

	//The text fields that will display info about the current inventory objects
	[SerializeField]
	private TextMeshProUGUI totalValueText, totalWeightText, noItemsText, itemNameText, itemDescriptionText, itemValueText, itemWeightText, itemRarityText, itemDamageText;

	//Because we only want to display the damage element when we select a weapon, we need a reference to it to be able to hide it. We also have a reference to the whole info panel so we can hide it when we want to
	[SerializeField]
	private GameObject itemInfoParent, itemDamageParent;

	#endregion

	#endregion

	#region Private Functions

	//We use Awake in stead of Start as a means to call this method before anything else - including other Start calls
	void Awake () 
	{
		//We define this as the singleton of this class. Note that in practice it is smart to do some actual steps to ensure this is the only copy
		control = this;

		//Fetch all item ScriptableObjects and fill them in the left panel
		FillAllItemsPanel();

		//Set the location for where our JSON file containing the inventory will be stored
		InitializeJson();

		//Finally, we initialize the inventory itself
		InitializeInventory();
	}

	//This function fills the right panel. It is only populated once, at the start of the scene, and is therefore quite simple
	void FillAllItemsPanel()
	{
		//This is where the magic happens. We ask Unity to load all InventoryItems (ScriptableObjects) it finds in the Resources folder, then put it into the helper array
		Resources.LoadAll("", typeof(InventoryItem));
		availableItemsHelper = Resources.FindObjectsOfTypeAll<InventoryItem>();
		
		if (availableItemsHelper.Length > 0)
		{
			//As long as there is at least 1 item, we run through each one in the array
			foreach (InventoryItem item in availableItemsHelper)
			{
				//We instantiate the prefab that the specific item requests. This is defined in the ScriptableObject, but in this demonstration, there is only one prefab used
				GameObject newItem = Instantiate(item.UIPrefab, allItemsPanel);

				//Finally, we call the new item's Setup function to change its sprite and references to the ones defined in the ScriptableObject
				newItem.GetComponent<ItemInitializer>().Setup(item, true);
			}
		}
	}
	void InitializeJson()
	{
		/*We define where our JSON file is located. We use a special folder called StreamingAssets. This folder is not compiled by Unity,
		 * and as such, all files there are accessible as files even in a build. In a completed game project, you would probably save
		 * user data somewhere else, but it is convenient for us to find this file within the project's file system when testing. */
		jsonPath = Application.streamingAssetsPath + "/inventory.json";
		
		//Just in case the file doesn't exist yet - if we don't find it, create it
		if (!File.Exists(jsonPath))
		{
			File.Create(jsonPath);
		}
	}

	void InitializeInventory()
	{
		//If we have selected the boolean Load On Start in the inspector, we load our inventory. Otherwise we just initialize the list as a new, empty one
		if (loadOnStart)
		{
			LoadInventory();
		}
		else
		{
			currentInventory = new List<InventoryItem>();
		}
	}

	/*This function works similarily to the FillAllItemsPanel() above, but this time we start by flushing it, and make sure we can update it every time we want to
	This would not be the most optimized way if you had, say, thousands of items in your inventory, but for our purposes it works fine and is easy to grasp */
	void UpdateInventory()
	{
		//We start by destroying all the physical representations of the inventory, effectively emptying the shown inventory
		if (currentInventoryUI != null)
		{
			foreach (GameObject UIItem in currentInventoryUI)
			{
				Destroy(UIItem);
			}
		}

		//Then we also reset the list itself
		currentInventoryUI = new List<GameObject>();

		//We also define some local variables we use below
		int totalValue = 0;
		int totalWeight = 0;
		int noItems = 0;

		//This loop is nearly identical to the one above - in the interest of saving space it is sparesly commented
		foreach (InventoryItem item in currentInventory)
		{
			GameObject newItem = Instantiate(item.UIPrefab, inventoryPanel);

			currentInventoryUI.Add(newItem);

			//Note that here we call the Setup() function with the bool flipped to false, in order to get the other functionality when clicked later
			newItem.GetComponent<ItemInitializer>().Setup(item, false);

			//This is where we calculate the total value, weight and number of items
			totalValue += item.value;
			totalWeight += item.weight;
			noItems++;
		}
		//We cast the final value number to a string in order to change the text
		totalValueText.text = totalValue.ToString();
		totalWeightText.text = totalWeight.ToString();
		noItemsText.text = noItems.ToString();
	}

	#endregion

	#region Public Functions

	/*This function is called by the "ItemInitializer" class. (Note that it is public) It does something based on wether the ItemInitializer tells us its in the inventory or not. 
	In principle this could be expanded upon to be a custom value (enum) that encodes whatever functionality you want for the situation */
	public void ItemSelected(InventoryItem type, bool selectionMenu)
	{
		if(selectionMenu == true)
		{
			currentInventory.Add(type);
		}
		else
		{
			currentInventory.Remove(type);
			ResetItemUI();
		}
		//Since we made sure that the UpdateInventory function can be re-called any time, we can now do this very cleanly
		UpdateInventory();
	}

	//When we hover over an item, we update the UI elements to reflect the data in said element
	public void ItemHoveredOver(InventoryItem type)
	{
		//First we activate the info panel
		itemInfoParent.SetActive(true);

		//Then, we update all the text objects to reflect the hovered object
		itemNameText.text = type.name;
		itemDescriptionText.text = type.Description;
		itemValueText.text = type.value.ToString();
		itemWeightText.text = type.weight.ToString();
		itemRarityText.text = type.rarity.ToString();

		//We only display the attack stat if the item is a Weapon. We can check if it is the derived class using the IS keyword
		if(type is InventoryWeapon) 
		{
			itemDamageParent.SetActive(true);

			//When we know that the variable "type" is of the type "InventoryWeapon" we can do the explicit cast:
			itemDamageText.text = ((InventoryWeapon)type).baseDamage.ToString();
		}
	}

	public void ResetItemUI()
	{
		itemDamageParent.SetActive(false);
		itemInfoParent.SetActive(false);
	}

	/*The last part of this script deals with JSON.
	This first one is called when we want to save the game. Note it is public, as we're callind this from a button's inspector */
	public void SaveInventory()
	{
		//We define a new Inventory-class and cast our inventory list to it, as further explained on the bottom of this script
		Inventory playerInv = new Inventory
		{
			savedInventory = hashTableHolder.ParseInventoryToReference(currentInventory)
		};

		//The JSONUtility converts our Object's public list into JSON format, but we need to save it manually using File
		string jsonToSave = JsonUtility.ToJson(playerInv, true);
		File.WriteAllText(jsonPath, jsonToSave);
	}
	
	//This loads the inventory from JSON, again, note that it is public
	public void LoadInventory()
	{
		if (File.Exists(jsonPath))
		{
			//Set the string to whatever is in the JSON file
			jsonContents = File.ReadAllText(jsonPath);

			//We define a new instance of our Serializable class to hold our list as we load it
			Inventory playerInv = JsonUtility.FromJson<Inventory>(jsonContents);

			//Then we cast that back into our main inventory list
			currentInventory = hashTableHolder.ParseInventoryFromReference(playerInv);

			//Finally, update the inventory display
			UpdateInventory();

		}
	}

	#endregion
}

/*This is a helper class. We use it in order to save our list of objects to JSON, as in Unity, JSON is generated only from all *public fields of a class*
* This means we must "wrap" our list in a Serializable (important) class when we want to save or load it, and then retrieve the info back out from the list
* The Serializable property means we can treat the class as an object, calling for example "Inventory inv = new Inventory" and create an instance of the class
* Also note we don't actually inherit from Monobehaviour - we in fact do not inherit at all	*/
[System.Serializable]
public class Inventory
{
	public List<string> savedInventory;
}