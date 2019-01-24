using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceMechanic : MonoBehaviour {
    [FMODUnity.EventRef]
    public string glowStart = "event:/Objects/Glow_Start";
    public static FMOD.Studio.EventInstance GlowStart;

    //Player Settings
    GameObject player;
    GameObject playerModel;
    GameObject playerCamera;
    GameObject landingIndicatorObject;

    Animator playerAnim;
    PlayerController playerScript;
    Rigidbody playerRig;

    //MoustacheBoi Settings
    public GameObject moustacheBoiCutscene;
    GameObject moustacheBoiTarget;
    Animator moustacheBoiAnim;

    int moustacheBoiSpeed = 10, abilitySpeed = 3, playerAbilitySpeed = 3;

    //WarmthSource Settings
    public GameObject warmthSource;
    public GameObject warmthSourceAnimObject;
    Animator warmthSourceAnimator;
    GameObject playerAbilityTarget;
    GameObject warmthSourceTarget;
    public GameObject playerAbility, moustacheBoiAbility;
    public int takeAbilityRange = 30;

    //Cutscene Settings
    public GameObject firstCutsceneCamera, secondCutsceneCamera, thirdCutsceneCamera, fourthCutsceneCamera, fifthCutsceneCamera;
    GameObject cutsceneCameraOneTransformTarget;
    public GameObject invisibleWall;

    //cutscene 1 (creature gives ability)
    bool creatureToSource, abilityCreatureMoves, firstCutsceneFinished;
    GameObject playerTransformTarget;
    GameObject cutsceneCameraTwoTransformTarget;

    //cutscene 2 (player gives ability)
    bool abilityFoundPlayer, playerabilityMoves, secondCutsceneFinished, runOnce;
    public GameObject socialChoiceTrigger, competenceChoiceTrigger;
    GameObject playerTransformTargetTwo;
    Vector3 cameraTwoSecondPosition;
    Quaternion cameraTwoSecondRotation;

    //Decision resolution
    bool returnAbility, sacrificeAbility, runFinalCoroutineOnce;
    CompetenceChoice competentScript;
    SocialChoice socialScript;
    GameObject cutsceneCameraThreeTransformTarget;
    GameObject cutsceneCameraThreeTransformTargetTwo;

    public GameObject moustacheBoiEnding;

    //Thomas Shit
    public GameObject backTrackWallBlock;
    public GameObject creatureAbilityTarget;
    public GameObject playerLookAtTarget;
    //public GameObject playerRot;

    public GameObject playerAbilityLight;
    Light playerAbilityLightIntensity;
    public GameObject creatureAbilityLight;
    Light creatureAbilityLightIntensity;

    private void Awake()
    {
        warmthSourceAnimator = warmthSourceAnimObject.GetComponent<Animator>();

        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();
        playerCamera = GameObject.Find("Main Camera");
        landingIndicatorObject = GameObject.Find("Shadow");

        moustacheBoiTarget = GameObject.Find("MoustacheBoiTarget");
        moustacheBoiAnim = moustacheBoiCutscene.GetComponent<Animator>();

        playerAbilityTarget = GameObject.Find("PlayerAbilityTarget");
        warmthSourceTarget = GameObject.Find("WarmthSourceTarget");

        competentScript = competenceChoiceTrigger.GetComponent<CompetenceChoice>();
        socialScript = socialChoiceTrigger.GetComponent<SocialChoice>();

        playerTransformTarget = GameObject.Find("PlayerTarget");
        playerTransformTargetTwo = GameObject.Find("PlayerTargetTwo");

        cutsceneCameraOneTransformTarget = GameObject.Find("CameraTarget");
        cutsceneCameraTwoTransformTarget = GameObject.Find("CameraTargetTwo");
        cutsceneCameraThreeTransformTarget = GameObject.Find("CameraTargetThree");
        cutsceneCameraThreeTransformTargetTwo = GameObject.Find("CameraTargetFour");

        GlowStart = FMODUnity.RuntimeManager.CreateInstance(glowStart);

        //Thomas Shit
        moustacheBoiAnim.SetBool("isFlying", true);
        moustacheBoiAbility.transform.localScale = new Vector3(0, 0, 0);
        playerAbility.transform.localScale = new Vector3(0, 0, 0);

        playerAbilityLightIntensity = playerAbilityLight.GetComponent<Light>();
        creatureAbilityLightIntensity = creatureAbilityLight.GetComponent<Light>();
        playerAbilityLightIntensity.intensity = 0f;
        creatureAbilityLightIntensity.intensity = 0f;
    }

    private void FixedUpdate()
    {
        //FIRST CUTSCENE
        if (creatureToSource)
        {
            creatureMoves();
        }
        if (abilityCreatureMoves)
        {
            CreatureAbilityMoves();
        }
        if (playerabilityMoves)
        {
            //player.transform.LookAt(playerLookAtTarget.transform.position);
            PlayerabilityMovesToSource();
        }

        //SECOND CUTSCENE
        if (Vector3.Distance(warmthSource.transform.position, player.transform.position) < takeAbilityRange)
        {
            TakePlayerAbility();
        }

        //Choice made
        if (socialScript.playerChooseSocial || competentScript.playerChooseCompetence)
        {
            if (!runFinalCoroutineOnce)
            {
                StartCoroutine(ActivateWarmthSource());
                runFinalCoroutineOnce = true;
            }

            if (returnAbility)
            {
                RunReturnAbility();
            }

            if (sacrificeAbility)
            {
                RunSacrificeAbility();
            }
        }
    }

    //SETUP FIRST CUTSCENE - CREATURE GIVES ABILITY
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            if (!firstCutsceneFinished)
            {
                playerScript.DisablePlayer();
                StartCoroutine(CreatureApproachesSource());
            }
        }
    }

    // FIRST CUTSCENE
    IEnumerator CreatureApproachesSource()
    {
        //SUPERFLOP HIERIN!
        //cutsceneCamera.SetActive(true);
        backTrackWallBlock.SetActive(true);
        firstCutsceneCamera.SetActive(true);
        player.transform.position = playerTransformTargetTwo.transform.position;
        landingIndicatorObject.transform.position = playerTransformTargetTwo.transform.position;
        //player.transform.rotation = Quaternion.Euler(-10, 66.223f, 0);
        player.transform.LookAt(warmthSource.transform.position);
        //yield return new WaitForSeconds(1.5F);

        //Creature Moves
        creatureToSource = true;
        yield return new WaitForSeconds(1.5F);
        moustacheBoiAnim.SetBool("isFlying", false);
        yield return new WaitForSeconds(1F);
        creatureToSource = false;

        //Ability is Lost
        GlowStart.start();
        //cutsceneCamera.transform.position = cutsceneCameraOneTransformTarget.transform.position;
        //cutsceneCamera.transform.rotation = cutsceneCameraOneTransformTarget.transform.rotation;
        moustacheBoiAnim.SetBool("isUsingAbility", true);
        yield return new WaitForSeconds(0.5F);
        moustacheBoiAbility.SetActive(true);
        abilityCreatureMoves = true;
        yield return new WaitForSeconds(2F);
        moustacheBoiAnim.SetBool("isUsingAbility", false);
        abilityCreatureMoves = false;
        yield return new WaitForSeconds(2F);
        StopFirstCutscene();
    }

    void creatureMoves()
    {
        moustacheBoiCutscene.transform.position = Vector3.MoveTowards(moustacheBoiCutscene.transform.position, moustacheBoiTarget.transform.position, moustacheBoiSpeed * Time.deltaTime);
    }

    void CreatureAbilityMoves()
    {
        moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, creatureAbilityTarget.transform.position, abilitySpeed * Time.deltaTime);
        moustacheBoiAbility.transform.localScale += new Vector3(0.00045f, 0.00045f, 0.00045f);
        moustacheBoiAbility.transform.localScale = new Vector3(Mathf.Clamp(moustacheBoiAbility.transform.localScale.x, 0, 0.05f), Mathf.Clamp(moustacheBoiAbility.transform.localScale.y, 0, 0.05f), Mathf.Clamp(moustacheBoiAbility.transform.localScale.z, 0, 0.05f));
        creatureAbilityLightIntensity.intensity += 0.1f;
        creatureAbilityLightIntensity.intensity = Mathf.Clamp(creatureAbilityLightIntensity.intensity, 0, 5f);
    }

    void StopFirstCutscene()
    {
        playerScript.EnablePlayer();
        //cutsceneCamera.SetActive(false);
        firstCutsceneCamera.SetActive(false);
        //cutsceneCamera.SetActive(false);
        firstCutsceneFinished = true;
    }

    //SETUP CUTSCENE TWO
    void TakePlayerAbility()
    {
        if (!secondCutsceneFinished) // wrm niet firstCutsceneFinished = true???
        {
            //cameraTwoSecondPosition = firstCutsceneCamera.transform.position;
            //cameraTwoSecondRotation = firstCutsceneCamera.transform.rotation;
            //firstCutsceneCamera.transform.position = cutsceneCameraTwoTransformTarget.transform.position;
            //firstCutsceneCamera.transform.rotation = cutsceneCameraTwoTransformTarget.transform.rotation;
            if (!runOnce)
            {
                StartCoroutine(PlayerLosesAbility());
                runOnce = true;
            }
        }
    }

    IEnumerator PlayerLosesAbility()
    {
        playerScript.DisablePlayer();
        secondCutsceneCamera.SetActive(true);
        player.transform.position = playerTransformTarget.transform.position;
        player.transform.LookAt(playerLookAtTarget.transform.position);
        //player.transform.localRotation = Quaternion.Euler(-120, 90, 0);
        //landingIndicatorObject.transform.position = playerTransformTargetTwo.transform.position;
        yield return new WaitForSeconds(1F);
        //Ability moves
        playerabilityMoves = true;
        //secondCutsceneCamera.transform.position = cameraTwoSecondPosition;
        //secondCutsceneCamera.transform.rotation = cameraTwoSecondRotation;
        yield return new WaitForSeconds(2F);
        playerabilityMoves = false;
        //yield return new WaitForSeconds(2F);
        //secondCutsceneCamera.SetActive(false);
        //player.transform.position = playerTransformTargetTwo.transform.position;
        //thirdCutsceneCamera.SetActive(true);

        yield return new WaitForSeconds(2f);
        //player loses ability
        playerScript.launchEnabled = false;

        //CREAUTRE NADENKEN ANIMAITE

        StopSecondCutscene();
    }

    void PlayerabilityMovesToSource()
    {
        if (!abilityFoundPlayer)
        {
            playerAbility.transform.position = player.transform.position;
            playerAbility.SetActive(true);
            abilityFoundPlayer = true;
        }
        playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, playerAbilityTarget.transform.position, playerAbilitySpeed * Time.deltaTime);
        playerAbility.transform.localScale += new Vector3(0.00045f, 0.00045f, 0.00045f);
        playerAbility.transform.localScale = new Vector3(Mathf.Clamp(playerAbility.transform.localScale.x, 0, 0.05f), Mathf.Clamp(playerAbility.transform.localScale.y, 0, 0.05f), Mathf.Clamp(playerAbility.transform.localScale.z, 0, 0.05f));
        playerAbilityLightIntensity.intensity += 0.1f;
        playerAbilityLightIntensity.intensity = Mathf.Clamp(playerAbilityLightIntensity.intensity, 0, 5f);
    }

    void StopSecondCutscene()
    {
        //set choice
        player.transform.position = playerTransformTargetTwo.transform.position;
        player.transform.LookAt(warmthSource.transform.position);
        socialChoiceTrigger.SetActive(true);
        competenceChoiceTrigger.SetActive(true);

        playerScript.EnablePlayer();
        secondCutsceneCamera.SetActive(false);
        secondCutsceneFinished = true;

        moustacheBoiAnim.SetBool("isWaitingForChoice", true);
    }

    //END OF THE MECHANIC
    void RunReturnAbility()
    {
        if (socialScript.playerChooseSocial)
        {
            moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, moustacheBoiCutscene.transform.position, abilitySpeed * Time.deltaTime);
            moustacheBoiAbility.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            moustacheBoiAbility.transform.localScale = new Vector3(Mathf.Clamp(moustacheBoiAbility.transform.localScale.x, 0.05f, 0), Mathf.Clamp(moustacheBoiAbility.transform.localScale.y, 0.05f, 0), Mathf.Clamp(moustacheBoiAbility.transform.localScale.z, 0.05f, 0));
            creatureAbilityLightIntensity.intensity -= 0.05f;
            creatureAbilityLightIntensity.intensity = Mathf.Clamp(creatureAbilityLightIntensity.intensity, 0, 5f);
        }
        if (competentScript.playerChooseCompetence)
        {
            playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, player.transform.position, abilitySpeed * Time.deltaTime);
            playerAbility.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            playerAbility.transform.localScale = new Vector3(Mathf.Clamp(playerAbility.transform.localScale.x, 0.05f, 0), Mathf.Clamp(playerAbility.transform.localScale.y, 0.05f, 0), Mathf.Clamp(playerAbility.transform.localScale.z, 0.05f, 0));
            playerAbilityLightIntensity.intensity -= 0.05f;
            playerAbilityLightIntensity.intensity = Mathf.Clamp(playerAbilityLightIntensity.intensity, 0, 5f);
        }
    }

    void RunSacrificeAbility()
    {
        if (socialScript.playerChooseSocial)
        {
            fifthCutsceneCamera.transform.LookAt(playerAbility.transform);
            playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, warmthSourceTarget.transform.position, 15 * Time.deltaTime);
        }
        if (competentScript.playerChooseCompetence)
        {
            //fourthCutsceneCamera.transform.LookAt(moustacheBoiAbility.transform);
            moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, warmthSourceTarget.transform.position, 15 * Time.deltaTime);
        }
    }

    IEnumerator ActivateWarmthSource()
    {
        moustacheBoiAnim.SetBool("isWaitingForChoice", false);
        playerScript.DisablePlayer();

        //EERST ANIMATIE CREATURE, DAN PAS ABILITIES BEWEGEN
        
        if (socialScript.playerChooseSocial)
        {
            fourthCutsceneCamera.SetActive(true);
            moustacheBoiAnim.SetBool("isSuperFlop", true);
            yield return new WaitForSeconds(3F);
        }
        else
        {
            thirdCutsceneCamera.SetActive(true);
            //moustacheBoiAnim.SetBool("isRejected", true);
            yield return new WaitForSeconds(3F);
        }

        moustacheBoiAnim.SetBool("isRejected", false);

        //fourthCutsceneCamera.transform.position = cutsceneCameraThreeTransformTarget.transform.position;
        //fourthCutsceneCamera.transform.rotation = cutsceneCameraThreeTransformTarget.transform.rotation;
        returnAbility = true;

        yield return new WaitForSeconds(2.5F);

        moustacheBoiAnim.SetBool("isSuperFlop", false);

        if (socialScript.playerChooseSocial)
        {
            moustacheBoiAbility.SetActive(false);
        }
        else
        {
            playerAbility.SetActive(false);
        }

        yield return new WaitForSeconds(0.5F);
        returnAbility = false;

        //fourthCutsceneCamera.transform.position = cutsceneCameraThreeTransformTargetTwo.transform.position;
        //fourthCutsceneCamera.transform.rotation = cutsceneCameraThreeTransformTargetTwo.transform.rotation;
        sacrificeAbility = true;

        yield return new WaitForSeconds(3F);
        sacrificeAbility = false;

        playerAbility.SetActive(false);
        moustacheBoiAbility.SetActive(false);
        socialChoiceTrigger.SetActive(false);
        competenceChoiceTrigger.SetActive(false);
        yield return new WaitForSeconds(1F);

        //VERWARM WERELD (NIEUWE CAMERA EN MODEL?)
        warmthSourceAnimator.SetBool("isOpening", true);
        warmthSource.SetActive(false);
        yield return new WaitForSeconds(2F);

        thirdCutsceneCamera.SetActive(false);
        fourthCutsceneCamera.SetActive(false);

        //set player settings
        playerScript.EnablePlayer();
        fifthCutsceneCamera.SetActive(false);
        invisibleWall.SetActive(false);
        //RESOLVE
        if (competentScript.playerChooseCompetence)
        {
            VariablesGlobal.chosenForCompetence = true;
            moustacheBoiAnim.SetBool("goodBye", true);
            playerScript.launchEnabled = true;
        }
        else
        {
            VariablesGlobal.chosenForSocial = true;
            moustacheBoiEnding.SetActive(true);
            moustacheBoiCutscene.SetActive(false);
        }
    }

}