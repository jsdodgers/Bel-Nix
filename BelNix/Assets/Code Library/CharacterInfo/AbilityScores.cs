using System;
namespace CharacterInfo
{
	public class AbilityScores
	{
		private int sturdy, perception, technique, well_versed;

		public AbilityScores (int STU, int PER, int TEC, int W_VER)
		{
			// Initialize default scores
			sturdy		= (STU > 0)   ? STU   : 1;
			perception	= (PER > 0)   ? PER   : 1;
			technique	= (TEC > 0)   ? TEC   : 1;
			well_versed	= (W_VER > 0) ? W_VER : 1;
		}

		public int STURDY()			{return sturdy;}
		public int PERCEPTION()		{return perception;}
		public int TECHNIQUE()		{return technique;}
		public int WELL_VERSED()	{return well_versed;}
	}
}

