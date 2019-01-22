using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    //public GameObject sneeuwstormTrigger;
    SphereCollider cutsceneCollider;
    public GameObject sneeuwParticlesPos;
    bool particlesFollowPlayer = false;
    public float distanceInFrontOfPlayer = 50;


    //COMBINING FUCKING SCRIPTS
    public Image image;
    bool FadingToWhite = false;

    Color tempColor;

    //bool screechPlayed = false;
    //GameObject creature;
    //AudioSource creatureScreech;
    //public AudioClip screech;
    public GameObject creatureFlyAlong;

    //float windStormStrength, particlesSpeed;
    //bool accelerateSnowstorm;
    [Header("Particle Settings")]
    public GameObject snowParticles;
    ParticleSystem snowParticlesSystem;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.MainModule main;
    public float stormIntensity = 3000f;

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

        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;

        cutsceneCollider = GetComponent<SphereCollider>();

        snowParticlesSystem = snowParticles.GetComponent<ParticleSystem>();
        emissionModule = snowParticlesSystem.emission;
        main = snowParticlesSystem.main;
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

        playerAnim.SetBool("curiousLook", true);

        particlesFollowPlayer = true;
    }

    void OnTriggerStay()
    {
        //Raycast totdat ie iets raakt. dan eenmalig sneeuw poefje. dan stoppen met het hele gebeuren.

        //controllerSwitch.Gravity();
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
        if (particlesFollowPlayer == true)
        {
            sneeuwParticlesPos.transform.position = new Vector3(player.transform.position.x + distanceInFrontOfPlayer, player.transform.position.y, player.transform.position.z);
        }

        if (FadingToWhite == true)
        {
            var tempColor = image.color;
            tempColor.a += 0.0028f;
            image.color = tempColor;

            stormIntensity += 10f;
            emissionModule.rate = stormIntensity;
        }
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
        controllerSwitch.launchEnabled = true;
        Level3Music.startMusic = true;
        Destroy(abilityPickUp);
        cutsceneCollider.enabled = false;
        FadingToWhite = true;

        while (image.color.a < 1)
        {
            yield return null;
        }

        creatureFlyAlong.SetActive(true);
        print("FUCKKKKKK");
        yield return new WaitForSeconds(1f);
        print("hallo?");
        AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("Level 3");
    }
}


