using System;
namespace CharacterInfo
{
	public enum RaceName {Berrind, Ashpian, Rorrul}
	public enum PrimalState {Reckless, Passive, Threatened}
	public enum CharacterBackground {FallenNoble, WhiteGem, Immigrant, Commoner, Servant, Unknown}
	public abstract class CharacterRace
	{

		public RaceName raceName;
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
		public static CharacterRace getRace(RaceName rName) {
			switch(rName)
			{
			case RaceName.Berrind:
				return new Race_Berrind();
			case RaceName.Ashpian:
				return new Race_Ashpian();
			case RaceName.Rorrul:
				return new Race_Rorrul();
			default:
				return new Race_Ashpian();
			}
		}
	}

	public class Race_Berrind : CharacterRace
	{
		public Race_Berrind() {raceName = RaceName.Berrind;}
		public override int 		getHealthModifier() 		{return -2;}
		public override int 		getComposureModifier()  	{return  2;}
		public override PrimalState getPrimalState()			{return PrimalState.Reckless;}
		public override string		getRaceString()				{return "Berrind";}
	}

	public class Race_Ashpian : CharacterRace
	{
		public Race_Ashpian() {raceName = RaceName.Ashpian;}
		public override int 		getHealthModifier() 		{return  0;}
		public override int 		getComposureModifier()  	{return  0;}
		public override PrimalState getPrimalState()			{return PrimalState.Passive;}
		public override string		getRaceString()				{return "Ashpian";}	
	}

	public class Race_Rorrul : CharacterRace
	{
		public Race_Rorrul() {raceName = RaceName.Rorrul;}
		public override int 		getHealthModifier() 		{return  2;}
		public override int 		getComposureModifier()  	{return -2;}
		public override PrimalState getPrimalState()			{return PrimalState.Threatened;}
		public override string		getRaceString()				{return "Rorrul";}
	}
}