using UnityEngine;
using System.Collections;

public enum DamageType  {Crushing, Piercing, Slashing, None}

public class ItemWeapon : EditorItem  {

	public int hit;
	public int range;
	public int numberOfDamageDice;
	public int diceType;
	public int damageBonus;
	public DamageType damageType;
	public int criticalChance;
	public int durabilityChance;
	public bool isRanged = false;
	public Vector2[] shape;
//	Weapon weapon;

	public override Item getItem()  {
		return getWeapon();
	}

	public virtual Weapon getWeapon()  {
//		if (weapon == null)
	/*	string s = AssetDatabase.GetAssetPath(spritePrefab);
		if (s != null && s.Length >= 17)  {
			s = s.Substring(17, s.Length - 17 - 7);
		}
		Debug.Log("Weapon Asset Path: " + s);*/
	//	if (s != null && s != "")
	//		GameObject.Instantiate(Resources.Load<GameObject>(s));
		return new Weapon(itemName, itemType, canPlaceInShoulder, gold, silver, copper, isKeyItem, inventoryTextureSpritePrefabName, layerAdd, hit, range, numberOfDamageDice, diceType, damageBonus, damageType, criticalChance, durabilityChance, isRanged, shape);
//		return weapon;
	}
/*
	public void Update()  {
		Debug.LogWarning("Weapon Update");
	}*/

}

