using System;
namespace CharacterInfo
{
	public class CharacterSheet
	{
		public AbilityScores abilityScores;
		public PersonalInformation personalInformation;
		public CharacterProgress characterProgress;
		public CombatScores combatScores;
		public SkillScores skillScores;
        public CharacterLoadout characterLoadout;
        public Inventory inventory;

		public CharacterSheet (AbilityScores abilityScore, PersonalInformation personalInfo, 
		                       CharacterProgress characterProg, CombatScores cScores, SkillScores sScores, Character character)
		{
			abilityScores			= abilityScore;
			personalInformation		= personalInfo;
			characterProgress		= characterProg;
			combatScores			= cScores;
			skillScores				= sScores;
			inventory = new Inventory();
			inventory.character = character;
            // Character Armor
            // Character Inventory
		}
	}


	public class CharacterConstants
	{
		public const int BASE_AC = 5;
	}
}

