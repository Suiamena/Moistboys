using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEncounter : MonoBehaviour {

    GameObject player;
    Renderer playerRenderer;
    Material playerMaterial;
    PlayerController controllerSwitch;
    Rigidbody playerBody;
    GameObject playerModel;
    Animator playerAnim;
    public GameObject destructibleBoi;
    public GameObject cutsceneCamera;
    GameObject wayPointDraak;
    GameObject creature;
    GameObject creatureBeweging;
    float distanceToWaypointDraak;
    bool playerLookAtCreature = false;
    bool playerLookAtSnowHeap = false;
    Quaternion oldDraakRot;
    Quaternion newDraakRot;
    public GameObject playerLookAtPoint;

    public GameObject snowExplosionPrefab;

    Animator creatureAnim;
    Animator creatureBewegingAnim;

    GameObject playerCamera;
    Animator cameraAnim;
    //Vector3 distanceToPlayerCam;
    float cameraDistance;

    bool dragonMoveToWaypoing = false;
    bool cameraMoving = false;

    float cameraSpeed = 0;

    BoxCollider triggerCollider;

    void Start ()
    {
        player = GameObject.Find("Character");
        creature = GameObject.Find("Mod_Creature");
        creatureBeweging = GameObject.Find("BewegingCreature");
        controllerSwitch = player.GetComponent<PlayerController>();
        playerBody = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();
        wayPointDraak = GameObject.Find("WaypointDraak");
        creatureAnim = creature.GetComponent<Animator>();
        creatureAnim.SetBool("isFlying", false);
        creatureBewegingAnim = creatureBeweging.GetComponent<Animator>();
        creatureBewegingAnim.SetBool("isPlaying", false);
        playerCamera = GameObject.Find("Main Camera");
        cameraAnim = cutsceneCamera.GetComponent<Animator>();
        triggerCollider = gameObject.GetComponent<BoxCollider>();
    }
	
    void OnTriggerEnter()
    {
        Level2Music.musicStage = 3.5f;
        cutsceneCamera.SetActive(true);
        StartCoroutine(CutsceneTime());
        controllerSwitch.DisablePlayer(true);
        playerBody.velocity = new Vector3(0, playerBody.velocity.y, 0);
        if (playerBody.velocity.y > 0)
        {
            playerBody.velocity = new Vector3(0, playerBody.velocity.y *-3, 0);
        }
        //playerAnim.SetBool("IsLaunching",false);
        //playerAnim.SetBool("IsBouncing", false);
        //playerAnim.SetBool("IsAirborne", false);

        //Teleport to position on cutscene start:
        //player.transform.position = wayPointDraak.transform.position;
        //player.transform.rotation = Quaternion.Euler(10, wayPointDraak.transform.rotation.y, wayPointDraak.transform.rotation.y);
        //
    }

    void OnTriggerStay()
    {
        //Move to position on cutscene start:
        if (dragonMoveToWaypoing == true)
        {
            if (playerLookAtSnowHeap == true)
            {
                player.transform.LookAt(playerLookAtPoint.transform.position);
            }
            player.transform.position = Vector3.MoveTowards(player.transform.position, wayPointDraak.transform.position, 4f*Time.deltaTime);
            distanceToWaypointDraak = Vector3.Distance(player.transform.position, wayPointDraak.transform.position);
            //distanceToWaypointDraak = player.transform.position - wayPointDraak.transform.position;
            //distanceToWaypointDraak = new Vector3(Mathf.Abs(distanceToWaypointDraak.x), distanceToWaypointDraak.y, distanceToWaypointDraak.z);
            if (distanceToWaypointDraak < 0.5f)
            {
                //playerAnim.SetBool("curiousLook", true);
                
                if (playerLookAtCreature == true)
                {
                    playerLookAtSnowHeap = false;
                    oldDraakRot = player.transform.rotation;
                    player.transform.LookAt(creature.transform.position);
                    newDraakRot = player.transform.rotation;
                    player.transform.rotation = Quaternion.Lerp(oldDraakRot, newDraakRot, 0.1f);
                }
            }
        }

    }

    IEnumerator CutsceneTime()
    {
        playerLookAtSnowHeap = true;

        yield return new WaitForSeconds(1f);
        dragonMoveToWaypoing = true;

        yield return new WaitForSeconds(4f);

        creatureBewegingAnim.SetBool("isPlaying", true); //2.04 seconden voor dat de keyframes het creature omhoog uit de sneeuw verplaatsen
        creatureAnim.SetBool("isFlying", true); // op seconde 2.04 moet het creature uit e sneeuw bewegen.

        // pas na 2.04 seconde moet dus alles gebeuren.
        yield return new WaitForSeconds(1f); // 8sec into cutscene

        //GameObject snowExplosion = Instantiate(snowExplosionPrefab, destructibleBoi.transform.position, Quaternion.identity);
        //snowExplosion.transform.position = destructibleBoi.transform.position;
        //Destroy(snowExplosion.gameObject, 2);
        snowExplosionPrefab.SetActive(true);
        Destroy(destructibleBoi);
        creatureAnim.SetBool("isFlying", false);
        creatureAnim.SetBool("isFlop", true);
        playerAnim.SetBool("curiousLook", true);

        //yield return new WaitForSeconds(0.2f);
        playerLookAtCreature = true;

        yield return new WaitForSeconds(3f); // 11 sec into cutscene
        creatureAnim.SetBool("isFlop", false);

        //11.45 seconden (5.45 seconden na creatureBewegingAnim) na aanvang van de cutscene begint het creature te bewegen.
        yield return new WaitForSeconds(0.5f); // 13 sec into cutscene
        creatureAnim.SetBool("isFlying", true);
        //camOnCreature = true;

        yield return new WaitForSeconds(4.5f);
        cameraMoving = true;
        //cutsceneCamera.SetActive(false);
        triggerCollider.enabled = false;
        cameraAnim.enabled = false;
        controllerSwitch.EnablePlayer();

        //Poging om beweging, waarmee de draak de cutscene in komt, te stoppen wanneer de cutscene afgelopen is.
        playerBody.velocity = new Vector3(0, 0, 0);
        //

        //Destroy(gameObject);
    }

	void Update ()
    {
        //distanceToPlayerCam = cutsceneCamera.transform.position - playerCamera.transform.position;
        //distanceToPlayerCam = new Vector3(Mathf.Abs(distanceToPlayerCam.x), distanceToPlayerCam.y, distanceToPlayerCam.z);
        //distanceToPlayerCam.x = Mathf.Abs(distanceToPlayerCam.x);
        //distanceToPlayerCam.y = Mathf.Abs(distanceToPlayerCam.y);
        //distanceToPlayerCam.z = Mathf.Abs(distanceToPlayerCam.z);
        //print(distanceToPlayerCam);

        if (cameraMoving == true)
        {
            cutsceneCamera.transform.position = Vector3.MoveTowards(cutsceneCamera.transform.position, playerCamera.transform.position, cameraSpeed* Time.deltaTime);
            cutsceneCamera.transform.rotation = Quaternion.RotateTowards(cutsceneCamera.transform.rotation, playerCamera.transform.rotation, 2.2f*cameraSpeed * Time.deltaTime);
            cameraSpeed += 1f;
        }

        cameraDistance = Vector3.Distance(cutsceneCamera.transform.position, playerCamera.transform.position);
        //print(cameraDistance);

        if (cameraDistance < 0.01f)
        {
            cutsceneCamera.SetActive(false);
            cameraMoving = false;
            Destroy(gameObject);
        }

        //if (distanceToPlayerCam.x < 0.7f && distanceToPlayerCam.y < 0.7f && distanceToPlayerCam.z < 0.7f)
        //{
        //    print("loooooooolll");
        //    //creatureAnim.SetBool("isFlying", false);
        //    cutsceneCamera.SetActive(false);
        //    cameraMoving = false;
        //    Destroy(gameObject);
        //}
    }
}


// op 13 seconden moet camera op de creature die om de draak beweegt staan
// 13 en een kwart/derde/halve seconden draait de camera verder de cave in
//15 seconden einde cutscene