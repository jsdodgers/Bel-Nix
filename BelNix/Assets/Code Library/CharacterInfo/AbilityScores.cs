using System;
namespace CharacterInfo
{
	public class AbilityScores
	{
		private int sturdy, perception, technique, well_versed;

		public AbilityScores (int stu, int per, int tec, int wVer)
		{
			// Initialize default scores
			sturdy		= (stu > 0)   ? stu   : 1;
			perception	= (per > 0)   ? per   : 1;
			technique	= (tec > 0)   ? tec   : 1;
			well_versed	= (wVer > 0) ? wVer : 1;
		}

		public int getSturdy()			{return sturdy;}
		public int getPerception()		{return perception;}
		public int getTechnique()		{return technique;}
		public int getWellVersed()		{return well_versed;}
	}
}

