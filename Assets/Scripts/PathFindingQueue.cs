﻿/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class PathFindingPipeline
{
    public Stack<PathFinder> pathFinder;
    public Task task;

    public PathFindingPipeline()
    {
        pathFinder = new Stack<PathFinder>();
    }
}

public class PathFindingQueue : MonoBehaviour
{
    private static PathFindingQueue _init;
    public static PathFindingQueue init
    {
        get { return _init; }
        private set { _init = value; }
    }

    public int maxQueue;
    public List<PathFindingPipeline> pipelines;

    private Coroutine scan;

    private void Awake()
    {
        init = this;

        pipelines = new List<PathFindingPipeline>();
    }

    public void Initialize()
    {
        for (int i = 0; i < maxQueue; i++)
        {
            pipelines.Add(new PathFindingPipeline());
        }

        //scan = StartCoroutine(Scan());
    }

    private void OnDestroy()
    {
        if (scan != null)
        {
            StopCoroutine(scan);
        }
    }

    private IEnumerator Scan()
    {
        while (true)
        {
            for(int i = 0;i < pipelines.Count;i++)
            {
                if (pipelines[i].pathFinder.Count > 0 && pipelines[i].task == null)
                {
                    CDebug.Log(this, "Pipeline=" + i + " PEEK pathFinders.Count=" + pipelines[i].pathFinder.Count, LogType.Warning);
                    pipelines[i].task = new Task(pipelines[i].pathFinder.Peek().Calculate);
                    pipelines[i].task.Start();
                }
                else if (pipelines[i].task != null && pipelines[i].task.IsCompleted)
                {
                    pipelines[i].pathFinder.Pop();
                    CDebug.Log(this, "Pipeline=" + i + " POP pathFinders.Count=" + pipelines[i].pathFinder.Count, LogType.Warning);
                    pipelines[i].task = null;
                }
            }

            int pipelineEmpty = 0;
            for(int i = 0;i < pipelines.Count;i++)
            {
                if(pipelines[i].pathFinder.Count == 0)
                {
                    pipelineEmpty++;
                }
            }

            if(pipelineEmpty == pipelines.Count)
            {
                yield break;
            }

            yield return null;
        }
    }

    public void Push(PathFinder pathFinder)
    {
        int lowestCountIndex = 0;
        int lowestCount = pipelines[0].pathFinder.Count;

        for(int i = 0;i < pipelines.Count;i++)
        {
            if (pipelines[i].pathFinder.Count <= lowestCount)
            {
                lowestCount = pipelines[i].pathFinder.Count;
                lowestCountIndex = i;
            }
        }

        if (scan != null)
        {
            StopCoroutine(scan);
        }

        pipelines[lowestCountIndex].pathFinder.Push(pathFinder);
        CDebug.Log(this, "Pipeline=" + lowestCountIndex + " PUSH pathFinders.Count=" + lowestCount, LogType.Warning);

        for(int i = 0;i < pipelines.Count;i++)
        {
            pipelines[i].task = null;
        }
        
        scan = StartCoroutine(Scan());
    }

}
