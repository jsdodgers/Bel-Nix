using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemType {Weapon = 0, Armor, Useable, Ammunition, Mechanical, Misc}
public enum ItemStackType {Applicator = 0, Gear, Frame, EnergySource, Trigger, Turret, None}
public enum ItemCode {None = 0, Item, Weapon, Armor, Turret, Trap, Frame, EnergySource, Trigger, Applicator, Gear, TestFrame, TestEnergySource, TestTrigger, TestApplicator, TestGear};


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
	public string inventoryTextureName = "";
	public Texture inventoryTexture;
	public List<Item> stack;
	public int layerAdd;
	public GameObject spritePrefab;
	public GameObject sprite;
	public const string delimiter = ",";
	public const string otherDelimiter = ":";
	public const int numSplit = 6;
	public static Item deserializeItem(ItemCode code, string itemData) {
		return deserializeItem(code, itemData, delimiter);
	}
	public static Item deserializeItem(ItemCode code, string itemData, string delim) {
		switch (code) {
		case ItemCode.Item:
			return new Item(itemData, delim);
		case ItemCode.Weapon:
			return new Weapon(itemData, delim);
		case ItemCode.Armor:
			return new Armor(itemData, delim);
		case ItemCode.Turret:
			return new Turret(itemData, delim);
		case ItemCode.Trap:
			return new Trap(itemData, delim);
		case ItemCode.Frame:
			return new Frame(itemData, delim);
		case ItemCode.EnergySource:
			return new EnergySource(itemData, delim);
		case ItemCode.Trigger:
			return new Trigger(itemData, delim);
		case ItemCode.Applicator:
			return new Applicator(itemData, delim);
		case ItemCode.Gear:
			return new Gear(itemData, delim);
		case ItemCode.TestFrame:
			return new TestFrame(itemData, delim);
		case ItemCode.TestEnergySource:
			return new TestEnergySource(itemData, delim);
		case ItemCode.TestTrigger:
			return new TestTrigger(itemData, delim);
		case ItemCode.TestApplicator:
			return new TestApplicator(itemData, delim);
		case ItemCode.TestGear:
			return new TestGear(itemData, delim);
		default:
			return new Item(itemData, delim);
		}
	}
	public Item(string itemData, string delim) : this() {
		string[] split = itemData.Split(delim.ToCharArray());
		itemStackType = (ItemStackType)int.Parse(split[0]);
		itemName = split[1];
		int money = int.Parse(split[2]);
		gold = money/10000;
		silver = (money/100)%100;
		copper = money%100;
		isKeyItem = int.Parse(split[3])==1;
		inventoryTextureName = split[4];
		if (inventoryTextureName != "")
			inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
		layerAdd = int.Parse(split[5]);

	}
	public virtual ItemCode getItemCode() {
		return ItemCode.Item;
	}
	public virtual string getItemData() {
		return getItemData(delimiter);
	}

	public virtual string getItemData(string delim) {
		return (int)itemStackType + delim +
			itemName + delim +
				(gold * 10000 + silver*100 + copper) + delim +
				(isKeyItem ? 1 : 0) + delim +
				(inventoryTextureName == null ? "" : inventoryTextureName) + delim +
				layerAdd;
	}
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
	public Weapon(string itemData, string delim) : base(itemData, delim) {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = 6;
		hit = int.Parse(split[curr++]);
		range = int.Parse(split[curr++]);
		numberOfDamageDice = int.Parse(split[curr++]);
		diceType = int.Parse(split[curr++]);
		damageBonus = int.Parse(split[curr++]);
		damageType = (DamageType)int.Parse(split[curr++]);
		criticalChance = int.Parse(split[curr++]);
		durabilityChance = int.Parse(split[curr++]);
		isRanged = int.Parse(split[curr++])==1;
		int shapeSize = int.Parse(split[curr++]);
		if (shapeSize != 0) {
			shape = new Vector2[shapeSize];
			for (int n=0;n<shapeSize;n++) {
				shape[n] = new Vector2(int.Parse(split[curr++]), int.Parse(split[curr++]));
			}
		}
	}
	public override string getItemData(string delim) {
		string shapeString = "0";
		if (shape != null) {
			shapeString = shape.Length.ToString();
			for (int n=0;n<shape.Length;n++) {
				shapeString += delim + shape[n].x + delimiter + shape[n].y;
			}
		}
		return base.getItemData(delim) + delim + 
			hit + delim +
				range + delim +
				numberOfDamageDice + delim +
				diceType + delim +
				damageBonus + delim +
				(int)damageType + delim +
				criticalChance + delim +
				durabilityChance + delim +
				(isRanged ? 1 : 0) + delim +
				shapeString;

	}
	public Weapon() {

	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.Weapon;
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
	public Armor(string itemData, string delim) : base(itemData, delim) {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = 6;
		armorType = (ArmorType)int.Parse(split[curr++]);
		AC = int.Parse(split[curr++]);
	}
	public override string getItemData(string delim) {
		return base.getItemData() + delim +
			(int)armorType + delim +
				AC;
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.Armor;
	}
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

