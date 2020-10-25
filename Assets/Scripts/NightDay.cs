/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NightDay : MonoBehaviour
{

    public static int sunrise;
    public static int sunset;

    public Image image;
    public float maxAlpha;
    public float minAlpha;

    private bool isSummer;
    private bool isAutumn;
    private int sunriseSaved;
    private int sunsetSaved;

    private void Start()
    {
        sunset = 17;
        sunrise = 5;
        sunriseSaved = sunrise;
        sunsetSaved = sunset;
    }

    private void Update()
    {

        //Debug.Log("Sunset: " + sunset + " Sunrise: " + sunrise);

        if (ClimateControl.isSpring)
        {
            sunrise = sunriseSaved;
            sunset = sunsetSaved;
            isSummer = false;
            isAutumn = false;
        }

        if (ClimateControl.isSummer && !isSummer)
        {
            sunrise -= 3;
            sunrise += 2;
            isSummer = true;
        }

        if ((ClimateControl.isAutumn || ClimateControl.isWinter) && !isAutumn)
        {
            sunrise += 3;
            sunset -= 2;
            isAutumn = true;
        }

        if (isNight())
        {
            Color color = image.color;
            color.a = Mathf.Lerp(image.color.a, maxAlpha, 0.5f * Time.deltaTime);
            image.color = color;
        }
        else if (!isNight())
        {
            Color color = image.color;
            color.a = Mathf.Lerp(image.color.a, minAlpha, 0.5f * Time.deltaTime);
            image.color = color;
        }
    }

    public static bool isNight()
    {

        if (Tick.seconds >= sunset)
        {
            return true;
        }
        else if (Tick.seconds >= sunrise)
        {
            return false;
        }

        return true;
    }
}
