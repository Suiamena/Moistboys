using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour {

    bool goUp, goDown, elevatorIsMoving;
    public int elevatorSpeed;

    public GameObject elevatorPlatform;
    public GameObject nextLocation;
    public GameObject wallObject;
    PlangeMuurInteractive wallScript;

    Vector3 startingLocation;
    bool startingLocationSet;

    //REWRITE ALL!

    private void Awake()
    {
        wallScript = wallObject.GetComponent<PlangeMuurInteractive>();
        nextLocation.transform.position = new Vector3(nextLocation.transform.position.x, nextLocation.transform.position.y - 3, nextLocation.transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!startingLocationSet)
        {
            startingLocation = elevatorPlatform.transform.position;
            startingLocationSet = true;
        }

        if (!elevatorIsMoving)
        {
            goUp = true;
            elevatorIsMoving = true;
            StartCoroutine(Move());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!elevatorIsMoving)
        {
            goDown = true;
            elevatorIsMoving = true;
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        if (goUp)
        {
            while ((elevatorPlatform.transform.position.SquareDistance(nextLocation.transform.position) > .1f))
            {
                elevatorPlatform.transform.position = Vector3.MoveTowards(elevatorPlatform.transform.position, nextLocation.transform.position, elevatorSpeed * Time.deltaTime);
                yield return null;
            }
        }
        if (goDown)
        {
            yield return new WaitForSeconds(1f);
            while ((elevatorPlatform.transform.position.SquareDistance(startingLocation) > .1f))
            {

                elevatorPlatform.transform.position = Vector3.MoveTowards(elevatorPlatform.transform.position, startingLocation, elevatorSpeed * Time.deltaTime);
                yield return null;
            }
        }
        goUp = false;
        goDown = false;
        elevatorIsMoving = false;

        wallScript.DisablePiccolo();
    }

}
