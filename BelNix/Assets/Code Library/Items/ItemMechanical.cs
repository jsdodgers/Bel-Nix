using System;
using UnityEngine;
using System.Collections;

public class ItemMechanical : Item1 {
	public ItemMechanical() {
		itemType = ItemType.Mechanical;
	}
}

public class Turret : ItemMechanical {
	public Frame frame;
	public Applicator applicator;
	public Gear gear;
	public EnergySource energySource;

	public Turret(Frame fr, Applicator app, Gear g, EnergySource es) {
		frame = fr;
		applicator = app;
		gear = g;
		energySource = es;
	}
}

public class Frame : ItemMechanical {

}

public class Applicator : ItemMechanical {

}

public class EnergySource : ItemMechanical {

}

public class Gear : ItemMechanical {

}
