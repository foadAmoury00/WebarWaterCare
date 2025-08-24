using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Events;

namespace MarksAssets.MotionSensorsWebGL {
    [DisallowMultipleComponent]
    public class MotionSensorsWebGL : MonoBehaviour {

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceMotionEvent {
            public struct Acceleration {[MarshalAs(UnmanagedType.R4)] public float x, y, z;}
            public struct AccelerationIncludingGravity {[MarshalAs(UnmanagedType.R4)]public float x, y, z;}
            public struct RotationRate {[MarshalAs(UnmanagedType.R4)]public float alpha, beta, gamma;}
            public Acceleration acceleration;
            public AccelerationIncludingGravity accelerationIncludingGravity;
            public RotationRate rotationRate;
            [MarshalAs(UnmanagedType.R4)] public float interval;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceOrientationEvent {
            [MarshalAs(UnmanagedType.R4)] public float alpha;
            [MarshalAs(UnmanagedType.R4)] public float beta;
            [MarshalAs(UnmanagedType.R4)] public float gamma;
            [MarshalAs(UnmanagedType.R4)] public float webkitCompassHeading;
            [MarshalAs(UnmanagedType.R4)] public float webkitCompassAccuracy;
            [MarshalAs(UnmanagedType.R4)] public float x;
            [MarshalAs(UnmanagedType.R4)] public float y;
            [MarshalAs(UnmanagedType.R4)] public float z;
            [MarshalAs(UnmanagedType.R4)] public float w;
            [MarshalAs(UnmanagedType.U1)] public bool absolute;
        }
    
        [Serializable] public class UnityEventString : UnityEvent<string>{};
        public UnityEventString                 permissionRequest;

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal", EntryPoint="MotionSensorsWebGL_setUnityFunctions")]
        private static extern void setUnityFunctions(Action<string> requestPermissionClbks);
        
        [DllImport("__Internal", EntryPoint="MotionSensorsWebGL_registerDevicemotionEvent")]
        private static extern void MotionSensorsWebGL_registerDevicemotionEvent(ref DeviceMotionEvent deviceMotionEvent);
        [DllImport("__Internal", EntryPoint="MotionSensorsWebGL_unregisterDevicemotionEvent")]
        private static extern void MotionSensorsWebGL_unregisterDevicemotionEvent();
        
        [DllImport("__Internal", EntryPoint="MotionSensorsWebGL_registerDeviceorientationEvent")]
        private static extern void MotionSensorsWebGL_registerDeviceorientationEvent(ref DeviceOrientationEvent deviceOrientationEvent);
        [DllImport("__Internal", EntryPoint="MotionSensorsWebGL_unregisterDeviceorientationEvent")]
        private static extern void MotionSensorsWebGL_unregisterDeviceorientationEvent();
        [DllImport("__Internal", EntryPoint="MotionSensorsWebGL_isRequestPermissionEverRequired")]
        private static extern bool MotionSensorsWebGL_isRequestPermissionEverRequired();
        [DllImport("__Internal", EntryPoint="MotionSensorsWebGL_requestPermission")]
        private static extern void MotionSensorsWebGL_requestPermission(bool userInteraction);
        #endif


        public static DeviceMotionEvent        deviceMotionEvent;
        public static DeviceOrientationEvent   deviceOrientationEvent;

        private static MotionSensorsWebGL        m_Instance             = null;
        public static MotionSensorsWebGL  instance { get => m_Instance; }

        void Awake() {
            if (m_Instance != null && m_Instance != this) {
                Destroy(this.gameObject);
            } else {
                m_Instance = this;
            }

            #if UNITY_WEBGL && !UNITY_EDITOR
			setUnityFunctions(requestPermissionClbks);
			#endif
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void requestPermissionClbks(string result) {
            m_Instance.permissionRequest.Invoke(result);
        }

        public void registerDevicemotionEvent() {
            #if UNITY_WEBGL && !UNITY_EDITOR
            MotionSensorsWebGL_registerDevicemotionEvent(ref deviceMotionEvent);
            #endif
        }

        public void unregisterDevicemotionEvent() {
            #if UNITY_WEBGL && !UNITY_EDITOR
            MotionSensorsWebGL_unregisterDevicemotionEvent();
            #endif
        }

        public void registerDeviceorientationEvent() {
            #if UNITY_WEBGL && !UNITY_EDITOR
            MotionSensorsWebGL_registerDeviceorientationEvent(ref deviceOrientationEvent);
            #endif
        }

        public void unregisterDeviceorientationEvent() {
            #if UNITY_WEBGL && !UNITY_EDITOR
            MotionSensorsWebGL_unregisterDeviceorientationEvent();
            #endif
        }

        public bool isRequestPermissionEverRequired() {
            #if UNITY_WEBGL && !UNITY_EDITOR
            return MotionSensorsWebGL_isRequestPermissionEverRequired();
            #else
            return false;
            #endif
        }

        public void requestPermission(bool userInteraction = false) {
            #if UNITY_WEBGL && !UNITY_EDITOR
            MotionSensorsWebGL_requestPermission(userInteraction);
            #endif
        }
    }
}
