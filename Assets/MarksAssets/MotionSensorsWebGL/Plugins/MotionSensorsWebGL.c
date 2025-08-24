#include "emscripten.h"

void (*MotionSensorsWebGL_requestPermissionClbks_ref)(const char *);

void MotionSensorsWebGL_setUnityFunctions(void (*requestPermissionClbks) (const char *)) {
	if (MotionSensorsWebGL_requestPermissionClbks_ref) return;
	MotionSensorsWebGL_requestPermissionClbks_ref = requestPermissionClbks;
}

void EMSCRIPTEN_KEEPALIVE MotionSensorsWebGL_requestPermissionClbks(const char *result) {MotionSensorsWebGL_requestPermissionClbks_ref(result);}