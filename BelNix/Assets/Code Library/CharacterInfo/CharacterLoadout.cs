using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace CharacterInfo
{
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
	public class CharacterLoadoutActual {
		public Armor headSlot;
		public Armor chestSlot;
		public Armor gloveSlot;
		public Armor pantsSlot;
		public Armor bootsSlot;
		public Weapon rightHand;
		public Weapon leftHand;
		public Item shoulderSlot;

		public CharacterLoadoutActual(CharacterLoadout loadout) {
			if (loadout.headSlot) headSlot = loadout.headSlot.getArmor();
			if (loadout.chestSlot) chestSlot = loadout.chestSlot.getArmor();
			if (loadout.gloveSlot) gloveSlot = loadout.gloveSlot.getArmor();
			if (loadout.pantsSlot) pantsSlot = loadout.pantsSlot.getArmor();
			if (loadout.bootsSlot) bootsSlot = loadout.bootsSlot.getArmor();
			if (loadout.rightHand) rightHand = loadout.rightHand.getWeapon();
			if (loadout.leftHand) leftHand = loadout.leftHand.getWeapon();
			if (loadout.shoulderSlot) shoulderSlot = loadout.shoulderSlot.getItem();
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
}
