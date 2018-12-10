using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceMechanic : MonoBehaviour {

    //Player Settings
    GameObject player;
    GameObject playerModel;
    GameObject playerCamera;

    Animator playerAnim;
    PlayerController playerScript;
    Rigidbody playerRig;

    //MoustacheBoi Settings
    GameObject moustacheBoiCutscene;
    GameObject moustacheBoiTarget;

    int moustacheBoiSpeed = 10, abilitySpeed = 3, playerAbilitySpeed = 6;

    //WarmthSource Settings
    GameObject warmthSource;
    public GameObject warmthSourceOpen;
    GameObject playerAbilityTarget;
    GameObject warmthSourceTarget;
    public GameObject playerAbility, moustacheBoiAbility;
    public int takeAbilityRange = 30;

    //Cutscene Settings
    public GameObject cutsceneCamera, secondCutsceneCamera, thirdCutsceneCamera;

    //cutscene 1
    bool creatureMoves, abilityMoves, cutsceneFinished;

    //cutscene 2
    bool abilityFoundPlayer, playerAbilityMoves, secondCutsceneFinished;
    public GameObject socialChoiceTrigger, competenceChoiceTrigger;

    //Decision resolution
    bool decisionIsResolving, runFinalCoroutineOnce;
    CompetenceChoice competentScript;
    SocialChoice socialScript;

    public GameObject moustacheBoiEnding;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();
        playerCamera = GameObject.Find("Main Camera");

        moustacheBoiCutscene = GameObject.Find("MoustacheBoiCutscene");
        moustacheBoiTarget = GameObject.Find("MoustacheBoiTarget");

        warmthSource = GameObject.Find("WarmthSourceCutscene");
        playerAbilityTarget = GameObject.Find("PlayerAbilityTarget");
        warmthSourceTarget = GameObject.Find("WarmthSourceTarget");

        competentScript = competenceChoiceTrigger.GetComponent<CompetenceChoice>();
        socialScript = socialChoiceTrigger.GetComponent<SocialChoice>();
    }

    private void FixedUpdate()
    {
        if (creatureMoves)
        {
            CreatureMoves();
        }
        if (abilityMoves)
        {
            CreatureAbilityMoves();
        }
        if (playerAbilityMoves)
        {
            PlayerAbilityMovesToSource();
        }

        //After cutscene
        TakePlayerAbility();

        //Choice made
        if (socialScript.playerChooseSocial || competentScript.playerChooseCompetence)
        {
            if (!decisionIsResolving)
            {
                ResolveDecision();
            }
        }
    }

    //SETUP CUTSCENE
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            if (!cutsceneFinished)
            {
                DisablePlayer();
                cutsceneCamera.SetActive(true);
                StartCoroutine(CreatureApproachesSource());
            }
        }
    }

    IEnumerator CreatureApproachesSource()
    {
        //Creature Moves
        creatureMoves = true;
        yield return new WaitForSeconds(3F);
        creatureMoves = false;

        //Ability is Lost
        moustacheBoiAbility.SetActive(true);
        abilityMoves = true;
        yield return new WaitForSeconds(2F);
        abilityMoves = false;
        yield return new WaitForSeconds(2F);

        StopCutscene();
    }

    void CreatureMoves()
    {
        moustacheBoiCutscene.transform.position = Vector3.MoveTowards(moustacheBoiCutscene.transform.position, moustacheBoiTarget.transform.position, moustacheBoiSpeed * Time.deltaTime);
    }

    void CreatureAbilityMoves()
    {
        moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, new Vector3(moustacheBoiAbility.transform.position.x, moustacheBoiAbility.transform.position.y + 3, moustacheBoiAbility.transform.position.z), abilitySpeed * Time.deltaTime);
    }

    void StopCutscene()
    {
        EnablePlayer();
        cutsceneCamera.SetActive(false);
        cutsceneFinished = true;
    }

    void TakePlayerAbility()
    {
        if (Vector3.Distance(warmthSource.transform.position, player.transform.position) < takeAbilityRange)
        {
            if (!secondCutsceneFinished)
            {
                DisablePlayer();
                secondCutsceneCamera.SetActive(true);
                StartCoroutine(PlayerLosesAbility());
            }
        }
    }

    IEnumerator PlayerLosesAbility()
    {
        //Ability moves
        playerAbility.SetActive(true);
        playerAbilityMoves = true;
        yield return new WaitForSeconds(4F);
        playerAbilityMoves = false;
        yield return new WaitForSeconds(2F);
        //player loses ability
        playerScript.canLaunch = false;
        StopSecondCutscene();
    }

    void PlayerAbilityMovesToSource()
    {
        if (!abilityFoundPlayer)
        {
            playerAbility.transform.position = player.transform.position;
            abilityFoundPlayer = true;
        }
        playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, playerAbilityTarget.transform.position, playerAbilitySpeed * Time.deltaTime);
    }

    void StopSecondCutscene()
    {
        //set choice
        socialChoiceTrigger.SetActive(true);
        competenceChoiceTrigger.SetActive(true);

        EnablePlayer();
        secondCutsceneCamera.SetActive(false);
        secondCutsceneFinished = true;
    }

    //END OF THE MECHANIC
    void ResolveDecision()
    {
        if (socialScript.playerChooseSocial)
        {
            moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, moustacheBoiCutscene.transform.position, abilitySpeed * Time.deltaTime);
            playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, warmthSourceTarget.transform.position, 15 * Time.deltaTime);
        }
        if (competentScript.playerChooseCompetence)
        {
            playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, player.transform.position, abilitySpeed * Time.deltaTime);
            moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, warmthSourceTarget.transform.position, 15 * Time.deltaTime);
        }
        if (!runFinalCoroutineOnce)
        {
            StartCoroutine(ActivateWarmthSource());
            runFinalCoroutineOnce = true;
        }
    }

    IEnumerator ActivateWarmthSource()
    {
        DisablePlayer();
        thirdCutsceneCamera.SetActive(true);

        yield return new WaitForSeconds(5F);
        decisionIsResolving = true;
        playerAbility.SetActive(false);
        moustacheBoiAbility.SetActive(false);
        socialChoiceTrigger.SetActive(false);
        competenceChoiceTrigger.SetActive(false);
        warmthSourceOpen.SetActive(true);
        warmthSource.SetActive(false);
        yield return new WaitForSeconds(2F);
        //set player settings
        EnablePlayer();
        thirdCutsceneCamera.SetActive(false);
        //RESOLVE
        if (competentScript.playerChooseCompetence)
        {
            Level5Music.musicStage = 11.5f;
            playerScript.canLaunch = true;
        }
        else
        {
            Level5Music.musicStage = 12.5f;
            moustacheBoiEnding.SetActive(true);
        }
    }

    void DisablePlayer()
    {
        playerRig.velocity = new Vector3(0, 0, 0);
        playerScript.enabled = false;
        playerAnim.SetBool("IsLaunching", false);
        playerAnim.SetBool("IsBouncing", false);
        player.transform.position = new Vector3(player.transform.position.x, 17.03f, player.transform.position.z);
        playerCamera.SetActive(false);
    }

    void EnablePlayer()
    {
        playerScript.enabled = true;
        playerCamera.SetActive(true);
        playerRig.velocity = new Vector3(0, 0, 0);
    }

}