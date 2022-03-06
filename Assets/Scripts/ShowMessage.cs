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
    private System.Action<bool> exeAction;
    public ShowMessage SetMessage(string _title, string _message, string _confirm, Sprite _icon, System.Action<bool> exe)
    {
        return SetMessage(_title, _message, _confirm, "", _icon, exe);
    }

    private void OnDestroy()
    {
        exeAction = null;
    }

    public ShowMessage SetMessage(string _title, string _message, string _confirm, string _deny, Sprite _icon, System.Action<bool> exe)
    {
        if (_deny != "")
        {
            LanguageHandler.init.Text(deny, _deny);
        }

        LanguageHandler.init.Text(title, _title);
        if (_icon != null)
        {
            icon.sprite = _icon;
        }

        LanguageHandler.init.Text(message, _message);
        LanguageHandler.init.Text(confirm, _confirm);
        GameObject temp = Instantiate(window, window.transform.parent);
        background.transform.SetAsLastSibling();
        temp.transform.SetAsLastSibling();
        temp.SetActive(true);
        background.SetActive(true);
        ShowMessage showMessage = temp.GetComponent<ShowMessage>();
        showMessage.exeAction = exe;

        return temp.GetComponent<ShowMessage>();
    }

    public void SetReponse(bool _response)
    {
        CDebug.Log(this,"Response="+_response);

        background.SetActive(false);
        response = _response;
        if (exeAction != null)
        {
            exeAction(_response);
        }
        exeAction = null;
        Destroy(gameObject);
    }

}
