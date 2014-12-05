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
	public int getRange() {
		return range;
	}
	public Turret(Frame fr, Applicator app, Gear g, EnergySource es) {
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
	public virtual int getDurability() {
		return 0;
	}
	public virtual int getHardness() {
		return 0;
	}
	public virtual int getSize() {
		return 0;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class TestFrame : Frame {
	public override int getDurability() {
		return 5;
	}
	public override int getHardness() {
		return 17;
	}
	public override int getSize () {
		return 2;
	}
	public TestFrame() {
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/Frame");
	}
}

public class Applicator :  Weapon, ItemMechanical {
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0)};
	}
	public Applicator() {
		itemType = ItemType.Weapon;
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
	public virtual int getMaxTurns() {
		return 0;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0)};
	}
}

public class TestEnergySource : EnergySource {
	public override int getMaxTurns() {
		return 5;
	}
	public TestEnergySource() {
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
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/Gear");
	}
}

public class Trigger : Item, ItemMechanical {
	public virtual int triggerTimes() {
		return 0;
	}
	public override Vector2[] getShape() {
		return new Vector2[] {new Vector2(0,0), new Vector2(0,1), new Vector2(1,0), new Vector2(1,1)};
	}
}

public class TestTrigger : Trigger {
	public override int triggerTimes() {
		return 1;
	}
	public TestTrigger() {
		inventoryTexture = Resources.Load<Texture>("Units/Turrets/Trigger");
	}
}
