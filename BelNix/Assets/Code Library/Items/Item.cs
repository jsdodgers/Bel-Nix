using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public enum ItemType  {Weapon = 0, Armor, Useable, Ammunition, Mechanical, Misc}
public enum ItemStackType  {Applicator = 0, Gear, Frame, EnergySource, Trigger, Turret, None}
public enum ItemCode  {None = 0, Item, Weapon, Armor, Turret, Trap, Frame, EnergySource, Trigger, Applicator, Gear, TestFrame, TestEnergySource, TestTrigger, TestApplicator, TestGear, WeaponMechanical, TriggerM1, TriggerM2, TriggerM3, TriggerM4, TriggerM5, FrameM1, FrameM2, FrameM3, FrameM4, FrameM5, EnergySourceM1, EnergySourceM2, EnergySourceM3, EnergySourceM4, EnergySourceM5, GearM1, GearM2, GearM3, GearM4, GearM5, Knives, BuzzSaws, Medicinal};

public class EditorItem : MonoBehaviour  {
	public string itemName;
	public string inventoryTextureSpritePrefabName;
	public bool canPlaceInShoulder;
	public ItemType itemType;
	public int gold, silver, copper;
	public bool isKeyItem;
//	public Sprite inventoryTexture;
//	public GameObject spritePrefab;
	public int layerAdd;
	public virtual Item getItem()  {
		return new Item(itemName, itemType, canPlaceInShoulder, gold, silver, copper, isKeyItem, inventoryTextureSpritePrefabName, layerAdd);
	}
}

public class Item  {
/*	public static Dictionary<string,string> prefabs = new Dictionary<string,string>()  {  {"Bronze Plank", "Units/Weapons/Male_Base_Shortsword"}
	};
	public static Dictionary<string,string> inventoryTextures = new Dictionary<string,string>()  {  {"Bronze Plank", "Units/Weapons/Male_Base_Shortsword"}
	};*/
	public ItemStackType itemStackType = ItemStackType.None;
	public string itemName;
	public ItemType itemType;
	public int gold, silver, copper;
	public bool isKeyItem;
	public bool canPlaceInShoulder;
	public string inventoryTextureName = "";
	public Sprite inventoryTexture;
	public List<Item> stack;
	public int layerAdd;
//	public string spritePrefabString;
	public GameObject spritePrefab;
	public GameObject sprite;
	public const string delimiter = ",";
	public const string otherDelimiter = ":";
	public const int numSplit = 7;
	public static Item deserializeItem(ItemCode code, string itemData)  {
		return deserializeItem(code, itemData, delimiter);
	}
	public static Item deserializeItem(ItemCode code, string itemData, string delim)  {
		switch (code)  {
		case ItemCode.Item:
			return new Item(itemData, delim);
		case ItemCode.Weapon:
			return new Weapon(itemData, delim);
		case ItemCode.WeaponMechanical:
			return new WeaponMechanical(itemData, delim);
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
		case ItemCode.TriggerM1:
			return new TriggerM1(itemData, delim);
		case ItemCode.TriggerM2:
			return new TriggerM2(itemData, delim);
		case ItemCode.TriggerM3:
			return new TriggerM3(itemData, delim);
		case ItemCode.FrameM1:
			return new FrameM1(itemData, delim);
		case ItemCode.FrameM2:
			return new FrameM2(itemData, delim);
		case ItemCode.FrameM3:
			return new FrameM3(itemData, delim);
		case ItemCode.EnergySourceM1:
			return new EnergySourceM1(itemData, delim);
		case ItemCode.EnergySourceM2:
			return new EnergySourceM2(itemData, delim);
		case ItemCode.EnergySourceM3:
			return new EnergySourceM3(itemData, delim);
		case ItemCode.GearM1:
			return new GearM1(itemData, delim);
		case ItemCode.GearM2:
			return new GearM2(itemData, delim);
		case ItemCode.GearM3:
			return new GearM3(itemData, delim);
		case ItemCode.BuzzSaws:
			return new BuzzSaws(itemData, delim);
		case ItemCode.Knives:
			return new Knives(itemData, delim);
		case ItemCode.Medicinal:
			return new Medicinal(itemData, delim);
		default:
			return new Item(itemData, delim);
		}
	}
	public Item(string itemData, string delim) : this()  {
		string[] split = itemData.Split(delim.ToCharArray());
		itemStackType = (ItemStackType)int.Parse(split[0]);
		itemName = split[1];
		canPlaceInShoulder = int.Parse(split[2]) == 1;
		int money = int.Parse(split[3]);
		gold = money/10000;
		silver = (money/100)%100;
		copper = money%100;
		isKeyItem = int.Parse(split[4])==1;
	/*	string[] textures = split[5].Split(textureDelim.ToCharArray());
		setInventoryTextureName(textures.Length > 0 ? textures[0] : "");
		string s = (textures.Length > 1 ? textures[1] : "");
		spritePrefabString = s;
		if (s != "" && s != null)  {
			spritePrefab = Resources.Load<GameObject>(s);
		}*/
		setInventoryTextureName(split[5]);
		layerAdd = int.Parse(split[6]);

	}
	public void setInventoryTextureName(string s)  {
		inventoryTextureName = s;
		ItemTextureObject to = ItemPrefab.getPrefab(s);
		if (to.name == s)  {
			inventoryTexture = to.texture;
			spritePrefab = to.sprite;
		}
		else {
		}
		/*
		if (inventoryTextureName == null) inventoryTexture = null;
		else if (inventoryTextureName != "") inventoryTexture = Resources.Load<Sprite>(inventoryTextureName);*/
	}
	public void setInventoryTexture(Sprite s)  {
		inventoryTexture = s;
	/*	inventoryTextureName = AssetDatabase.GetAssetPath(inventoryTexture);
		if (inventoryTextureName != null && inventoryTextureName.Length >= 17)  {
			inventoryTextureName = inventoryTextureName.Substring(17, inventoryTextureName.Length - 17 - 4);
		}*/
	}
/*	public void setSpritePrefabName(string s)  {
		spritePrefabString = s;

//		if (spritePrefabString == null) spritePrefab = null;
//		else if (spritePrefabString != "") spritePrefab = Resources.Load<Sprite>(spritePrefabString);
	}*/

	public virtual ItemCode getItemCode()  {
		return ItemCode.Item;
	}
	public virtual string getItemData()  {
		return getItemData(delimiter);
	}

	public virtual string getBlackMarketText() {
		return itemName;
	}
	public string getBlackMarketPriceText() {
		return (gold == 0 ? "" : gold + "g ") + (silver == 0 && gold == 0 ? "" : silver + "s ") + copper + "c";
	}
	public int getPrice() {
		return gold * 10000 + silver * 100 + copper;
	}
	public string textureDelim = "@";
	public virtual string getItemData(string delim)  {
		Debug.Log("Item Get Data: " + itemName);

	//	Debug.Log(spritePrefabString);
		return (int)itemStackType + delim +
			itemName + delim +
				(canPlaceInShoulder ? 1 : 0) + delim +
				(gold * 10000 + silver*100 + copper) + delim +
				(isKeyItem ? 1 : 0) + delim +
				(inventoryTextureName == null ? "" : inventoryTextureName) + delim +
//				(spritePrefabString!= null && spritePrefabString!="" ? textureDelim + spritePrefabString : "") + delim +
				layerAdd;
	}
	public virtual Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0)};
	}
	public Item(string itemName, ItemType itemType, bool canPlaceShoulder, int gold, int silver, int copper, bool isKeyItem, string inventoryTextureSpritePrefabName, int layerAdd) : this()  {
	//	string inventoryTextureName = inventoryTextures[inventoryTextureSpritePrefabName];
	//	string spritePrefabName = prefabs[inventoryTextureSpritePrefabName];
		this.itemName = itemName;
		this.itemType = itemType;
		this.canPlaceInShoulder = canPlaceShoulder;
		this.gold = gold;
		this.silver = silver;
		this.copper = copper;
		this.isKeyItem = isKeyItem;
		setInventoryTextureName(inventoryTextureSpritePrefabName);
		/*if (inventoryTexture != null)
			setInventoryTexture(inventoryTexture);
		if (spritePrefab != null)
			this.spritePrefab = spritePrefab;*/
	//	this.spritePrefabString = spritePrefabName;
		this.layerAdd = layerAdd;
	/*	string s = AssetDatabase.GetAssetPath(spritePrefab);
		if (s != null && s.Length >= 17)  {
			s = s.Substring(17, s.Length - 17 - 7);
		}
		spritePrefabString = s;*/

	}
	public Vector2 getSize()  {
		int maxWidth = 1;
		int maxHeight = 1;
		Vector2[] shape = getShape();
		for (int n=1;n<shape.Length;n++)  {
			maxWidth = Mathf.Max(maxWidth, (int)shape[n].x+1);
			maxHeight = Mathf.Max(maxHeight, (int)shape[n].y+1);
		}
		return new Vector2(maxWidth, maxHeight);
	}
	public Vector2 getBottomRightCell()  {
		int maxWidth = 0;
		int yMax = (int)getSize().y - 1;
		Vector2[] shape = getShape();
		for (int n=1;n<shape.Length;n++)  {
			if ((int)shape[n].y!=yMax) continue;
			maxWidth = Mathf.Max(maxWidth, (int)shape[n].x);
		}
		return new Vector2(maxWidth, yMax);
	}
	public Item()  {
		stack = new List<Item>();
	}

	public bool removeItemFromStack(Item i)  {
		if (i==this) return false;
		if (i.itemStackType!=itemStackType) return false;
		if (stack.Contains(i))  {
			stack.Remove(i);
			return true;
		}
		return false;
	}

	public Item popStack()  {
		if (stack.Count==0) return null;
		Item i = stack[stack.Count-1];
		stack.Remove(i);
		return i;
	}
	public Item addToStack(Item i)  {
		stack.Add(i);
		return i;
	}
	public int stackSize()  {
		return stack.Count+1;
	}
	
}

public class WeaponMechanical : Weapon, ItemMechanical  {
	public bool overClocked = false;
	public StackType getStackType()  {
		return StackType.None;
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.WeaponMechanical;
	}
	public WeaponMechanical(string itemName, ItemType itemType, bool canPlaceItemInShoulder, int gold, int silver, int copper, bool isKeyItem, string inventoryTextureSpritePrefabName, int layerAdd, int hit, int range, int numberOfDamageDice, int diceType, int damageBonus, DamageType damageType, int criticalChance, int durabilityChance, bool isRanged, Vector2[] shape)
	: base(itemName, itemType, canPlaceItemInShoulder, gold, silver, copper, isKeyItem, inventoryTextureSpritePrefabName, layerAdd, hit, range, numberOfDamageDice, diceType, damageBonus, damageType, criticalChance, durabilityChance, isRanged, shape)  {
	}
	public WeaponMechanical(string itemData, string delim) : base(itemData, delim)  {

	}
}

public class Weapon : Item  {
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
	public Weapon(string itemData, string delim) : base(itemData, delim)  {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
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
		if (shapeSize != 0)  {
			shape = new Vector2[shapeSize];
			for (int n=0;n<shapeSize;n++)  {
				shape[n] = new Vector2(int.Parse(split[curr++]), int.Parse(split[curr++]));
			}
		}
	}
	public override string getBlackMarketText() {
		return itemName + "\n" + numberOfDamageDice + "d" + diceType + " " + (isRanged ? "Ranged" : "Melee") + " Damage" + "\nRange: " + range + "\nCrit: " + criticalChance + "%";
	}
	public override string getItemData(string delim)  {
		Debug.Log("Weapon Get Data: " + itemName);
		string shapeString = "0";
		if (shape != null)  {
			shapeString = shape.Length.ToString();
			for (int n=0;n<shape.Length;n++)  {
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
	public Weapon()  {

	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Weapon;
	}
	public Weapon(string itemName, ItemType itemType, bool canPlaceItemInShoulder, int gold, int silver, int copper, bool isKeyItem, string inventoryTextureSpritePrefabName, int layerAdd, int hit, int range, int numberOfDamageDice, int diceType, int damageBonus, DamageType damageType, int criticalChance, int durabilityChance, bool isRanged, Vector2[] shape) :
	base(itemName, itemType, canPlaceItemInShoulder, gold, silver, copper, isKeyItem, inventoryTextureSpritePrefabName, layerAdd)  {
		//this.layerAdd = layerAdd;
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

	
	public override Vector2[] getShape()  {
		return shape;
	}
	
	
	public int rollDamage()  {
		return rollDamage(false);
	}
	
	public int rollDamage(bool critical)  {
		int damageDealt = 0;
		for(int i = 0; i < numberOfDamageDice; i++)  {
			damageDealt += (critical ? diceType : Random.Range(1, diceType+1));
		}
		Debug.Log ("Damage Dealt:" + damageDealt);
		return damageDealt;
	}
}

public class Medicinal : Weapon  {
	public int numberOfUses = 4;
	
	public override string getBlackMarketText() {
		return itemName + "\n" + numberOfDamageDice + "d" + diceType + " Healing";
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Medicinal;
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}


	public Medicinal(string itemData, string delim) : base(itemData, delim)  {

	}

	public Medicinal()  {
		
	}
	public Medicinal(string itemName, ItemType itemType, bool canPlaceItemInShoulder, int gold, int silver, int copper, bool isKeyItem, string inventoryTextureSpritePrefabName, int layerAdd, int hit, int range, int numberOfDamageDice, int diceType, int damageBonus, DamageType damageType, int criticalChance, int durabilityChance, bool isRanged, Vector2[] shape) :
	base(itemName, itemType, canPlaceItemInShoulder, gold, silver, copper, isKeyItem, inventoryTextureSpritePrefabName, layerAdd, hit, range, numberOfDamageDice, diceType, damageBonus, damageType, criticalChance, durabilityChance, isRanged, shape)  {

	}
}

public class Armor : Item  {
	public ArmorType armorType;
	public int AC;
	public Armor(string itemData, string delim) : base(itemData, delim)  {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		armorType = (ArmorType)int.Parse(split[curr++]);
		AC = int.Parse(split[curr++]);
	}
	public override string getBlackMarketText() {
		return itemName + "\nAC: " + AC;
	}
	public override string getItemData(string delim)  {
	//	return AC + "  " + (int)armorType;
		return base.getItemData(delim) + delim +
			(int)armorType + delim +
				AC;
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Armor;
	}
	public Armor(string itemName, ItemType itemType, bool canPlaceItemInShoulder, int gold, int silver, int copper, bool isKeyItem, string inventoryTextureSpritePrefabName, int layerAdd, ArmorType armorType, int AC) :
	base(itemName, itemType, canPlaceItemInShoulder, gold, silver, copper, isKeyItem, inventoryTextureSpritePrefabName, layerAdd)  {
		this.armorType = armorType;
		this.AC = AC;
	}
	
	public override Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

