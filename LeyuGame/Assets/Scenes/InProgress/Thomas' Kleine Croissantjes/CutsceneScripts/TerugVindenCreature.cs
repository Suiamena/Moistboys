using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerugVindenCreature : MonoBehaviour
{

    public GameObject player;
    PlayerController controllerSwitch;
    Rigidbody playerBody;
    GameObject playerModel;
    Animator playerAnim;
    public GameObject destructibleBoi;
    public GameObject destructibleCreature;
    public GameObject cutsceneCamera;
    public GameObject wayPointDraak;
    public GameObject creature;
    GameObject creatureBeweging;
    Vector3 distanceToWaypointDraak;

    public GameObject snowExplosionPrefab;

    Animator creatureAnim;
    Animator creatureBewegingAnim;

    GameObject playerCamera;
    Animator cameraAnim;
    Vector3 distanceToPlayerCam;

    bool dragonMoveToWaypoing = false;
    bool cameraMoving = false;

    float cameraDistance;
    float cameraSpeed = 0;

    BoxCollider triggerCollider;

    void Start()
    {
        creatureBeweging = GameObject.Find("BewegingCreature");
        controllerSwitch = player.GetComponent<PlayerController>();
        playerBody = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();
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
        Level4Music.musicStage = 8.8f;
        cutsceneCamera.SetActive(true);
        StartCoroutine(CutsceneTime());
        controllerSwitch.enabled = false;
        playerBody.velocity = new Vector3(0, playerBody.velocity.y, 0);
        if (playerBody.velocity.y > 0)
        {
            playerBody.velocity = new Vector3(0, playerBody.velocity.y * -3, 0);
        }
        playerAnim.SetBool("IsLaunching", false);
        //playerAnim.SetBool("IsBouncing", false);
        playerAnim.SetBool("IsAirborne", false);

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
            player.transform.position = Vector3.MoveTowards(player.transform.position, wayPointDraak.transform.position, 4f * Time.deltaTime);
            distanceToWaypointDraak = player.transform.position - wayPointDraak.transform.position;
            distanceToWaypointDraak = new Vector3(Mathf.Abs(distanceToWaypointDraak.x), distanceToWaypointDraak.y, distanceToWaypointDraak.z);
            if (distanceToWaypointDraak.x < 0.5f)
            {
                playerAnim.SetBool("IsBouncing", false);
            }
            else
            {
                player.transform.LookAt(creature.transform.position);
            }
        }

    }

    IEnumerator CutsceneTime()
    {
        //yield return new WaitForSeconds(1f);
        dragonMoveToWaypoing = true;

        yield return new WaitForSeconds(1.5f);
        //2.04 seconden voor dat de keyframes het creature omhoog uit de sneeuw verplaatsen
        creatureAnim.SetBool("isFlying", true); // op seconde 2.04 moet het creature uit e sneeuw bewegen.
        creatureBewegingAnim.SetBool("isPlaying", true);

        // pas na 2.04 seconde moet dus alles gebeuren.
        yield return new WaitForSeconds(0.5f); // 8sec into cutscene

        //GameObject snowExplosion = Instantiate(snowExplosionPrefab, destructibleBoi.transform.position, Quaternion.identity);
        //snowExplosion.transform.position = destructibleBoi.transform.position;
        //Destroy(snowExplosion.gameObject, 2);
        snowExplosionPrefab.SetActive(true);
        Destroy(destructibleBoi);
        creatureAnim.SetBool("isFlying", false);
        creatureAnim.SetBool("isFlop", true);

        yield return new WaitForSeconds(3f); // 11 sec into cutscene
        creatureAnim.SetBool("isFlop", false);

        //11.45 seconden (5.45 seconden na creatureBewegingAnim) na aanvang van de cutscene begint het creature te bewegen.
        yield return new WaitForSeconds(0.5f); // 13 sec into cutscene
        creatureAnim.SetBool("isFlying", true);
        //camOnCreature = true;

        yield return new WaitForSeconds(6f);
        cameraMoving = true;
        //cutsceneCamera.SetActive(false);
        triggerCollider.enabled = false;
        cameraAnim.enabled = false;
        controllerSwitch.enabled = true;

        //Poging om beweging, waarmee de draak de cutscene in komt, te stoppen wanneer de cutscene afgelopen is.
        playerBody.velocity = new Vector3(0, 0, 0);
        //
        //Destroy(gameObject);
    }

    void Update()
    {
        if (cameraMoving == true)
        {
            cutsceneCamera.transform.position = Vector3.MoveTowards(cutsceneCamera.transform.position, playerCamera.transform.position, cameraSpeed * Time.deltaTime);
            cutsceneCamera.transform.rotation = Quaternion.RotateTowards(cutsceneCamera.transform.rotation, playerCamera.transform.rotation, 4*cameraSpeed * Time.deltaTime);
            cameraSpeed += 1f;
        }

        cameraDistance = Vector3.Distance(cutsceneCamera.transform.position, playerCamera.transform.position);
        print(cameraDistance);

        if (cameraDistance < 0.1f)
        {
            Destroy(destructibleCreature);
            cutsceneCamera.SetActive(false);
            cameraMoving = false;
            StartCoroutine(DelayForMusic());
            //Destroy(gameObject);
            //creatureAnim.SetBool("isFlying", false);
        }
    }

    IEnumerator DelayForMusic()
    {
        yield return new WaitForSeconds(1f);
        Level4Music.musicStage = 9.5f;
        Destroy(gameObject);
    }

}
