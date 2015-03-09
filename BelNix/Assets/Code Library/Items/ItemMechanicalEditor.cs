using System;
using UnityEngine;
using System.Collections;

public enum StackType  {None, Turret, Trap, Frame, Gear, Trigger, EnergySource, Knives, BuzzSaws};
public class ItemMechanicalEditor : EditorItem  {

	public ItemMechanicalEditor frame;
	public ItemMechanicalEditor applicator;
	public ItemMechanicalEditor gear;
	public ItemMechanicalEditor energySource;
	public ItemMechanicalEditor trigger;
	public string creatorId = "";
	public int range = 5;


	
	public ItemCode itemCode;
	
	public override Item getItem()  {
		switch (itemCode)  {
		case ItemCode.Turret:
			return new Turret(creatorId, (Frame)frame.getItem(), (Applicator)applicator.getItem(), (Gear)gear.getItem(), (EnergySource)energySource.getItem());
		case ItemCode.Trap:
			return new Trap(creatorId, (Frame)frame.getItem(), (Applicator)applicator.getItem(), (Gear)gear.getItem(), (Trigger)trigger.getItem());
		case ItemCode.Frame:
			return new Frame();
		case ItemCode.TestFrame:
			return new TestFrame();
		case ItemCode.Applicator:
			return new Applicator();
		case ItemCode.TestApplicator:
			return new TestApplicator();
		case ItemCode.Gear:
			return new Gear();
		case ItemCode.TestGear:
			return new TestGear();
		case ItemCode.EnergySource:
			return new EnergySource();
		case ItemCode.TestEnergySource:
			return new TestEnergySource();
		case ItemCode.Trigger:
			return new Trigger();
		case ItemCode.TestTrigger:
			return new TestTrigger();
		case ItemCode.TriggerM1:
			return new TriggerM1();
		case ItemCode.TriggerM2:
			return new TriggerM1();
		case ItemCode.TriggerM3:
			return new TriggerM1();
		case ItemCode.GearM1:
			return new GearM1();
		case ItemCode.GearM2:
			return new GearM2();
		case ItemCode.GearM3:
			return new GearM3();
		case ItemCode.EnergySourceM1:
			return new EnergySourceM1();
		case ItemCode.EnergySourceM2:
			return new EnergySourceM2();
		case ItemCode.EnergySourceM3:
			return new EnergySourceM3();
		case ItemCode.FrameM1:
			return new FrameM1();
		case ItemCode.FrameM2:
			return new FrameM2();
		case ItemCode.FrameM3:
			return new FrameM3();
		case ItemCode.Knives:
			return new Knives();
		case ItemCode.BuzzSaws:
			return new BuzzSaws();
		default:
			return null;
		}
	}

}


public interface ItemMechanical  {
	StackType getStackType();
}

public class Turret : Item, ItemMechanical  {
	public Frame frame;
	public Applicator applicator;
	public Gear gear;
	public EnergySource energySource;
	public string creatorId = "";
	const int range = 5;
	public override string getBlackMarketText() {
		return itemName + "\nFrame: " + frame.itemName + "\nGear: " + gear.itemName + "\nApplicator: " + applicator.itemName + "\nEnergy Source: " + energySource.itemName;
	}
	public StackType getStackType()  {
		return StackType.Turret;
	}
	public override string getItemData (string delim)  {
		string s = base.getItemData(delim) + delim;
			s += (frame == null ? 0 + delim : 
			 (int)frame.getItemCode() + delim +
		      frame.getItemData(otherDelimiter) + delim);
				s += (int)applicator.getItemCode() + delim +
			applicator.getItemData(otherDelimiter) + delim;
				s += (int)gear.getItemCode() + delim +
			gear.getItemData(otherDelimiter) + delim;
				s += (int)energySource.getItemCode() + delim +
			energySource.getItemData(otherDelimiter) + delim;
				s += creatorId;
		return s;
		return base.getItemData(delim) + delim +
			(frame == null ? 0 + delim : 
			 (int)frame.getItemCode() + delim +
			 frame.getItemData(otherDelimiter) + delim) +
				(int)applicator.getItemCode() + delim +
				applicator.getItemData(otherDelimiter) + delim +
				(int)gear.getItemCode() + delim +
				gear.getItemData(otherDelimiter) + delim +
				(int)energySource.getItemCode() + delim +
				energySource.getItemData(otherDelimiter) + delim +
				creatorId;
	}
	public Turret(string itemData, string delim) : base(itemData, delim)  {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		ItemCode frameCode = (ItemCode)int.Parse(split[curr++]);
		if (frameCode != ItemCode.None)
			frame = (Frame)Item.deserializeItem(frameCode, split[curr++], otherDelimiter);
		applicator = (Applicator)Item.deserializeItem((ItemCode)int.Parse(split[curr++]), split[curr++], otherDelimiter);
		gear = (Gear)Item.deserializeItem((ItemCode)int.Parse(split[curr++]), split[curr++], otherDelimiter);
		energySource = (EnergySource)Item.deserializeItem((ItemCode)int.Parse(split[curr++]), split[curr++], otherDelimiter);
		if (curr < split.Length)
			creatorId = split[curr++];
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Turret;
	}
	public int turnsLeft()  {
		return energySource.turnsLeft;
	}
	public int maxTurns()  {
		return energySource.getMaxTurns();
	}
	public bool hasUsesLeft()  {
		return energySource.hasUsesLeft();
	}
	public bool use()  {
		return energySource.use();
	}
	public int rollDamage()  {
		return applicator.rollDamage() + gear.additionalDamage();
	}
	public int getRange()  {
		return range;
	}
	public bool takeDamage(int amount)  {
		return frame.takeDamage(amount);
	}
	public bool isDestroyed()  {
		return frame.isDestroyed();
	}
	public int getHealth()  {
		return frame.healthLeft;
	}
	public int getMaxHealth()  {
		return frame.getDurability();
	}
	public Turret(string creator, Frame fr, Applicator app, Gear g, EnergySource es)  {
		itemName = "Turret";
		itemStackType = ItemStackType.Turret;
		frame = fr;
		applicator = app;
		gear = g;
		energySource = es;
		setInventoryTextureName("Turret");//"Units/Turrets/TurretPlaceholder");
		creatorId = creator;
	}
	public Turret()  {
	}
	public override Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class Trap : Item, ItemMechanical  {
	public Frame frame;
	public Gear gear;
	public Applicator applicator;
	public Trigger trigger;
	public string creatorId = "";
	public bool removeTrap = false;
	public override string getBlackMarketText() {
		return itemName + "\nFrame: " + frame.itemName + "\nGear: " + gear.itemName + "\nApplicator: " + applicator.itemName + "\nTrigger: " + trigger.itemName;
	}
	public StackType getStackType()  {
		return StackType.Trap;
	}
	public override string getItemData (string delim)  {
		return base.getItemData(delim) + delim +
			(frame == null ? 0 + delim : 
			 (int)frame.getItemCode() + delim +
			 frame.getItemData(otherDelimiter) + delim) +
				(int)applicator.getItemCode() + delim +
				applicator.getItemData(otherDelimiter) + delim +
				(int)gear.getItemCode() + delim +
				gear.getItemData(otherDelimiter) + delim +
				(int)trigger.getItemCode() + delim +
				trigger.getItemData(otherDelimiter) + delim +
				creatorId;
	}
	public Trap(string itemData, string delim) : base(itemData, delim)  {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		ItemCode frameCode = (ItemCode)int.Parse(split[curr++]);
		if (frameCode != ItemCode.None)
			frame = (Frame)Item.deserializeItem(frameCode, split[curr++], otherDelimiter);
		applicator = (Applicator)Item.deserializeItem((ItemCode)int.Parse(split[curr++]), split[curr++], otherDelimiter);
		gear = (Gear)Item.deserializeItem((ItemCode)int.Parse(split[curr++]), split[curr++], otherDelimiter);
		trigger = (Trigger)Item.deserializeItem((ItemCode)int.Parse(split[curr++]), split[curr++], otherDelimiter);
		if (curr < split.Length)
			creatorId = split[curr++];
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Trap;	
	}
	public int getMaxSize()  {
		return frame.getSize();
	}
	
	public bool hasUsesLeft()  {
		return trigger.hasUsesLeft();
	}
	public bool use()  {
		return trigger.use();
	}
	public int rollDamage()  {
		return applicator.rollDamage() + gear.additionalDamage();
	}
	public Trap(string creator, Frame fr, Applicator app, Gear g, Trigger tr)  {
		itemName = "Trap";
		frame = fr;
		applicator = app;
		gear = g;
		trigger = tr;
		setInventoryTextureName("Trap");
		creatorId = creator;
	}
	public override Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class Frame : Item, ItemMechanical  {
	internal int healthLeft;
	public StackType getStackType()  {
		return StackType.Frame;
	}
	public override string getItemData (string delim)  {
		return base.getItemData(delim) + delim +
			healthLeft;
	}
	public Frame(string itemData, string delim) : base(itemData, delim)  {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		healthLeft = int.Parse(split[curr++]);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Frame;
	}
	public override string getBlackMarketText() {
		return itemName + "\nDurability: " + getDurability() + "\nHardness: " + getHardness() + "\nSize: " + getSize();
	}
	public Frame()  {
		itemStackType = ItemStackType.Frame;
		healthLeft = getDurability();
	}
	public virtual int getDurability()  {
		return 0;
	}
	public virtual int getHardness()  {
		return 0;
	}
	public new virtual int getSize()  {
		return 0;
	}
	public bool isDestroyed()  {
		return healthLeft <= 0;
	}
	public bool takeDamage(int amount)  {
		healthLeft-=amount;
		Debug.Log("Took Damage: " + amount + "  left: " + healthLeft);
		return isDestroyed();
	}
	public override Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class TestFrame : Frame  {
	public TestFrame(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.TestFrame;
	}
	public override int getDurability()  {
		return 5 + 60;
	}
	public override int getHardness()  {
		return 10;
	}
	public override int getSize ()  {
		return 6;
	}
	public TestFrame()  {
		itemName = "Test Frame";
		setInventoryTextureName("Units/Turrets/Frame");
	}
}

public class FrameM1 : Frame  {
	public FrameM1(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.FrameM1;
	}
	public override int getDurability()  {
		return 5;
	}
	public override int getHardness()  {
		return 5;
	}
	public override int getSize ()  {
		return 1;
	}
	public FrameM1()  {
		itemName = "Frame Mark 1";
		setInventoryTextureName("FrameM1");
	}
}

public class FrameM2 : Frame  {
	public FrameM2(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.FrameM2;
	}
	public override int getDurability()  {
		return 7;
	}
	public override int getHardness()  {
		return 7;
	}
	public override int getSize ()  {
		return 2;
	}
	public FrameM2()  {
		itemName = "Frame Mark 2";
		setInventoryTextureName("FrameM2");
	}
}

public class FrameM3 : Frame  {
	public FrameM3(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.FrameM3;
	}
	public override int getDurability()  {
		return 10;
	}
	public override int getHardness()  {
		return 10;
	}
	public override int getSize ()  {
		return 3;
	}
	public FrameM3()  {
		itemName = "Frame Mark 3";
		setInventoryTextureName("FrameM3");
	}
}

public class Applicator :  Weapon, ItemMechanical  {
	public virtual StackType getStackType()  {
		return StackType.None;
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public Applicator(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Applicator;
	}
	public override Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0)};
	}
	public Applicator()  {
		itemType = ItemType.Weapon;
		itemStackType = ItemStackType.Applicator;
		gold = 0;
		silver = 0;
		copper = 0;
		isKeyItem = false;
		hit = 0;
		range = 0;
		numberOfDamageDice = 0;
		diceType = 0;
		damageBonus = 0;
		damageType = DamageType.Piercing;
		criticalChance = 0;
		durabilityChance = 0;
		isRanged = false;
	}
}

public class TestApplicator : Applicator  {
	public TestApplicator(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.TestApplicator;
	}
	public TestApplicator()  {
		itemName = "Test Applicator";
		copper = 30;
		range = 1;
		numberOfDamageDice = 1;
		diceType = 6;
		damageType = DamageType.Piercing;
		criticalChance = 5;
		durabilityChance = 70;
		setInventoryTextureName("Units/Turrets/Applicator");
	}
}

public class Knives : Applicator  {
	public override StackType getStackType()  {
		return StackType.Knives;
	}
	public Knives(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Knives;
	}
	public Knives()  {
		itemName = "Knives";
		copper = 30;
		range = 1;
		numberOfDamageDice = 1;
		diceType = 4;
		damageType = DamageType.Piercing;
		criticalChance = 0;
		durabilityChance = 70;
		setInventoryTextureName("Knives");
	}
}

public class BuzzSaws : Applicator  {
	public override StackType getStackType()  {
		return StackType.BuzzSaws;
	}
	public BuzzSaws(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.BuzzSaws;
	}
	public BuzzSaws()  {
		itemName = "Buzz Saws";
		copper = 30;
		range = 1;
		numberOfDamageDice = 1;
		diceType = 6;
		damageType = DamageType.Piercing;
		criticalChance = 0;
		durabilityChance = 70;
		setInventoryTextureName("Buzz Saws");
	}
}

	

public class EnergySource :  Item, ItemMechanical  {
	public StackType getStackType()  {
		return StackType.EnergySource;
	}
	public override string getItemData (string delim)  {
		return base.getItemData (delim) + delim +
			turnsLeft;
	}
	public EnergySource(string itemData, string delim) : base(itemData, delim)  {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		turnsLeft = getMaxTurns();// int.Parse(split[curr++]);
	}
	public override string getBlackMarketText() {
		return itemName + "\nLasts " + getMaxTurns() + " Turns";
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.EnergySource;
	}
	public int turnsLeft;
	public EnergySource()  {
		turnsLeft = getMaxTurns();
	}
	public bool use()  {
		turnsLeft--;
		return turnsLeft<=0;
	}
	public bool hasUsesLeft()  {
		return turnsLeft > 0;
	}
	public virtual int getMaxTurns()  {
		return 0;
	}
	public override Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0)};
	}
}

public class TestEnergySource : EnergySource  {
	public TestEnergySource(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.TestEnergySource;
	}
	public override int getMaxTurns()  {
		return 3;
	}
	public TestEnergySource()  {
		itemName = "Test Energy Source";
		setInventoryTextureName("Units/Turrets/EnergySource");
	}
}

public class EnergySourceM1 : EnergySource  {
	public EnergySourceM1(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.EnergySourceM1;
	}
	public override int getMaxTurns()  {
		return 3;
	}
	public EnergySourceM1()  {
		itemName = "Energy Source Mark 1";
		setInventoryTextureName("Energy Source M1");
	}
}

public class EnergySourceM2 : EnergySource  {
	public EnergySourceM2(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.EnergySourceM2;
	}
	public override int getMaxTurns()  {
		return 5;
	}
	public EnergySourceM2()  {
		itemName = "Energy Source Mark 2";
		setInventoryTextureName("Energy Source M2");
	}
}

public class EnergySourceM3 : EnergySource  {
	public EnergySourceM3(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.EnergySourceM3;
	}
	public override int getMaxTurns()  {
		return 7;
	}
	public EnergySourceM3()  {
		itemName = "Energy Source Mark 3";
		setInventoryTextureName("Energy Source M3");
	}
}

public class Gear :  Item, ItemMechanical  {
	public StackType getStackType()  {
		return StackType.Gear;
	}
	public Gear(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public Gear()  {

	}
	public override string getBlackMarketText() {
		return itemName + "\n" + additionalDamage() + " Additional Damage";
	}
	
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Gear;
	}
	public virtual int additionalDamage()  {
		return 0;
	}
	public override Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0)};
	}
}

public class TestGear : Gear  {
	public TestGear(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.TestGear;
	}
	public override int additionalDamage()  {
		return 2;
	}
	public TestGear()  {
		itemName = "Test Gear";
		setInventoryTextureName("Units/Turrets/Gear");
	}
}

public class GearM1 : Gear  {
	public GearM1(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.GearM1;
	}
	public override int additionalDamage()  {
		return 0;
	}
	public GearM1()  {
		itemName = "Gear Mark 1";
		setInventoryTextureName("Gear M1");
	}
}

public class GearM2 : Gear  {
	public GearM2(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.GearM2;
	}
	public override int additionalDamage()  {
		return 1;
	}
	public GearM2()  {
		itemName = "Gear Mark 2";
		setInventoryTextureName("Gear M2");
	}
}

public class GearM3 : Gear  {
	public GearM3(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.GearM3;
	}
	public override int additionalDamage()  {
		return 2;
	}
	public GearM3()  {
		itemName = "Gear Mark 3";
		setInventoryTextureName("Gear M3");
	}
}

public class Trigger : Item, ItemMechanical  {
	public StackType getStackType()  {
		return StackType.Trigger;
	}
	public override string getBlackMarketText() {
		return itemName + "\nTriggers " + triggerTimes() + " Times";
	}
	public override string getItemData (string delim)  {
		return base.getItemData (delim) + delim +
			usesLeft;
	}
	public Trigger(string itemData, string delim) : base(itemData, delim)  {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		usesLeft = int.Parse(split[curr++]);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.Trigger;
	}
	public int usesLeft;
	public Trigger()  {
		usesLeft = triggerTimes();
	}
	public bool use()  {
		usesLeft--;
		return usesLeft<=0;
	}
	public bool hasUsesLeft()  {
		return usesLeft > 0;
	}
	public virtual int triggerTimes()  {
		return 0;
	}
	public override Vector2[] getShape()  {
		return new Vector2[]  {new Vector2(0,0)};
	}
}

public class TestTrigger : Trigger  {
	public TestTrigger(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.TestTrigger;	
	}
	public override int triggerTimes()  {
		return 3;
	}
	public TestTrigger()  {
		itemName = "Test Trigger";
		setInventoryTextureName("Trigger M1");
	}
}

public class TriggerM1 : Trigger  {
	public TriggerM1(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.TriggerM1;	
	}
	public override int triggerTimes()  {
		return 1;
	}
	public TriggerM1()  {
		itemName = "Trigger Mark 1";
		setInventoryTextureName("Trigger M1");
	}
}

public class TriggerM2 : Trigger  {
	public TriggerM2(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.TriggerM2;	
	}
	public override int triggerTimes()  {
		return 2;
	}
	public TriggerM2()  {
		itemName = "Trigger Mark 2";
		setInventoryTextureName("Trigger M2");
	}
}

public class TriggerM3 : Trigger  {
	public TriggerM3(string itemData, string delim) : base(itemData, delim)   {
		
	}
	public override string getItemData(string delim) {
		return base.getItemData(delim);
	}
	public override ItemCode getItemCode ()  {
		return ItemCode.TriggerM3;	
	}
	public override int triggerTimes()  {
		return 3;
	}
	public TriggerM3()  {
		itemName = "Trigger Mark 3";
		setInventoryTextureName("Trigger M3");
	}
}
