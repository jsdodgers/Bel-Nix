using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using CharacterInfo;

public class CharacterLoadout : MonoBehaviour
{
// Slots needed
//      - Question? Will we be able to equip things besides equipment? For example, could key items like 
//          briefcases, wallets, etc be held in main/offhand
//          I need to know whether to make these slots for Item or for Equipment

	public ItemArmor headSlot;
	public EditorItem shoulderSlot;
	public ItemArmor chestSlot;
	public ItemArmor gloveSlot;
	public ItemArmor pantsSlot;
	public ItemArmor bootsSlot;
	public ItemWeapon rightHand;
	public ItemWeapon leftHand;
}

public class SpriteOrder {
	public GameObject sprite;
	public int order;
	public SpriteOrder(GameObject sprite, int order) {
		this.sprite = sprite;
		this.order = order;
	}
}

public class CharacterLoadoutActual {
	public Armor headSlot;
	public Armor chestSlot;
	public Armor gloveSlot;
	public Armor pantsSlot;
	public Armor bootsSlot;
	public Weapon rightHand;
	public Weapon leftHand;
	public Item shoulderSlot;
	public Character character;
	public List<SpriteOrder> sprites;


	public int getOrder(InventorySlot slot) {
		switch (slot) {
		case InventorySlot.RightHand:
			return 9;
		case InventorySlot.LeftHand:
			return 9;
		case InventorySlot.Glove:
			return 8;
		case InventorySlot.Head:
			return 7;
		case InventorySlot.Shoulder:
			return 6;
		case InventorySlot.Chest:
			return 5;
		case InventorySlot.Pants:
			return 4;
		case InventorySlot.Boots:
			return 3;
		default:
			return 0;
		}
	}

	public void setItemInSlot(InventorySlot itemSlot, Item item, CharacterColors colors = null) {
		removeSprite(getItemInSlot(itemSlot));
		if (item.spritePrefab != null) {
			if (colors==null) colors = character.characterSheet.characterColors;
			GameObject sprite = GameObject.Instantiate(item.spritePrefab) as GameObject;
			SpriteRenderer sr = sprite.GetComponent<SpriteRenderer>();
			switch (itemSlot) {
			case InventorySlot.Head:
			case InventorySlot.Chest:
				sr.color = colors.primaryColor;
				break;
			case InventorySlot.Boots:
			case InventorySlot.Pants:
				sr.color = colors.secondaryColor;
				break;
			default:
				return;
			}
			item.sprite = sprite;
			sprite.transform.parent = character.unit.transform;
			sprite.transform.localPosition = new Vector3(0,0,0);
			sprite.transform.localEulerAngles = new Vector3(0, 0, 0);
			sprites.Add(new SpriteOrder(item.sprite, getOrder(itemSlot)));
		}
		switch (itemSlot) {
		case InventorySlot.Head:
			headSlot = (Armor)item;
			break;
		case InventorySlot.Chest:
			chestSlot = (Armor)item;
			break;
		case InventorySlot.Glove:
			gloveSlot = (Armor)item;
			break;
		case InventorySlot.Pants:
			pantsSlot = (Armor)item;
			break;
		case InventorySlot.Boots:
			bootsSlot = (Armor)item;
			break;
		case InventorySlot.RightHand:
			rightHand = (Weapon)item;
			break;
		case InventorySlot.LeftHand:
			leftHand = (Weapon)item;
			break;
		case InventorySlot.Shoulder:
			shoulderSlot = item;
			break;
		}
	}

	public void removeSprite(Item i) {
		if (i==null) return;
		if (i.sprite != null) {
//			if (sprites.Contains(i.sprite)) sprites.Remove(i.sprite);
			foreach (SpriteOrder sprite in sprites) {
				if (sprite.sprite == i.sprite) {
					sprites.Remove(sprite);
					break;
				}
			}
			GameObject.Destroy(i.sprite);
			i.sprite = null;
		}
	}

	public Item getItemInSlot(InventorySlot itemSlot) {
		switch (itemSlot) {
		case InventorySlot.Head:
			return headSlot;
		case InventorySlot.Chest:
			return chestSlot;
		case InventorySlot.Glove:
			return gloveSlot;
		case InventorySlot.Pants:
			return pantsSlot;
		case InventorySlot.Boots:
			return bootsSlot;
		case InventorySlot.RightHand:
			return rightHand;
		case InventorySlot.LeftHand:
			return leftHand;
		case InventorySlot.Shoulder:
			return shoulderSlot;
		default:
			return null;
		}
	}

	public CharacterLoadoutActual(CharacterLoadout loadout, Character character, CharacterColors colors) {
		sprites = new List<SpriteOrder>();
		this.character = character;
		if (loadout.headSlot) setItemInSlot(InventorySlot.Head, loadout.headSlot.getArmor(), colors);
		if (loadout.chestSlot) setItemInSlot(InventorySlot.Chest, loadout.chestSlot.getArmor(), colors);
		if (loadout.gloveSlot) setItemInSlot(InventorySlot.Glove, loadout.gloveSlot.getArmor(), colors);
		if (loadout.pantsSlot) setItemInSlot(InventorySlot.Pants, loadout.pantsSlot.getArmor(), colors);
		if (loadout.bootsSlot) setItemInSlot(InventorySlot.Boots, loadout.bootsSlot.getArmor(), colors);
		if (loadout.rightHand) setItemInSlot(InventorySlot.RightHand, loadout.rightHand.getWeapon(), colors);
		if (loadout.leftHand) setItemInSlot(InventorySlot.LeftHand, loadout.leftHand.getWeapon(), colors);
		if (loadout.shoulderSlot) setItemInSlot(InventorySlot.Shoulder, loadout.shoulderSlot.getItem(), colors);
	}

	public int getAC()
	{
		// sum up and return the AC from all equipped armor
		
		// For now, just return 10, fill this in later when items and armor are more fleshed out.
		int shoulderSlotAC = 0;
		if(shoulderSlot != null && shoulderSlot is Armor)
			shoulderSlotAC = ((Armor) shoulderSlot).AC;
		
		return CharacterConstants.BASE_AC + (headSlot != null ? headSlot.AC : 0) + shoulderSlotAC + (chestSlot != null ? chestSlot.AC : 0)
			+ (gloveSlot != null ? gloveSlot.AC : 0) + (pantsSlot != null ? pantsSlot.AC : 0) + (bootsSlot != null ? bootsSlot.AC : 0);
	}

}
