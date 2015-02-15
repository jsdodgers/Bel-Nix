using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGameUnit : MonoBehaviour {

	public Image body;
	public Image boots;
	public Image pants;
	public Image shirt;
	public Image hair;
	public Image weapon;
	public Text name;
	public Text level;
	public Text experience;
	public GameObject deceased;
	public Sprite maleBaseSprite;

	public void setUnit(Unit u) {
	//	u.resetAllSprites();
		setBody(maleBaseSprite, u.characterSheet.characterSheet.characterColors.characterColor);
		Armor b = u.characterSheet.characterSheet.characterLoadout.bootsSlot;
		if (b != null) setBoots(b.spritePrefab.GetComponent<SpriteRenderer>().sprite, u.characterSheet.characterSheet.characterColors.secondaryColor);
		else boots.gameObject.SetActive(false);
		Armor p = u.characterSheet.characterSheet.characterLoadout.pantsSlot;
		if (p != null) setPants(p.spritePrefab.GetComponent<SpriteRenderer>().sprite, u.characterSheet.characterSheet.characterColors.secondaryColor);
		else pants.gameObject.SetActive(false);
		Armor s = u.characterSheet.characterSheet.characterLoadout.chestSlot;
		if (s != null) setShirt(s.spritePrefab.GetComponent<SpriteRenderer>().sprite, u.characterSheet.characterSheet.characterColors.primaryColor);
		else shirt.gameObject.SetActive(false);
		Weapon w = u.characterSheet.characterSheet.characterLoadout.rightHand;
		if (w != null) setWeapon(w.spritePrefab.GetComponent<SpriteRenderer>().sprite);
		else weapon.gameObject.SetActive(false);
		if (u.hairSprite != null) setHair(u.characterSheet.characterSheet.personalInformation.getCharacterHairStyle().getHairPrefab().GetComponent<SpriteRenderer>().sprite, u.characterSheet.characterSheet.characterColors.headColor);
		else hair.gameObject.SetActive(false);
		setName(u.getName());
		setExperienceLevel(u.characterSheet.characterSheet.characterProgress.getCharacterExperience(), u.characterSheet.characterSheet.characterProgress.getCharacterLevel());
		setDead(u.isDead());
	}

	public void setBody(Sprite spr, Color c) {
		body.sprite = spr;
		body.color = c;
		body.gameObject.SetActive(true);
	}

	public void setBoots(Sprite spr, Color c) {
		boots.sprite = spr;
		boots.color = c;
		boots.gameObject.SetActive(true);
	}

	public void setPants(Sprite spr, Color c) {
		pants.sprite = spr;
		pants.color = c;
		pants.gameObject.SetActive(true);
	}

	public void setShirt(Sprite spr, Color c) {
		shirt.sprite = spr;
		shirt.color = c;
		shirt.gameObject.SetActive(true);
	}

	public void setHair(Sprite spr, Color c) {
		hair.sprite = spr;
		hair.color = c;
		hair.gameObject.SetActive(true);
	}

	public void setWeapon(Sprite spr) {
		weapon.sprite = spr;
	//	weapon.color = c;
		weapon.gameObject.SetActive(true);
	}

	public void setExperienceLevel(int exp, int lev) {
		int totalExp = lev * 100;
		experience.text = UnitGUI.getSmallCapsString("Experience: " + exp + "/" + totalExp, 10);
		level.text = UnitGUI.getSmallCapsString("Level: " + lev,10) + (exp >= totalExp ? "<color=green>" + UnitGUI.getSmallCapsString("(Level Up!)",10) + "</color>" : "");
	}

	public void setName(string n) {
		name.text = UnitGUI.getSmallCapsString(n, 10);
	}

	public void setDead(bool dead) {
		deceased.SetActive(dead);
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
