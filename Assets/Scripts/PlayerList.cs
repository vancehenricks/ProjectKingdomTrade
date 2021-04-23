﻿/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    private static PlayerList _init;

    public static PlayerList init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public PlayerInfo playerInfo;
    public Dictionary<long, PlayerInfo> players;

    private void Awake()
    {
        init = this;
    }

    private void Start()
    {
        players = new Dictionary<long, PlayerInfo>();
    }

    public PlayerInfo Instantiate()
    {
        PlayerInfo temp = Instantiate<PlayerInfo>(playerInfo, playerInfo.transform.parent);
        temp.Initialize();
        players.Add(temp.playerId, temp);

        return temp;
    }
}
