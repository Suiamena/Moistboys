using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoustacheBoiAudio : MonoBehaviour
{
    [FMODUnity.EventRef]
    string flaps = "event:/Moustache_Boy/Flaps";
    static FMOD.Studio.EventInstance Flaps;

    string screeches = "event:/Moustache_Boy/Screeches";
    static FMOD.Studio.EventInstance Screeches;

    string wall_rumble = "event:/Moustache_Boy/Wall_Rumble";
    static FMOD.Studio.EventInstance Wall_Rumble;

    void Awake()
    {
        //moustache boy
        Flaps = FMODUnity.RuntimeManager.CreateInstance(flaps);
        Screeches = FMODUnity.RuntimeManager.CreateInstance(screeches);
        Wall_Rumble = FMODUnity.RuntimeManager.CreateInstance(wall_rumble);
    }

    private void Update()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Flaps, GetComponent<Transform>(), GetComponent<Rigidbody>());
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Screeches, GetComponent<Transform>(), GetComponent<Rigidbody>());
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Wall_Rumble, GetComponent<Transform>(), GetComponent<Rigidbody>());

        Flaps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        Screeches.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        Wall_Rumble.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
    }

    public static void PlayFlaps()
    {
        Flaps.start();
    }

    public static void StopFlaps()
    {
        Flaps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public static void PlayScreeches()
    {
        Screeches.start();
    }

    public static void PlayRumble()
    {
        Wall_Rumble.start();
    }

}


