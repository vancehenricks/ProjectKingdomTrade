/* Copyright 2020 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{
    public static PlayerList init;

    public PlayerInfo playerInfo;
    public Dictionary<long, PlayerInfo> players;

    private void Start()
    {
        init = this;
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
