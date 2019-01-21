using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour {

    //PICCOLO
    Vector3 previousCreatureLocation;
    public bool PlayerHasTouchedElevator;
    bool goUp, goDown, elevatorIsMoving, creaturePiccoloCoroutineRunOnce;
    public int elevatorSpeed;

    public GameObject elevatorPlatform;
    public GameObject nextLocation;
    public GameObject wallObject;
    PlangeMuurInteractive wallScript;

    public GameObject player;
    public GameObject elevatorRadio;
    public GameObject elevatorBell;

    public GameObject moustacheBoi;

    Vector3 startingLocation;
    bool startingLocationSet;

    float distance;

    private void Awake()
    {
        wallScript = wallObject.GetComponent<PlangeMuurInteractive>();
        nextLocation.transform.position = new Vector3(nextLocation.transform.position.x, nextLocation.transform.position.y - 3, nextLocation.transform.position.z);
    }

    private void Update()
    {
        //reset elevator
        if (!wallScript.sequenceIsRunning) {
            PlayerHasTouchedElevator = false;
        }

        //piccolo
        if (PlayerHasTouchedElevator && !goDown) {
            if (player.transform.position.y < transform.position.y) {
                if (!creaturePiccoloCoroutineRunOnce)
                {
                    Debug.Log("keeps running");
                    StartCoroutine(CreatureBackToElevator());
                    creaturePiccoloCoroutineRunOnce = true;
                }
            }
        }
        if (wallScript.creatureBecamePiccolo) {
            Debug.Log(creaturePiccoloCoroutineRunOnce);
            if (!creaturePiccoloCoroutineRunOnce) {
                StartCoroutine(CreaturePiccolo());
                creaturePiccoloCoroutineRunOnce = true;
            }
        }
    }

    IEnumerator CreatureBackToElevator()
    {
        wallScript.creatureBecamePiccolo = true;
        while (moustacheBoi.transform.position.SquareDistance(transform.position) > .1f) {
            moustacheBoi.transform.LookAt(player.transform.position);
            moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, transform.position, (20 * 2f) * Time.deltaTime);
            yield return null;
        }
        creaturePiccoloCoroutineRunOnce = false;
    }

    IEnumerator CreaturePiccolo()
    {
        while (wallScript.creatureBecamePiccolo) {
            moustacheBoi.transform.position = new Vector3(moustacheBoi.transform.position.x, transform.position.y, moustacheBoi.transform.position.z);
            yield return null;
        }
        creaturePiccoloCoroutineRunOnce = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHasTouchedElevator = true;
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
        goDown = true;
        if (!elevatorIsMoving) {
            elevatorIsMoving = true;
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        if (goUp) {
            elevatorRadio.SetActive(true);
            distance = Mathf.Abs(elevatorPlatform.transform.position.y - nextLocation.transform.position.y);
            while (distance > .1f) {
                distance = Mathf.Abs(elevatorPlatform.transform.position.y - nextLocation.transform.position.y);
                elevatorPlatform.transform.position = Vector3.MoveTowards(
                    new Vector3(elevatorPlatform.transform.position.x, elevatorPlatform.transform.position.y, elevatorPlatform.transform.position.z),
                    new Vector3(elevatorPlatform.transform.position.x, nextLocation.transform.position.y, elevatorPlatform.transform.position.z), elevatorSpeed * Time.deltaTime);
                yield return null;
            }
            wallScript.DisablePiccolo();
        }
        if (goDown) {
            yield return new WaitForSeconds(1f);
            distance = Mathf.Abs(elevatorPlatform.transform.position.y - startingLocation.y);
            while (distance > .1f) {
                distance = Mathf.Abs(elevatorPlatform.transform.position.y - startingLocation.y);
                elevatorPlatform.transform.position = Vector3.MoveTowards(
                    new Vector3(elevatorPlatform.transform.position.x, elevatorPlatform.transform.position.y, elevatorPlatform.transform.position.z),
                    new Vector3(elevatorPlatform.transform.position.x, startingLocation.y, elevatorPlatform.transform.position.z), elevatorSpeed * Time.deltaTime);
                yield return null;
            }
        }
        elevatorBell.SetActive(true);
        elevatorRadio.SetActive(false);
        goUp = false;
        goDown = false;
        elevatorIsMoving = false;
        if (goDown)
        {
            elevatorIsMoving = true;
            StartCoroutine(Move());
        }
        yield return new WaitForSeconds(1f);
        elevatorBell.SetActive(false);
    }

}
