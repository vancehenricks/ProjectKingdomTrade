/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, June 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenRightClick : MonoBehaviour
{

    public GenerateCells generateCells;
    public OptionGenerator optionGenerator;
    public TileInfoRaycaster tileInfoRaycaster;
    public Vector3 offset;
    public GraphicRaycaster graphicsRaycaster;
    public EventSystem eventSystem;

    public bool forceDisplay;
    public bool doNotDisplay;
    public bool showOptions;
    public bool whiteList;
    public bool multiSelect;
    public bool UseDefaultCursor;
    public List<TileInfo> include;

    private PointerEventData pointerEventData;

    private void Start()
    {
        generateCells.Initialize();
        ExecuteCommands execute = Command;
        ExecuteCommands pre = PreCommand;
        CommandPipeline.Add(execute, 1000);
        CommandPipeline.Add(pre, 50);
    }

    private void PreCommand()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            ResetValues();
            //Debug.Log("PreCommand set doNotDisplay" + doNotDisplay);
        }
    }

    private void Command()
    {
        //Debug.Log("OpenRightClick");

        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Retriving new TileInfo");
            tileInfoRaycaster.GetTileInfosFromPos(Input.mousePosition);
        }

        if ((Input.GetButtonUp("Fire2") || forceDisplay))
        {
            if (UseDefaultCursor)
            {
                CursorReplace.currentCursor = CursorType.Default;
            }

            //Debug.Log("Fire2");
            forceDisplay = false;

            List<RaycastResult> results = FireRayCast();

            if (results.Count > 0)
            {
                return;
            }

            //Debug.Log("Command set doNotDisplay" + doNotDisplay);
            if (doNotDisplay)
            {
                doNotDisplay = false;
                return;
            }

            if (TileInfoRaycaster.tileInfos.Count == 1)
            {
                optionGenerator.Initialize();
                optionGenerator.transform.position = Input.mousePosition + offset;
                optionGenerator.Display(TileInfoRaycaster.tileInfos[0]);

                MultiSelect.selectedTiles.Clear();
                MultiSelect.selectedTiles.Add(TileInfoRaycaster.tileInfos[0]);
                MultiSelect.Relay();
                return;
            }

            generateCells.include.Clear();
            generateCells.include.AddRange(include);
            generateCells.whiteList = whiteList;
            generateCells.showOptions = showOptions;
            generateCells.multiSelect = multiSelect;
            generateCells.Display();
            generateCells.transform.position = Input.mousePosition + offset;
            generateCells.gameObject.SetActive(true);
        }
        else if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire3") ||
            Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            List<RaycastResult> results = FireRayCast();

            if (results.Count > 0)
            {
                return;
            }

            ResetValues();
        }
        else if (Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Horizontal") > 0 ||
            Input.GetAxis("Vertical") < 0 || Input.GetAxis("Vertical") > 0)
        {
            ResetValues();
        }
    }

    private List<RaycastResult> FireRayCast()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicsRaycaster.Raycast(pointerEventData, results);

        return results;
    }

    public void ResetValues()
    {
        generateCells.gameObject.SetActive(false);
        optionGenerator.SetActiveAll(false);
        optionGenerator.gameObject.SetActive(false);
        UseDefaultCursor = true;
        showOptions = true;
        whiteList = false;
        multiSelect = true;
    }
}
