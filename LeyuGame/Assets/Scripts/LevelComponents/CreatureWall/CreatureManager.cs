using UnityEngine;

namespace Creature
{
	public interface ICreature
	{
		ICreature RequestCreature ();
		ICreature PassOnCreature ();
	}

	public static class CreatureManager
	{
		public static GameObject activeCreature = null;
		public static ICreature activeScript = null;
	}
}