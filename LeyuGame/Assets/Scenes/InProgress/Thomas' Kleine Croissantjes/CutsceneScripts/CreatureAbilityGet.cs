using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAbilityGet : MonoBehaviour {

    GameObject player;
    PlayerController controllerSwitch;
    Rigidbody playerBody;
    GameObject playerModel;
    Animator playerAnim;
    public GameObject cutsceneCamera;
    GameObject wayPointDraak;
    GameObject creature;
    Vector3 distanceToWaypointDraak;

    Animator creatureAnim;

    GameObject abilityPickUp;
    Animator abilityAnim;
    bool movingToCreature = false;

    bool camOnCreature = false;

    void Start()
    {
        player = GameObject.Find("Character");
        creature = GameObject.Find("MOD_Moustacheboi_ANIM_IDLE");
        abilityPickUp = GameObject.Find("VFX_Ability_Pickup");

        controllerSwitch = player.GetComponent<PlayerController>();
        playerBody = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();
        wayPointDraak = GameObject.Find("WaypointDraak2");
        creatureAnim = creature.GetComponent<Animator>();
        creatureAnim.SetBool("IsPlaying", false);

        abilityAnim = abilityPickUp.GetComponent<Animator>();
        abilityAnim.SetBool("IsPlaying", false);
    }

    void OnTriggerEnter()
    {
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
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPointDraak.transform.position, 10 * Time.deltaTime);
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

        if (movingToCreature == true)
        {
        abilityPickUp.transform.position = Vector3.MoveTowards(abilityPickUp.transform.position, creature.transform.position, 5 * Time.deltaTime);
        abilityPickUp.transform.localScale -= new Vector3(0.008f, 0.008f, 0.008f);
        abilityPickUp.transform.localScale = new Vector3(Mathf.Clamp(abilityPickUp.transform.localScale.x,0,5), Mathf.Clamp(abilityPickUp.transform.localScale.y, 0, 5), Mathf.Clamp(abilityPickUp.transform.localScale.z, 0, 5));
        }
    }

    IEnumerator CutsceneTime()
    {
        //creatureAnim.SetBool("IsPlaying", true);
        //camOnCreature = true;
        yield return new WaitForSeconds(1f);
        abilityAnim.SetBool("IsPlaying", true);

        yield return new WaitForSeconds(1.5f);
        movingToCreature = true;
        abilityAnim.enabled = false;

        yield return new WaitForSeconds(3f);

        cutsceneCamera.SetActive(false);
        controllerSwitch.enabled = true;

        //Poging om beweging, waarmee de draak de cutscene in komt, te stoppen wanneer de cutscene afgelopen is.
        playerBody.velocity = new Vector3(0, 0, 0);
        //
        Destroy(abilityPickUp);
        Destroy(gameObject);
    }

    void Update()
    {
        //if (movingToCreature == true)
        //{
        //    abilityPickUp.transform.position = Vector3.MoveTowards(abilityPickUp.transform.position, creature.transform.position, 10 * Time.deltaTime);
        //}
    }

}
