using UnityEngine;
using System.Collections;

public enum ItemType {WEAPON, ARMOR, USEABLE, AMMUNITION, MISC}

public class Item1 : MonoBehaviour {

	public string itemName;
	public ItemType itemType;
	public int gold, silver, copper;
	public bool isKeyItem;
}
