using UnityEngine;
using System.Collections;
using CharacterInfo;

public class Character : MonoBehaviour
{
	PersonalInformation personalInfo;
	CharacterProgress characterProgress;
	AbilityScores abilityScores;
	CombatScores combatScores;
	CharacterLoadout characterLoadout;
	SkillScores skillScores;
	CharacterSheet CHARACTER_SHEET;
	public ItemWeapon mainHand;

	// Use this for initialization
	void Start () 
	{
		// Personal Info first
		// Then Character Progress (class, talent)
		// Then Stats
		// then combat scores (this sets itself up using personal info, character class, and ability scores)
		// then skill scores
		characterLoadout = new CharacterLoadout();
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

	public void loadCharacter(string firstName, string lastName, CharacterRace mCRace, CharacterSex mCSex,
	                   CharacterBackground mCBackground, int height, int weight, CharacterClass mCClass,
	                   int mCSturdy, int mCPerception, int mCTechnique, int mCWellVersed)
	{
		int heightRemainder = height % 12;
		height -= heightRemainder;

		personalInfo = new PersonalInformation(new CharacterName(firstName, lastName), 
		                                       mCRace, mCSex, mCBackground, 
		                                       new CharacterHeight(height, heightRemainder), 
		                                       new CharacterWeight(weight));
		characterProgress = new CharacterProgress(mCClass);
		abilityScores = new AbilityScores(mCSturdy, mCPerception, mCTechnique, mCWellVersed);
		combatScores = new CombatScores(ref abilityScores, ref personalInfo, ref characterProgress);
		skillScores = new SkillScores(ref combatScores, ref characterProgress);
		
		CHARACTER_SHEET = new CharacterSheet(abilityScores, personalInfo, 
		                                     characterProgress, combatScores, skillScores);
	}




	PersonalInformation PERSONAL_INFORMATION;

	CharacterProgress CHARACTER_PROGRESS;

		// Talents


	AbilityScores ABILITY_SCORES;


		// CP - Max
		// CP - Current

	CombatScores COMBAT_SCORES;


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
