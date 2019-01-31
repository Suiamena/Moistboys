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
    public GameObject wayPointDraak;
    GameObject creature;
    GameObject creatureBeweging;
    float distanceToWaypointDraak;

    Animator creatureAnim;
    Animator creatureBewegingAnim;

    GameObject abilityPickUp;
    Animator abilityAnim;
    bool abilityMovingToCreature = false;

    bool camOnCreature = false;

    GameObject abilityLight;
    Light abilityLightIntensity;

    public GameObject destructibleCreature;
    public GameObject creatureCenter;
    bool dragonLookAtCreature = false;

    float cameraDistance;
    bool cameraMoving = false;
    float cameraSpeed = 0;
    Animator cameraAnim;
    public GameObject playerCamera;

    Quaternion oldDraakRot;
    Quaternion newDraakRot;

    //GLOWY BOI
    Renderer rend;
    public GameObject creatureModel;
    float glow;
    int glowBoiNow = 0;
    bool fuckingStopGlowing = false;

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
        creatureAnim = creature.GetComponent<Animator>();
        creatureBewegingAnim = creatureBeweging.GetComponent<Animator>();
        creatureBewegingAnim.SetBool("isPlaying", false);
        //creatureAnim.SetBool("IsPlaying", false);

        abilityAnim = abilityPickUp.GetComponent<Animator>();
        abilityAnim.SetBool("IsPlaying", false);

        abilityLight = GameObject.Find("BlueLight");
        abilityLightIntensity = abilityLight.GetComponent<Light>();

        cameraAnim = cutsceneCamera.GetComponent<Animator>();

        //GLOWY BOI
        rend = creatureModel.GetComponent<SkinnedMeshRenderer>();
        rend.material.shader = Shader.Find("SHAD_Creature_Glow");
    }

    void OnTriggerEnter()
    {
        Level2Music.musicStage = 3.8f;
        cutsceneCamera.SetActive(true);
        StartCoroutine(CutsceneTime());
        controllerSwitch.DisablePlayer(true);
        playerBody.velocity = new Vector3(0, playerBody.velocity.y, 0);
        if (playerBody.velocity.y > 0)
        {
            playerBody.velocity = new Vector3(0, playerBody.velocity.y * -3, 0);
        }
        //playerAnim.SetBool("IsLaunching", false);
        //playerAnim.SetBool("IsBouncing", false);
        //playerAnim.SetBool("IsAirborne", false);
        dragonLookAtCreature = true;
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
        distanceToWaypointDraak = Vector3.Distance(player.transform.position, wayPointDraak.transform.position);
        //print("distance Dragon to Waypoint:"+ distanceToWaypointDraak);

        if (dragonLookAtCreature == true)
        {
            oldDraakRot = player.transform.rotation;
            player.transform.LookAt(creatureCenter.transform.position);
            newDraakRot = player.transform.rotation;
            player.transform.rotation = Quaternion.Lerp(oldDraakRot, newDraakRot, 0.1f);
        }

        if (distanceToWaypointDraak < 0.5f)
        {
            //playerAnim.SetBool("reset", true);
        }

        if (abilityMovingToCreature == true)
        {
        abilityPickUp.transform.position = Vector3.MoveTowards(abilityPickUp.transform.position, creatureCenter.transform.position, 3 * Time.deltaTime);
        abilityPickUp.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
        abilityPickUp.transform.localScale = new Vector3(Mathf.Clamp(abilityPickUp.transform.localScale.x,0,5), Mathf.Clamp(abilityPickUp.transform.localScale.y, 0, 5), Mathf.Clamp(abilityPickUp.transform.localScale.z, 0, 5));
        abilityLightIntensity.intensity -= 0.02f;
        }

        rend.material.SetFloat("_GlowStrength", glow);

        if (glowBoiNow == 1)
        {
            glow += 0.04f;
        }

        if (glow >= 1)
        {
            glowBoiNow = 0;
            fuckingStopGlowing = true;
        }

        if (fuckingStopGlowing == true)
        {
            glow -= 0.015f;

            if (glow <= 0)
            {
                fuckingStopGlowing = false;
            }
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
        abilityMovingToCreature = true;
        playerAnim.SetBool("curiousLook", true);

        yield return new WaitForSeconds(1.5f);
        glowBoiNow = 1;

        yield return new WaitForSeconds(0.5f);
        creatureBewegingAnim.SetBool("isPlaying", true);
        creatureAnim.SetBool("isUsingAbility", false);

        yield return new WaitForSeconds(1.5f);
        creatureAnim.SetBool("isFlying", true);

        yield return new WaitForSeconds(3.5f);
        dragonLookAtCreature = false;

        yield return new WaitForSeconds(1.5f);
        cameraMoving = true;
        controllerSwitch.EnablePlayer();
        cameraAnim.enabled = false;
        //Poging om beweging, waarmee de draak de cutscene in komt, te stoppen wanneer de cutscene afgelopen is.
        //playerBody.velocity = new Vector3(0, 0, 0);
        //
        abilityMovingToCreature = false;
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
            cutsceneCamera.transform.rotation = Quaternion.RotateTowards(cutsceneCamera.transform.rotation, playerCamera.transform.rotation, 1f * cameraSpeed * Time.deltaTime);
            cameraSpeed += 2f;
        }

        cameraDistance = Vector3.Distance(cutsceneCamera.transform.position, playerCamera.transform.position);
        //print(cameraDistance);

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
