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
    GameObject creatureBeweging;
    Vector3 distanceToWaypointDraak;

    Animator creatureAnim;
    Animator creatureBewegingAnim;

    GameObject abilityPickUp;
    Animator abilityAnim;
    bool movingToCreature = false;

    bool camOnCreature = false;

    GameObject abilityLight;
    Light abilityLightIntensity;

    public GameObject destructibleCreature;

    float cameraDistance;
    bool cameraMoving = false;
    float cameraSpeed = 0;
    Animator cameraAnim;
    public GameObject playerCamera;

    void Start()
    {
        player = GameObject.Find("Character");
        creature = GameObject.Find("AbilityCreature");
        creatureBeweging = GameObject.Find("BewegingNaAbility");
        abilityPickUp = GameObject.Find("VFX_AbilityPickup_Blue");

        controllerSwitch = player.GetComponent<PlayerController>();
        playerBody = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();
        wayPointDraak = GameObject.Find("WaypointDraak2");
        creatureAnim = creature.GetComponent<Animator>();
        creatureBewegingAnim = creatureBeweging.GetComponent<Animator>();
        creatureBewegingAnim.SetBool("isPlaying", false);
        //creatureAnim.SetBool("IsPlaying", false);

        abilityAnim = abilityPickUp.GetComponent<Animator>();
        abilityAnim.SetBool("IsPlaying", false);

        abilityLight = GameObject.Find("BlueLight");
        abilityLightIntensity = abilityLight.GetComponent<Light>();

        cameraAnim = cutsceneCamera.GetComponent<Animator>();
    }

    void OnTriggerEnter()
    {
        Level2Music.musicStage = 3.8f;
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

        creatureAnim.SetBool("isFlying", false);

        //Teleport to position on cutscene start:
        //player.transform.position = wayPointDraak.transform.position;
        //player.transform.rotation = Quaternion.Euler(10, wayPointDraak.transform.rotation.y, wayPointDraak.transform.rotation.y);
        //
    }

    void OnTriggerStay()
    {
        //Move to position on cutscene start:
        player.transform.position = Vector3.MoveTowards(player.transform.position, wayPointDraak.transform.position, 20 * Time.deltaTime);
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
        abilityPickUp.transform.position = Vector3.MoveTowards(abilityPickUp.transform.position, creature.transform.position, 3 * Time.deltaTime);
        abilityPickUp.transform.localScale -= new Vector3(0.02f, 0.02f, 0.02f);
        abilityPickUp.transform.localScale = new Vector3(Mathf.Clamp(abilityPickUp.transform.localScale.x,0,5), Mathf.Clamp(abilityPickUp.transform.localScale.y, 0, 5), Mathf.Clamp(abilityPickUp.transform.localScale.z, 0, 5));
        abilityLightIntensity.intensity -= 0.02f;
        player.transform.LookAt(creature.transform.position);
        }
    }

    IEnumerator CutsceneTime()
    {
        //creatureAnim.SetBool("IsPlaying", true);
        //camOnCreature = true;
        yield return new WaitForSeconds(0.7f);
        abilityAnim.SetBool("IsPlaying", true);

        yield return new WaitForSeconds(0.3f);
        creatureAnim.SetBool("isFlying", false);
        creatureAnim.SetBool("isFlop", true);

        yield return new WaitForSeconds(1.3f);
        creatureAnim.SetBool("isFlop", false);

        yield return new WaitForSeconds(1.7f);
        abilityAnim.enabled = false;
        print("Ability Gone");
        creatureAnim.SetBool("isUsingAbility", true);
        movingToCreature = true;

        yield return new WaitForSeconds(2f);
        creatureBewegingAnim.SetBool("isPlaying", true);
        creatureAnim.SetBool("isUsingAbility", false);

        yield return new WaitForSeconds(1.5f);
        creatureAnim.SetBool("isFlying", true);

        yield return new WaitForSeconds(5f);
        cameraMoving = true;
        controllerSwitch.enabled = true;
        cameraAnim.enabled = false;
        //Poging om beweging, waarmee de draak de cutscene in komt, te stoppen wanneer de cutscene afgelopen is.
        //playerBody.velocity = new Vector3(0, 0, 0);
        //
        Level2Music.musicStage = 4.5f;
        Destroy(destructibleCreature);
        Destroy(abilityPickUp);

        //Cutscene duration = 7seconden
    }

    void Update()
    {
        if (cameraMoving == true)
        {
            cutsceneCamera.transform.position = Vector3.MoveTowards(cutsceneCamera.transform.position, playerCamera.transform.position, cameraSpeed * Time.deltaTime);
            cutsceneCamera.transform.rotation = Quaternion.RotateTowards(cutsceneCamera.transform.rotation, playerCamera.transform.rotation, 2.8f * cameraSpeed * Time.deltaTime);
            cameraSpeed += 3f;
        }

        cameraDistance = Vector3.Distance(cutsceneCamera.transform.position, playerCamera.transform.position);
        print(cameraDistance);

        if (cameraDistance < 0.01f)
        {
            cutsceneCamera.SetActive(false);
            cameraMoving = false;
            Destroy(gameObject);
        }
        //if (movingToCreature == true)
        //{
        //    abilityPickUp.transform.position = Vector3.MoveTowards(abilityPickUp.transform.position, creature.transform.position, 10 * Time.deltaTime);
        //}
    }

}
