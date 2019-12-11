using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirection : MonoBehaviour {
    public Camera cam;

    public void Awake() {
        cam = FindObjectOfType<Camera>();
    }
    public Vector3 GetCameraForward() {
        Vector3 F = cam.transform.forward;
        F.y = 0;
        return F.normalized;
    }

    public Vector3 GetCameraRight() {
        Vector3 R = cam.transform.right;
        R.y = 0;
        return R.normalized;
    }

    public Vector3 GetCameraUp() {
        Vector3 U = cam.transform.up;
        U.y = 0;
        return U.normalized;
    }
}