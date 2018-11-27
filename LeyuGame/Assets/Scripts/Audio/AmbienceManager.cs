using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour {

    [FMODUnity.EventRef]
    public string wind = "event:/Ambience/Wind";
    public string wolf = "event:/Creatures/Wolf";
    public  string ground = "event:/Vegetation/Amethyst_Ground";
    public string awoo = "event:/Creatures/Awoo";

    public FMOD.Studio.EventInstance Wind;
    public FMOD.Studio.EventInstance Wolf;
    public FMOD.Studio.EventInstance Amethyst_Ground;
    public FMOD.Studio.EventInstance Awoo;

    private void Awake()
    {
        Wind = FMODUnity.RuntimeManager.CreateInstance(wind);
        Wolf = FMODUnity.RuntimeManager.CreateInstance(wolf);
        Amethyst_Ground = FMODUnity.RuntimeManager.CreateInstance(ground);
        Awoo = FMODUnity.RuntimeManager.CreateInstance(awoo);

        Wind.start();
        Wolf.start();
        Amethyst_Ground.start();
        Awoo.start();
    }

}
