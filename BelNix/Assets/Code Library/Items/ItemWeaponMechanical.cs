using UnityEngine;
using System.Collections;


public class ItemWeaponMechanical : ItemWeapon  {
	public override Item getItem ()  {
		return getWeapon();
	}

	public override Weapon getWeapon()  {
		return new WeaponMechanical(itemName, itemType, canPlaceInShoulder, gold, silver, copper, isKeyItem, inventoryTextureSpritePrefabName, layerAdd, hit, range, numberOfDamageDice, diceType, damageBonus, damageType, criticalChance, durabilityChance, isRanged, shape);
	}
}