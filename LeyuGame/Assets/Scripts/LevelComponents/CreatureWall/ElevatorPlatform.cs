using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour {

    bool goingUp, goingDown, playerIsOnElevator;
    public int elevatorSpeed;

    public GameObject elevatorPlatform;
    public GameObject nextLocation;

    Vector3 startingLocation;

    private void Awake()
    {
        startingLocation = elevatorPlatform.transform.position;
        nextLocation.transform.position = new Vector3(nextLocation.transform.position.x, nextLocation.transform.position.y - 3, nextLocation.transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        playerIsOnElevator = true;
        if (!goingDown && playerIsOnElevator)
        {
            StartCoroutine(MoveUp());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerIsOnElevator = false;
        StartCoroutine(GoDown());
    }

    IEnumerator MoveUp()
    {
        goingUp = true;
        while ((elevatorPlatform.transform.position.SquareDistance(nextLocation.transform.position) > .1f))
        {
            elevatorPlatform.transform.position = Vector3.MoveTowards(elevatorPlatform.transform.position, nextLocation.transform.position, elevatorSpeed * Time.deltaTime);
            yield return null;
        }
        goingUp = false;
        StartCoroutine(GoDown());
    }

    IEnumerator GoDown()
    {
        if (!goingUp && !playerIsOnElevator)
        {
            yield return new WaitForSeconds(1f);
            goingDown = true;
            while ((elevatorPlatform.transform.position.SquareDistance(startingLocation) > .1f))
            {
                elevatorPlatform.transform.position = Vector3.MoveTowards(elevatorPlatform.transform.position, startingLocation, elevatorSpeed * Time.deltaTime);
                yield return null;
            }
            goingDown = false;
        }
    }

}
