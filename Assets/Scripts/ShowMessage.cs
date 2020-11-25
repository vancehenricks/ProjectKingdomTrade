/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ShowMessage : MonoBehaviour
{

    public GameObject window;
    public Text message;
    public Text title;
    public Text confirm;
    public Text deny;
    public Image icon;
    public GameObject background;

    public bool response { get; private set; }

    public delegate void ExecuteAction(bool response);
    private ExecuteAction exeAction;

    public ShowMessage SetMessage(string t, string m, string c, Sprite i, ExecuteAction exe)
    {
        return SetMessage(t, m, c, "", i, exe);
    }

    private void OnDestroy()
    {
        exeAction = null;
    }

    public ShowMessage SetMessage(string t, string m, string c, string d, Sprite i, ExecuteAction exe)
    {
        if (d != "")
        {
            deny.text = d;
        }

        title.text = t;
        if (i != null)
        {
            icon.sprite = i;
        }

        message.text = m;
        confirm.text = c;
        GameObject temp = Instantiate(window, window.transform.parent);
        background.transform.SetAsLastSibling();
        temp.transform.SetAsLastSibling();
        temp.SetActive(true);
        background.SetActive(true);
        ShowMessage msg = temp.GetComponent<ShowMessage>();
        msg.exeAction = exe;

        return temp.GetComponent<ShowMessage>();
    }

    public void SetReponse(bool r)
    {
        background.SetActive(false);
        response = r;
        if (exeAction != null)
        {
            exeAction(r);
        }
        exeAction = null;
        Destroy(gameObject);
    }

}
