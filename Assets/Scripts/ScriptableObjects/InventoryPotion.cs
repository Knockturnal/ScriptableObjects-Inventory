using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//We define another enum for our different stats. This may be done in another script in a finished project
public enum PlayerStat { Health, Strength, Stamina, Mana };

//Allow for this item to be created through menus
[CreateAssetMenu(fileName = "Generic Potion", menuName = "Inventory Item/Potion")]
public class InventoryPotion : InventoryItem
{
    [Space(10)]
    [Header("Potion stats:", order = 1)]
    //The stat this potion influences
    public PlayerStat stat;
    //The strength of this potion
    [Range(0,10)]
    public float pointsPerSeconds;
    //The duration of the potion 0 = instant
    [Range(0, 60)]
    public float duration;
}
