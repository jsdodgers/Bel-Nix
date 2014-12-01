using UnityEngine;
using System.Collections;

public enum ItemType {Weapon, Armor, Useable, Ammunition, Mechanical, Misc}

public class Item1 : MonoBehaviour {

	public string itemName;
	public ItemType itemType;
	public int gold, silver, copper;
	public bool isKeyItem;
}
