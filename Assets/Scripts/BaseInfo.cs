/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, August 2021
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInfo : MonoBehaviour, IEquatable<BaseInfo>
{
    public TileCollider tileCollider;
    public string tileType;
    public string subType;
    public long tileId;
    public Vector2Int tileLocation;

    public virtual void Initialize()
    {
        tileId = Tools.UniqueId;        
    }

    public override bool Equals(object obj)
    {
        return Equals (obj as BaseInfo);
    }

    public bool Equals(BaseInfo other)
    {
        if (other != null && tileId == other.tileId)
        {
            return true;
        }
        
        return false;
    }

    public override int GetHashCode()
    {
        return tileId.GetHashCode();
    } 
}
