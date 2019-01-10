﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour {

    public GameObject playerModel;

    private void Awake()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(1f);
        playerModel.transform.rotation = Quaternion.Euler(0, 90, 0);
        yield return new WaitForSeconds(1f);
        playerModel.transform.rotation = Quaternion.Euler(0, 180, 0);
        yield return new WaitForSeconds(1f);
        playerModel.transform.rotation = Quaternion.Euler(0, 270, 0);
    }

}
