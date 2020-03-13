using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/* This script is necessary to make the item prefabs update with the info from the ScriptableObjects. 
 * It also handles the return path when the item is interacted with in UI (button is clicked on).
 * Please note that we are using the namespace UnityEngine.Eventsystems (above) and implementing the interface IPointerHandlers (below). 
 * These are needed for us to be notified when the user interacts with a UI Element. */
public class ItemInitializer : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
	//The image on the prefab to which we want to change the sprite
	public Image itemImage;
	
	//The ScriptableObject that this object is a representation of
	private InventoryItem type;

	//This bool determines wether the item is in the left or right panel - in other words, what we want to do when the player clicks on it
	private bool selectionMenu;

	//We call this method when we want the prefab to change based on the inputs we give. Note the "public" keyword, which means we can call this method from another class
	public void Setup(InventoryItem item, bool isInSelectionMenu)
	{
		type = item;
		selectionMenu = isInSelectionMenu;
		UpdateDisplay();
	}

	//This was separated out into its own function so that we in theory could change the sprite in the ScriptableObject at runtime and then call this function. Not necessary, but could be useful
	public void UpdateDisplay()
	{
		itemImage.sprite = type.UISprite;
	}


	/* The following functions are called by the Unity EventSystem when the user interacts with the UI element this script is attached to. Don't worry too much about *how* this works,
	 * just focus on understanding how we use it. To get these callbacks (as they are called) we do three steps:
	 * 1. Import the namespace (Top of the script, "using UnityEngine.EventSystems")
	 * 2. Implement the interfaces (On the line where we declare the class, "Monobehaviour, IPointerEnterHandler, IPointerClickHandler). Note there are other handlers for other user events
	 * 3. Create the callback functions. These are the ones below. */

	public void OnPointerClick(PointerEventData eventData)
	{
		/* We call a public function from the InventoryController and tell it what sort of item we clicked on, as well as wether we're in the left or right panel
		 * The reference "control" is a static singleton reference to the controller */

		InventoryController.control.ItemSelected(type, selectionMenu);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		//Same as above, we tell the InventoryController what object we hover over, so it can display different things in the UI

		InventoryController.control.ItemHoveredOver(type);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		//Same as above, we tell the InventoryController what *no longer* hover over, so it can display different things in the UI

		InventoryController.control.ResetItemUI();
	}
}
