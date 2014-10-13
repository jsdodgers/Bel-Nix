using System;
namespace CharacterInfo
{
	public class CharacterSheet
	{
		public AbilityScores ABILITY_SCORES;
		public PersonalInformation PERSONAL_INFORMATION;
		public CharacterProgress CHARACTER_PROGRESS;
		public CombatScores COMBAT_SCORES;
		public SkillScores SKILL_SCORES;

		public CharacterSheet (AbilityScores abilityScores, PersonalInformation personalInformation, 
		                       CharacterProgress characterProgress, CombatScores combatScores, SkillScores skillScores)
		{
			ABILITY_SCORES			= abilityScores;
			PERSONAL_INFORMATION	= personalInformation;
			CHARACTER_PROGRESS		= characterProgress;
			COMBAT_SCORES			= combatScores;
			SKILL_SCORES			= skillScores;
		}
	}
}

