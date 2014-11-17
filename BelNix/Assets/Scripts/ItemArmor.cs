using UnityEngine;
using System.Collections;

public enum ArmorType {HEAD, SHOULDER, CHEST, GLOVES, PANTS, BOOTS}

public class ItemArmor : Item1 {

	public ArmorType armorType;
	public int AC;
}
