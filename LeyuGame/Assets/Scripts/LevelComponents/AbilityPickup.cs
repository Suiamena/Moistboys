using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AbilityPickup : MonoBehaviour
{
    GameObject abilitySphere;

    GameObject player;
    GameObject playerModel;
    PlayerController playerScript;
    Rigidbody playerRig;
    Animator playerAnim;

    public GameObject musicManager;

    private void Awake()
    {
        abilitySphere = GameObject.Find("AbilityPickup");

        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(GrabAbility());
        }
    }

    IEnumerator GrabAbility()
    {
        yield return new WaitForSeconds(1F);
        playerScript.launchEnabled = true;
        //SET UI ACTIVE AS WELL!
        musicManager.SetActive(true);
        Destroy(abilitySphere);
        Destroy(gameObject);
    }

}
