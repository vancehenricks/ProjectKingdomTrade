/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedPlaceHolder : MonoBehaviour
{
    public Text xOffsetText;
    public Text yOffsetText;
    public Text scaleText;

    private float _xOffset;
    private float _yOffset;
    private float _scale;

    public float xOffset
    {
        get
        {
            return _xOffset;
        }
        private set
        {
            _xOffset = value;
            xOffsetText.text = _xOffset.ToString("0.00");
        }
    }

    public float yOffset
    {
        get
        {
            return _yOffset;
        }
        private set
        {
            _yOffset = value;
            yOffsetText.text = _yOffset.ToString("0.00");
        }
    }

    public float scale
    {
        get
        {
            return _scale;
        }
        private set
        {
            _scale = value;
            scaleText.text = _scale.ToString("0.00");
        }
    }

    public void Randomize()
    {
       xOffset = Random.Range(-1000f, 1000f);
       yOffset = Random.Range(-1000f, 1000f);
       scale = 3;
    }

}
