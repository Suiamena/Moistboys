using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSettings : MonoBehaviour
{
	public enum LevelSixChoices { Launch, CreatureWall, NoChoiceMade };
	public LevelSixChoices levelSixChoice = LevelSixChoices.NoChoiceMade;

	void Awake ()
	{
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded (Scene scene, LoadSceneMode loadSceneMode)
	{
		PlayerController player = FindObjectOfType<PlayerController>();
		int sceneIndex = scene.buildIndex;
		switch (sceneIndex) {
			default:
                levelSixChoice = LevelSixChoices.NoChoiceMade;
				break;
			case 1:
				player.creatureWallsEnabled = false;
				player.launchEnabled = false;
				break;
			case 2:
				player.creatureWallsEnabled = false;
				player.launchEnabled = false;
				break;
			case 3:
				player.creatureWallsEnabled = false;
				player.launchEnabled = true;
				break;
			case 4:
				player.creatureWallsEnabled = true;
				player.launchEnabled = true;
				break;
			case 5:
				player.creatureWallsEnabled = true;
				player.launchEnabled = true;
				break;
			case 6:
				switch (levelSixChoice) {
					case LevelSixChoices.NoChoiceMade:
						player.launchEnabled = true;
						player.creatureWallsEnabled = true;
						break;
					case LevelSixChoices.Launch:
						player.launchEnabled = true;
						player.creatureWallsEnabled = false;
						break;
					case LevelSixChoices.CreatureWall:
						player.launchEnabled = false;
						player.creatureWallsEnabled = true;
						break;
				}
				break;
		}
	}
}