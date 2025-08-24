mergeInto(LibraryManager.library, {
	MotionSensorsWebGL_registerDevicemotionEvent: function (devicemotion) {
		Module['MotionSensorsWebGL'].devicemotionRef = Module['MotionSensorsWebGL'].devicemotionRef || devicemotion;
		Module['MotionSensorsWebGL'].devicemotionArr = Module['MotionSensorsWebGL'].devicemotionArr || new Float32Array(buffer, devicemotion, 10);
		window.addEventListener("devicemotion", Module['MotionSensorsWebGL'].devicemotion);
    },
	MotionSensorsWebGL_unregisterDevicemotionEvent: function () {
		window.removeEventListener("devicemotion", Module['MotionSensorsWebGL'].devicemotion);
		Module['MotionSensorsWebGL'].devicemotionRef = Module['MotionSensorsWebGL'].devicemotionArr = undefined;
    },
	MotionSensorsWebGL_registerDeviceorientationEvent: function (deviceorientation) {
		Module['MotionSensorsWebGL'].deviceorientationRef = Module['MotionSensorsWebGL'].deviceorientationRef || deviceorientation;
		Module['MotionSensorsWebGL'].deviceorientationArr = Module['MotionSensorsWebGL'].deviceorientationArr || new Float32Array(buffer, deviceorientation, 9);
		Module['MotionSensorsWebGL'].deviceorientationLast= Module['MotionSensorsWebGL'].deviceorientationLast || new Uint8Array(buffer, deviceorientation + 36, 1);
		
		window.addEventListener("deviceorientation", Module['MotionSensorsWebGL'].deviceorientation);
    },
	MotionSensorsWebGL_unregisterDeviceorientationEvent: function () {
		window.removeEventListener("deviceorientation", Module['MotionSensorsWebGL'].deviceorientation);
		Module['MotionSensorsWebGL'].deviceorientationRef = Module['MotionSensorsWebGL'].deviceorientationArr = Module['MotionSensorsWebGL'].deviceorientationLast = undefined;
		Module['MotionSensorsWebGL'].deltaY = Module['MotionSensorsWebGL'].previousYaw = 0;
    },
	MotionSensorsWebGL_isRequestPermissionEverRequired: function() {
		if (typeof DeviceOrientationEvent.requestPermission === 'function')
			return true;
		else
			return false;
	},
	MotionSensorsWebGL_requestPermission: function(userInteraction) {
		function requestPermission() {
			Module['MotionSensorsWebGL'].requestPermission()
			.then(function (result) {return result;},function (err) {console.error(err);return err;})
			.then(function (result) {Module['MotionSensorsWebGL'].requestPermissionClbks(JSON.stringify(Object.getOwnPropertyNames(Object.getPrototypeOf(result)).reduce(function(accumulator, currentValue) { return accumulator[currentValue] = result[currentValue], accumulator}, {})));});
		}
		
		if (userInteraction)
			document.documentElement.addEventListener('pointerup', requestPermission, { once: true });
		else
			requestPermission();
	}
});