using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour {

    bool elevatorIsRunning;
    int elevatorSpeed = 15;

    public GameObject elevatorPlatform;
    public GameObject nextLocation;

    Vector3 startingLocation;

    private void Awake()
    {
        startingLocation = elevatorPlatform.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        elevatorIsRunning = true;
        StartCoroutine(MoveUp());
    }

    private void OnTriggerExit(Collider other)
    {
        elevatorIsRunning = false;
        StartCoroutine(GoDown());
    }

    IEnumerator MoveUp()
    {
        while ((elevatorPlatform.transform.position.SquareDistance(nextLocation.transform.position) > .1f) && elevatorIsRunning)
        {
            elevatorPlatform.transform.position = Vector3.MoveTowards(elevatorPlatform.transform.position, nextLocation.transform.position, elevatorSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator GoDown()
    {
        while ((elevatorPlatform.transform.position.SquareDistance(startingLocation) > .1f) && !elevatorIsRunning)
        {
            elevatorPlatform.transform.position = Vector3.MoveTowards(elevatorPlatform.transform.position, startingLocation, elevatorSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
