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


        public bool spendMoney(int copper, int silver, int gold)
        {
            // Check for invalid input
            if (copper < 0 || silver < 0 || gold < 0)
                throw new InvalidOperationException("Invalid Parameter: Can't spend negative money.");
            if (!enoughMoney(copper, silver, gold))
                return false;

            GOLD -= gold;
            spendFromPools(silver, ref SILVER, ref GOLD);
            spendFromPools(copper, ref COPPER, ref SILVER);
            return true;
        }

        // Compare the amount being spent against the money in the purse
        public bool enoughMoney(int copper, int silver, int gold)
        {
            int purseTotal = COPPER + (SILVER*100) + (GOLD*10000);
            int spentTotal = copper + (silver*100) + (gold*10000);
            return purseTotal >= spentTotal;
        }

        private bool spendFromPools(int amount, ref int lowPool, ref int highPool)
        {
            if (amount == 0)
                return true;
            if (lowPool - amount < 0)    // Pull from the higher pool
            {
                int lowAmountNeeded = amount % 100;
                int highAmountNeeded = 0;
                if (lowPool < lowAmountNeeded)
                    highAmountNeeded = Mathf.CeilToInt(amount / 100);
                else
                    highAmountNeeded = Mathf.FloorToInt(amount / 100);
                if (highPool >= highAmountNeeded)        // if there's enough in the higher pool to pull from
                {
                    highPool -= highAmountNeeded;
                    lowPool = (lowPool + (highAmountNeeded * 100)) - amount;
                    return true;
                }
                else return false;
            }
            else
            {
                lowPool -= amount;
                return true;
            }
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

