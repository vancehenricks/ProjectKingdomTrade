/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, November 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionHandler : MonoBehaviour
{
    public static TransitionHandler instance;

    public GameObject MainMenu;

    private void Awake()
    {
        instance = this;
        MainMenu.SetActive(TransitionData.loadMainMenu);
    }

    public void Transition()
    {
        SceneManager.LoadScene("sandbox", LoadSceneMode.Single);
    }

    public void TransitionWithoutMenu()
    {
        TransitionData.loadMainMenu = false;
        Transition();
    }
}
