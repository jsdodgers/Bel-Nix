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
        public CharacterArmor characterArmor;
        public Inventory inventory;

		public CharacterSheet (AbilityScores abilityScore, PersonalInformation personalInfo, 
		                       CharacterProgress characterProg, CombatScores cScores, SkillScores sScores)
		{
			abilityScores			= abilityScore;
			personalInformation		= personalInfo;
			characterProgress		= characterProg;
			combatScores			= cScores;
			skillScores				= sScores;
            // Character Armor
            // Character Inventory
		}
	}
}

