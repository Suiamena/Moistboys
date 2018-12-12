using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{

    // MOUSTACHE BOY
    [FMODUnity.EventRef]
    public string flaps = "event:/Moustache_Boy/Flaps";
    public FMOD.Studio.EventInstance Flaps;

    public string screeches = "event:/Moustache_Boy/Screeches";
    public FMOD.Studio.EventInstance Screeches;

    public string wall_rumble = "event:/Moustache_Boy/Wall_Rumble";
    public FMOD.Studio.EventInstance Wall_Rumble;



    void Awake()
    {

        //moustache boy
        Flaps = FMODUnity.RuntimeManager.CreateInstance(flaps); // talk with lenny about placement
        Screeches = FMODUnity.RuntimeManager.CreateInstance(screeches);
        Wall_Rumble = FMODUnity.RuntimeManager.CreateInstance(wall_rumble); // Play when wall spawns
    }

}


