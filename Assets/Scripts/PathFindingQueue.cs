/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
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
    public Queue<PathFinder> pathFinder;
    public Task task;

    public PathFindingPipeline()
    {
        pathFinder = new Queue<PathFinder>();
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

    //public int maxQueue;
    public PathFindingPipeline pipeline;

    private Coroutine scan;

    public void Initialize()
    {
        scan = StartCoroutine(Scan());
    }

    private void Awake()
    {
        init = this;
        pipeline = new PathFindingPipeline();
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
            if(pipeline.pathFinder.Count == 0)
            {
                yield return null;
                continue;
            }

            CDebug.Log(this, "PEEK pathFinders.Count=" + pipeline.pathFinder.Count, LogType.Warning);
            pipeline.task = new Task(pipeline.pathFinder.Peek().Calculate);
            pipeline.task.Start();

            while(!pipeline.task.IsCompleted)
            {
                yield return null;
            }   

            pipeline.pathFinder.Dequeue();
            CDebug.Log(this, "DEQUEUE pathFinders.Count=" + pipeline.pathFinder.Count, LogType.Warning);
            pipeline.task = null;
            
            yield return null;
        }
    }

    public void Enqueue(PathFinder pathFinder)
    {
        pipeline.pathFinder.Enqueue(pathFinder);
        CDebug.Log(this, "ENQUEUE pathFinders.Count=" + pipeline.pathFinder.Count, LogType.Warning);
    }

}
