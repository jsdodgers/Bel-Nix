using System;
using UnityEngine;
using System.Collections.Generic;

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

	public struct ItemReturn {
		public Item item;
		public Vector2 slot;
		public ItemReturn(Item i, Vector2 s) {
			item = i;
			slot = s;
		}
	}

	public class InventoryItemSlot {
		public Item item;
		public int index;
		public InventoryItemSlot itemSlot;
		public List<InventoryItemSlot> otherSlots;
		public InventoryItemSlot(int index) 				{ this.index = index; otherSlots = new List<InventoryItemSlot>(); }
		public bool hasItem() 							{ return item!=null || itemSlot != null; }
		public Item getItem() 							{ return itemSlot.item; }
		public void removeItem()						{ item = null; itemSlot = null; otherSlots = new List<InventoryItemSlot>(); }
		public void addOtherSlot(InventoryItemSlot slot) 	{ otherSlots.Add(slot); }
	}

	public class Inventory {
		public Character character;
		private Purse cPurse;
		public InventoryItemSlot[] inventory;

		public Inventory () {
			inventory = new InventoryItemSlot[16];
			for (int n=0;n<16;n++) {
				inventory[n] = new InventoryItemSlot(n);
			}
		}

		public int getStackabilityOfItem(Item i) {
			if (character==null) return 1;
			return character.stackabilityOfItem(i);
		}

		public int getIndexForSlot(Vector2 v) {
			if (v.x < 0 || v.y < 0 || v.x > 3 || v.y > 3) return -1;
			return ((int)v.x) + ((int)v.y)*4;
		}
		public Vector2 getSlotForIndex(int slot) {
			return new Vector2(slot%4,slot/4);
		}
		public bool canInsertItemInSlot(Item i, Vector2 slot) {
			if (i==null) return false;
			foreach (Vector2 itemSlot in i.getShape()) {
				Vector2 actualSlot = slot + itemSlot;
				int index = getIndexForSlot(actualSlot);
				if (index==-1 || inventory[index].hasItem()) return false;
			}
			return true;
		}
		public void insertItemInSlot(Item i, Vector2 slot) {
			if (!canInsertItemInSlot(i,slot)) return;
			int ind = getIndexForSlot(slot);
			InventoryItemSlot sl = inventory[ind];
			sl.item = i;
			sl.itemSlot = sl;
			foreach (Vector2 itemSlot in i.getShape()) {
				Vector2 actualSlot = slot + itemSlot;
				int index = getIndexForSlot(actualSlot);
				InventoryItemSlot invenSlot = inventory[index];
				if (itemSlot.x==0 && itemSlot.y==0) {
					continue;
				}
				else {
					sl.addOtherSlot(invenSlot);
					invenSlot.itemSlot = sl;
				}
			}
		}
		public ItemReturn removeItemFromSlot(Vector2 slot) {
			InventoryItemSlot sl = inventory[getIndexForSlot(slot)];
			InventoryItemSlot actualSlot = sl.itemSlot;
			Vector2 actualSlotVec = getSlotForIndex(actualSlot.index);
			Item i = actualSlot.getItem();
			foreach (InventoryItemSlot slots in actualSlot.otherSlots) {
				slots.removeItem();
			}
			actualSlot.removeItem();
			return new ItemReturn(i, slot - actualSlotVec);
		}
	}
}

