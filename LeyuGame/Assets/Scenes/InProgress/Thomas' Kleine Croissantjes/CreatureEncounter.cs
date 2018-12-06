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
    Vector3 distanceToWaypointDraak;

    public ParticleSystem snowExplosionPrefab;

    void Start ()
    {
        player = GameObject.Find("Character");
        creature = GameObject.Find("MOD_Moustacheboi_ANIM_IDLE");
        controllerSwitch = player.GetComponent<PlayerController>();
        playerBody = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();
        wayPointDraak = GameObject.Find("WaypointDraak");
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
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPointDraak.transform.position, 10*Time.deltaTime);
        player.transform.LookAt(creature.transform.position);
        distanceToWaypointDraak = player.transform.position - wayPointDraak.transform.position;
        distanceToWaypointDraak = new Vector3(Mathf.Abs(distanceToWaypointDraak.x), distanceToWaypointDraak.y, distanceToWaypointDraak.z);
        if (distanceToWaypointDraak.x < 0.5f)
        {
            playerAnim.SetBool("IsBouncing", false);
        }
    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(1.8f);
        ParticleSystem snowExplosion = Instantiate(snowExplosionPrefab) as ParticleSystem;

        yield return new WaitForSeconds(2f);
        Destroy(destructibleBoi);

        yield return new WaitForSeconds(3f);
        cutsceneCamera.SetActive(false);
        controllerSwitch.enabled = true;
        playerBody.velocity = new Vector3(0, 0, 0);
        Destroy(gameObject);
    }

	void Update ()
    {
		
	}
}


//ParticleSystem breakParticles = Instantiate(breakParticlesPrefab) as ParticleSystem;
//breakParticles.transform.position = transform.position;
       // breakParticles.Emit(100);
       // Destroy(breakParticles.gameObject, breakParticles.main.duration);