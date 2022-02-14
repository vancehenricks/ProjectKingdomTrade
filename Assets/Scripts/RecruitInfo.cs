/* Copyright 2022 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2022
 */

using System.Collections.Generic;

public class RecruitInfo
{
    public List<TileInfo> towns;
    public TileInfo baseInfo;
    public float timeLeft;
    
    public RecruitInfo(RecruitButton _recruitButton)
    {
        towns = _recruitButton.towns;
        baseInfo = _recruitButton.baseInfo;

        UnitInfo baseUnit = baseInfo as UnitInfo;
        timeLeft =  baseUnit.spawnTime;
    }
}