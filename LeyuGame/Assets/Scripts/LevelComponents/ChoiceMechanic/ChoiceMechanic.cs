﻿using System.Collections;
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
    Animator moustacheBoiAnim;

    int moustacheBoiSpeed = 10, abilitySpeed = 3, playerAbilitySpeed = 6;

    //WarmthSource Settings
    GameObject warmthSource;
    public GameObject warmthSourceOpen;
    GameObject playerAbilityTarget;
    GameObject warmthSourceTarget;
    public GameObject playerAbility, moustacheBoiAbility;
    public int takeAbilityRange = 30;

    //Cutscene Settings
    public GameObject cutsceneCamera, secondCutsceneCamera, thirdCutsceneCamera, fourthCutsceneCamera;
    GameObject cutsceneCameraOneTransformTarget;

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

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerModel = GameObject.Find("MOD_Draak");
        playerAnim = playerModel.GetComponent<Animator>();
        playerCamera = GameObject.Find("Main Camera");

        moustacheBoiCutscene = GameObject.Find("Mod_Creature");
        moustacheBoiTarget = GameObject.Find("MoustacheBoiTarget");
        moustacheBoiAnim = moustacheBoiCutscene.GetComponent<Animator>();

        warmthSource = GameObject.Find("WarmthSourceCutscene");
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

    IEnumerator CreatureApproachesSource()
    {
        //SUPERFLOP HIERIN!
        cutsceneCamera.SetActive(true);
        player.transform.position = playerTransformTarget.transform.position;
        player.transform.rotation = Quaternion.Euler(-10, 20, 0);
        moustacheBoiAnim.SetBool("isFlying", true);
        yield return new WaitForSeconds(1.5F);

        //Creature Moves
        creatureToSource = true;
        yield return new WaitForSeconds(1.5F);
        moustacheBoiAnim.SetBool("isFlying", false);
        yield return new WaitForSeconds(1F);
        creatureToSource = false;

        //Ability is Lost
        //ANIMATIE USE ABILITY
        cutsceneCamera.transform.position = cutsceneCameraOneTransformTarget.transform.position;
        cutsceneCamera.transform.rotation = cutsceneCameraOneTransformTarget.transform.rotation;
        moustacheBoiAnim.SetBool("isUsingAbility", true);
        yield return new WaitForSeconds(1F);
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
        moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, new Vector3(moustacheBoiAbility.transform.position.x, moustacheBoiAbility.transform.position.y + 3, moustacheBoiAbility.transform.position.z), abilitySpeed * Time.deltaTime);
    }

    void StopFirstCutscene()
    {
        playerScript.EnablePlayer();
        cutsceneCamera.SetActive(false);
        firstCutsceneFinished = true;
    }

    //SETUP CUTSCENE TWO
    void TakePlayerAbility()
    {
        if (!secondCutsceneFinished)
        {
            playerScript.DisablePlayer();
            cameraTwoSecondPosition = secondCutsceneCamera.transform.position;
            cameraTwoSecondRotation = secondCutsceneCamera.transform.rotation;
            secondCutsceneCamera.transform.position = cutsceneCameraTwoTransformTarget.transform.position;
            secondCutsceneCamera.transform.rotation = cutsceneCameraTwoTransformTarget.transform.rotation;
            if (!runOnce)
            {
                StartCoroutine(PlayerLosesAbility());
                runOnce = true;
            }
        }
    }

    IEnumerator PlayerLosesAbility()
    {
        secondCutsceneCamera.SetActive(true);
        player.transform.position = playerTransformTargetTwo.transform.position;
        player.transform.rotation = Quaternion.Euler(0, -10, 0);
        yield return new WaitForSeconds(1F);
        //Ability moves
        playerabilityMoves = true;
        yield return new WaitForSeconds(2F);
        secondCutsceneCamera.transform.position = cameraTwoSecondPosition;
        secondCutsceneCamera.transform.rotation = cameraTwoSecondRotation;
        yield return new WaitForSeconds(2F);
        playerabilityMoves = false;
        yield return new WaitForSeconds(2F);
        //player loses ability
        playerScript.canLaunch = false;

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
    }

    void StopSecondCutscene()
    {
        //set choice
        socialChoiceTrigger.SetActive(true);
        competenceChoiceTrigger.SetActive(true);

        playerScript.EnablePlayer();
        secondCutsceneCamera.SetActive(false);
        secondCutsceneFinished = true;
    }

    //END OF THE MECHANIC
    void RunReturnAbility()
    {
        if (socialScript.playerChooseSocial)
        {
            moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, moustacheBoiCutscene.transform.position, abilitySpeed * Time.deltaTime);
        }
        if (competentScript.playerChooseCompetence)
        {
            playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, player.transform.position, abilitySpeed * Time.deltaTime);
        }
    }

    void RunSacrificeAbility()
    {
        if (socialScript.playerChooseSocial)
        {
            thirdCutsceneCamera.transform.LookAt(playerAbility.transform);
            playerAbility.transform.position = Vector3.MoveTowards(playerAbility.transform.position, warmthSourceTarget.transform.position, 15 * Time.deltaTime);
        }
        if (competentScript.playerChooseCompetence)
        {
            thirdCutsceneCamera.transform.LookAt(moustacheBoiAbility.transform);
            moustacheBoiAbility.transform.position = Vector3.MoveTowards(moustacheBoiAbility.transform.position, warmthSourceTarget.transform.position, 15 * Time.deltaTime);
        }
    }

    IEnumerator ActivateWarmthSource()
    {
        playerScript.DisablePlayer();

        //EERST ANIMATIE CREATURE, DAN PAS ABILITIES BEWEGEN
        thirdCutsceneCamera.SetActive(true);
        if (socialScript.playerChooseSocial)
        {
            moustacheBoiAnim.SetBool("isSuperFlop", true);
            yield return new WaitForSeconds(3.5F);
        }
        else
        {
            moustacheBoiAnim.SetBool("isRejected", true);
            yield return new WaitForSeconds(12F);
        }

        moustacheBoiAnim.SetBool("isRejected", false);

        thirdCutsceneCamera.transform.position = cutsceneCameraThreeTransformTarget.transform.position;
        thirdCutsceneCamera.transform.rotation = cutsceneCameraThreeTransformTarget.transform.rotation;

        if (socialScript.playerChooseSocial)
        {
            thirdCutsceneCamera.transform.LookAt(moustacheBoiCutscene.transform);
            returnAbility = true;
        }
        else
        {
            thirdCutsceneCamera.transform.LookAt(player.transform);
            returnAbility = true;
        }

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

        thirdCutsceneCamera.transform.position = cutsceneCameraThreeTransformTargetTwo.transform.position;
        thirdCutsceneCamera.transform.rotation = cutsceneCameraThreeTransformTargetTwo.transform.rotation;
        sacrificeAbility = true;

        yield return new WaitForSeconds(3F);
        sacrificeAbility = false;

        playerAbility.SetActive(false);
        moustacheBoiAbility.SetActive(false);
        socialChoiceTrigger.SetActive(false);
        competenceChoiceTrigger.SetActive(false);
        yield return new WaitForSeconds(1F);

        //VERWARM WERELD (NIEUWE CAMERA EN MODEL?)
        warmthSourceOpen.SetActive(true);
        warmthSource.SetActive(false);
        yield return new WaitForSeconds(2F);

        thirdCutsceneCamera.SetActive(false);

        //set player settings
        playerScript.EnablePlayer();
        fourthCutsceneCamera.SetActive(false);
        //RESOLVE
        if (competentScript.playerChooseCompetence)
        {
            moustacheBoiAnim.SetBool("goodBye", true);
            playerScript.canLaunch = true;
        }
        else
        {
            moustacheBoiEnding.SetActive(true);
            moustacheBoiCutscene.SetActive(false);
        }
    }

}