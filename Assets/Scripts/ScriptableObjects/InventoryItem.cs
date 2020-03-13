using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This line makes the ScriptableObject available in the "Create Asset" menu, accessed by right clicking in the asset hierarchy
[CreateAssetMenu(fileName = "Generic Item", menuName = "Inventory Item/Generic Item")]

//Note we derive from the class ScriptableObject
public class InventoryItem : ScriptableObject
{
	public new string name = "Generic Item";

	[HideInInspector]
	public GameObject UIPrefab;

	public Sprite UISprite;

	public string Description = "Generic Desription";

	//The following are not used for anything practical, but are examples of what values you could assign to your items

	[Range(0, 10000)]
	public int value;

	[Range(0, 100)]
	public int weight;

	//We declare this enum in the top of the InventoryController class. We use this simply so we can represent rarity with a word, and not just a number
	public Rarity rarity;
}