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
            goingUp = true;
            StartCoroutine(MoveUp());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerIsOnElevator = false;
        if (!goingUp && !playerIsOnElevator)
        {
            goingDown = true;
            StartCoroutine(GoDown());
        }
    }

    IEnumerator MoveUp()
    {
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
        yield return new WaitForSeconds(1f);
        while ((elevatorPlatform.transform.position.SquareDistance(startingLocation) > .1f))
        {
            elevatorPlatform.transform.position = Vector3.MoveTowards(elevatorPlatform.transform.position, startingLocation, elevatorSpeed * Time.deltaTime);
            yield return null;
        }
        goingDown = false;
    }

}
