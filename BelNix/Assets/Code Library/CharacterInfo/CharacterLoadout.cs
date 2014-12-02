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
		public Item shoulderSlot;
		public ItemArmor chestSlot;
		public ItemArmor gloveSlot;
		public ItemArmor pantsSlot;
		public ItemArmor bootsSlot;
		public ItemWeapon rightHand;
		public ItemWeapon leftHand;

        public int getAC()
        {
            // sum up and return the AC from all equipped armor

            // For now, just return 10, fill this in later when items and armor are more fleshed out.
			int shoulderSlotAC = 0;
			if(shoulderSlot != null && shoulderSlot is Armor)
				shoulderSlotAC = ((Armor) shoulderSlot).AC;

			return CharacterConstants.BASE_AC + (headSlot != null ? headSlot.getArmor().AC : 0) + shoulderSlotAC + (chestSlot != null ? chestSlot.getArmor().AC : 0)
				+ (gloveSlot != null ? gloveSlot.getArmor().AC : 0) + (pantsSlot != null ? pantsSlot.getArmor().AC : 0) + (bootsSlot != null ? bootsSlot.getArmor().AC : 0);
        }
    }
}
