using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSnowTwo : MonoBehaviour
{
    //snowBlocksArray is an array with one array
    GameObject[][] snowBlocksArray = new GameObject[20][];
    Vector3 spawnLocation = new Vector3(80, 0, 0);

    public GameObject snowPrefab;

    public int arrayLength = 20;
    public float respawnTimerLength;

    bool deSpawn, startTimer;

    void Awake()
    {
        GlobalVariables.areaTwoSnowLeft = (arrayLength * arrayLength) - 1;

        for (int x = 0; x < snowBlocksArray.Length; x++) {
            snowBlocksArray[x] = new GameObject[arrayLength];
            for (int y = 0; y < arrayLength; y++) {
                snowBlocksArray[x][y] = Instantiate(snowPrefab, spawnLocation, Quaternion.identity);
                snowBlocksArray[x][y].transform.position = new Vector3(x + 80, 0, y);
                snowBlocksArray[x][y].transform.name = (x * snowBlocksArray.Length + y).ToString();
            }
        }
    }

    void FixedUpdate()
    {
        if (GlobalVariables.areaTwoSnowLeft <= 1 && deSpawn == false && GlobalVariables.areasCleared != 3) {
            if (!startTimer) {
                GlobalVariables.areasCleared += 1;
                startTimer = true;
                StartCoroutine(RespawnTimer());
            }
        }
        //SpawnSnowBlock();
    }

    void SpawnSnowHandler()
    {
        deSpawn = false;
        GlobalVariables.areaTwoSnowLeft = (arrayLength * arrayLength) - 1;
        for (int x = 0; x < snowBlocksArray.Length; x++) {
            snowBlocksArray[x] = new GameObject[arrayLength];
            for (int y = 0; y < arrayLength; y++) {
                snowBlocksArray[x][y] = Instantiate(snowPrefab, spawnLocation, Quaternion.identity);
                snowBlocksArray[x][y].SetActive(true);
                snowBlocksArray[x][y].transform.position = new Vector3(x + 80, 0, y);
                snowBlocksArray[x][y].transform.name = (x * snowBlocksArray.Length + y).ToString();
            }
        }
    }

    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnTimerLength);
        if (GlobalVariables.areasCleared == 3)
        {
        }
        else
        {
            startTimer = false;
            deSpawn = true;
            GlobalVariables.areasCleared -= 1;
            SpawnSnowHandler();
        }
    }

}
