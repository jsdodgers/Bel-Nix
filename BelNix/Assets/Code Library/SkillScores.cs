using System;
namespace CharacterInfo
{
	public enum Skill {ATHLETICS = 0, MELEE, RANGED, STEALTH, MECHANICAL, MEDICINAL, HISTORICAL, POLITICAL}
	public class SkillScores
	{
		private CombatScores cScores;
		private CharacterProgress cProgress;

		private int physiqueMod, prowessMod, masteryMod, knowledgeMod;
		private int[] scores = new int[8];

		public SkillScores (ref CombatScores combatScores, ref CharacterProgress characterProgress)
		{
			cScores = combatScores;
			cProgress = characterProgress;
			physiqueMod 	= cScores.INITIATIVE();
			prowessMod		= cScores.CRITICAL();
			masteryMod		= cScores.HANDLING();
			knowledgeMod	= cScores.DOMINION();
			scores = cProgress.CHARACTER_CLASS().CLASS_MODIFIERS().SKILL_MODIFERS();
		}
		public void incrementScore(Skill skill)
		{
			scores[(int)skill]++;
		}

		// The 8 skills are divided into four groups (first 2 in group 1, second in group 2, etc)
		// Each group has a corresponding modifier added for the final skill score.
		public int getScore(Skill skill)
		{
			int modifier;

			if(skill.Equals(Skill.ATHLETICS) || skill.Equals(Skill.MELEE))
				modifier = physiqueMod;
			else if(skill.Equals(Skill.RANGED) || skill.Equals(Skill.STEALTH))
				modifier = prowessMod;
			else if(skill.Equals(Skill.MECHANICAL) || skill.Equals(Skill.MEDICINAL))
				modifier = masteryMod;
			else
				modifier = knowledgeMod;

			return scores[(int)skill] + modifier;
		}
	}
}

