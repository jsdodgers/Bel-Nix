using System;
namespace CharacterInfo
{
	public enum Skill {Athletics = 0, Melee, Ranged, Stealth, Mechanical, Medicinal, Historical, Political}
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
			physiqueMod 	= cScores.getInitiative();
			prowessMod		= cScores.getCritical();
			masteryMod		= cScores.getHandling();
			knowledgeMod	= cScores.getDominion();
			scores = cProgress.getCharacterClass().getClassModifiers().getSkillModifiers();
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

			if(skill.Equals(Skill.Athletics) || skill.Equals(Skill.Melee))
				modifier = physiqueMod;
			else if(skill.Equals(Skill.Ranged) || skill.Equals(Skill.Stealth))
				modifier = prowessMod;
			else if(skill.Equals(Skill.Mechanical) || skill.Equals(Skill.Medicinal))
				modifier = masteryMod;
			else
				modifier = knowledgeMod;

			return scores[(int)skill] + modifier;
		}
	}
}

