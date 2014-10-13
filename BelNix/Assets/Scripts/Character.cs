using UnityEngine;
using System.Collections;
using CharacterInfo;

public class Character : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		// Personal Info first
		// Then Character Progress (class, talent)
		// Then Stats
		// then combat scores (this sets itself up using personal info, character class, and ability scores)
		// then skill scores

		// Let's experiement by recreating the enigmatic Dr. Alfred Clearwater
		PersonalInformation personalInfo = new PersonalInformation(new CharacterName("Alfred", "Clearwater"), 
		                                                           new Race_Berrind(), CharacterSex.MALE, 
		                                                           CharacterBackground.WHITE_GEM, 
		                                                           new CharacterHeight(5, 9), 
		                                                           new CharacterWeight(155));
		CharacterProgress characterProgress = new CharacterProgress(new Class_Researcher());
		AbilityScores abilityScores = new AbilityScores(2, 0, 2, 1);
		CombatScores combatScores = new CombatScores(ref abilityScores, ref personalInfo, ref characterProgress);
		SkillScores skillScores = new SkillScores(ref combatScores, ref characterProgress);

		CharacterSheet DR_CLEARWATER = new CharacterSheet(abilityScores, personalInfo, 
		                                                  characterProgress, combatScores, skillScores);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


	PersonalInformation PERSONAL_INFORMATION;

	CharacterProgress CHARACTER_PROGRESS;

		// Talents


	AbilityScores ABILITY_SCORES;


		// CP - Max
		// CP - Current

	CombatScores COMBAT_SCORES;


	// Class Features (Skills)


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
