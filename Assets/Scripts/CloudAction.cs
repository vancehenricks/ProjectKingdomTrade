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
    public GameObject obj;
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

    private float liveTimeCounter;
    private float liveTime;
    private Vector3 pos;
    private Image objImage;

    private void Start()
    {

        if (grid.rect.width > defaultWidth && influenceByGrid)
        {
            maxLiveTime *= (grid.rect.width / divideWidth);
        }
        liveTime = Random.Range(minLiveTime, maxLiveTime);

        objImage = obj.GetComponent<Image>();

        StartCoroutine(FadeIn(0.1f));
    }

    private void Update()
    {
        if (Tick.speed > 0)
        {

            pos = obj.transform.position;
            diffXA = obj.transform.position.x - posA.transform.position.x;
            diffXB = obj.transform.position.x - posB.transform.position.x;

            obj.transform.position = new Vector3(pos.x + (Tick.speed * speedModifier) * Time.deltaTime, pos.y, cloudCycle.zLevel);
            liveTimeCounter = liveTimeCounter + (Tick.speed * Time.deltaTime);

            //Debug.Log(diffXA);
            if (diffXA >= -offsetDespawn && diffXA <= offsetDespawn
                || diffXB >= -offsetDespawn && diffXB <= offsetDespawn
                || liveTimeCounter >= liveTime || markedForDestroy)
            {

                StartCoroutine(FadeOut(0.1f));
            }
        }
    }

    private IEnumerator FadeIn(float value)
    {
        Color color = objImage.color;

        for (int i = 0; i < durationBeforeDisplay; i++)
        {
            if (Tick.speed == 0)
            {
                i--;
            }

            yield return new WaitForSeconds(1f);
        }

        while (color.a < maxAlpha)
        {
            color.a += value;
            objImage.color = color;

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator FadeOut(float value)
    {
        Color color = objImage.color;

        while (color.a > 0)
        {
            color.a -= value;
            objImage.color = color;

            yield return new WaitForSeconds(0.1f);
        }

        Destroy(obj);
        cloudCycle.counter--;
    }
}
