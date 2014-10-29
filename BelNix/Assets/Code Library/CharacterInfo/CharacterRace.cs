using System;
namespace CharacterInfo
{
	public enum PrimalState {RECKLESS, PASSIVE, THREATENED}
	public enum CharacterBackground {FALLEN_NOBLE, WHITE_GEM, IMMIGRANT, COMMONER, SERVANT, UNKNOWN}
	public abstract class CharacterRace
	{
		//racial modifiers
		public abstract int 		HEALTH_MODIFIER();
		public abstract int 		COMPOSURE_MODIFIER();
		public abstract PrimalState PRIMAL_STATE();
		public abstract string getRaceString();
		public string getPrimalStateString()
		{
			switch(PRIMAL_STATE())
			{
			case PrimalState.RECKLESS:
				return "Reckless";
				break;
			case PrimalState.PASSIVE:
				return "Passive";
				break;
			case PrimalState.THREATENED:
				return "Threatened";
				break;
			default:
				return "";
				break;
			}
		}
	}

	public class Race_Berrind : CharacterRace
	{
		public override int 		HEALTH_MODIFIER() 		{return -2;}
		public override int 		COMPOSURE_MODIFIER()  	{return  2;}
		public override PrimalState PRIMAL_STATE()			{return PrimalState.RECKLESS;}
		public override string		getRaceString()			{return "Berrind";}
	}

	public class Race_Ashpian : CharacterRace
	{
		public override int 		HEALTH_MODIFIER() 		{return  0;}
		public override int 		COMPOSURE_MODIFIER()  	{return  0;}
		public override PrimalState PRIMAL_STATE()			{return PrimalState.PASSIVE;}
		public override string		getRaceString()			{return "Ashpian";}	
	}

	public class Race_Rorrul : CharacterRace
	{
		public override int 		HEALTH_MODIFIER() 		{return  2;}
		public override int 		COMPOSURE_MODIFIER()  	{return -2;}
		public override PrimalState PRIMAL_STATE()			{return PrimalState.THREATENED;}
		public override string		getRaceString()			{return "Rorrul";}
	}
}