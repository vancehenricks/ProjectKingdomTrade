/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2021
 */

public struct TileConfig
{
    public string tileType;
    public string subType;
    public string sprite;
    public string freezingSprite;
    public string autumnSprite;
    public string summerSprite;
    public float freezingTemp;
    public float autumnTemp;
    public float summerTemp;
    public float travelTime;
    public float spawnHeightMin;
    public float spawnHeightMax;
    public float spawnChance;
    public string[] spawnableTile;
    public int units;
    public float travelSpeed;
    public int attackDistance;
    public string[] options;
    public float killChance;
    public float deathChance;
    public Walkable[] nonWalkable;
}