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

public class CloudAction : MonoBehaviour
{
    public string type;
    public float spawnChance;
    public CloudCycle cloudCycle;
    public RectTransform grid;
    public float divideWidth;
    public float defaultWidth;
    public bool influenceByGrid;
    public float minLiveTime;
    public float maxLiveTime;
    public float speedModifier;
    public Transform posA;
    public Transform posB;
    public float offsetDespawn;
    public float diffXA;
    public float diffXB;

    public float maxAlpha;
    public float durationBeforeDisplay;
    public bool markedForDestroy;

    public float collidePoints;
    public float liveTimeCounter;
    private float liveTime;
    private Vector3 pos;
    public Image image;

    private void Start()
    {

        if (grid.rect.width > defaultWidth && influenceByGrid)
        {
            maxLiveTime *= (grid.rect.width / divideWidth);
        }

        liveTime = Random.Range(minLiveTime, maxLiveTime);

        StartCoroutine(FadeIn(0.1f));
    }

    private void Update()
    {
        if (Tick.init.speed > 0)
        {

            pos = transform.position;
            diffXA = transform.position.x - posA.transform.position.x;
            diffXB = transform.position.x - posB.transform.position.x;

            float posX = pos.x + (Tick.init.speed * speedModifier) * Time.deltaTime;

            Tools.SetDirection(transform, new Vector3(posX,0f));

            gameObject.transform.position = new Vector3(pos.x + (Tick.init.speed * speedModifier) * Time.deltaTime, pos.y, cloudCycle.zLevel);
            liveTimeCounter = liveTimeCounter + (Tick.init.speed * Time.deltaTime);

            //Debug.Log(diffXA);
            if (diffXA >= -offsetDespawn && diffXA <= offsetDespawn
                || diffXB >= -offsetDespawn && diffXB <= offsetDespawn
                || liveTimeCounter >= liveTime || markedForDestroy || speedModifier == 0f)
            {
                StartCoroutine(FadeOut(0.1f));
            }
        }
    }

    private IEnumerator FadeIn(float value)
    {
        Color color = image.color;

        for (int i = 0; i < durationBeforeDisplay; i++)
        {
            if (Tick.init.speed == 0)
            {
                i--;
            }

            yield return new WaitForSeconds(1f);
        }

        while (color.a < maxAlpha)
        {
            color.a += value;
            image.color = color;

            yield return null;
        }
    }

    private void OnDestroy()
    {
        cloudCycle.clouds.Remove(this);
        cloudCycle.counter--;
    }

    private IEnumerator FadeOut(float value)
    {
        Color color = image.color;

        while (color.a > 0)
        {
            color.a -= value;
            image.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}
