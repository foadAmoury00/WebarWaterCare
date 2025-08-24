using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class EventSystemForCamera : EventSystem
{

    /// <summary>
    /// This script For the Bage foucs after the camera alert to give the permission the page went out of focus this script prevent this
    /// </summary>
    /// <param name="hasFocus"></param>
    protected override void OnApplicationFocus(bool hasFocus)
    {
        // Do Nothing
    }

}
