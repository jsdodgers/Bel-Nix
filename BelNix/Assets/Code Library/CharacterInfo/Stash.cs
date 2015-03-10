using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stash {

	public List<Item> items = new List<Item>();
	public int money;

	public string moneyString() {
		return moneyString(money%100, (money/100)%100, money/10000);
	}
	public static string moneyString(int c, int s, int g)  {
		return (g > 0 ? g + "g " : "") + (s > 0 || g > 0 ? s + "s " : "") + c + "c";
	}

	public bool canAfford(int price) {
		return money >= price;
	}

	public void removeItem(Item i) {
		if (items.Contains(i)) items.Remove(i);
		saveStash();
	}

	public void addItem(Item i) {
		items.Add(i);
		while (i.stackSize() > 1) {
			items.Add(i.popStack());
		}
		saveStash();
	}

	public bool hasItem(Item i) {
		return items.Contains(i);
	}

	public void addMoney(int mon) {
		money += mon;
		saveStash();
	}

	public void spendMoney(int mon) {
		money -= mon;
		saveStash();
	}

	public void setMoney(int mon) {
		money = mon;
		saveStash();
	}

	public void takeAllMoney(Purse p) {
		if (p != null) {
			money += p.money;
			p.money = 0;
		}
	}

	public void saveStash() {
		Saves.saveStash(getStashString());
	}

	const string delimiter = ";";
	public string getStashString() {
		string stashString = money + delimiter + items.Count + delimiter;
		foreach (Item i in items) {
			stashString += (int)i.getItemCode() + delimiter + i.getItemData() + delimiter;
		}
		return stashString;
	}

	public void loadStash() {
		string stashString = Saves.getStashString();
		string[] split = stashString.Split(delimiter.ToCharArray());
		int curr = 0;
		money = int.Parse(split[curr++]);
		int numItems = int.Parse(split[curr++]);
		for (int n=0;n<numItems;n++)  {
			ItemCode code = (ItemCode)int.Parse(split[curr++]);
			string itemData = split[curr++];
			items.Add(Item.deserializeItem(code, itemData));

		}
	}

}
