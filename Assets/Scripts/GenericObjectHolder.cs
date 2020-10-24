/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, September 2020
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericObjectHolder : MonoBehaviour {
    
    public List<GameObject> objects;

    public T GetComponent<T>(int index)
    {
        return objects[index].GetComponent<T>();
    }
}
