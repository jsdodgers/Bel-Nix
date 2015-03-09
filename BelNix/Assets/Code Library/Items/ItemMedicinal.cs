using UnityEngine;
using System.Collections;

public class ItemMedicinal : ItemWeapon  {

	public override Item getItem()  {
		return getMedKit();
	}
	
	public virtual Medicinal getMedKit()  {
		//		if (weapon == null)
		/*	string s = AssetDatabase.GetAssetPath(spritePrefab);
		if (s != null && s.Length >= 17)  {
			s = s.Substring(17, s.Length - 17 - 7);
		}
		Debug.Log("Weapon Asset Path: " + s);*/
		//	if (s != null && s != "")
		//		GameObject.Instantiate(Resources.Load<GameObject>(s));
		return new Medicinal(itemName, itemType, canPlaceInShoulder, gold, silver, copper, isKeyItem, inventoryTextureSpritePrefabName, layerAdd, hit, range, numberOfDamageDice, diceType, damageBonus, damageType, criticalChance, durabilityChance, isRanged, shape);
		//		return weapon;
	}
}
