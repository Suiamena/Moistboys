using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSnow : MonoBehaviour
{
    //snowBlocksArray is an array with one array
    GameObject[][] snowBlocksArray = new GameObject[20][];
    Vector3 spawnLocation = new Vector3(0, 0, 0);

    public GameObject snowPrefab;

    public int arrayLength = 20;
    public float respawnTimerLength;

    bool deSpawn, startTimer;

    //float spawnRate = 0.1f;
    //float nextSpawnTime = 1f;

    //int snowBlockIdentity;
    //int lineIdentity;

    void Awake()
    {
        GlobalVariables.areaOneSnowLeft = (arrayLength * arrayLength) - 1;

        for (int x = 0; x < snowBlocksArray.Length; x++)
        {
            snowBlocksArray[x] = new GameObject[arrayLength];
            for (int y = 0; y < arrayLength; y++)
            {
                snowBlocksArray[x][y] = Instantiate(snowPrefab, spawnLocation, Quaternion.identity);
                snowBlocksArray[x][y].transform.position = new Vector3(x, 0, y);
                snowBlocksArray[x][y].transform.name = (x * snowBlocksArray.Length + y).ToString();
            }
        }

        //snowBlocksArray[12][2].SetActive(true);

        //snowBlocksArray[0] = new GameObject[arrayLength];
        //the first (and only) element of snowBlocksArray is an array of five objects;
    }

    void FixedUpdate()
    {
        if (GlobalVariables.areaOneSnowLeft <= 1 && deSpawn == false && GlobalVariables.areasCleared != 3) {
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
        GlobalVariables.areaOneSnowLeft = (arrayLength * arrayLength) - 1;
        for (int x = 0; x < snowBlocksArray.Length; x++)
            {
                snowBlocksArray[x] = new GameObject[arrayLength];
                for (int y = 0; y < arrayLength; y++)
                {
                    snowBlocksArray[x][y] = Instantiate(snowPrefab, spawnLocation, Quaternion.identity);
                    snowBlocksArray[x][y].SetActive(true);
                    snowBlocksArray[x][y].transform.position = new Vector3(x, 0, y);
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

    //void SpawnSnowBlock()
    //{
    //    if (Time.time > nextSpawnTime) {
    //        //KEEP SPAWNING AS LONG AS THE GRID ISN'T FULL
    //        if (snowBlockIdentity < arrayLength) {
    //            nextSpawnTime = Time.time + spawnRate;

    //            //GET NEW SNOWBLOCK IN ARRAY
    //            snowBlocksArray[0][snowBlockIdentity] = snowPrefab;

    //            //INSTANTIATE ALL SNOWBLOCKS
    //            if (!deSpawn) {
    //                Instantiate(snowBlocksArray[0][snowBlockIdentity], spawnLocation, transform.rotation);
    //            }
    //            else {
    //                Debug.Log("spawn");

    //                //GameObject[][] array = snowBlocksArray;
    //                //GameObject g = array[0][snowBlockIdentity];
    //                //g.SetActive(false);

    //                //objectToSpawn.SetActive(false);

    //                //snowBlocksArray[0][snowBlockIdentity].SetActive(false);

    //                //foreach (GameObject[] array in snowBlocksArray)
    //                //{
    //                //    foreach (GameObject g in array)
    //                //    {
    //                //        g.SetActive(false);
    //                //    }
    //                //}
    //            }

    //            //SET NEXT SPAWNLOCATION AND SNOWBLOCK IDENTITY
    //            spawnLocation += new Vector3(1, 0, 0);
    //            snowBlockIdentity += 1;

    //            //START A NEW LINE
    //            if (snowBlockIdentity == arrayLength) {
    //                if (lineIdentity != arrayLength) {
    //                    lineIdentity += 1;
    //                    spawnLocation = new Vector3(0, 0, 0);
    //                    spawnLocation -= new Vector3(0, 0, lineIdentity);
    //                    snowBlockIdentity = 0;
    //                }

    //            }

    //        }
    //        else {
    //            Debug.Log("done");
    //            snowBlockIdentity = 0;
    //            lineIdentity = 0;
    //            spawnLocation = new Vector3(0, 0, 0);
    //            deSpawn = true;
    //        }
    //    }
    //}

}
