/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, Febuary 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectedProfile : MonoBehaviour, IPointerClickHandler
{

    public GameObject profile;
    public GameObject selectionIcon;
    public Image shade;


    public void OnPointerClick(PointerEventData eventData)
    {

        //ProfileGenerator.playerColor = shade.color;

        selectionIcon.transform.position = this.transform.position;
        selectionIcon.transform.SetParent(profile.transform);
        selectionIcon.transform.SetAsLastSibling();
        selectionIcon.SetActive(true);
    }
}
