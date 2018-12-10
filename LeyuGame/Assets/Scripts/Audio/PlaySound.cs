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

    // DRAGON
    public string walljump = "event:/Dragon/Walljump";
    public FMOD.Studio.EventInstance Walljump;

    public string dragon_screeches = "event:/Dragon/Dragon_Screeches";
    public FMOD.Studio.EventInstance Dragon_Screeches;
    public FMOD.Studio.ParameterInstance Dragon_ScreechesParameter;


    void Awake()
    {

        //moustache boy
        Flaps = FMODUnity.RuntimeManager.CreateInstance(flaps); // talk with lenny about placement
        Screeches = FMODUnity.RuntimeManager.CreateInstance(screeches);
        Wall_Rumble = FMODUnity.RuntimeManager.CreateInstance(wall_rumble); // Play when wall spawns

        Walljump = FMODUnity.RuntimeManager.CreateInstance(walljump); // if walljump, Audio.Walljump.start();
        Dragon_Screeches = FMODUnity.RuntimeManager.CreateInstance(dragon_screeches); //start when screech nodig, Audio.Dragon_Screeches.start(); (or .Play();)
        Dragon_Screeches.getParameter("Screech", out Dragon_ScreechesParameter);
    }

}


