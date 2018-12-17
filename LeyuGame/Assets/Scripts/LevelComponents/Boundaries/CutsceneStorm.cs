using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneStorm : MonoBehaviour {

    public GameObject storm;
    GameObject player;

    bool followPlayer = true;

    private void Awake()
    {
        player = GameObject.Find("Character");
    }

    private void Update()
    {
        if (followPlayer)
        {
            storm.transform.position = new Vector3(player.transform.position.x + 5, player.transform.position.y, player.transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            followPlayer = true;
            storm.SetActive(true);
        }
    }

}
