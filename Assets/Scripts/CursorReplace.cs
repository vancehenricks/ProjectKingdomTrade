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

public class CursorReplace : MonoBehaviour
{


    public List<Texture2D> _cursorArray;
    public CursorMode _cursorMode;
    public Vector2 _hotSpot;

    public static List<Texture2D> cursorArray;
    public static CursorMode cursorMode;
    public static Vector2 hotSpot;
    //public CursorLockMode lockState = CursorLockMode.Locked;

    private static Texture2D currentTexture2D;
    private static Texture2D previousTexture2D;

    private static CursorType _currentCursor;
    public static CursorType currentCursor
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
                
                Texture2D texture = cursorArray[(int)value];
                currentTexture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
                #if UNITY_EDITOR
                currentTexture2D.alphaIsTransparency = true;
                #endif
                currentTexture2D.SetPixels32(texture.GetPixels32());
            }

            Cursor.SetCursor(currentTexture2D, hotSpot, cursorMode);
            _currentCursor = value;
        }
    }

    private void Awake()
    {
        cursorArray = _cursorArray;
        cursorMode = _cursorMode;
        hotSpot = _hotSpot;
        currentCursor = CursorType.Default;
        previousTexture2D = cursorArray[0];
    }

    public static void SetCurrentCursorAsPrevious()
    {
        previousTexture2D = currentTexture2D;
    }
}
