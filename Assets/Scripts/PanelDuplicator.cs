/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, January 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDuplicator : MonoBehaviour {

	public GameObject panel;
	public List<GameObject> lPanel;
	public int numberOfPanels; //test options

	private int numberCounter; //test options

	// Use this for initialization
	private void Start () {
		lPanel = new List<GameObject>();
	}
	
	// Update is called once per frame
	private void Update () {

		//test option
		if(numberCounter < numberOfPanels) {
			AddPanel();
			numberCounter++;
		}
	}

	public void AddPanel()
	{
		GameObject tempPanel = Instantiate(panel, panel.transform);
		Debug.Log("Creating " + tempPanel.name + " based of " + panel.name);
		tempPanel.transform.SetParent(panel.transform.parent);
		Debug.Log("Current Cloned Count: " + lPanel.Count);
		Debug.Log("Setting " + tempPanel.name + " visibility to true");
		tempPanel.SetActive(true);
		lPanel.Add(tempPanel);
	}
}
