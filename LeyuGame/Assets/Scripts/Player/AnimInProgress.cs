using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimInProgress : MonoBehaviour {

    public Animator anim;

	
	void Update ()
    {
        RunAnimation();
	}

    void RunAnimation()
    {
        if (Input.GetButtonDown("A Button"))
        {
            Debug.Log("running");
            anim.SetBool("IsLaunching", true);
        }
    }

}
