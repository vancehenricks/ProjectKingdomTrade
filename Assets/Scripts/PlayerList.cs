/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
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

    public PlayerInfo basePlayerInfo;

    private PlayerInfo _defaultPlayerInfo;
    public PlayerInfo defaultPlayerInfo
    {
        get
        {
            return _defaultPlayerInfo;
        }
        private set
        {
            _defaultPlayerInfo = value;
        }
    }

    public Dictionary<long, PlayerInfo> players;

    private void Awake()
    {
        players = new Dictionary<long, PlayerInfo>();
        Instantiate();
        init = this;
    }


    public PlayerInfo Instantiate()
    {
        PlayerInfo temp = Instantiate<PlayerInfo>(basePlayerInfo, basePlayerInfo.transform.parent);
        temp.Initialize();

        if (defaultPlayerInfo == null)
        {
            defaultPlayerInfo = temp;
        }

        players.Add(temp.playerId, temp);

        return temp;
    }
}
