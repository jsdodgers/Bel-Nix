using System;
using UnityEngine;
namespace CharacterInfo
{
	public enum LifeStatus {Alive, Unconscious, Dying, Dead}
	public class CombatScores
	{
		private AbilityScores abilityScores;
		private PersonalInformation personalInformation;
		private CharacterProgress characterProgress;
		
		private int currentHealth;
		private int currentComposure;

		private LifeStatus lifeStatus = LifeStatus.Alive;

		public CombatScores (AbilityScores abilityScore, PersonalInformation personalInfo, 
		                     CharacterProgress characterProg)
		{
			abilityScores 			= abilityScore;
			personalInformation 	= personalInfo;
			characterProgress 		= characterProg;

			currentHealth 		= getMaxHealth();
			currentComposure 	= getMaxComposure();
		}


		public int getMaxHealth() {
			return 	abilityScores.getSturdy() + abilityScores.getPerception(0) + 
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
		public int setHealth(int health) {
			if (health <= getMaxHealth()) currentHealth = health;
			return currentHealth;
		}
		public int setComposure(int composure) {
			if (composure <= getMaxComposure()) currentComposure = composure;
			return currentComposure;
		}
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
			if (currentHealth <= -getMaxHealth()) {
				die();
			}
			else if (currentHealth < 0) {
				dying();
			}
			else if (currentHealth == 0) {
				faint();
			}
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

		public void dying() {
			lifeStatus = LifeStatus.Dying;
		}

		public void die()
		{
			lifeStatus = LifeStatus.Dead;
		}

		public void recover()
		{
			currentHealth = 0;
			lifeStatus = LifeStatus.Unconscious;
		}

		public LifeStatus checkLifeStatus()
		{
			return lifeStatus;
		}

		public bool isDead() {
			return lifeStatus == LifeStatus.Dead;
		}

		public bool isUnconscious() {
			return lifeStatus == LifeStatus.Unconscious;
		}

		public bool isDying() {
			return lifeStatus == LifeStatus.Dying;
		}

		public int getInitiative() 	{return calculateMod(abilityScores.getSturdy());}		// Initiative is based on Sturdy
		public int getCritical(bool marked)	{return calculateMod(abilityScores.getPerception((marked?2:0)));} // Critical is based on Perception
		public int getHandling()	{return calculateMod(abilityScores.getTechnique());}	// Handling is based on Technique
		public int getDominion()	{return calculateMod(abilityScores.getWellVersed());}// Dominion is based on Well-Versed

		public int getSturdyMod() { return getInitiative(); }
		public int getPerceptionMod(bool marked) { return getCritical(marked); }
		public int getTechniqueMod() { return getHandling(); }
		public int getWellVersedMod() { return getDominion(); }

		// Helper for calculating modifiers from base stats
		private int calculateMod(int baseStat)
		{
			return (int) Mathf.Floor(baseStat / 2);
		}
	}
}