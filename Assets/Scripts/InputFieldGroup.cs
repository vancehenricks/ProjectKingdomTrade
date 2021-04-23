/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldGroup : MonoBehaviour
{
    public List<InputFieldOverride> inputFields;
    public KeyCode key;

    public void Update()
    {
        if (InputOverride.init.GetKeyUp(key,InputOverride.init.currentFocus))
        {
            bool isNextIndex = false;

            foreach (InputField inputField in inputFields)
            {
                if(isNextIndex && inputField.IsActive())
                {
                    inputField.ActivateInputField();
                    return;
                }
                else if (inputField.isFocused)
                {
                    inputField.DeactivateInputField();
                    isNextIndex = true;
                }
            }

            inputFields[0].ActivateInputField();

        }
    }
}
