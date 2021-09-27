/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class Spawnable
{
    public float speedModifier;
    public Transform posA;
    public Transform posB;
    public Transform posB2;
    public int minMonth;
    public int maxMonth;

    public Spawnable(float _speedModifier, Transform _posA, Transform _posB, Transform _posB2)
    {
        speedModifier = _speedModifier;
        posA = _posA;
        posB = _posB;
        posB2 = _posB2;
    }

}

public class CloudCycle : MonoBehaviour
{


    public int counter;
    public CloudAction baseCloud;
    public CloudAction baseTornado;
    //public GameObject spawner;
    public List<Spawnable> spawnables;
    public RectTransform grid;
    public bool useGridInfluence;
    public int maxSpawn;
    public float zLevel;
    public List<CloudAction> clouds;
    public KeyCode hideClouds;
    public bool hasSpawnDoubled;
    public bool visible;
    public int tickCountMax;

    private int maxSpawnSaved;
    private bool hasStarted;
    private int tickCount;

    private static CloudCycle _init;
    public static CloudCycle init
    {
        get { return _init; }
        private set { _init = value; }
    }

    private void Awake()
    {
        init = this;
    }

    private void Update()
    {
        if (InputOverride.init.GetKeyUp(hideClouds))
        {
            visible = !visible;
            foreach (CloudAction cloud in clouds)
            {
                cloud.visible = visible;
            }
        }
    }

    // Update is called once per frame
    private void TickUpdate()
    {
        if(tickCountMax < 0) return;

        if (hasStarted && tickCount++ > tickCountMax)
        {
            tickCount = 0;

            foreach (Spawnable spawnable in spawnables)
            {
                if (Tick.init.month >= spawnable.minMonth && Tick.init.month <= spawnable.maxMonth)
                {
                    GenerateClouds(spawnable);
                }
            }
        }
    }

    public void Initialize()
    {
        Tick.init.tickUpdate += TickUpdate;
        clouds = new List<CloudAction>();

        if (grid != null && useGridInfluence)
        {
            RectTransform baseCloudRect = baseCloud.GetComponent<RectTransform>();
            float height = (grid.rect.height / baseCloudRect.rect.height)/2;
            maxSpawn = Mathf.RoundToInt(height);
        }

        maxSpawnSaved = maxSpawn;

        foreach (Spawnable spawnable in spawnables)
        {
            if (Tick.init.month >= spawnable.minMonth && Tick.init.month <= spawnable.maxMonth)
            {
                for (int i = 0; i < maxSpawn; i++)
                {
                    GenerateClouds(spawnable, true, true);
                }
            }
        }

        hasStarted = true;
    }

    public void GenerateTornado(CloudAction cloudAction1, CloudAction cloudAction2)
    {
        if (cloudAction1.markedForDestroy || cloudAction2.markedForDestroy) return;

        if (cloudAction1.subType == "Cloud" && cloudAction2.subType == "Cloud" && cloudAction1.posA == cloudAction2.posA)
        {
            cloudAction1.markedForDestroy = true;
            cloudAction2.markedForDestroy = true;
            return;
        }

        //float result = cloudAction1.speedModifier + cloudAction2.speedModifier;

        if (Random.Range(0f,1f) <= baseTornado.spawnChance)
        {
            CloudAction tornado = Instantiate<CloudAction>(baseTornado, cloudAction1.transform.parent);
            tornado.name = baseTornado.name;
            tornado.visible = visible;
            tornado.speedModifier = cloudAction1.speedModifier + cloudAction2.speedModifier;

            if (cloudAction1.speedModifier > cloudAction2.speedModifier)
            {
                tornado.posA = cloudAction1.posA;
                tornado.posB = cloudAction1.posB;
            }
            else
            {
                tornado.posA = cloudAction2.posA;
                tornado.posB = cloudAction2.posB;
            }

            tornado.transform.position = cloudAction1.transform.position;
            clouds.Add(tornado);
            tornado.gameObject.SetActive(true);
            counter++;
        }

        cloudAction1.markedForDestroy = true;
        cloudAction2.markedForDestroy = true;
    }

    private void GenerateClouds(Spawnable spawnable, bool initialStart = false, bool ignoreSpawnChance = false)
    {
        Transform _posB = spawnable.posB;

        if (initialStart)
        {
            _posB = spawnable.posB2;
        }

        if ((ClimateControl.init.isAutumn || ClimateControl.init.isWinter) && !hasSpawnDoubled)
        {
            maxSpawn *= 2;
            hasSpawnDoubled = true;
        }

        if (ClimateControl.init.isSpring || ClimateControl.init.isSummer)
        {
            maxSpawn = maxSpawnSaved;
            hasSpawnDoubled = false;
        }

        if (counter <= maxSpawn && (Random.Range(0f,1f) <= baseCloud.spawnChance || ignoreSpawnChance))
        {
            float x = Random.Range(spawnable.posA.position.x, _posB.position.x);
            float y = Random.Range(spawnable.posA.position.y, _posB.position.y);

            CloudAction cloud = Instantiate<CloudAction>(baseCloud, baseCloud.transform.parent);
            cloud.name = baseCloud.name;
            cloud.visible = visible;
            cloud.speedModifier = spawnable.speedModifier;
            cloud.transform.position = new Vector3(x, y, zLevel);
            cloud.posA = spawnable.posA;
            cloud.posB = spawnable.posB2;
            clouds.Add(cloud);
            cloud.gameObject.SetActive(true);
            counter++;

        }
    }
}
