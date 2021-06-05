using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObjectHandler : MonoBehaviour
{
    private static HideObjectHandler _init;

    public static HideObjectHandler init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public Dictionary<string, HideObject> hideObjects;

    private void Awake()
    {
        init = this;
        hideObjects = new Dictionary<string, HideObject>();
    }
}
