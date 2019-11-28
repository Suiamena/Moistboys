using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour {

    //Adaptable variables
    public GameObject player;
    public int rotationSensitivity;

    TestBouncePlayer playerScript;
    float currentCamPosition, nextCamPosition, transitionSpeed, heightDifference, maxHeightDifference = 1.5f;
    bool followPlayer;

    private void Awake()
    {
        playerScript = player.GetComponent<TestBouncePlayer>();
    }

    private void Update()
    {
        FollowPlayer();
        CalculateHeightDifference();
        ExecuteCameraMovement();
    }

    //The camera must follow the player when he is launching, is falling of is bouncing higher than the camera
    void FollowPlayer()
    {
        if (playerScript.isLaunching || transform.position.y > player.transform.position.y || heightDifference > 2) {
            followPlayer = true;
        }
        else {
            followPlayer = false;
        }
    }

    //Calculate the difference between the camera anchor's y position and the player's y position
    //This difference is used to determine whether the player is bouncing above the camera or not
    void CalculateHeightDifference()
    {
        heightDifference = Mathf.Abs(transform.position.y - player.transform.position.y);
    }

    void ExecuteCameraMovement()
    {
        if (followPlayer) {
            //set smoothtransition
            currentCamPosition = transform.position.y;
            nextCamPosition = player.transform.position.y;
            if (playerScript.isLaunching) {
                transitionSpeed = 15f;
            }
            else {
                transitionSpeed = 3f;
            }
            //follow player
            transform.position = new Vector3(player.transform.position.x, Mathf.Lerp(currentCamPosition, nextCamPosition, transitionSpeed * Time.deltaTime), player.transform.position.z);
        }
        else {
            //stay put
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        }
        transform.rotation = player.transform.rotation;
    }

}
