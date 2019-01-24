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
    public GameObject playerAureool;
	public Creature.CreatureFlyAlong flyAlongDoos;

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

    bool screechPlayed = false;
    AudioSource creatureScreech;
    public GameObject creatureFlyAlong;
    public GameObject creature;

    public GameObject cutsceneCam;
    float cameraDistance;
    bool cameraMoving = false;
    float cameraSpeed = 0;

    //float windStormStrength, particlesSpeed;
    //bool accelerateSnowstorm;
    [Header("Particle Settings")]
    public GameObject snowParticles;
    ParticleSystem snowParticlesSystem;
    ParticleSystem.EmissionModule emissionModule;
    float baseStormIntensity = 1500f;
    float maxStormIntensity = 5000f;
    float stormIntensityTime = 3f;
    float stormIntensityTimer = 0;
    float stormIntensityPerSec;

    //SHADER STUFF
    Renderer rend;
    public GameObject aureoolModel;
    float aureoolVisibility;
    bool aureoolAppear = false;
    //float currentGlow;


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

        creatureScreech = creature.GetComponent<AudioSource>();

        stormIntensityPerSec = (maxStormIntensity - baseStormIntensity) / stormIntensityTime;

        //SHADER STUFF
        //LENNY TRYOUT
        aureoolModel.GetComponent<Renderer>().sharedMaterial.SetFloat("_Cutoff", 1);

        rend = aureoolModel.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("SHAD_Dragon_Symbol_Glow");

        Debug.Log(rend);
    }

    void OnTriggerEnter()
    {
        //landingIndicatorObject.SetActive(false);
        StartCoroutine(CutsceneTime());
        cutsceneCam.SetActive(true);
        controllerSwitch.DisablePlayer(true);
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
            abilityLightIntensity.intensity -= 0.01f;
            abilityPickUp.transform.position = Vector3.MoveTowards(abilityPickUp.transform.position, new Vector3(player.transform.position.x, player.transform.position.y +1, player.transform.position.z), 5 * Time.deltaTime);
            abilityPickUp.transform.localScale -= new Vector3(0.015f, 0.015f, 0.015f);
            abilityPickUp.transform.localScale = new Vector3(Mathf.Clamp(abilityPickUp.transform.localScale.x, 0, 5), Mathf.Clamp(abilityPickUp.transform.localScale.y, 0, 5), Mathf.Clamp(abilityPickUp.transform.localScale.z, 0, 5));
        }

        if (cameraMoving == false)
        {
            cutsceneCam.transform.LookAt(abilityPickUp.transform.position);
            cutsceneCam.transform.position = Vector3.MoveTowards(cutsceneCam.transform.position, abilityPickUp.transform.position, -0.5f * Time.deltaTime);
        }
    }

    void Update ()
    {
        aureoolModel.GetComponent<Renderer>().sharedMaterial.SetFloat("_Cutoff", 1);
        if (particlesFollowPlayer == true)
        {
            sneeuwParticlesPos.transform.position = new Vector3(player.transform.position.x + distanceInFrontOfPlayer, player.transform.position.y, player.transform.position.z);
        }

        if (FadingToWhite == true)
        {
            var tempColor = image.color;
            tempColor.a += 0.0028f;
            image.color = tempColor;

            if (stormIntensityTimer < stormIntensityTime)
            {
                stormIntensityTimer += Time.deltaTime;
                baseStormIntensity += stormIntensityPerSec * Time.deltaTime;
                emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(baseStormIntensity);

            }
        }

        if (cameraMoving == true)
        {
            cutsceneCam.transform.position = Vector3.MoveTowards(cutsceneCam.transform.position, playerCamera.transform.position, cameraSpeed * Time.deltaTime);
            cutsceneCam.transform.rotation = Quaternion.RotateTowards(cutsceneCam.transform.rotation, playerCamera.transform.rotation, 4.2f * cameraSpeed * Time.deltaTime);
            cameraSpeed += 1f;
            cameraDistance = Vector3.Distance(cutsceneCam.transform.position, playerCamera.transform.position);
            //print(cameraDistance);
            if (cameraDistance < 0.01f)
            {
                cutsceneCam.SetActive(false);
                cameraMoving = false;
            }
        }
        
        //SHADER STUFF
        if (aureoolAppear == true)
        {
            //glow = 0.05f * energyAmount;
            rend.material.SetFloat("_Cutoff", aureoolVisibility);
            //aureoolVisibility = Mathf.Lerp(aureoolVisibility, 0.5f, 0.5f);
            print(aureoolVisibility + "...");
        }

    }

    IEnumerator CutsceneTime()
    {
        Level3Music.musicStage = 6.5f;
        creatureFlyAlong.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        abilityAnim.SetBool("IsPlaying", true);

        yield return new WaitForSeconds(1.8f);
        abilityAnim.enabled = false;
        movingToCreature = true;

        yield return new WaitForSeconds(4f);
        playerAureool.SetActive(true);
        aureoolAppear = true;
        controllerSwitch.EnablePlayer();
        cameraMoving = true;
        controllerSwitch.launchEnabled = true;
        Level3Music.startMusic = true;
        Destroy(abilityPickUp);
        cutsceneCollider.enabled = false;
        FadingToWhite = true;

        while (image.color.a < 0.5)
        {
            yield return null;
        }

        creatureScreech.Play();
		flyAlongDoos.FlyAway();

        while (image.color.a < 1)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("Level 3");
    }
}