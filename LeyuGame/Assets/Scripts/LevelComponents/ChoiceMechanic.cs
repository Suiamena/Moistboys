using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceMechanic : MonoBehaviour {

    GameObject moustacheBoiCutscene;
    GameObject warmthSourceCutscene;

    int moustacheBoiSpeed = 10;

    private void Awake()
    {
        moustacheBoiCutscene = GameObject.Find("MoustacheBoiCutscene");
        warmthSourceCutscene = GameObject.Find("WarmthSourceCutscene");
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            moustacheBoiCutscene.transform.position = Vector3.MoveTowards(moustacheBoiCutscene.transform.position, warmthSourceCutscene.transform.position, moustacheBoiSpeed * Time.deltaTime);
            Debug.Log("start cutscene");
        }
    }
}
