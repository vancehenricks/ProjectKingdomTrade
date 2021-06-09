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
    private static NightDay _init;

    public static NightDay init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public int sunrise;
    public int sunset;

    public Image image;
    public float maxAlpha;
    public float minAlpha;

    private bool isSummer;
    private bool isAutumn;
    private int sunriseSaved;
    private int sunsetSaved;

    private void Start()
    {
        init = this;
        sunset = 17;
        sunrise = 5;
        sunriseSaved = sunrise;
        sunsetSaved = sunset;

        CDebug.Log(this, "sunset=" + sunset + " sunrise=" + sunrise);
    }

    private void Update()
    {
        if (ClimateControl.init.isSpring)
        {
            sunrise = sunriseSaved;
            sunset = sunsetSaved;
            isSummer = false;
            isAutumn = false;
        }

        if (ClimateControl.init.isSummer && !isSummer)
        {
            sunrise -= 3;
            sunrise += 2;
            isSummer = true;
        }

        if ((ClimateControl.init.isAutumn || ClimateControl.init.isWinter) && !isAutumn)
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

    public bool isNight()
    {

        if (Tick.init.seconds >= sunset)
        {
            return true;
        }
        else if (Tick.init.seconds >= sunrise)
        {
            return false;
        }

        return true;
    }
}
