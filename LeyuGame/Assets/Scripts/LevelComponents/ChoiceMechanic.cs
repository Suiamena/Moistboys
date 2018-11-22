using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceMechanic : MonoBehaviour {

    //Player Settings
    GameObject player;
    GameObject playerCamera;

    PlayerController playerScript;
    Rigidbody playerRig;

    //MoustacheBoi Settings
    GameObject moustacheBoiCutscene;
    GameObject moustacheBoiTarget;

    int moustacheBoiSpeed = 10, abilitySpeed = 3, playerAbilitySpeed = 6;

    //WarmthSource Settings
    GameObject warmthSource;
    GameObject warmthSourceOpen;
    GameObject playerAbilityTarget;
    GameObject warmthSourceTarget;
    public GameObject playerAbility, moustacheBoiAbility;
    public int takeAbilityRange = 30;

    //Cutscene Settings
    public GameObject cutsceneCamera, secondCutsceneCamera, thirdCutsceneCamera;

    //cutscene 1
    bool creatureMoves, abilityMoves, cutsceneFinished;

    //cutscene 2
    bool playerAbilityMoves, playerPushing, secondCutsceneFinished;
    public GameObject socialChoiceTrigger, competenceChoiceTrigger;

    //Decision resolution
    bool decisionIsResolving;
    CompetenceChoice competentScript;
    SocialChoice socialScript;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerCamera = GameObject.Find("Main Camera");

        moustacheBoiCutscene = GameObject.Find("MoustacheBoiCutscene");
        moustacheBoiTarget = GameObject.Find("MoustacheBoiTarget");

        warmthSource = GameObject.Find("WarmthSourceCutscene");
        warmthSourceOpen = GameObject.Find("WarmthSourceOpen");
        playerAbilityTarget = GameObject.Find("PlayerAbilityTarget");
        warmthSourceTarget = GameObject.Find("WarmthSourceTarget");

        competentScript = competenceChoiceTrigger.GetComponent<CompetenceChoice>();
        socialScript = socialChoiceTrigger.GetComponent<SocialChoice>();
    }

    private void FixedUpdate()
    {
        //Cutscene
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
        if (playerPushing)
        {
            PlayerPushedBack();
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
                //stop player from moving
                playerRig.velocity = new Vector3(0, 0, 0);
                playerScript.enabled = false;
                //set camera
                cutsceneCamera.SetActive(true);
                playerCamera.SetActive(false);

                StartCoroutine(CreatureApproachesSource());
            }
        }
    }

    IEnumerator CreatureApproachesSource()
    {
        //PLAY CUTSCENE

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
        playerScript.enabled = true;
        playerCamera.SetActive(true);
        cutsceneCamera.SetActive(false);
        playerRig.velocity = new Vector3(0, 0, 0);
        cutsceneFinished = true;
    }


    //AFTER CUTSCENE
    void TakePlayerAbility()
    {
        if (Vector3.Distance(warmthSource.transform.position, player.transform.position) < takeAbilityRange)
        {
            if (!secondCutsceneFinished)
            {
                //stop player from moving
                playerRig.velocity = new Vector3(0, 0, 0);
                playerScript.enabled = false;
                //set camera
                secondCutsceneCamera.SetActive(true);
                playerCamera.SetActive(false);

                StartCoroutine(PlayerLosesAbility());
            }
        }
    }

    IEnumerator PlayerLosesAbility()
    {
        //PLAY CUTSCENE

        //Ability moves
        playerAbility.transform.position = player.transform.position;
        playerAbility.SetActive(true);
        playerAbilityMoves = true;
        yield return new WaitForSeconds(3F);
        playerAbilityMoves = false;

        //Push player back
        playerPushing = true;
        yield return new WaitForSeconds(2F);
        playerPushing = false;

        //player loses ability
        playerScript.canLaunch = false;

        StopSecondCutscene();
    }

    void PlayerAbilityMovesToSource()
    {
        playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, playerAbilityTarget.transform.position, playerAbilitySpeed * Time.deltaTime);
    }

    void PlayerPushedBack()
    {
        playerRig.velocity = new Vector3(0, 0, -10);
    }

    void StopSecondCutscene()
    {
        //set choice
        socialChoiceTrigger.SetActive(true);
        competenceChoiceTrigger.SetActive(true);

        //set player settings
        playerCamera.SetActive(true);
        playerScript.enabled = true;
        secondCutsceneCamera.SetActive(false);
        playerRig.velocity = new Vector3(0, 0, 0);
        secondCutsceneFinished = true;
    }

    //END OF THE MECHANIC
    void ResolveDecision()
    {
        if (socialScript.playerChooseSocial)
        {
            moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, moustacheBoiCutscene.transform.position, abilitySpeed * Time.deltaTime);
            playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, warmthSourceTarget.transform.position, playerAbilitySpeed * Time.deltaTime);
            Debug.Log("social!");
        }
        if (competentScript.playerChooseCompetence)
        {
            playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, player.transform.position, playerAbilitySpeed * Time.deltaTime);
            moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, warmthSourceTarget.transform.position, playerAbilitySpeed * Time.deltaTime);
            //playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, player.transform.position, playerAbilitySpeed * Time.deltaTime);

            Debug.Log("competent");
        }
        StartCoroutine(ActivateWarmthSource());
    }

    IEnumerator ActivateWarmthSource()
    {
        //stop player from moving
        playerRig.velocity = new Vector3(0, 0, 0);
        playerScript.enabled = false;

        //set camera
        secondCutsceneCamera.SetActive(true);
        playerCamera.SetActive(false);

        yield return new WaitForSeconds(5F);
        decisionIsResolving = true;
        playerAbility.SetActive(false);
        moustacheBoiAbility.SetActive(false);
        warmthSourceOpen.SetActive(true);
        warmthSource.SetActive(false);
    }

}
