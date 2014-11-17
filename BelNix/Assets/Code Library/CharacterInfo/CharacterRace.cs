using System;
namespace CharacterInfo
{
	public enum PrimalState {Reckless, Passive, Threatened}
	public enum CharacterBackground {FallenNoble, WhiteGem, Immigrant, Commoner, Servant, Unknown}
	public abstract class CharacterRace
	{
		//racial modifiers
		public abstract int 		getHealthModifier();
		public abstract int 		getComposureModifier();
		public abstract PrimalState getPrimalState();
		public abstract string getRaceString();
		public string getPrimalStateString()
		{
			switch(getPrimalState())
			{
			case PrimalState.Reckless:
				return "Reckless";
			case PrimalState.Passive:
				return "Passive";
			case PrimalState.Threatened:
				return "Threatened";
			default:
				return "";
			}
		}
	}

	public class Race_Berrind : CharacterRace
	{
		public override int 		getHealthModifier() 		{return -2;}
		public override int 		getComposureModifier()  	{return  2;}
		public override PrimalState getPrimalState()			{return PrimalState.Reckless;}
		public override string		getRaceString()				{return "Berrind";}
	}

	public class Race_Ashpian : CharacterRace
	{
		public override int 		getHealthModifier() 		{return  0;}
		public override int 		getComposureModifier()  	{return  0;}
		public override PrimalState getPrimalState()			{return PrimalState.Passive;}
		public override string		getRaceString()				{return "Ashpian";}	
	}

	public class Race_Rorrul : CharacterRace
	{
		public override int 		getHealthModifier() 		{return  2;}
		public override int 		getComposureModifier()  	{return -2;}
		public override PrimalState getPrimalState()			{return PrimalState.Threatened;}
		public override string		getRaceString()				{return "Rorrul";}
	}
}