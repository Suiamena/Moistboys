using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelTwo : MonoBehaviour {

    GameObject creatureInSnow;
    public GameObject creatureOutSnow;

    GameObject player;
    GameObject playerModel;
    PlayerController playerScript;
    Rigidbody playerRig;
    Animator playerAnim;

    private void Awake()
    {
        creatureInSnow = GameObject.Find("CreatureInSnow");

        player = GameObject.Find("Character");
        playerModel = GameObject.Find("MOD_Draak");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerAnim = playerModel.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        transform.LookAt(player.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(MeetCreature());
        }
    }

    IEnumerator MeetCreature()
    {
        player.transform.position = new Vector3(player.transform.position.x, creatureOutSnow.transform.position.y, player.transform.position.z);
        playerAnim.SetBool("IsLaunching", false);
        playerAnim.SetBool("IsBouncing", false);
        playerRig.velocity = new Vector3(0, 0, 0);
        playerScript.enabled = false;
        yield return new WaitForSeconds(2F);
        creatureInSnow.SetActive(false);
        creatureOutSnow.SetActive(true);
        yield return new WaitForSeconds(3F);

        SceneManager.LoadScene("Level2_rough");
    }

}
