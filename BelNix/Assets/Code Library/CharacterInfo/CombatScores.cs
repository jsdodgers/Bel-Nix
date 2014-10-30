using System;
using UnityEngine;
namespace CharacterInfo
{
	public enum LifeStatus {ALIVE, UNCONSCIOUS, DEAD}
	public class CombatScores
	{
		private AbilityScores ABILITY_SCORES;
		private PersonalInformation PERSONAL_INFORMATION;
		private CharacterProgress CHARACTER_PROGRESS;
		
		private int currentHealth;
		private int currentComposure;

		private LifeStatus lifeStatus = LifeStatus.ALIVE;

		public CombatScores (ref AbilityScores abilityScores, ref PersonalInformation personalInformation, 
		                     ref CharacterProgress characterProgress)
		{
			ABILITY_SCORES 			= abilityScores;
			PERSONAL_INFORMATION 	= personalInformation;
			CHARACTER_PROGRESS 		= characterProgress;

			currentHealth 		= MAX_HEALTH();
			currentComposure 	= MAX_COMPOSURE();
		}


		public int MAX_HEALTH()
		{
			return 	ABILITY_SCORES.STURDY() + ABILITY_SCORES.PERCEPTION() + 
					PERSONAL_INFORMATION.CHARACTER_RACE().HEALTH_MODIFIER() +
					CHARACTER_PROGRESS.CHARACTER_CLASS().CLASS_MODIFIERS().HEALTH_MODIFIER();
		}

		public int MAX_COMPOSURE()
		{
			return 	ABILITY_SCORES.TECHNIQUE() + ABILITY_SCORES.WELL_VERSED() +
					PERSONAL_INFORMATION.CHARACTER_RACE().COMPOSURE_MODIFIER() +
					CHARACTER_PROGRESS.CHARACTER_CLASS().CLASS_MODIFIERS().COMPOSURE_MODIFIER();
		}

		public int CURRENT_HEALTH() 	{ return currentHealth; }
		public int CURRENT_COMPOSURE()  { return currentComposure; }

		public int addHealth(int addedHealth)
		{
			currentHealth += addedHealth;
			return currentHealth;
		}
		public int addComposure(int addedComposure)
		{
			currentComposure += addedComposure;
			return currentComposure;
		}

		public int loseHealth(int lostHealth)
		{
			currentHealth -= lostHealth;
			return currentHealth;
		}
		public int loseComposure(int lostComposure)
		{
			currentComposure -= lostComposure;
			return currentComposure;
		}

		public void faint()
		{
			lifeStatus = LifeStatus.UNCONSCIOUS;
		}

		public void die()
		{
			lifeStatus = LifeStatus.DEAD;
		}

		public void recover()
		{
			lifeStatus = LifeStatus.ALIVE;
		}

		public LifeStatus checkLifeStatus()
		{
			return lifeStatus;
		}

		public int INITIATIVE() {return CalculateMod(ABILITY_SCORES.STURDY());}		// Initiative is based on Sturdy
		public int CRITICAL()	{return CalculateMod(ABILITY_SCORES.PERCEPTION());} // Critical is based on Perception
		public int HANDLING()	{return CalculateMod(ABILITY_SCORES.TECHNIQUE());}	// Handling is based on Technique
		public int DOMINION()	{return CalculateMod(ABILITY_SCORES.WELL_VERSED());}// Dominion is based on Well-Versed

		// Helper for calculating modifiers from base stats
		private int CalculateMod(int baseStat)
		{
			return (int) Mathf.Floor(baseStat / 2);
		}
	}
}