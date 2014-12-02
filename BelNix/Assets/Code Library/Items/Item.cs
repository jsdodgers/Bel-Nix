using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType {Weapon, Armor, Useable, Ammunition, Mechanical, Misc}


public class Item {
	
	public string itemName;
	public ItemType itemType;
	public int gold, silver, copper;
	public bool isKeyItem;
	public Texture inventoryTexture;
	public List<Item> stack;
	public virtual Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0)};
	}
	public Vector2 getSize() {
		int maxWidth = 1;
		int maxHeight = 1;
		Vector2[] shape = getShape();
		for (int n=1;n<shape.Length;n++) {
			maxWidth = Mathf.Max(maxWidth, (int)shape[n].x+1);
			maxHeight = Mathf.Max(maxHeight, (int)shape[n].y+1);
		}
		return new Vector2(maxWidth, maxHeight);
	}
	
}

public class Weapon : Item {
	public int hit;
	public int range;
	public int numberOfDamageDice;
	public int diceType;
	public int damageBonus;
	public DamageType damageType;
	public int criticalChance;
	public int durabilityChance;
	public bool isRanged = false;
	public Vector2[] shape;

	public Weapon() {

	}

	public Weapon(string itemName, ItemType itemType, int gold, int silver, int copper, bool isKeyItem, Texture2D inventoryTexture, int hit, int range, int numberOfDamageDice, int diceType, int damageBonus, DamageType damageType, int criticalChance, int durabilityChance, bool isRanged, Vector2[] shape) {
		this.itemName = itemName;
		this.itemType = itemType;
		this.gold = gold;
		this.silver = silver;
		this.copper = copper;
		this.isKeyItem = isKeyItem;
		this.inventoryTexture = inventoryTexture;
		this.hit = hit;
		this.range = range;
		this.numberOfDamageDice = numberOfDamageDice;
		this.diceType = diceType;
		this.damageBonus = damageBonus;
		this.damageType = damageType;
		this.criticalChance = criticalChance;
		this.durabilityChance = durabilityChance;
		this.isRanged = isRanged;
		this.shape = shape;
	}

	
	public override Vector2[] getShape() {
		return shape;
	}
	
	
	public int rollDamage() {
		return rollDamage(false);
	}
	
	public int rollDamage(bool critical)
	{
		int damageDealt = 0;
		for(int i = 0; i < numberOfDamageDice; i++)
		{
			damageDealt += (critical ? diceType : Random.Range(1, diceType+1));
		}
		
		return damageDealt;
	}
}

public class Armor : Item {
	
	public ArmorType armorType;
	public int AC;
	public Armor(string itemName, ItemType itemType, int gold, int silver, int copper, bool isKeyItem, Texture2D inventoryTexture, ArmorType armorType, int AC) {
		this.itemName = itemName;
		this.itemType = itemType;
		this.gold = gold;
		this.silver = silver;
		this.copper = copper;
		this.isKeyItem = isKeyItem;
		this.inventoryTexture = inventoryTexture;
		this.armorType = armorType;
		this.AC = AC;
	}
	
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

