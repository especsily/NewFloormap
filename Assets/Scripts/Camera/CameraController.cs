﻿/**
 * Camera orbit controls.
 */

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ProBuilder2.Examples
{
    public class CameraController : MonoBehaviour
    {
        public float distance = 0f;

        const string INPUT_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
        const string INPUT_MOUSE_X = "Mouse X";
        const string INPUT_MOUSE_Y = "Mouse Y";

        // how fast the camera orbits
        [Range(2f, 15f)]
        [SerializeField] private float orbitSpeed = 6f;

        // how fast the camera zooms in and out
        [Range(.3f, 5f)]
        [SerializeField] private float zoomSpeed = .8f;

        // how fast the camera move
        //[Range(1f, 10f)]
        [SerializeField] private float dragSpeed = 1f;

        // how fast the idle camera movement is
        [HideInInspector]
        public float idleRotation = 0f;

        // private Vector2 dir = new Vector2(.8f, .2f);
        [SerializeField] private Vector2 dir;

        [Header("Target locked!")]
        [SerializeField] private bool isLockedToTarget;
        public GameObject targetPoint;
        [HideInInspector]
        public bool isStarted = false;

        [Header("Camera options")]
        [SerializeField] private float MIN_CAM_DISTANCE = 10f;
        [SerializeField] private float MAX_CAM_DISTANCE = 200f;
        [SerializeField] private float MAX_Y_ROTATION = 80f;
        [SerializeField] private float MIN_Y_ROTATION = 20f;
        [SerializeField] private float MAX_DRAG_DISTANCE = 200f;

        //camera drag
        private Vector3 startTargetPos;
        private float mouseX, mouseY;
        private Vector3 offsetX, offsetY, offset;

        void Start()
        {
            startTargetPos = targetPoint.transform.position;
        }

        public bool MouseScreenCheck()
        {
#if UNITY_EDITOR
            if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Handles.GetMainGameViewSize().x - 1 || Input.mousePosition.y >= Handles.GetMainGameViewSize().y - 1)
            {
                return false;
            }
#else
            if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) {
                return false;
            }
#endif
            else
            {
                return true;
            }
        }

        void LateUpdate()
        {
            Vector3 eulerRotation = transform.localRotation.eulerAngles;
            eulerRotation.z = 0f;

            // orbits
            if (Input.GetMouseButton(0) && isStarted)
            {
                float rot_x = Input.GetAxis(INPUT_MOUSE_X);
                float rot_y = -Input.GetAxis(INPUT_MOUSE_Y);

                eulerRotation.x += rot_y * orbitSpeed;
                if (eulerRotation.x <= MIN_Y_ROTATION)
                    eulerRotation.x = MIN_Y_ROTATION;
                else if (eulerRotation.x >= MAX_Y_ROTATION)
                    eulerRotation.x = MAX_Y_ROTATION;
                eulerRotation.y += rot_x * orbitSpeed;

                // idle direction is derived from last user input.
                dir.x = rot_x;
                dir.y = rot_y;
                dir.Normalize();

                transform.localRotation = Quaternion.Euler(eulerRotation);
            }
            // else
            // {
            //     eulerRotation.y += Time.deltaTime * idleRotation * dir.x;
            //     eulerRotation.x += Time.deltaTime * Mathf.PerlinNoise(Time.time, 0f) * idleRotation * dir.y;
            // }
            if (!isLockedToTarget)
                transform.localPosition = transform.localRotation * (Vector3.forward * -distance);
            else
                transform.localPosition = transform.localRotation * (Vector3.forward * -distance);
            //drag camera
            if (Input.GetMouseButton(1) && isStarted)
            {
                mouseX = Input.GetAxis(INPUT_MOUSE_X);
                mouseY = Input.GetAxis(INPUT_MOUSE_Y);

                if (mouseX != 0)
                {
                    offsetX = transform.right.normalized * dragSpeed * mouseX;
                }
                if (mouseY != 0)
                {
                    float rotationY = transform.rotation.eulerAngles.y;
                    //rotate vector
                    offsetY = Quaternion.Euler(0, rotationY, 0) * targetPoint.transform.forward * dragSpeed * mouseY;
                }

                offset = offsetX + offsetY;
                if ((mouseX != 0 || mouseY != 0) && MouseScreenCheck() && Vector3.Distance(targetPoint.transform.position - offset, startTargetPos) < MAX_DRAG_DISTANCE)
                {
                    offset.y = 0;
                    targetPoint.transform.position -= offset;
                }

                transform.localRotation = Quaternion.Euler(eulerRotation);
                if (!isLockedToTarget)
                    transform.position = transform.localRotation * (Vector3.forward * -distance);
                else
                    transform.position = targetPoint.transform.position + transform.localRotation * (Vector3.forward * -distance);
            }

            //update the pos and rotation y base


            //zoom in/ zoom out
            if (Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL) != 0f && isStarted)
            {
                float delta = Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL);

                if (!isLockedToTarget)
                {
                    distance -= delta * (distance / MAX_CAM_DISTANCE) * (zoomSpeed * 1000) * Time.deltaTime;
                    distance = Mathf.Clamp(distance, MIN_CAM_DISTANCE, MAX_CAM_DISTANCE);
                    transform.position = transform.localRotation * (Vector3.forward * -distance);
                }
                else
                {
                    distance -= delta * (distance / MAX_CAM_DISTANCE) * (zoomSpeed * 1000) * Time.deltaTime;
                    distance = Mathf.Clamp(distance, MIN_CAM_DISTANCE, MAX_CAM_DISTANCE);
                    transform.position = targetPoint.transform.position + transform.localRotation * (Vector3.forward * -distance);
                }
            }
        }
    }
}