using System;
public class CharacterSheet
{
	public AbilityScores abilityScores;
	public PersonalInformation personalInformation;
	public CharacterProgress characterProgress;
	public CombatScores combatScores;
	public SkillScores skillScores;
public CharacterLoadoutActual characterLoadout;
public Inventory inventory;
	public CharacterColors characterColors;

	public CharacterSheet (AbilityScores abilityScore, PersonalInformation personalInfo, 
	                       CharacterProgress characterProg, CombatScores cScores, SkillScores sScores, CharacterColors characterColors, Character character, CharacterLoadout loadout)
	{
		abilityScores			= abilityScore;
		personalInformation		= personalInfo;
		characterProgress		= characterProg;
		combatScores			= cScores;
		skillScores				= sScores;
		inventory = new Inventory();
		inventory.character = character;
		this.characterColors = characterColors;
		characterLoadout = new CharacterLoadoutActual(loadout, character, characterColors);
    // Character Armor
    // Character Inventory
	}
}


public class CharacterConstants
{
	public const int BASE_AC = 5;
}

