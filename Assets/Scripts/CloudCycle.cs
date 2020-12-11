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

public class CloudCycle : MonoBehaviour
{
    public int counter;
    public List<GameObject> baseCloud;
    //public GameObject spawner;
    public Transform posA;
    public Transform posB;
    public RectTransform grid;
    public bool useGridInfluence;
    public int maxSpawn;
    public int spawnChance;
    public float zLevel;
    public List<GameObject> clouds;
    public KeyCode hideClouds;

    private int maxSpawnSaved;
    private int obsoluteMaxSpawn;
    private bool hasSpawnDoubled;
    private bool hasStarted;
    private bool hide;


    private void Awake()
    {
        Tick.tickUpdate += TickUpdate;
    }

    private void Start()
    {
        clouds = new List<GameObject>();
        obsoluteMaxSpawn = maxSpawn;
        hide = true;
    }

    private void Update()
    {
        if (InputOverride.GetKeyUp(hideClouds))
        {
            hide = !hide;
            foreach (GameObject cloud in clouds)
            {
                Image image = cloud.GetComponent<Image>();
                image.enabled = hide;
            }
        }
    }

    // Update is called once per frame
    private void TickUpdate()
    {
        if (hasStarted)
        {
            GenerateClouds();
        }
    }

    public void Initialize()
    {

        counter = 0;
        maxSpawn = obsoluteMaxSpawn;

        foreach (GameObject cloud in clouds)
        {
            Destroy(cloud);
        }
        clouds.Clear();

        if (grid != null && useGridInfluence)
        {
            RectTransform baseCloudRect = baseCloud[0].GetComponent<RectTransform>();
            float width = (grid.rect.width / baseCloudRect.rect.width);
            maxSpawn = maxSpawn * (int)width;
        }

        maxSpawnSaved = maxSpawn;

        hasStarted = true;
    }

    private void GenerateClouds()
    {

        if ((ClimateControl.isAutumn || ClimateControl.isWinter) && !hasSpawnDoubled)
        {
            maxSpawn *= 2;
            hasSpawnDoubled = true;
        }

        if (ClimateControl.isSpring || ClimateControl.isSummer)
        {
            maxSpawn = maxSpawnSaved;
            hasSpawnDoubled = false;
        }

        //Debug.Log(Time.deltaTime);
        int chance = Random.Range(0, 100);

        if (chance < spawnChance && counter <= maxSpawn)
        {

            int index = Random.Range(0, baseCloud.Count);
            float x = Random.Range(posA.position.x, posB.position.x);
            float y = Random.Range(posA.position.y, posB.position.y);

            Vector3 spawnerPos = new Vector3(x, y, zLevel);
            //spawner.transform.position = spawnerPos;

            GameObject cloud = Instantiate(baseCloud[index]);
            cloud.transform.position = spawnerPos;
            cloud.transform.SetParent(baseCloud[index].transform.parent);
            cloud.transform.localScale = baseCloud[index].transform.localScale;
            clouds.Add(cloud);
            Image image = cloud.GetComponent<Image>();
            image.enabled = hide;
            cloud.SetActive(true);
            counter++;

        }
    }
}
