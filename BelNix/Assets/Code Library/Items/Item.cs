﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType {Weapon, Armor, Useable, Ammunition, Mechanical, Misc}
public enum ItemStackType {Applicator, Gear, Frame, EnergySource, Trigger, Turret, None}


public class EditorItem : MonoBehaviour {
	public string itemName;
	public ItemType itemType;
	public int gold, silver, copper;
	public bool isKeyItem;
	public Texture2D inventoryTexture;
	public GameObject spritePrefab;
	public int layerAdd;
	public Item getItem() {
		return new Item(itemName, itemType, gold, silver, copper, isKeyItem, inventoryTexture, spritePrefab, layerAdd);
	}
}

public class Item {
	public ItemStackType itemStackType = ItemStackType.None;
	public string itemName;
	public ItemType itemType;
	public int gold, silver, copper;
	public bool isKeyItem;
	public Texture inventoryTexture;
	public List<Item> stack;
	public int layerAdd;
	public GameObject spritePrefab;
	public GameObject sprite;
	public virtual Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0)};
	}
	public Item(string itemName, ItemType itemType, int gold, int silver, int copper, bool isKeyItem, Texture2D inventoryTexture, GameObject spritePrefab, int layerAdd) : this() {
		this.itemName = itemName;
		this.itemType = itemType;
		this.gold = gold;
		this.silver = silver;
		this.copper = copper;
		this.isKeyItem = isKeyItem;
		this.inventoryTexture = inventoryTexture;
		this.spritePrefab = spritePrefab;
		this.layerAdd = layerAdd;
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
	public Vector2 getBottomRightCell() {
		int maxWidth = 0;
		int yMax = (int)getSize().y - 1;
		Vector2[] shape = getShape();
		for (int n=1;n<shape.Length;n++) {
			if ((int)shape[n].y!=yMax) continue;
			maxWidth = Mathf.Max(maxWidth, (int)shape[n].x);
		}
		return new Vector2(maxWidth, yMax);
	}
	public Item() {
		stack = new List<Item>();
	}

	public bool removeItemFromStack(Item i) {
		if (i==this) return false;
		if (i.itemStackType!=itemStackType) return false;
		if (stack.Contains(i)) {
			stack.Remove(i);
			return true;
		}
		return false;
	}

	public Item popStack() {
		if (stack.Count==0) return null;
		Item i = stack[stack.Count-1];
		stack.Remove(i);
		return i;
	}
	public Item addToStack(Item i) {
		stack.Add(i);
		return i;
	}
	public int stackSize() {
		return stack.Count+1;
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

	public Weapon(string itemName, ItemType itemType, int gold, int silver, int copper, bool isKeyItem, Texture2D inventoryTexture, GameObject spritePrefab, int layerAdd, int hit, int range, int numberOfDamageDice, int diceType, int damageBonus, DamageType damageType, int criticalChance, int durabilityChance, bool isRanged, Vector2[] shape) {
		this.itemName = itemName;
		this.itemType = itemType;
		this.gold = gold;
		this.silver = silver;
		this.copper = copper;
		this.isKeyItem = isKeyItem;
		this.inventoryTexture = inventoryTexture;
		this.spritePrefab = spritePrefab;
		this.layerAdd = layerAdd;
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
	public Armor(string itemName, ItemType itemType, int gold, int silver, int copper, bool isKeyItem, Texture2D inventoryTexture, GameObject spritePrefab, int layerAdd, ArmorType armorType, int AC) {
		this.itemName = itemName;
		this.itemType = itemType;
		this.gold = gold;
		this.silver = silver;
		this.copper = copper;
		this.isKeyItem = isKeyItem;
		this.inventoryTexture = inventoryTexture;
		this.layerAdd = layerAdd;
		this.spritePrefab = spritePrefab;
		this.armorType = armorType;
		this.AC = AC;
	}
	
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}
