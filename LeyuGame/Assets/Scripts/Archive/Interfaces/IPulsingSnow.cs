using UnityEngine;

public interface IPulsingSnow
{
	/// <summary>
	/// Called by the OnTriggerStay of the PulsingSnow's Pulse (every frame!)
	/// </summary>
	/// <param name="origin">The origin of the pulse.</param>
	/// <param name="pushVelocity">The velocity used to push the player away from the wave.</param>
	void HitByPulsingSnow (Vector3 origin, float pushVelocity);
}