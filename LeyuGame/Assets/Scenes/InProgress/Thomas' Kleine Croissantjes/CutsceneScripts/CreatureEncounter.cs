using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEncounter : MonoBehaviour {

    GameObject player;
    PlayerController controllerSwitch;
    Rigidbody playerBody;
    GameObject playerModel;
    Animator playerAnim;
    public GameObject destructibleBoi;
    public GameObject cutsceneCamera;
    GameObject wayPointDraak;
    GameObject creature;
    GameObject creatureBeweging;
    Vector3 distanceToWaypointDraak;

    public GameObject snowExplosionPrefab;

    Animator creatureAnim;
    Animator creatureBewegingAnim;

    bool camOnCreature = false;
    bool dragonMoveToWaypoing = false;

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
    }
	
    void OnTriggerEnter()
    {
        cutsceneCamera.SetActive(true);
        StartCoroutine(CutsceneTime());
        controllerSwitch.enabled = false;
        playerBody.velocity = new Vector3(0, playerBody.velocity.y, 0);
        if (playerBody.velocity.y > 0)
        {
            playerBody.velocity = new Vector3(0, playerBody.velocity.y *-3, 0);
        }
        playerAnim.SetBool("IsLaunching",false);
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
            player.transform.position = Vector3.MoveTowards(player.transform.position, wayPointDraak.transform.position, 4f*Time.deltaTime);
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
        yield return new WaitForSeconds(1f);
        dragonMoveToWaypoing = true;

        yield return new WaitForSeconds(6f);

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

        yield return new WaitForSeconds(3f); // 11 sec into cutscene
        creatureAnim.SetBool("isFlop", false);

        //11.45 seconden (5.45 seconden na creatureBewegingAnim) na aanvang van de cutscene begint het creature te bewegen.
        yield return new WaitForSeconds(0.5f); // 13 sec into cutscene
        creatureAnim.SetBool("isFlying", true);
        //camOnCreature = true;

        yield return new WaitForSeconds(4.5f);
        creatureAnim.SetBool("isFlying", false);
        cutsceneCamera.SetActive(false);
        controllerSwitch.enabled = true;

        //Poging om beweging, waarmee de draak de cutscene in komt, te stoppen wanneer de cutscene afgelopen is.
        playerBody.velocity = new Vector3(0, 0, 0);
        //

        Destroy(gameObject);
    }

    /*
	void Update ()
    {
		if (camOnCreature == true)
        {
            cutsceneCamera.transform.LookAt(creature.transform.position);
        }
	}
    */
}


// op 13 seconden moet camera op de creature die om de draak beweegt staan
// 13 en een kwart/derde/halve seconden draait de camera verder de cave in
//15 seconden einde cutscene