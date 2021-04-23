/* Copyright 2019 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, March 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraDraggableWindow : MonoBehaviour/*,IDragHandler*/
{

    public Camera cm;
    public float keySpeedModifier;
    public float boundary;
    public KeyCode mouseLockKey;
    public bool isMouseLock;
    public float mouseSpeedModifier;
    //public float mouseSpeedModifierDrag;
    public float defaultSpeed;

    public OpenRightClick openRightClick;

    private void Update()
    {
        keySpeedModifier = defaultSpeed * ((cm.transform.position.z / 1000f) - 1);
        MoveCamera(InputOverride.init.GetAxis("Horizontal"), InputOverride.init.GetAxis("Vertical"), keySpeedModifier);

        if (!isMouseLock)
        {
            if (Input.mousePosition.x < 0 + boundary)
            {
                //Debug.Log("Moving left");
                MoveXCamera(-1);
            }

            if (Input.mousePosition.x > Screen.width - boundary)
            {
                //Debug.Log("Moving right");
                MoveXCamera(1);
            }

            if (Input.mousePosition.y > Screen.height - boundary)
            {
                //Debug.Log("Moving up");
                MoveYCamera(1);
            }

            if (Input.mousePosition.y < 0 + boundary)
            {
                //Debug.Log("Moving down");
                MoveYCamera(-1);
            }
        }

        if (InputOverride.init.GetKeyDown(mouseLockKey))
        {
            isMouseLock = !isMouseLock;
        }

    }

    /* public void OnDrag(PointerEventData eventData)
     {
         if ((Input.GetButton("Fire1") || Input.GetButton("Fire3")) && isMouseLock)
         {
             //float speed = defaultSpeed * Time.deltaTime * 100;
             //MoveCamera(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), speed);
         }
     }*/


    private void MoveCamera(float x, float y, float speed)
    {

        float xx = cm.transform.position.x - x * speed;
        float yy = cm.transform.position.y - y * speed;

        cm.transform.position = new Vector3(xx, yy, cm.transform.position.z);
        //openRightClick.ResetValues();
    }

    private void MoveXCamera(float direction)
    {

        float speed = Time.deltaTime * defaultSpeed * mouseSpeedModifier;

        cm.transform.position = new Vector3(direction > 0 ? (cm.transform.position.x + speed) : (cm.transform.position.x - speed), cm.transform.position.y, cm.transform.position.z);
        openRightClick.ResetValues();
    }

    private void MoveYCamera(float direction)
    {

        float speed = Time.deltaTime * defaultSpeed * mouseSpeedModifier;

        cm.transform.position = new Vector3(cm.transform.position.x, direction > 0 ? (cm.transform.position.y + speed) : (cm.transform.position.y - speed), cm.transform.position.z);
        openRightClick.ResetValues();
    }

}