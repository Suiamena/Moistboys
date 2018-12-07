using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{

    [Header("fmod Parameters")]

    // BACKGROUND MUSIC
    [FMODUnity.EventRef]
    public string music = "event:/Music";
    public FMOD.Studio.EventInstance Music;
    public FMOD.Studio.ParameterInstance MusicParameter;

    public static float musicStage;

    // AMBIENCE
    [FMODUnity.EventRef]
    public string ambience = "event:/Ambience/Ambience";
    public FMOD.Studio.EventInstance Ambience;
    public FMOD.Studio.ParameterInstance WindParameter;
    public FMOD.Studio.ParameterInstance AmethystParameter;
    public FMOD.Studio.ParameterInstance InsideParameter;
    public FMOD.Studio.ParameterInstance AreaParameter;

    public float windStage;
    public float amethystStage;
    public float insideStage;
    public float areaStage;

    // OBJECTS
    [FMODUnity.EventRef]
    public string lighttower = "event:/Objects/Lighttower";
    public FMOD.Studio.EventInstance Lighttower;

    // MOUSTACHE BOY
    [FMODUnity.EventRef]
    public string flaps = "event:/Moustache_Boy/Flaps";
    public FMOD.Studio.EventInstance Flaps;

    public string screeches = "event:/Moustache_Boy/Screeches";
    public FMOD.Studio.EventInstance Screeches;

    public string wall_rumble = "event:/Moustache_Boy/Wall_Rumble";
    public FMOD.Studio.EventInstance Wall_Rumble;

    // DRAGON
    [FMODUnity.EventRef]
    public string airjump = "event:/Dragon/Airjump";
    public FMOD.Studio.EventInstance Airjump;

    public string walljump = "event:/Dragon/Walljump";
    public FMOD.Studio.EventInstance Walljump;

    public string dragon_screeches = "event:/Dragon/Dragon_Screeches";
    public FMOD.Studio.EventInstance Dragon_Screeches;
    public FMOD.Studio.ParameterInstance Dragon_ScreechesParameter;


    void Awake()
    {
        //music
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Music.getParameter("Music", out MusicParameter);
        musicStage = 0f;

        //ambience
        Ambience = FMODUnity.RuntimeManager.CreateInstance(ambience);
        Ambience.getParameter("Wind", out WindParameter);
        Ambience.getParameter("Amethyst", out AmethystParameter);
        Ambience.getParameter("Inside", out InsideParameter);
        Ambience.getParameter("Area", out AreaParameter);

        windStage = 0f; //wind op 0.5f, van 0.5 naar 1 lerpen bij de boundary
        amethystStage = 0f;
        insideStage = 0f;
        areaStage = 0f; //bij het laatste level, areaStage op 1 en wind op 0

        //objects
        Lighttower = FMODUnity.RuntimeManager.CreateInstance(lighttower); //always play at object

        //moustache boy
        Flaps = FMODUnity.RuntimeManager.CreateInstance(flaps); // talk with lenny about placement
        Screeches = FMODUnity.RuntimeManager.CreateInstance(screeches);
        Wall_Rumble = FMODUnity.RuntimeManager.CreateInstance(wall_rumble); // Play when wall spawns

        //dragon
        Airjump = FMODUnity.RuntimeManager.CreateInstance(airjump); //if airjump, Audio.Airjump.start();
        Walljump = FMODUnity.RuntimeManager.CreateInstance(walljump); // if walljump, Audio.Walljump.start();
        Dragon_Screeches = FMODUnity.RuntimeManager.CreateInstance(dragon_screeches); //start when screech nodig, Audio.Dragon_Screeches.start(); (or .Play();)
        Dragon_Screeches.getParameter("Screech", out Dragon_ScreechesParameter);

        //START MUSIC AND AMBIENCE
        Music.start();
        Ambience.start();
    }


    void Update()
    {
        //music
        MusicParameter.setValue(musicStage);

        //ambience
        WindParameter.setValue(windStage);
        AmethystParameter.setValue(amethystStage);
        InsideParameter.setValue(insideStage);
        AreaParameter.setValue(areaStage);
    }

}


