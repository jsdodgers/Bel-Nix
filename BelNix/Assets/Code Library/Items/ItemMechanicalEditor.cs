using System;
using UnityEngine;
using System.Collections;


public class ItemMechanicalEditor : EditorItem {

	public ItemMechanicalEditor frame;
	public ItemMechanicalEditor applicator;
	public ItemMechanicalEditor gear;
	public ItemMechanicalEditor energySource;
	public ItemMechanicalEditor trigger;
	public string creatorId = "";
	public int range = 5;


	
	public ItemCode itemCode;
	
	public override Item getItem() {
		switch (itemCode) {
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
		default:
			return null;
		}
	}

}


public interface ItemMechanical {

}

public class Turret : Item, ItemMechanical {
	public Frame frame;
	public Applicator applicator;
	public Gear gear;
	public EnergySource energySource;
	public string creatorId = "";
	const int range = 5;
	public override string getItemData (string delim)
	{
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
	public Turret(string itemData, string delim) : base(itemData, delim) {
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
	public override ItemCode getItemCode ()
	{
		return ItemCode.Turret;
	}
	public int turnsLeft() {
		return energySource.turnsLeft;
	}
	public int maxTurns() {
		return energySource.getMaxTurns();
	}
	public bool hasUsesLeft() {
		return energySource.hasUsesLeft();
	}
	public bool use() {
		return energySource.use();
	}
	public int rollDamage() {
		return applicator.rollDamage() + gear.additionalDamage();
	}
	public int getRange() {
		return range;
	}
	public bool takeDamage(int amount) {
		return frame.takeDamage(amount);
	}
	public bool isDestroyed() {
		return frame.isDestroyed();
	}
	public int getHealth() {
		return frame.healthLeft;
	}
	public int getMaxHealth() {
		return frame.getDurability();
	}
	public Turret(string creator, Frame fr, Applicator app, Gear g, EnergySource es) {
		itemStackType = ItemStackType.Turret;
		frame = fr;
		applicator = app;
		gear = g;
		energySource = es;
		inventoryTextureName = "Units/Turrets/TurretPlaceholder";
		inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
		creatorId = creator;
	}
	public Turret() {
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class Trap : Item, ItemMechanical {
	public Frame frame;
	public Gear gear;
	public Applicator applicator;
	public Trigger trigger;
	string creatorId = "";
	public override string getItemData (string delim)
	{
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
	public Trap(string itemData, string delim) : base(itemData, delim) {
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
	public override ItemCode getItemCode ()
	{
		return ItemCode.Trap;	
	}
	public int getMaxSize() {
		return frame.getSize();
	}
	
	public bool hasUsesLeft() {
		return trigger.hasUsesLeft();
	}
	public bool use() {
		return trigger.use();
	}
	public int rollDamage() {
		return applicator.rollDamage() + gear.additionalDamage();
	}
	public Trap(string creator, Frame fr, Applicator app, Gear g, Trigger tr) {
		frame = fr;
		applicator = app;
		gear = g;
		trigger = tr;
		inventoryTextureName = "Units/Turrets/Trap";
		inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
		creatorId = creator;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class Frame : Item, ItemMechanical {
	internal int healthLeft;
	public override string getItemData (string delim)
	{
		return base.getItemData(delim) + delim +
			healthLeft;
	}
	public Frame(string itemData, string delim) : base(itemData, delim) {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		healthLeft = int.Parse(split[curr++]);
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.Frame;
	}
	public Frame() {
		itemStackType = ItemStackType.Frame;
		healthLeft = getDurability();
	}
	public virtual int getDurability() {
		return 0;
	}
	public virtual int getHardness() {
		return 0;
	}
	public new virtual int getSize() {
		return 0;
	}
	public bool isDestroyed() {
		return healthLeft <= 0;
	}
	public bool takeDamage(int amount) {
		healthLeft-=amount;
		Debug.Log("Took Damage: " + amount + "  left: " + healthLeft);
		return isDestroyed();
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class TestFrame : Frame {
	public TestFrame(string itemData, string delim) : base(itemData, delim)  {

	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.TestFrame;
	}
	public override int getDurability() {
		return 5 + 60;
	}
	public override int getHardness() {
		return 10;
	}
	public override int getSize () {
		return 6;
	}
	public TestFrame() {
		itemName = "Test Frame";
		inventoryTextureName = "Units/Turrets/Frame";
		inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
	}
}

public class Applicator :  Weapon, ItemMechanical {
	public Applicator(string itemData, string delim) : base(itemData, delim)  {
		
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.Applicator;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0)};
	}
	public Applicator() {
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

public class TestApplicator : Applicator {
	public TestApplicator(string itemData, string delim) : base(itemData, delim)  {
		
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.TestApplicator;
	}
	public TestApplicator() {
		itemName = "Test Applicator";
		copper = 30;
		range = 1;
		numberOfDamageDice = 1;
		diceType = 6;
		damageType = DamageType.Piercing;
		criticalChance = 5;
		durabilityChance = 70;
		inventoryTextureName = "Units/Turrets/Applicator";
		inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
	}
}
	

public class EnergySource :  Item, ItemMechanical {
	public override string getItemData (string delim)
	{
		return base.getItemData (delim) + delim +
			turnsLeft;
	}
	public EnergySource(string itemData, string delim) : base(itemData, delim) {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		turnsLeft = int.Parse(split[curr++]);
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.EnergySource;
	}
	public int turnsLeft;
	public EnergySource() {
		turnsLeft = getMaxTurns();
	}
	public bool use() {
		turnsLeft--;
		return turnsLeft<=0;
	}
	public bool hasUsesLeft() {
		return turnsLeft > 0;
	}
	public virtual int getMaxTurns() {
		return 0;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0)};
	}
}

public class TestEnergySource : EnergySource {
	public TestEnergySource(string itemData, string delim) : base(itemData, delim)  {
		
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.TestEnergySource;
	}
	public override int getMaxTurns() {
		return 2;
	}
	public TestEnergySource() {
		itemName = "Test Energy Source";
		inventoryTextureName = "Units/Turrets/EnergySource";
		inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
	}
}

public class Gear :  Item, ItemMechanical {
	public Gear(string itemData, string delim) : base(itemData, delim)  {
		
	}
	public Gear() {

	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.Gear;
	}
	public virtual int additionalDamage() {
		return 0;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0)};
	}
}

public class TestGear : Gear {
	public TestGear(string itemData, string delim) : base(itemData, delim)  {
		
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.TestGear;
	}
	public override int additionalDamage() {
		return 2;
	}
	public TestGear() {
		itemName = "Test Gear";
		inventoryTextureName = "Units/Turrets/Gear";
		inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
	}
}

public class Trigger : Item, ItemMechanical {
	public override string getItemData (string delim)
	{
		return base.getItemData (delim) + delim +
			usesLeft;
	}
	public Trigger(string itemData, string delim) : base(itemData, delim) {
		string[] split = itemData.Split(delim.ToCharArray());
		int curr = numSplit;
		usesLeft = int.Parse(split[curr++]);
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.Trigger;
	}
	public int usesLeft;
	public Trigger() {
		usesLeft = triggerTimes();
	}
	public bool use() {
		usesLeft--;
		return usesLeft<=0;
	}
	public bool hasUsesLeft() {
		return usesLeft > 0;
	}
	public virtual int triggerTimes() {
		return 0;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class TestTrigger : Trigger {
	public TestTrigger(string itemData, string delim) : base(itemData, delim)  {
		
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.TestTrigger;	
	}
	public override int triggerTimes() {
		return 3;
	}
	public TestTrigger() {
		itemName = "Test Trigger";
		inventoryTextureName = "Units/Turrets/Trigger";
		inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
	}
}

public class TriggerM1 : Trigger {
	public TriggerM1(string itemData, string delim) : base(itemData, delim)  {
		
	}
	public override ItemCode getItemCode ()
	{
		return ItemCode.TriggerM1;	
	}
	public override int triggerTimes() {
		return 1;
	}
	public TriggerM1() {
		itemName = "Trigger Mark 1";
		inventoryTextureName = "Units/Turrets/Trigger";
		inventoryTexture = Resources.Load<Texture>(inventoryTextureName);
	}
}
