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

    static bool playerIsInside;

    private void Awake()
    {
        //ambience
        Ambience = FMODUnity.RuntimeManager.CreateInstance(ambience);
        Ambience.getParameter("Wind", out WindParameter);
        Ambience.getParameter("Amethyst", out AmethystParameter);
        Ambience.getParameter("Inside", out InsideParameter);
        Ambience.getParameter("Area", out AreaParameter);

        Ambience.start();
    }

    void Update()
    {
        WindParameter.setValue(windStage);
        AmethystParameter.setValue(amethystStage);
        InsideParameter.setValue(insideStage);
        AreaParameter.setValue(areaStage);
    }

    public static void ToggleAmbience()
    {
        if (playerIsInside)
        {
            playerIsInside = false;
            insideStage = 0f;
        }
        else
        {
            playerIsInside = true;
            insideStage = 1f;
        }
    }

    public static void IncreaseWindSound(float windGrowthSpeed)
    {
        windStage += windGrowthSpeed;
        windStage = Mathf.Clamp(windStage, 0.4f, 0.7f);
    }

    public static void DecreaseWindSound()
    {
        windStage = 0.4f;
    }

}
