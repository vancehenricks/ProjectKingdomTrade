/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldOverride : InputField
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        InputOverride.init.currentFocus = gameObject;
        base.OnPointerDown(eventData);
    }

    public new void ActivateInputField()
    {
        InputOverride.init.currentFocus = gameObject;
        base.ActivateInputField();
    }

    public new void DeactivateInputField()
    {
        InputOverride.init.currentFocus = null;
        base.DeactivateInputField();
    }
}
