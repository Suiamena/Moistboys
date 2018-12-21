using UnityEngine;

public interface ISnowball
{
    void HitBySnowball(float pushForce, float pushTime, Vector3 snowballPosition);
}