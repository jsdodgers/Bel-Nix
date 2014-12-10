using System;
using UnityEngine;
using System.Collections;

public interface ItemMechanical {

}

public class Turret : Item, ItemMechanical {
	public Frame frame;
	public Applicator applicator;
	public Gear gear;
	public EnergySource energySource;
	const int range = 5;
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
	public Turret(Frame fr, Applicator app, Gear g, EnergySource es) {
		itemStackType = ItemStackType.Turret;
		frame = fr;
		applicator = app;
		gear = g;
		energySource = es;
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/TurretPlaceholder");
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
	public Trap(Frame fr, Applicator app, Gear g, Trigger tr) {
		frame = fr;
		applicator = app;
		gear = g;
		trigger = tr;
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/Trap");
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class Frame : Item, ItemMechanical {
	int healthLeft;
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
	public virtual int getSize() {
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
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/Frame");
	}
}

public class Applicator :  Weapon, ItemMechanical {
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
	public TestApplicator() {
		itemName = "Test Applicator";
		copper = 30;
		range = 1;
		numberOfDamageDice = 1;
		diceType = 6;
		damageType = DamageType.Piercing;
		criticalChance = 5;
		durabilityChance = 70;
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/Applicator");
	}
}
	

public class EnergySource :  Item, ItemMechanical {
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
	public override int getMaxTurns() {
		return 2;
	}
	public TestEnergySource() {
		itemName = "Test Energy Source";
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/EnergySource");
	}
}

public class Gear :  Item, ItemMechanical {
	public virtual int additionalDamage() {
		return 0;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0)};
	}
}

public class TestGear : Gear {
	public override int additionalDamage() {
		return 2;
	}
	public TestGear() {
		itemName = "Test Gear";
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/Gear");
	}
}

public class Trigger : Item, ItemMechanical {
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
	public override int triggerTimes() {
		return 3;
	}
	public TestTrigger() {
		itemName = "Test Trigger";
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/Trigger");
	}
}
