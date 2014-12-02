using UnityEngine;
using System.Collections;

public enum ArmorType {Head, Shoulder, Chest, Gloves, Pants, Boots}

public class ItemArmor : Item1 {

	public ArmorType armorType;
	public int AC;
}
