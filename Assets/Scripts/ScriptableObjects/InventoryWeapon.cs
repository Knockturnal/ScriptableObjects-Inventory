using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This line makes the ScriptableObject available in the "Create Asset" menu, accessed by right clicking in the asset hierarchy
[CreateAssetMenu(fileName = "Generic Weapon", menuName = "Inventory Item/Weapon")]

/* Note we derive (or inherit) from the class InventoryItem. This means we will get all the same fields, but can add more too. 
 * These new fields serve to illustrate how you might use derived ScriptableObject classes with inheritance */
public class InventoryWeapon : InventoryItem
{
	[Range(1, 100)]
	public int baseDamage = 1;
}