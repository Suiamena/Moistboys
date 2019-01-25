using UnityEngine;

public class CommunicateChoice : MonoBehaviour
{
	void Start ()
	{
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

		if (VariablesGlobal.chosenForCompetence)
			playerController.launchEnabled = true;
		else
			playerController.launchEnabled = false;

        if (VariablesGlobal.chosenForSocial)
            Debug.Log("nope");
        //playerController.creatureWallsEnabled = true;
        else
            playerController.creatureWallsEnabled = false;
	}
}