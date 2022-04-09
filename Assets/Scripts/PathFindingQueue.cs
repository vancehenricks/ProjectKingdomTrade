/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, May 2021
 */

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PathFindingQueue : MonoBehaviour
{
    private static PathFindingQueue _init;
    public static PathFindingQueue init
    {
        get { return _init; }
        private set { _init = value; }
    }

    //public int maxQueue;
    public Queue<PathFinder> pathFinderQueue;

    private Coroutine scan;

    public void Initialize()
    {
        scan = StartCoroutine(Scan());
    }

    private void Awake()
    {
        init = this;
        pathFinderQueue = new Queue<PathFinder>();
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
            if(pathFinderQueue.Count == 0)
            {
                yield return null;
                continue;
            }

            CDebug.Log(this, "PEEK pathFinders.Count=" + pathFinderQueue.Count, LogType.Warning);
            Task task = new Task(pathFinderQueue.Dequeue().Calculate);
            task.Start();

            while(!task.IsCompleted)
            {
                yield return null;
            }   

            //pipeline.pathFinder.Dequeue();
            CDebug.Log(this, "DEQUEUE pathFinders.Count=" + pathFinderQueue.Count, LogType.Warning);
        }
    }

    public void Enqueue(PathFinder pathFinder)
    {
        pathFinderQueue.Enqueue(pathFinder);
        CDebug.Log(this, "ENQUEUE pathFinders.Count=" + pathFinderQueue.Count, LogType.Warning);
    }

}
