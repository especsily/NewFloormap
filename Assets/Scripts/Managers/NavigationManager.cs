using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class NavigationManager : MonoBehaviour, INavigation
{
  [Header("Data config")]
  [SerializeField] private List<Waypoints> listPath;
  [SerializeField] private List<Floor> listFloor;
  private List<Floor> highlightFloors;
  private Material lastMaterial;

  [Header("Pointer")]
  [SerializeField] private GameObject beginArrow;
  [SerializeField] private bool isContinous;
  [SerializeField] private bool showPointer;
  [SerializeField] private GameObject pointerPrefab;
  [SerializeField] private float pointerSpeed;
  private float pointerTime;
  private bool isShowPointer = false;
  private List<GameObject> listPointer;

  [Header("Trail")]
  private float trailTime;
  // public float trailStartWidth;
  // public float trailEndWidth;
  private float timer;
  private string currentRoomID;
  [Header("Floor materials")]
  [SerializeField] private Material wood;
  [SerializeField] private Material denim_blue;
  [SerializeField] private Material tile;
  [SerializeField] private Material grass;

  void Awake()
  {
    timer = 0;
    listPointer = new List<GameObject>();

    if (isContinous)
    {
      pointerTime = 1.5f;
      trailTime = 2f;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (isShowPointer)
    {
      timer += Time.deltaTime;
      if (timer >= pointerTime)
      {
        timer = 0;
        CreateNewPointer(currentRoomID, pointerPrefab, trailTime);
      }
    }
  }
  float CalculatePathLength(Vector3[] points)
  {
    if (points.Length < 2)
      return 0;

    Vector3 previousCorner = points[0];
    float lengthSoFar = 0.0F;
    int i = 1;
    while (i < points.Length)
    {
      Vector3 currentCorner = points[i];
      lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
      previousCorner = currentCorner;
      i++;
    }
    return lengthSoFar;
  }

  void CreateNewPointer(string roomID, GameObject pointerPrefab, float trailTime)
  {
    Waypoints currentPath = listPath.Where(path => path != null && path.roomID.Trim() == roomID.Trim()).FirstOrDefault();

    if (currentPath != null)
    {
      Vector3[] points = currentPath.waypoints.Select(x => x.transform.position).ToArray();

      float pathLength = CalculatePathLength(points);

      if (!isContinous)
      {
        pointerTime = pathLength / pointerSpeed;
        trailTime = pathLength / 40;
      }

      bool needCreate = true;
      if (listPointer.Count > 0)
      {
        for (int i = 0; i < listPointer.Count; i++)
        {
          if (!listPointer[i].gameObject.activeSelf)
          {
            needCreate = false;
            listPointer[i].transform.position = points[0];
            listPointer[i].gameObject.SetActive(true);

            var trail = listPointer[i].GetComponent<Pointer>().trail;
            trail.time = trailTime;

            if (showPointer)
            {
              listPointer[i].GetComponent<Pointer>().arrowSprite.enabled = true;
            }

            listPointer[i].transform.DOPath(points, pathLength / pointerSpeed, PathType.Linear, PathMode.Full3D)
            .SetEase(Ease.Linear)
            .SetLookAt(0.01f)
            .OnComplete(() =>
            {
              listPointer[i].GetComponent<Pointer>().TurnOffMesh();
            });
            break;
          }
        }
      }

      if (needCreate)
      {
        GameObject pointer = Instantiate(pointerPrefab, points[0], Quaternion.identity, this.transform);

        if (!showPointer)
        {
          pointer.GetComponent<Pointer>().arrowSprite.enabled = false;
        }

        pointer.transform.DOPath(points, pathLength / pointerSpeed, PathType.Linear, PathMode.Full3D)
        .SetEase(Ease.Linear)
        .SetLookAt(0.01f)
        .OnComplete(() =>
        {
          pointer.GetComponent<Pointer>().TurnOffMesh();
        });

        var trail = pointer.GetComponent<Pointer>().trail;
        trail.time = trailTime;

        listPointer.Add(pointer);
      }
    }
  }

  public void StopNavigation()
  {
    beginArrow.gameObject.SetActive(true);
    isShowPointer = false;
    for (int i = 0; i < listPointer.Count; i++)
    {
      if (listPointer[i].activeSelf)
      {
        listPointer[i].gameObject.SetActive(false);
        listPointer[i].GetComponent<Pointer>().trail.Clear();
      }
    }
  }

  public void ShowNavigation(string roomID)
  {
    beginArrow.gameObject.SetActive(false);
    isShowPointer = true;
    timer = 0;
    currentRoomID = roomID;
    CreateNewPointer(roomID, pointerPrefab, trailTime);
  }

  public void ShowHighlightFloor(string roomID, Color floorColor)
  {
    highlightFloors = listFloor.Where(x => x.roomID.Trim() == roomID.Trim()).ToList();
    if (highlightFloors.Count > 0)
    {
      lastMaterial = highlightFloors[0].GetComponent<Floor>().mesh.sharedMaterial;
      for (int i = 0; i < highlightFloors.Count; i++)
      {
        var floor = highlightFloors[i].GetComponent<Floor>();

        Material mt = null;
        if (roomID.Contains("Room") || roomID.Contains("OpenSpace"))
        {
          mt = denim_blue;
          mt.SetColor("_EmissionColor", Color.green);
        }
        else if (roomID.Contains("Balcony"))
        {
          mt = grass;
          mt.SetColor("_EmissionColor", new Color(0, 80f / 255f, 0));
        }
        else if (roomID.Contains("WC"))
        {
          mt = tile;
          mt.SetColor("_EmissionColor", new Color(0, 80f / 255f, 0));
        }
        else if (roomID.Contains("Cafeteria"))
        {
          mt = wood;
          mt.SetColor("_EmissionColor", new Color(0, 120f / 255f, 0));
        }

        if (mt != null)
        {
          mt.color = Color.white;
          mt.EnableKeyword("_EMISSION");
          floor.mesh.sharedMaterial = mt;
        }
      }
    }
  }

  public void StopHighlightFloor()
  {
    if (highlightFloors != null)
    {
      for (int i = 0; i < highlightFloors.Count; i++)
      {
        highlightFloors[i].GetComponent<Floor>().mesh.sharedMaterial = lastMaterial;
      }
    }
  }
}
