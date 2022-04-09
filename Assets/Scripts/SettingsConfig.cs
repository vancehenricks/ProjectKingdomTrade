/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using UnityEngine;

public struct SettingsConfig
{
    public string font;
    public string language;
    public int logLevel;
    public int maxFps;
    public bool vSync;
    public int tileOcclusion;
    public int maxPathFindingCache;
    //public int maxPathFindingQueue;
    public int maxHits;
    public string hideTerritoryKey;
    public string hideCloudsKey;
    public string hideUIKey;
    public bool cursorEdgeMove;
    public string cursorEdgeMoveKey;
    public string centerCameraKey;
    public string debugWindowKey;
    public string showConsole;
    public float zoomSpeed;
    public float minZoomScale;
    public float maxZoomScale;
    public float maxZoomMultiplier;
    public int maxTimeSpeed;
    public int[] recruitMultiplier;
}
