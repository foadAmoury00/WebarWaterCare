using UnityEngine;
using MarksAssets.MotionSensorsWebGL;
using UnityEngine.Events;

public class RotateWebGL : MonoBehaviour {
    public UnityEvent onPermissionGranted;
    //public UnityEvent onPermissionDenied;
    public UnityEvent onNotAllowedError;
    //public UnityEvent onDeviceNotSendingDataError;
    //public UnityEvent onUnsupportedAPIError;
    //public UnityEvent onOtherError;
    public Transform target;
    void Start() {
        enabled = false;
        if (target == null)
		    target = transform;
        //PICK ONE OF THE SOLUTIONS FOR PERMISSION REQUEST AND REMOVE/COMMENT THE OTHERS

        /*
            //SOLUTION 1

            //Attempts to request permission to use gyroscope and accelerometer. As of the time of writing this will work on all browsers, except Safari. On Safari this will only work if the user granted permission before and the browser didn't forget about it.
            //You should always call requestPermission, even if the browser never requires permission request.
            //The method also checks if the API is available and the hardware is sending data. On a desktop for example you will get an error because there is no gyroscope/accelerometer.
            //The requestPermissionClbk will be called regardless if the browser actually requires permission or not(if you call requestPermission)
            //A much better solution would be to use the https://developer.mozilla.org/en-US/docs/Web/API/Permissions_API api, but as of the time of writing Safari doesn't support it. Using the local storage is also a bad idea because the permission can be revoked at any time and the local storage would be outdated.
            //This method will make the requestPermissionClbk method be called and pass the result, which can be a success or failure.
        */
        MotionSensorsWebGL.instance.requestPermission();

        
        /*
        //SOLUTION 2

        //The code below will check if requesting permission at least once is ever required to use the gyroscope/accelerometer.
        //I say at least once because after the user granted permission the first time, if he/she closes the browser and open again,
        //it won't be necessary to request permission again, unless the browser forgets it (by clearing the cache or using private mode for example)
        //you can use the code below to always show a button to request permission if the browser requires it at least once. As of the time of writing, Safari is the only browser that requires so.

        if (MotionSensorWebGL.instance.isRequestPermissionEverRequired())//if permission is required at least once, user needs to press a button on pointerdown to request permission
            permissionBtn.SetActive(true);
        else {//if permission request is not necessary, register both events
            MotionSensorWebGL.instance.requestPermission();//even if permission never required, call this method anyway. It also checks if the API is available and the hardware is sending data. On a desktop for example you will get an error because there is no gyroscope/accelerometer.
        }
        */

        /*
        //SOLUTION 3

        //The simplest solution. Just have the button to ask for permission always enabled, even if requesting permission is never required.
        //On the hierarchy there is a disabled button under the canvas. Enable it, and you're done.
        */
    }

    void Update() {
        target.rotation = new Quaternion(MotionSensorsWebGL.deviceOrientationEvent.x, MotionSensorsWebGL.deviceOrientationEvent.y, MotionSensorsWebGL.deviceOrientationEvent.z, MotionSensorsWebGL.deviceOrientationEvent.w);
    }

    //callback that is invoked from MotionSensorsWebGL after requestPermission() is called, passing the result of the permission request attempt.
    //this can be a success or failure.
    public void requestPermissionClbk(string result) {
        //ps: don't use result == or result.Equals. Some results have more information, like the NotAllowedError. Use 'Contains' instead.
        if (result.Contains("\"name\":\"PermissionState\",\"message\":\"Granted\"")) {//permission granted, accelerometer/gyroscope are ready to use. Register events
            onPermissionGranted.Invoke();
        } else if (result.Contains("\"name\":\"NotAllowedError\"")) {//This error happens if the user attempts to request permission without user interaction. As of the time of writing, Safari is the only browser that requires so. After the user grants permission, this error won't happen again until the browser forgets the user already granted permission(by clearing the cache, or using private mode, etc). You can remove this elseif if you didn't choose SOLUTION 1
            onNotAllowedError.Invoke();
        } else if (result.Contains("\"name\":\"PermissionState\",\"message\":\"Denied\"")) {//User explicitly denied permission. What will you do?
            //onPermissionDenied.Invoke();
        } else if (result.Contains("\"name\":\"DeviceNotSendingDataError\",\"message\":\"Data not being sent. Gyroscope/Accelerometer might not be present, malfunctioning, or disabled. Is this a desktop?\"")) {//hardware not sending data. Probably running on a desktop browser, or the gyroscope/accelerometer is disabled.
            //onDeviceNotSendingDataError.Invoke();
        } else if (result.Contains("\"name\":\"UnsupportedAPIError\",\"message\":\"Your browser does not support the \'DeviceOrientationEvent\' and/or \'DeviceMotionEvent\' APIs\"")) {//browser doesn't support accessing device's hardware.
            //onUnsupportedAPIError.Invoke();
        } else {//some other random error
            //onOtherError.Invoke();
        }
    }

    public void StartRotating() {
        MotionSensorsWebGL.instance.registerDeviceorientationEvent();
        enabled = true;
    }
    public void StopRotating() {
        enabled = false;
        MotionSensorsWebGL.instance.unregisterDeviceorientationEvent();
    }
}
