/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorType
{
    Previous = -1,
    Default = 0,
    Move,
    Zoom,
    Build,
    Attack
}

public class CursorReplace: MonoBehaviour
{
    private static CursorReplace _init;

    public static CursorReplace init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public List<Texture2D> cursorArray;
    public CursorMode cursorMode;
    public Vector2 hotSpot;
    //public CursorLockMode lockState = CursorLockMode.Locked;

    private Texture2D currentTexture2D;
    private Texture2D previousTexture2D;

    private CursorType _currentCursor;

    public CursorType currentCursor
    {
        get
        {
            return _currentCursor;
        }

        set
        {
            if (value == CursorType.Previous)
            {
                currentTexture2D = previousTexture2D;
            }
            else
            {
                //had to go with this complex way of passing same Texture2D to avoid Cursor warning despite using same config
                //issue with unity editor where it mixes different texture2D despite getting from the right array

                CDebug.Log(this, "CursorType=" + value + "," + (int)value);

                Texture2D texture = cursorArray[(int)value];
                currentTexture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
                #if UNITY_EDITOR
                currentTexture2D.alphaIsTransparency = true;
                #endif
                currentTexture2D.SetPixels32(texture.GetPixels32());
                texture.Apply(false);
            }

            Cursor.SetCursor(currentTexture2D, hotSpot, cursorMode);
            _currentCursor = value;
        }
    }

    private void Awake()
    {
        init = this;
        currentCursor = CursorType.Default;
        SetCurrentCursorAsPrevious();
    }

    public void SetCurrentCursorAsPrevious()
    {
        previousTexture2D = currentTexture2D;
    }
}
