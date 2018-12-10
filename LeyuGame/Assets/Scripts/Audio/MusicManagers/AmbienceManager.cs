using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour {

    // AMBIENCE
    [FMODUnity.EventRef]
    public string ambience = "event:/Ambience/Ambience";
    public static FMOD.Studio.EventInstance Ambience;
    public static FMOD.Studio.ParameterInstance WindParameter;
    public static FMOD.Studio.ParameterInstance AmethystParameter;
    public static FMOD.Studio.ParameterInstance InsideParameter;
    public static FMOD.Studio.ParameterInstance AreaParameter;

    public static float windStage;
    public static float amethystStage;
    public static float insideStage;
    public static float areaStage;

    private void Awake()
    {
        //ambience
        Ambience = FMODUnity.RuntimeManager.CreateInstance(ambience);
        Ambience.getParameter("Wind", out WindParameter);
        Ambience.getParameter("Amethyst", out AmethystParameter);
        Ambience.getParameter("Inside", out InsideParameter);
        Ambience.getParameter("Area", out AreaParameter);

        windStage = 0.45f; //wind op 0.5f, van 0.5 naar 1 lerpen bij de boundary
        amethystStage = 1f;
        insideStage = 0f;
        areaStage = 0f; //bij het laatste level, areaStage op 1 en wind op 0

        Ambience.start();
    }

    void Update()
    {
        //ambience
        WindParameter.setValue(windStage);
        AmethystParameter.setValue(amethystStage);
        InsideParameter.setValue(insideStage);
        AreaParameter.setValue(areaStage);
    }

}
