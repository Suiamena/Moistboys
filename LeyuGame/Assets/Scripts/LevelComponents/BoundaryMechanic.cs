using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryMechanic : MonoBehaviour {

    //BOUNDARY
    Vector3 boundaryStartingPosition;
    float windStrength;
    float currentX, startingX, Xdifference;

    //acceleration higher than 0.5 is very strong!
    public float windStrengthAcceleration;

    private void Awake()
    {
        boundaryStartingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        startingX = boundaryStartingPosition.x;
        currentX = transform.position.x;
        Xdifference = startingX - currentX;
        Xdifference = Mathf.Abs(Xdifference);
        if (Xdifference <= 1)
        {
            windStrength = 10;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, boundaryStartingPosition, windStrength * Time.deltaTime);
            windStrength += windStrengthAcceleration;
            windStrength = Mathf.Clamp(windStrength, 10, 80);
        }

    }

}
