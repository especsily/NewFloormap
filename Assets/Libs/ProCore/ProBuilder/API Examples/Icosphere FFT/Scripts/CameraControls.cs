/**
 * Camera orbit controls.
 */

using UnityEngine;
using System.Collections;

namespace ProBuilder2.Examples
{
    public class CameraControls : MonoBehaviour
    {
        // the current distance from pivot point (locked to Vector3.zero)
        public float distance = 0f;

        const string INPUT_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
        const string INPUT_MOUSE_X = "Mouse X";
        const string INPUT_MOUSE_Y = "Mouse Y";
        const float MIN_CAM_DISTANCE = 10f;
        const float MAX_CAM_DISTANCE = 200f;

        // how fast the camera orbits
        [Range(2f, 15f)]
        public float orbitSpeed = 6f;

        // how fast the camera zooms in and out
        [Range(.3f, 5f)]
        public float zoomSpeed = .8f;


        // how fast the idle camera movement is
        public float idleRotation = 1f;

        // private Vector2 dir = new Vector2(.8f, .2f);
        public Vector2 dir;

        [Header("Target locked!")]
        public bool isLockedToTarget;
        public bool isStarted = false;
        public GameObject targetPoint;

        void Start()
        {
            // if (isLockedToTarget)
            //     distance = Vector3.Distance(transform.position, targetPoint.transform.position);
            // else
            //     distance = Vector3.Distance(transform.position, Vector3.zero);
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
                eulerRotation.y += rot_x * orbitSpeed;

                // idle direction is derived from last user input.
                dir.x = rot_x;
                dir.y = rot_y;
                dir.Normalize();
            }
            else
            {
                eulerRotation.y += Time.deltaTime * idleRotation * dir.x;
                eulerRotation.x += Time.deltaTime * Mathf.PerlinNoise(Time.time, 0f) * idleRotation * dir.y;
            }

            //update the pos and rotation y base
            transform.localRotation = Quaternion.Euler(eulerRotation);
            if (!isLockedToTarget)
                transform.position = transform.localRotation * (Vector3.forward * -distance);
            else
                transform.position = targetPoint.transform.position + transform.localRotation * (Vector3.forward * -distance);

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
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                GetComponent<FreeCam>().enabled = true;
                GetComponent<CameraControls>().enabled = false;
            }
        }
    }
}