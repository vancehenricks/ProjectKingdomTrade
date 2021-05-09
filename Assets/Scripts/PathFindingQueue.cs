/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */
using DebugHandler;
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

        scan = StartCoroutine(Scan());
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
            foreach (PathFindingPipeline pipeline in pipelines)
            {
                if (pipeline.pathFinder.Count > 0 && pipeline.task == null)
                {
                    CDebug.Log(this, "PEEK pathFinders.Count=" + pipeline.pathFinder.Count, LogType.Warning);
                    pipeline.task = new Task(pipeline.pathFinder.Peek().Calculate);
                    pipeline.task.Start();
                }
                else if (pipeline.task != null && pipeline.task.IsCompleted)
                {
                    pipeline.pathFinder.Pop();
                    CDebug.Log(this, "POP pathFinders.Count=" + pipeline.pathFinder.Count, LogType.Warning);
                    pipeline.task = null;
                }
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
                lowestCount = i;
                lowestCountIndex = i;
            }
        }

        pipelines[lowestCountIndex].pathFinder.Push(pathFinder);
    }

}
