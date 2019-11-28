using UnityEngine;
using System.Collections;

public interface ISnowTornado
{
    IEnumerator HitBySnowTornado(Transform tornadoTrans, Vector3 playerOffsetFromCenter, float spinSpeed, float playerLerpFactor, Vector3 releaseVelocity);
}