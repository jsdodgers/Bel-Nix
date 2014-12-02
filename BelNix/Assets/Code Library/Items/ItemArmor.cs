using UnityEngine;
using System.Collections;

public enum ArmorType {Head, Shoulder, Chest, Gloves, Pants, Boots}

[RequireComponent (typeof (Character))]
public class ItemArmor : MonoBehaviour {
	public string itemName;
	public ItemType itemType;
	public int gold, silver, copper;
	public bool isKeyItem;
	public Texture2D inventoryTexture;

	public ArmorType armorType;
	public int AC;
	Armor armor;
	public Armor getArmor() {
		if (armor == null)
			armor = new Armor(itemName, itemType, gold, silver, copper, isKeyItem, inventoryTexture, armorType, AC);
		return armor;
	}
}
