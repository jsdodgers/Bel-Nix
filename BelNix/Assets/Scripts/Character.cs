using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public struct Hit {
	public int hit;
	public bool crit;
	public Hit(int h, bool c) {hit = h; crit = c;}
}

public class Character
{
	private PersonalInformation personalInfo;
	private CharacterProgress characterProgress;
	private AbilityScores abilityScores;
	private CombatScores combatScores;
	private CharacterLoadout characterLoadout;
	private SkillScores skillScores;
	public CharacterSheet characterSheet;
	public Unit unit;
	public string characterId;
//	public ItemWeapon mainHand;


	//bool flanking() {
    //    return Combat.flanking(this.unit);
		//Vector3 pos = unit.position;
		//Vector3 enemyPos = unit.attackEnemy.position;
		//int eX = (int)enemyPos.x, eY = (int)-enemyPos.y;
		//int pX = (int)pos.x, pY = (int)-pos.y;
		//int flankX = (pX == eX ? pX : eX - (pX - eX));
		//int flankY = (pY == eY ? pY : eY - (pY - eY));
		//return unit.mapGenerator.tiles[flankX, flankY].hasAlly(unit);
	//}

	public void setCharacterLoadout(CharacterLoadout cl) {
		characterLoadout = cl;
	}

	public List<SpriteOrder> getSprites() {
		return characterSheet.characterLoadout.sprites;
	}

	public int rollForSkill(Skill skill, bool favoredRace = false, int dieType = 10, int dieRoll = -1) {
		if (dieRoll == -1) dieRoll = Random.Range(1, dieType + 1);
		return characterSheet.skillScores.getScore(skill) + (favoredRace?1:0) + dieRoll;
	}

	public int rollDamage(Unit enemy) {
		return rollDamage(enemy, false);
	}

	public int rollDamage(Unit enemy, bool critical) {
		return unit.getWeapon().rollDamage(critical) + (critical ? combatScores.getCritical(unit.hasMarkOn(enemy)) : unit.sneakAttackBonus(enemy));
	}

	public int overloadDamage() {
		return unit.getWeapon().numberOfDamageDice * unit.getWeapon().diceType + characterSheet.combatScores.getHandling();
	}

	//public Hit rollHit() {
    //    return Combat.rollHit(this.unit);
		//int rand = Random.Range(1,21);
		//Debug.Log(skillScores);
		//int critChance = characterSheet.characterLoadout.rightHand.criticalChance;
		//return new Hit(skillScores.getScore(Skill.Melee) + rand + (flanking() ? 2 : 0), rand * 5 > 100 - critChance);
	//}

	public int stackabilityOfItem(Item i) {
		if (i is ItemMechanical) {
			if (characterSheet.characterProgress.getClassFeatures().Contains(ClassFeature.Efficient_Storage))
				return 3;
		}
		return 1;
	}


	void Start () 
	{
		// Personal Info first
		// Then Character Progress (class, talent)
		// Then Stats
		// then combat scores (this sets itself up using personal info, character class, and ability scores)
		// then skill scores
		//characterLoadout = gameObject.GetComponent<CharacterLoadout>();
		// Let's experiement by recreating the enigmatic Dr. Alfred Clearwater
		/*PersonalInformation personalInfo = new PersonalInformation(new CharacterName("Alfred", "Clearwater"), 
		                                                           new Race_Berrind(), CharacterSex.MALE, 
		                                                           CharacterBackground.WHITE_GEM, 
		                                                           new CharacterHeight(5, 9), 
		                                                           new CharacterWeight(155));
		CharacterProgress characterProgress = new CharacterProgress(new Class_Researcher());
		AbilityScores abilityScores = new AbilityScores(5, 1, 4, 2);
		CombatScores combatScores = new CombatScores(ref abilityScores, ref personalInfo, ref characterProgress);
		SkillScores skillScores = new SkillScores(ref combatScores, ref characterProgress);

		CharacterSheet DR_CLEARWATER = new CharacterSheet(abilityScores, personalInfo, 
		                                                  characterProgress, combatScores, skillScores);*/
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public virtual void loadData() {
		Debug.Log("????");
	}

	public virtual void loadData(string textFile2) {
		loadCharacterFromTextFile(textFile2);
	}

	public void loadCharacter(string firstName, string lastName, CharacterSex mCSex, CharacterRace mCRace, int age,
	                   CharacterBackground mCBackground, int height, int weight, CharacterClass mCClass,
	                   int mCSturdy, int mCPerception, int mCTechnique, int mCWellVersed,
	                          Color characterColor, Color headColor, Color primaryColor, Color secondaryColor, CharacterHairStyle hairStyle)
	{
		int heightRemainder = height % 12;
		height -= heightRemainder;

		personalInfo = new PersonalInformation(new CharacterName(firstName, lastName), 
		                                       mCSex, mCRace, mCBackground, new CharacterAge(age),
		                                       new CharacterHeight(height, heightRemainder), 
		                                       new CharacterWeight(weight), hairStyle);
		characterProgress = new CharacterProgress(mCClass);
		abilityScores = new AbilityScores(mCSturdy, mCPerception, mCTechnique, mCWellVersed);
		combatScores = new CombatScores(abilityScores, personalInfo, characterProgress);
		skillScores = new SkillScores(combatScores, characterProgress);
		CharacterColors characterColors = new CharacterColors(characterColor, headColor, primaryColor, secondaryColor);
		characterSheet = new CharacterSheet(abilityScores, personalInfo, 
		                                     characterProgress, combatScores, skillScores, characterColors, this, characterLoadout);

	}

	public void loadCharacterFromTextFile(string fileName) {
	//	TextAsset text = Resources.Load<TextAsset>("Saves/" + fileName);
	//	string data = text.text;
		string data = Saves.getCharactersString(fileName);
		string[] components = data.Split(new char[]{';'});
		int curr = 0;
		string firstName = components[curr++];
		string lastName = components[curr++];
		int sex = int.Parse(components[curr++]);
		CharacterSex sexC = (sex==0 ? CharacterSex.Male : (sex==1 ? CharacterSex.Female : CharacterSex.Other));
		int race = int.Parse(components[curr++]);
		CharacterRace raceC = CharacterRace.getRace(race == 0 ? RaceName.Berrind : (race == 1 ? RaceName.Ashpian : RaceName.Rorrul));
		int background = int.Parse (components[curr++]);
		CharacterBackground backgroundC = (background == 0 ? (race==0 ? CharacterBackground.FallenNoble : (race == 1 ? CharacterBackground.Commoner : CharacterBackground.Servant)) : (race==0 ? CharacterBackground.WhiteGem : (race == 1 ? CharacterBackground.Immigrant : CharacterBackground.Unknown)));
		int age = int.Parse(components[curr++]);
		int height = int.Parse(components[curr++]);
		int weight = int.Parse(components[curr++]);
		int class1 = int.Parse(components[curr++]);
		ClassName className = (class1==0 ? ClassName.ExSoldier : (class1==1 ? ClassName.Engineer : (class1==2 ? ClassName.Investigator : (class1==3 ? ClassName.Researcher : ClassName.Orator))));
		int sturdy = int.Parse(components[curr++]);
		int perception = int.Parse(components[curr++]);
		int technique = int.Parse(components[curr++]);
		int wellVersed = int.Parse(components[curr++]);
		int athletics = int.Parse(components[curr++]);
		int melee = int.Parse(components[curr++]);
		int ranged = int.Parse(components[curr++]);
		int stealth = int.Parse(components[curr++]);
		int mechanical = int.Parse(components[curr++]);
		int medicinal = int.Parse(components[curr++]);
		int historical = int.Parse(components[curr++]);
		int political = int.Parse(components[curr++]);
		Color characterColor = new Color(int.Parse(components[curr++])/255.0f,int.Parse(components[curr++])/255.0f,int.Parse(components[curr++])/255.0f);
		Debug.Log(characterColor.r + " " + characterColor.g + " " + characterColor.b);
		Color headColor = new Color(int.Parse(components[curr++])/255.0f,int.Parse(components[curr++])/255.0f,int.Parse(components[curr++])/255.0f);
		Color primaryColor = new Color(int.Parse(components[curr++])/255.0f,int.Parse(components[curr++])/255.0f,int.Parse(components[curr++])/255.0f);
		Color secondaryColor = new Color(int.Parse(components[curr++])/255.0f,int.Parse(components[curr++])/255.0f,int.Parse(components[curr++])/255.0f);
		int hairStyle = 0;
		int level = 1;
		int experience = 0;
		if (curr < components.Length-1)
			hairStyle = int.Parse(components[curr++]);
		if (curr < components.Length-1)
			level = int.Parse(components[curr++]);
		if (curr < components.Length-1)
			experience = int.Parse(components[curr++]);
		int money = 0;
		int health = 100000;
		int composure = 100000;
		int numFeatures = 0;
		int numItems = 0;
		int focus = 0;
		if (curr < components.Length-1)
			money = int.Parse(components[curr++]);
		if (curr < components.Length-1)
			health = int.Parse(components[curr++]);
		if (curr < components.Length-1)
			composure = int.Parse(components[curr++]);
		if (curr < components.Length-1)
			numFeatures = int.Parse(components[curr++]);
		int[] features = new int[numFeatures];
		for (int n=0;n<numFeatures;n++) {
			if (curr<components.Length-1)
				features[n] = int.Parse(components[curr++]);
		}
		if (curr < components.Length-1)
			focus = int.Parse(components[curr++]);
		if (curr < components.Length-1)
			numItems = int.Parse(components[curr++]);
		personalInfo = new PersonalInformation(new CharacterName(firstName,lastName), sexC,
		                                       raceC, backgroundC, new CharacterAge(age), new CharacterHeight(height),
		                                       new CharacterWeight(weight), new CharacterHairStyle(hairStyle));
		characterProgress = new CharacterProgress(CharacterClass.getClass(className));
		abilityScores = new AbilityScores(sturdy, perception, technique, wellVersed);
		combatScores = new CombatScores(abilityScores, personalInfo, characterProgress);
		skillScores = new SkillScores(combatScores, characterProgress);
		CharacterColors characterColors = new CharacterColors(characterColor, headColor, primaryColor, secondaryColor);
		characterSheet = new CharacterSheet(abilityScores, personalInfo, characterProgress, combatScores, skillScores, characterColors, this, characterLoadout);
		skillScores.incrementScore(Skill.Athletics,athletics);
		skillScores.incrementScore(Skill.Melee,melee);
		skillScores.incrementScore(Skill.Ranged,ranged);
		skillScores.incrementScore(Skill.Stealth,stealth);
		skillScores.incrementScore(Skill.Mechanical,mechanical);
		skillScores.incrementScore(Skill.Medicinal,medicinal);
		skillScores.incrementScore(Skill.Historical,historical);
		skillScores.incrementScore(Skill.Political,political);
		characterProgress.setLevel(level);
		characterProgress.setExperience(experience);
		characterSheet.inventory.purse.receiveMoney(money);
		combatScores.setHealth(health);
		combatScores.setComposure(composure);
		characterProgress.getCharacterClass().chosenFeatures = features;
		characterProgress.setWeaponFocus(focus);
		Inventory inv = characterSheet.inventory;
		for (int n=0;n<numItems;n++) {
			int slot = int.Parse(components[curr++]);
			ItemCode code = (ItemCode)int.Parse(components[curr++]);
			string itemData = components[curr++];
			Debug.Log(slot + ": " + code + "\n" + itemData);
			Item i = Item.deserializeItem(code, itemData);
			if (slot < 100) {
				if (inv.inventory[slot].item!=null) {
					if(inv.itemCanStackWith(inv.inventory[slot].item, i)) {
						inv.inventory[slot].item.addToStack(i);
					}
				}
				else if (inv.canInsertItemInSlot(i, Inventory.getSlotForIndex(slot))) {
					inv.insertItemInSlot(i, Inventory.getSlotForIndex(slot));
				}
			}
			else {
				characterSheet.characterLoadout.setItemInSlot(getArmorSlot(slot), i);
			}
				//Inventory stuff
		}
		//Right Weapon = 100;
		//Left Weapon = 110;
		//Head = 120;
		//Shoulder = 130;
		//Chest = 140;
		//Legs = 150;
		//Boots = 160;
		//Gloves = 170;
		if (curr < components.Length-1)
			characterProgress.setFavoredRace(int.Parse(components[curr++]));

	}

	public static InventorySlot getArmorSlot(int i) {
		switch (i) {
		case 100:
			return InventorySlot.RightHand;
		case 110:
			return InventorySlot.LeftHand;
		case 120:
			return InventorySlot.Head;
		case 130:
			return InventorySlot.Shoulder;
		case 140:
			return InventorySlot.Chest;
		case 150:
			return InventorySlot.Pants;
		case 160:
			return InventorySlot.Boots;
		case 170:
			return InventorySlot.Glove;
		case 180:
			return InventorySlot.Back;
		default:
			return InventorySlot.None;
		}
	}

	public static int getArmorSlotIndex (InventorySlot i) {
		switch (i) {
		case InventorySlot.RightHand:
			return 100;
		case InventorySlot.LeftHand:
			return 110;
		case InventorySlot.Head:
			return 120;
		case InventorySlot.Shoulder:
			return 130;
		case InventorySlot.Chest:
			return 140;
		case InventorySlot.Pants:
			return 150;
		case InventorySlot.Boots:
			return 160;
		case InventorySlot.Glove:
			return 170;
		case InventorySlot.Back:
			return 180;
		default:
			return 200;
		}
	}

	
	public void deleteCharacter() {
		if (characterId == null) return;
		Saves.deleteCharacter(characterId);
	}
	
	public void saveCharacter() {
		if (characterId == null) return;
		Saves.saveCharacter(characterId, getCharacterString());
	}
	
	const string delimiter = ";";
	public string getCharacterString() {
		string characterStr = "";
		//********PERSONAL INFORMATION********\\
		//Adding player first name.
		characterStr += personalInfo.getCharacterName().firstName + delimiter;
		characterStr += personalInfo.getCharacterName().lastName + delimiter;
		CharacterSex sex = personalInfo.getCharacterSex();
		characterStr += (sex==CharacterSex.Male ? 0 : (sex==CharacterSex.Female ? 1 : 2)) + delimiter;
		RaceName race = personalInfo.getCharacterRace().raceName;
		characterStr += (race == RaceName.Berrind ? 0 : (race == RaceName.Ashpian ? 1 : 2)) + delimiter;
		CharacterBackground background = personalInfo.getCharacterBackground();
		characterStr += (background == CharacterBackground.FallenNoble || background == CharacterBackground.Commoner || background == CharacterBackground.Servant ? 0 : 1) + delimiter;
		characterStr += personalInfo.getCharacterAge().age + delimiter;
		characterStr += (personalInfo.getCharacterHeight().feet * 12 + personalInfo.getCharacterHeight().inches) + delimiter;
		characterStr += personalInfo.getCharacterWeight().weight + delimiter;
		ClassName clas = characterProgress.getCharacterClass().getClassName();
		characterStr += (clas == ClassName.ExSoldier ? 0 : (clas == ClassName.Engineer ? 1 : (clas == ClassName.Investigator ? 2 : (clas == ClassName.Researcher ? 3 : 4)))) + delimiter;
		characterStr += abilityScores.getSturdy() + delimiter;
		characterStr += abilityScores.getPerception(0) + delimiter;
		characterStr += abilityScores.getTechnique() + delimiter;
		characterStr += abilityScores.getWellVersed() + delimiter;
		foreach (int score in skillScores.scores) {
			characterStr += score + delimiter;
		}
		CharacterColors colors = characterSheet.characterColors;
		characterStr += colorString(colors.characterColor);
		characterStr += colorString(colors.headColor);
		characterStr += colorString(colors.primaryColor);
		characterStr += colorString(colors.secondaryColor);
		characterStr += characterSheet.personalInformation.getCharacterHairStyle().hairStyle + delimiter;
		characterStr += characterProgress.getCharacterLevel() + delimiter;
		characterStr += characterProgress.getCharacterExperience() + delimiter;
		//*********Hair*********\\
		characterStr += characterSheet.inventory.purse.money + delimiter;
		characterStr += characterSheet.combatScores.getCurrentHealth() + delimiter;
		characterStr += characterSheet.combatScores.getCurrentComposure() + delimiter;
		int[] features = characterSheet.characterProgress.getCharacterClass().chosenFeatures;
		characterStr += features.Length + delimiter;
		foreach (int feature in features) {
			characterStr += feature + delimiter;
		}
		characterStr += characterSheet.characterProgress.getWeaponFocusAsNumber() + delimiter;
		string inventoryString = "";
		int inventorySize = 0;
		foreach (InventoryItemSlot slot in characterSheet.inventory.inventory) {
			if (slot.item != null) {
				inventorySize++;
				inventoryString += slot.index + delimiter;
				inventoryString += (int)slot.item.getItemCode() + delimiter;
				inventoryString += slot.item.getItemData() + delimiter;
				if (slot.item.stackSize() > 0) {
					foreach (Item i in slot.item.stack) {
						inventorySize++;
						inventoryString += slot.index + delimiter;
						inventoryString += (int)i.getItemCode() + delimiter;
						inventoryString += i.getItemData() + delimiter;
					}
				}
			}
		}
		foreach (InventorySlot slot in UnitGUI.armorSlots) {
			Item i = characterSheet.characterLoadout.getItemInSlot(slot);
			if (i != null) {
				inventorySize++;
				inventoryString += getArmorSlotIndex(slot) + delimiter;
				inventoryString += (int)i.getItemCode() + delimiter;
				inventoryString += i.getItemData() + delimiter;
			}
		}
		characterStr += inventorySize + delimiter + inventoryString;
		characterStr += characterSheet.characterProgress.getFavoredRaceAsNumber() + delimiter;
		return characterStr;
	}
	
	
	static string colorString(Color c) {
		return ((int)(c.r*255)) + delimiter + ((int)(c.g*255)) + delimiter + ((int)(c.b*255)) + delimiter;
	}

	// Class Features (Skills)

	public CharacterLoadout getCharacterLoadout()
	{
		return characterLoadout;
	}

	// Equipment
		// Head
		// Chest
		// Gloves
		// Pants
		// Boots
		// Back
		// Shoulder (Armor)
		
		// Weapon/Item (Main Hand)
		// Weapon/Item (Off Hand)
		// Shoulder (Weapon/Item)

	// Inventory
		// 2D Cell Grid

	// Wealth
}
