using System;
namespace CharacterInfo
{
	public enum PrimalState {RECKLESS, PASSIVE, THREATENED}
	public abstract class CharacterRace
	{
		//racial modifiers
		public abstract int 		HEALTH_MODIFIER();
		public abstract int 		COMPOSURE_MODIFIER();
		public abstract PrimalState PRIMAL_STATE();
	}

	public class Race_Berrind : CharacterRace
	{
		public override int 		HEALTH_MODIFIER() 		{return -2;}
		public override int 		COMPOSURE_MODIFIER()  	{return  2;}
		public override PrimalState PRIMAL_STATE()			{return PrimalState.RECKLESS;}
	}

	public class Race_Ashbian : CharacterRace
	{
		public override int 		HEALTH_MODIFIER() 		{return  0;}
		public override int 		COMPOSURE_MODIFIER()  	{return  0;}
		public override PrimalState PRIMAL_STATE()			{return PrimalState.PASSIVE;}		
	}

	public class Race_Rorrul : CharacterRace
	{
		public override int 		HEALTH_MODIFIER() 		{return  2;}
		public override int 		COMPOSURE_MODIFIER()  	{return -2;}
		public override PrimalState PRIMAL_STATE()			{return PrimalState.THREATENED;}
	}
}