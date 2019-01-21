using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAbilityGetScript : MonoBehaviour {

    GameObject player;
    GameObject playerCamera;
    PlayerController controllerSwitch;
    Rigidbody playerBody;
    GameObject playerModel;
    Animator playerAnim;

    GameObject abilityPlant;
    GameObject abilityPickUp;
    Animator abilityAnim;
    bool movingToCreature = false;

    GameObject abilityLight;
    Light abilityLightIntensity;

    public GameObject sneeuwstormTrigger;

    void Start ()
    {
        player = GameObject.Find("Character");
        playerCamera = GameObject.Find("Main Camera");
        abilityPickUp = GameObject.Find("DragonAbilityPickUp");
        abilityPlant = GameObject.Find("bezier2");

        controllerSwitch = player.GetComponent<PlayerController>();
        playerBody = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();

        abilityAnim = abilityPickUp.GetComponent<Animator>();
        abilityAnim.SetBool("IsPlaying", false);

        abilityLight = GameObject.Find("OrangeLight");
        abilityLightIntensity = abilityLight.GetComponent<Light>();
    }

    void OnTriggerEnter()
    {
        //landingIndicatorObject.SetActive(false);
        StartCoroutine(CutsceneTime());
        controllerSwitch.DisablePlayer();
        playerBody.velocity = new Vector3(0, playerBody.velocity.y, 0);
        if (playerBody.velocity.y > 0)
        {
            playerBody.velocity = new Vector3(0, playerBody.velocity.y * -3, 0);
        }

        playerAnim.SetBool("IsLaunching", false);
        playerAnim.SetBool("IsBouncing", false);
        playerAnim.SetBool("IsAirborne", false);
    }

    void OnTriggerStay()
    {
        player.transform.LookAt(gameObject.transform.position);
        playerCamera.transform.LookAt(abilityPickUp.transform.position);

        if (movingToCreature == true)
        {
            //MOGELIJK WAYPOINT VOOR ABILITY IMPLEMENTEREN IN PLAYER CHARACTER
            abilityLightIntensity.intensity -= 0.02f;
            abilityPickUp.transform.position = Vector3.MoveTowards(abilityPickUp.transform.position, new Vector3(player.transform.position.x, player.transform.position.y +1, player.transform.position.z), 5 * Time.deltaTime);
            abilityPickUp.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            abilityPickUp.transform.localScale = new Vector3(Mathf.Clamp(abilityPickUp.transform.localScale.x, 0, 5), Mathf.Clamp(abilityPickUp.transform.localScale.y, 0, 5), Mathf.Clamp(abilityPickUp.transform.localScale.z, 0, 5));
        }
    }

    void Update ()
    {

    }

    IEnumerator CutsceneTime()
    {
        Level3Music.musicStage = 6.5f;
        yield return new WaitForSeconds(0.5f);
        abilityAnim.SetBool("IsPlaying", true);

        yield return new WaitForSeconds(1.8f);
        abilityAnim.enabled = false;
        movingToCreature = true;

        yield return new WaitForSeconds(4f);
        controllerSwitch.EnablePlayer();
        Level3Music.startMusic = true;

        controllerSwitch.launchEnabled = true;

        //Poging om beweging, waarmee de draak de cutscene in komt, te stoppen wanneer de cutscene afgelopen is.
        //playerBody.velocity = new Vector3(0, 0, 0);
        //

        //ACTIVATE LAUNCH
        Destroy(abilityPickUp);
        sneeuwstormTrigger.SetActive(true);
        Destroy(gameObject);
    }
}
