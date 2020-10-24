/* Copyright (C) 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, February 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NewGameAction : MonoBehaviour
{

	public Text kingdomName;
	public Text width;
	public Text height;
	public RectTransform grid;
	public ResetCenter resetCenter;
	public ShowMessage message;
	public ShowMessage confirmationMessage;
	public OpenWindow openWindow;
	public CloseWindow closeWindow;
	/*public CloudCycle cloudCycle;*/
	public CloudCycle cloudCycle2;
	public CelestialCycle celestialCycle;
	public Tick tick;

	private int w;
	private int h;

	public void DoAction ()
    {

		bool wResult = int.TryParse(width.text, out w);
		bool hResult = int.TryParse(height.text, out h);

		grid.sizeDelta = new Vector2(500f, 500f);

		if(wResult && hResult || width.text == "" && height.text == "")
        {

			if(width.text == "" && height.text == "")
            {
				w = h = 500;
			}

			if(w > 1000 && h > 1000)
            {
				ShowMessage show = confirmationMessage.SetMessage("Warning", 
                    "Map size might be too big could cause the game to be unresponsive. Would you like to continue?", 
                    "Yes", "No", null, CallbackYes);

				if(!show.response)
                {
					return;
				}
			}
			DoNewGameLogic();

		}
        else
        {
			message.SetMessage("Invalid Input", "Decimal values only in map size", "OK", null, null);
		}
			
	}

	private void CallbackYes (bool response)
    {
		if(response)
        {
			DoNewGameLogic();
		}
	}
		
	private void DoNewGameLogic()
    {
		grid.sizeDelta = new Vector2(w, h);
		closeWindow.DoClose();
		openWindow.DoOpen();
		SyncSize.doSync();
		resetCenter.DoAction();
		MapGenerator.init.Initialize();
		//cloudCycle.Initialize();
		cloudCycle2.Initialize();
		celestialCycle.Initialize();
		tick.Initialize();
	}
}
