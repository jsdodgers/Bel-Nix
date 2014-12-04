using UnityEngine;
using System.Collections;

public enum ArmorType {Head, Shoulder, Chest, Gloves, Pants, Boots}

[RequireComponent (typeof (Character))]
public class ItemArmor : EditorItem {


	public ArmorType armorType;
	public int AC;
	//Armor armor;
	public Armor getArmor() {
	//	if (armor == null)
			return new Armor(itemName, itemType, gold, silver, copper, isKeyItem, inventoryTexture, armorType, AC);
	//	return armor;
	}
}
