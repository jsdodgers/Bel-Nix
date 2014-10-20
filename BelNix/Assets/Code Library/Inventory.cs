using System;
using UnityEngine;
namespace CharacterInfo
{
    public struct Purse
    {
        private int COPPER;
        private int SILVER;
        private int GOLD;
        public void receiveMoney(int copper, int silver, int gold)
        {
            if (copper < 0 || silver < 0 || gold < 0)
                throw new InvalidOperationException("Invalid Parameter: Can't receive negative money");
            int silverRemainder = 0;
            int goldRemainder = 0;
            if (COPPER + copper >= 100)
            {
                silverRemainder = Mathf.FloorToInt((COPPER + copper) / 100);
                COPPER = (COPPER + copper) % 100;
            }
            else
                COPPER += copper;

            if (SILVER + silver + silverRemainder >= 100)
            {
                goldRemainder = Mathf.FloorToInt((SILVER + silver + silverRemainder) / 100);
                SILVER = (SILVER + silver + silverRemainder) % 100;
            }
            else
                SILVER += silver;

            GOLD += (gold + goldRemainder);
        }
        public void spendMoney(int copper, int silver, int gold)
        {
            if (copper < 0 || silver < 0 || gold < 0)
                throw new InvalidOperationException("Invalid Parameter: Can't spend negative money");
            if (GOLD < gold)
                throw new InvalidOperationException("Invalid Parameter: Can't spend negative money (not enough gold)");
            if (SILVER < silver)
                throw new InvalidOperationException("Invalid Parameter: Can't spend negative money (not enough silver)");
            if (COPPER < copper)
                throw new InvalidOperationException("Invalid Parameter: Can't spend negative money (not enough copper)");

        }
    }
	public class Inventory
	{
		private Purse cPurse;

		public Inventory ()
		{
		}
	}
}

