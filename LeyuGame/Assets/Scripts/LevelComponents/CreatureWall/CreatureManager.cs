using UnityEngine;

namespace Creature
{
	public interface ICreature
	{
		Transform GiveAwayCreature ();
		void ReceiveCreature (Transform creatureTrans);
	}

	public static class CreatureManager
	{
		public static ICreature activeFlyAlongScript = null;
	}
}