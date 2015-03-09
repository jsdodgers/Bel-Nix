using System;
public enum Skill  {Athletics = 0, Melee, Ranged, Stealth, Mechanical, Medicinal, Historical, Political}
public class SkillScores {
	private CombatScores cScores;
	private CharacterProgress cProgress;

	public int[] scores = new int[8];

	public SkillScores (CombatScores combatScores, CharacterProgress characterProgress)  {
		cScores = combatScores;
		cProgress = characterProgress;
		scores = cProgress.getCharacterClass().getClassModifiers().getSkillModifiers();
	}
	public void incrementScore(Skill skill)  {
		incrementScore(skill, 1);
	}

	public void incrementScore(Skill skill, int amount)  {
		scores[(int)skill]+=amount;
	}

	public void setScore(Skill skill, int amount)  {
		scores[(int)skill] = amount;
	}

	// The 8 skills are divided into four groups (first 2 in group 1, second in group 2, etc)
	// Each group has a corresponding modifier added for the final skill score.
	public int getScore(Skill skill)  {
		int modifier;


		switch (skill)  {
		case Skill.Athletics:
		case Skill.Melee:
			modifier = cScores.getInitiative();
			break;
		case Skill.Ranged:
		case Skill.Stealth:
			modifier = cScores.getCritical(false);
			break;
		case Skill.Mechanical:
		case Skill.Medicinal:
			modifier = cScores.getHandling();
			break;
		default:
			modifier = cScores.getDominion();
			break;
		}

		return scores[(int)skill] + modifier;
	}
}

