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

    float distance;

    private void Awake()
    {
        wallScript = wallObject.GetComponent<PlangeMuurInteractive>();
        nextLocation.transform.position = new Vector3(nextLocation.transform.position.x, nextLocation.transform.position.y - 3, nextLocation.transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!startingLocationSet) {
            startingLocation = elevatorPlatform.transform.position;
            startingLocationSet = true;
        }

        if (!elevatorIsMoving) {
            goUp = true;
            elevatorIsMoving = true;
            StartCoroutine(Move());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!elevatorIsMoving) {
            goDown = true;
            elevatorIsMoving = true;
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        if (goUp) {
            distance = Mathf.Abs(elevatorPlatform.transform.position.y - nextLocation.transform.position.y);
            while (distance > .1f) {
                elevatorPlatform.transform.position = Vector3.MoveTowards(
                    new Vector3(elevatorPlatform.transform.position.x, elevatorPlatform.transform.position.y, elevatorPlatform.transform.position.z),
                    new Vector3(elevatorPlatform.transform.position.x, nextLocation.transform.position.y, elevatorPlatform.transform.position.z), elevatorSpeed * Time.deltaTime);
                yield return null;
            }
        }
        if (goDown) {
            yield return new WaitForSeconds(1f);
            distance = Mathf.Abs(elevatorPlatform.transform.position.y - startingLocation.y);
            while (distance > .1f) {
                elevatorPlatform.transform.position = Vector3.MoveTowards(
                    new Vector3(elevatorPlatform.transform.position.x, elevatorPlatform.transform.position.y, elevatorPlatform.transform.position.z),
                    new Vector3(elevatorPlatform.transform.position.x, startingLocation.y, elevatorPlatform.transform.position.z), elevatorSpeed * Time.deltaTime);
                yield return null;
            }
        }
        goUp = false;
        goDown = false;
        elevatorIsMoving = false;
        wallScript.DisablePiccolo();
    }

}
