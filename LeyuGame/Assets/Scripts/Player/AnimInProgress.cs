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
            StartCoroutine(ExitLaunchAnim());
        }
    }

    IEnumerator ExitLaunchAnim()
    {
        anim.SetBool("IsLaunching", true);
        yield return new WaitForSeconds(0.5F);
        anim.SetBool("IsLaunching", false);
    }

}
