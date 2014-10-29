using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemInfo;

namespace CharacterInfo
{
    class CharacterArmor
    {
        // Slots needed
        //      - Question? Will we be able to equip things besides equipment? For example, could key items like 
        //          briefcases, wallets, etc be held in main/offhand
        //          I need to know whether to make these slots for Item or for Equipment
        Item HEAD, SHOULDER, CHEST, BACK, 
             GLOVES, PANTS, BOOTS, 
             WEAPON_RIGHT, WEAPON_LEFT;

        public int getAC()
        {
            // sum up and return the AC from all equipped armor

            // For now, just return 10, fill this in later when items and armor are more fleshed out.
            return 10;
        }
    }
}
