using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceChoice : MonoBehaviour
{

    public GameObject choiceMessage;

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            choiceMessage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            choiceMessage.SetActive(false);
        }
    }

}
