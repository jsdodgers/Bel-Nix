using System;
namespace CharacterInfo
{
	public class CharacterProgress
	{
		private CharacterClass cClass;
		private int cLevel;
		private int cExperience;
		public CharacterProgress (CharacterClass characterClass)
		{
			cClass		= characterClass;
			cLevel 		= 1;
			cExperience = 0;
		}
		public CharacterClass CHARACTER_CLASS() { return cClass; }
		public int CHARACTER_LEVEL() 			{ return cLevel; }
		public int CHARACTER_EXPERIENCE() 		{ return cExperience; }
	}
}

