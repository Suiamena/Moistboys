using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearRadius : MonoBehaviour {

    public GameObject playerPrefab;
    float playerSpeedZ;
    float playerSpeedX;
    float currentMaxPlayerSpeed;

    float radiusScale;

    Vector3 _radiusPosition;
    Rigidbody playerRig;

	void Awake ()
    {
        playerRig = playerPrefab.GetComponent<Rigidbody>();
    }

    //FIXED UPDATE CAUSES THE TRANSFORM TO LAG	
    void Update ()
    {
        //RADIUS FOLLOWS PLAYER
        _radiusPosition = new Vector3(playerPrefab.transform.position.x, playerPrefab.transform.position.y, playerPrefab.transform.position.z);
        transform.position = _radiusPosition;
    }

    private void FixedUpdate()
    {
        IncreaseRadius();
    }

    void IncreaseRadius()
    {
        //calculate player speed and clear radius
        playerSpeedZ = Mathf.Abs(playerRig.velocity.z);
        playerSpeedX = Mathf.Abs(playerRig.velocity.x);
        currentMaxPlayerSpeed = Mathf.Max(playerSpeedZ, playerSpeedX);
        //currentMaxPlayerSpeed will be different for each player!
        radiusScale = currentMaxPlayerSpeed / 10;

        transform.localScale = new Vector3(radiusScale, 1, radiusScale);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "SnowA1") {
            col.gameObject.SetActive(false);
            GlobalVariables.areaOneSnowLeft -= 1;
        }
        if (col.tag == "SnowA2")
        {
            col.gameObject.SetActive(false);
            GlobalVariables.areaTwoSnowLeft -= 1;
        }
        if (col.tag == "SnowA3")
        {
            col.gameObject.SetActive(false);
            GlobalVariables.areaThreeSnowLeft -= 1;
        }
        if (col.tag == "Snow") {
            col.gameObject.SetActive(false);
        }
    }

}
