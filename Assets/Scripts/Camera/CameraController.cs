/**
 * Camera orbit controls.
 */

using UnityEngine;
using System.Collections;
using UnityEditor;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
  const string INPUT_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
  const string INPUT_MOUSE_X = "Mouse X";
  const string INPUT_MOUSE_Y = "Mouse Y";

  // how fast the camera orbits
  [Range(2f, 15f)]
  [SerializeField] private float orbitSpeed = 6f;

  // how fast the camera zooms in and out
  [Range(.3f, 30f)]
  [SerializeField] private float zoomSpeed = .8f;

  [Header("Target locked!")]
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
  private Vector3 startCameraOffset;
  [SerializeField] private float startZoomOutTime;
  [SerializeField] private Vector3 offsetDistance;

  void Start()
  {
    startTargetPos = targetPoint.transform.position;
    startCameraOffset = transform.localPosition;
    DOTween.Sequence().Append(
      transform
        .DOLocalMove(transform.localPosition + transform.forward.ScaleTo(30), startZoomOutTime)
        .From()
        .SetEase(Ease.OutCubic)
        .OnComplete(() => isStarted = true)
    )
    .Join(
      targetPoint.transform.DORotate(new Vector3(0, 90f, 0), startZoomOutTime)
      .From()
      .SetEase(Ease.OutCubic)
    );
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
    if (!isStarted) return;

    Vector3 eulerRotation = targetPoint.transform.localRotation.eulerAngles;
    eulerRotation.z = 0f;

    // orbits
    if (Input.GetMouseButton(0))
    {
      float rot_x = Input.GetAxis(INPUT_MOUSE_X);
      float rot_y = -Input.GetAxis(INPUT_MOUSE_Y);

      eulerRotation.x += Mathf.Clamp(rot_y * orbitSpeed, MIN_Y_ROTATION, MAX_Y_ROTATION);
      eulerRotation.y += rot_x * orbitSpeed;

      targetPoint.transform.localRotation = Quaternion.Euler(eulerRotation);
    }
    // //drag camera
    // if (Input.GetMouseButton(1))
    // {
    //   mouseX = Input.GetAxis(INPUT_MOUSE_X);
    //   mouseY = Input.GetAxis(INPUT_MOUSE_Y);

    //   if (mouseX != 0)
    //   {
    //     offsetX = transform.right.normalized * dragSpeed * mouseX;
    //   }
    //   if (mouseY != 0)
    //   {
    //     float rotationY = transform.rotation.eulerAngles.y;
    //     //rotate vector
    //     offsetY = Quaternion.Euler(0, rotationY, 0) * targetPoint.transform.forward * dragSpeed * mouseY;
    //   }

    //   offset = offsetX + offsetY;
    //   if ((mouseX != 0 || mouseY != 0) && MouseScreenCheck() && Vector3.Distance(targetPoint.transform.position - offset, startTargetPos) < MAX_DRAG_DISTANCE)
    //   {
    //     offset.y = 0;
    //     targetPoint.transform.position -= offset;
    //   }

    //   transform.localRotation = Quaternion.Euler(eulerRotation);
    //   if (!isLockedToTarget)
    //     transform.position = transform.localRotation * (Vector3.forward * -distance);
    //   else
    //     transform.position = targetPoint.transform.position + transform.localRotation * (Vector3.forward * -distance);
    // }

    //update the pos and rotation y base


    //zoom in/ zoom out
    if (Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL) != 0f)
    {
      float delta = Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL);
      float currDistance = transform.localPosition.magnitude;
      if (
        (delta > 0 && currDistance > MIN_CAM_DISTANCE)
        || (delta < 0 && currDistance < MAX_CAM_DISTANCE)
      )
      {
        transform.position += transform.forward * delta * zoomSpeed;
      }
    }
  }

  public void ChangeOffsetMode(bool newOffsetMode)
  {
    transform.DOLocalMove(
      startCameraOffset + (newOffsetMode ? offsetDistance : Vector3.zero),
      0.5f
    ).SetEase(Ease.InOutCubic);
  }
}