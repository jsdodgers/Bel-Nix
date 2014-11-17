using System;
using UnityEngine;
namespace CharacterInfo
{
	public enum LifeStatus {Alive, Unconscious, Dead}
	public class CombatScores
	{
		private AbilityScores abilityScores;
		private PersonalInformation personalInformation;
		private CharacterProgress characterProgress;
		
		private int currentHealth;
		private int currentComposure;

		private LifeStatus lifeStatus = LifeStatus.Alive;

		public CombatScores (ref AbilityScores abilityScore, ref PersonalInformation personalInfo, 
		                     ref CharacterProgress characterProg)
		{
			abilityScores 			= abilityScore;
			personalInformation 	= personalInfo;
			characterProgress 		= characterProg;

			currentHealth 		= getMaxHealth();
			currentComposure 	= getMaxComposure();
		}


		public int getMaxHealth() {
			return 	abilityScores.getSturdy() + abilityScores.getPerception() + 
					personalInformation.getCharacterRace().getHealthModifier() +
					characterProgress.getCharacterClass().getClassModifiers().getHealthModifier();
		}

		public int getMaxComposure() {
			return 	abilityScores.getTechnique() + abilityScores.getWellVersed() +
					personalInformation.getCharacterRace().getComposureModifier() +
					characterProgress.getCharacterClass().getClassModifiers().getComposureModifier();
		}

		public int getCurrentHealth() 	{ return currentHealth; }
		public int getCurrentComposure()  { return currentComposure; }

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
			lifeStatus = LifeStatus.Unconscious;
		}

		public void die()
		{
			lifeStatus = LifeStatus.Dead;
		}

		public void recover()
		{
			lifeStatus = LifeStatus.Alive;
		}

		public LifeStatus checkLifeStatus()
		{
			return lifeStatus;
		}

		public int getInitiative() 	{return calculateMod(abilityScores.getSturdy());}		// Initiative is based on Sturdy
		public int getCritical()	{return calculateMod(abilityScores.getPerception());} // Critical is based on Perception
		public int getHandling()	{return calculateMod(abilityScores.getTechnique());}	// Handling is based on Technique
		public int getDominion()	{return calculateMod(abilityScores.getWellVersed());}// Dominion is based on Well-Versed

		// Helper for calculating modifiers from base stats
		private int calculateMod(int baseStat)
		{
			return (int) Mathf.Floor(baseStat / 2);
		}
	}
}