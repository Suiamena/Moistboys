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
			playerController.creatureWallsEnabled = true;
		else
			playerController.creatureWallsEnabled = false;
        Debug.Log(VariablesGlobal.chosenForCompetence);
        Debug.Log(VariablesGlobal.chosenForSocial);
	}
}