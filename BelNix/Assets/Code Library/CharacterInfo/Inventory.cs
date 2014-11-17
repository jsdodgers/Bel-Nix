using System;
using UnityEngine;
namespace CharacterInfo
{
    public struct Purse
    {
        private int copper;
        private int silver;
        private int gold;

        public void receiveMoney(int c, int s, int g)
        {
            if (c < 0 || s < 0 || g < 0)
                throw new InvalidOperationException("Invalid Parameter: Can't receive negative money");
            int silverRemainder = 0;
            int goldRemainder = 0;
            if (c + copper >= 100)
            {
                silverRemainder = Mathf.FloorToInt((c + copper) / 100);
                copper = (c + copper) % 100;
            }
            else
                copper += c;

            if (s + silver + silverRemainder >= 100)
            {
                goldRemainder = Mathf.FloorToInt((s + silver + silverRemainder) / 100);
                silver = (s + silver + silverRemainder) % 100;
            }
            else
                silver += s;

            gold += (g + goldRemainder);
        }


        public bool spendMoney(int c, int s, int g)
        {
            // Check for invalid input
            if (c < 0 || s < 0 || g < 0)
                throw new InvalidOperationException("Invalid Parameter: Can't spend negative money.");
            if (!enoughMoney(c, s, g))
                return false;

            gold -= g;
            spendFromPools(s, ref silver, ref gold);
            spendFromPools(c, ref copper, ref silver);
            return true;
        }

        // Compare the amount being spent against the money in the purse
        public bool enoughMoney(int c, int s, int g)
        {
            int purseTotal = copper + (silver*100) + (gold*10000);
            int spentTotal = c + (s*100) + (g*10000);
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

