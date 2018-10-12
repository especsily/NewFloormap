using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using ProBuilder2.Examples;

public class UIManager : MonoBehaviour, IAnimationUI
{
    public ISpreadSheet spreadsheetManager;
    public INavigation navigationManager;

    [Header("Set up list")]
    [SerializeField] private List<RoomInfoCanvas> listRoomInfo;
    // private List<RoomInfoCanvas> listShowingRoomInfo;

    private List<List<RoomData>> listData; // truy cap theo cau truc [row][col] bat dau tu row 0 va col 0

    [Header("Set up UI components")]
    /*
        illiat - mau do
        techkids - mau xanh nuoc bien
        cho thue - mau da cam
        empty - mau ghi 
    */
    [SerializeField] private GameObject roomsCanvas;
    [SerializeField] private RectTransform roomInfoCanvas, listInfoCanvas;
    [SerializeField] private Image infoIcon;
    [SerializeField] private Color iliatColor, techkidsColor, hiredColor, availableColor;
    [SerializeField] private GameObject roomInfoPrefab;

    private bool isShowingButtons = true;
    private bool isShowingInfo = false;
    private bool isInfoClicked = false;
    private string lastID = "";

    [Header("Set up time")]
    [SerializeField] private Text time;
    [SerializeField] private float morningTime, afternoonTime;

    [Header("Camera animation")]
    [SerializeField] private float cameraAnimTime;
    [SerializeField] private Vector3 cameraStartRotation;
    [SerializeField] private float cameraFinishRotation;
    [SerializeField] private float cameraStartDistance;
    [SerializeField] private float cameraFinishDistance;
    [SerializeField] private Image startPanel;
    [SerializeField] private float startPanelFadeTime;
    [SerializeField] private float pivotStartRotationY;
    private GameObject cameraPivot;

    void Start()
    {
        startPanel.color = Utilities.ChangeColorAlpha(startPanel.color, 1);
        cameraPivot = Camera.main.GetComponent<CameraController>().targetPoint;

        //set up list
        listData = new List<List<RoomData>>();

        //set up data
        StartCoroutine(SetupData());
    }

    void Update()
    {
        time.text = DateTime.Now.ToString(@"H\:mm");
    }
    // -------------------------------- Animations UI -------------------------------------
    public void HideAllRoomInfo()
    {
        for (int i = 0; i < listRoomInfo.Count; i++)
        {
            if (listRoomInfo[i].isShowingInfoFrame())
            {
                listRoomInfo[i].HideInfoFrame();
                break;
            }
        }
    }

    public IEnumerator StartCameraUI()
    {
        Camera.main.transform.eulerAngles = cameraStartRotation;
        // startRotationY = Camera.main.transform.eulerAngles.y;
        var cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.idleRotation = 0;
        cameraController.distance = cameraStartDistance;

        // cameraController.targetPoint.transform.rotation = Quaternion.Euler(0, pivotStartRotationY, 0);

        var rotationY = pivotStartRotationY;
        DOTween.To(() => rotationY, x => rotationY = x, cameraFinishRotation, cameraAnimTime);

        startPanel.DOColor(Utilities.ChangeColorAlpha(startPanel.color, 0), startPanelFadeTime).SetDelay(0.3f);

        var distance = cameraController.distance;
        DOTween.To(() => distance, x => distance = x, cameraFinishDistance, cameraAnimTime)
        .OnComplete(() =>
        {
            cameraController.isStarted = true;
            startPanel.gameObject.SetActive(false);
            AppearFromCenter(cameraController.targetPoint.transform.position);
        });

        while (true)
        {
            cameraPivot.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
            cameraController.distance = distance;
            if (rotationY >= cameraFinishRotation)
            {
                break;
            }
            yield return null;
        }
    }

    public void ClickRoomInfo(string id)
    {
        if (!isInfoClicked)
        {
            DisappearFromCenter(listRoomInfo.Where(x => x.roomID == id).FirstOrDefault().target.transform.position);
            isInfoClicked = true;
            lastID = id;
        }
        else
        {
            var last = listRoomInfo.Where(x => x.roomID == lastID).FirstOrDefault();
            last.GetComponent<RoomInfoCanvas>().iconFrame.transform.DOScale(new Vector2(0, 0), 0.2f)
            .OnComplete(() => last.target.gameObject.SetActive(false));
            var current = listRoomInfo.Where(x => x.roomID == id).FirstOrDefault();
            current.GetComponent<RoomInfoCanvas>().iconFrame.transform.DOScale(new Vector2(1, 1), 0.2f)
            .OnComplete(() => current.target.gameObject.SetActive(true));
            lastID = id;
        }
    }

    public void ReappearAllRoomInfo()
    {
        if (isInfoClicked)
        {
            AppearFromCenter(listRoomInfo.Where(x => x.roomID == lastID).FirstOrDefault().target.transform.position);
        }
        isInfoClicked = false;
    }

    public void AppearFromCenter(Vector3 center)
    {
        const float maxDis = 110;
        foreach (var roomInfo in listRoomInfo)
        {
            roomInfo.iconFrame.transform.DOScale(new Vector2(1, 1), 0.5f)
            .SetEase(Ease.OutBack, 2.5f)
            .SetDelay(Mathfx.Sinerp(0, 1, Vector3.Distance(roomInfo.target.transform.position, center) / maxDis))
            .OnStart(() => roomInfo.target.gameObject.SetActive(true));
        }
    }

    public void DisappearFromCenter(Vector3 center)
    {
        const float maxDis = 90;
        foreach (var roomInfo in listRoomInfo)
        {
            if (Vector3.Distance(roomInfo.target.transform.position, center) != 0)
            {
                roomInfo.iconFrame.transform.DOScale(new Vector2(0, 0), 0.3f)
                .SetEase(Ease.InBack, 2.5f)
                .SetDelay(Mathfx.Coserp(0, 1, (maxDis - Vector3.Distance(roomInfo.target.transform.position, center)) / maxDis))
                .OnComplete(() => roomInfo.target.gameObject.SetActive(false));
            }
        }
    }
    //---------------------------------------------------------------------------------

    // -------------------------------- Set up UI -------------------------------------
    void SetupRoomInfo()
    {
        if (listData.Count > 0)
        {
            for (int i = 0; i < listRoomInfo.Count; i++)
            {
                listRoomInfo[i].navigationManager = this.navigationManager;
                listRoomInfo[i].UIManager = this;

                string info = "";
                string roomID = "";
                GameObject roomInfo = null;

                //get info by roomID
                if (listRoomInfo[i].roomID.Trim().Length >= 4 && listRoomInfo[i].roomID.Trim().Substring(0, 4) == "Room")
                {
                    if (listRoomInfo[i].roomID.Trim().Length == 5)
                        roomID = listRoomInfo[i].roomID.Trim().Substring(listRoomInfo[i].roomID.Trim().Length - 1);
                    else
                        roomID = listRoomInfo[i].roomID.Trim().Substring(listRoomInfo[i].roomID.Trim().Length - 2);

                    info = listData[GetRowByTime()].Where(x => x.id == roomID).FirstOrDefault().info;
                    roomInfo = CreateRoomInfo(listInfoCanvas.transform, roomInfoPrefab, info, roomID);
                    SetupColor(listRoomInfo[i], roomID, info, roomInfo);
                }
                else if (listRoomInfo[i].roomID == "Cafeteria")
                {
                    roomID = listRoomInfo[i].roomID;
                    info = listData[GetRowByTime()].Where(x => x.id == "Cafeteria").FirstOrDefault().info;
                    roomInfo = CreateRoomInfo(listInfoCanvas.transform, roomInfoPrefab, info, roomID);
                    SetupColor(listRoomInfo[i], roomID, info, roomInfo);
                }
                else if (listRoomInfo[i].roomID == "OpenSpace")
                {
                    roomID = listRoomInfo[i].roomID;
                    info = listData[GetRowByTime()].Where(x => x.id == "OpenSpace").FirstOrDefault().info;
                    roomInfo = CreateRoomInfo(listInfoCanvas.transform, roomInfoPrefab, info, roomID);
                    SetupColor(listRoomInfo[i], roomID, info, roomInfo);
                }

                if (roomInfo != null)
                    roomInfo.GetComponent<RoomInfo>().id = listRoomInfo[i].roomID;
            }
        }
    }

    IEnumerator SetupData()
    {
        //lay spreadsheet ngay hom nay
        yield return new WaitUntil(() => spreadsheetManager.GetSpreadSheetData() != null);
        listData = spreadsheetManager.GetSpreadSheetData();
        SetupRoomInfo();
        StartCoroutine(StartCameraUI());
    }

    GameObject CreateRoomInfo(Transform parent, GameObject roomInfoPrefab, string info, string roomID)
    {
        var temp = Instantiate(roomInfoPrefab, Vector3.zero, Quaternion.identity, parent);

        //set up order text
        if (roomID == "Cafeteria")
        {
            temp.GetComponent<RoomInfo>().orderText.text = "CF";
        }
        else if (roomID == "OpenSpace")
        {
            temp.GetComponent<RoomInfo>().orderText.text = "OS";
        }
        else if (roomID != "")
        {
            temp.GetComponent<RoomInfo>().orderText.text = roomID;
        }

        //set up info text
        if (info != "")
        {
            temp.GetComponent<RoomInfo>().infoText.text = info.Trim();
        }
        else
        {
            temp.GetComponent<RoomInfo>().infoText.text = "Chưa cho thuê";
        }
        temp.GetComponent<RoomInfo>().UImanager = this;
        return temp;
    }

    void SetupColor(RoomInfoCanvas room, string roomID, string info, GameObject roomInfoGO)
    {
        var roomInfo = roomInfoGO.GetComponent<RoomInfo>();
        if (room != null && info != "")
        {
            if (listData[GetNoteRowByTime()]
            .Where(x => x.id.Trim().ToLower() == roomID.Trim().ToLower())
            .FirstOrDefault().info
            .Trim().Contains("Illiat"))
            {
                SetColor(roomInfo, room, iliatColor);
                return;
            }
            else if (listData[GetNoteRowByTime()]
            .Where(x => x.id.Trim().ToLower() == roomID.Trim().ToLower())
            .FirstOrDefault().info
            .Trim().Contains("Techkids"))
            {
                SetColor(roomInfo, room, techkidsColor);
                return;
            }
            else
            {
                SetColor(roomInfo, room, hiredColor);
                return;
            }
        }
        else
        {
            if (roomID != "Cafeteria" && roomID != "OpenSpace")
            {
                // room.roomInfo.text = "\n\nPhòng trống cho thuê!";
                room.iconFrame.color = availableColor;
                room.iconFrame1.color = availableColor;
                room.iconFrameColor = availableColor;
                room.infoFrame.color = availableColor;
                room.infoTop.color = availableColor;
                room.infoBot.color = availableColor;
            }
            roomInfo.orderText.color = availableColor;
            roomInfo.infoText.color = availableColor;
            return;
        }
    }

    void SetColor(RoomInfo roomInfo, RoomInfoCanvas room, Color color)
    {
        room.iconFrame.color = color;
        room.iconFrame1.color = color;
        room.iconFrameColor = color;
        room.infoTop.color = color;
        room.infoBot.color = color;
        room.infoFrame.color = color;
        roomInfo.orderText.color = color;
    }

    int GetRowByTime()
    {
        //row 1 3 5 la row ghi chu
        if (DateTime.Now.Hour + DateTime.Now.Minute / 60 <= morningTime)
            return 0;
        else if (DateTime.Now.Hour + DateTime.Now.Minute / 60 <= afternoonTime)
            return 2;
        else
            return 4;
    }

    int GetNoteRowByTime()
    {
        //row 1 3 5 la row ghi chu
        if (DateTime.Now.Hour + DateTime.Now.Minute / 60 <= morningTime)
            return 1;
        else if (DateTime.Now.Hour + DateTime.Now.Minute / 60 <= afternoonTime)
            return 3;
        else
            return 5;
    }
    // --------------------------------------------------------------------------------


    // -------------------------------- Button methods -------------------------------------
    public void ToggleInstruction()
    {
        if (isShowingButtons)
        {
            isShowingButtons = false;
            for (int i = 0; i < listRoomInfo.Count; i++)
            {
                listRoomInfo[i].target.gameObject.SetActive(false);
            }
            roomsCanvas.gameObject.SetActive(false);
        }
        else
        {
            isShowingButtons = true;
            for (int i = 0; i < listRoomInfo.Count; i++)
            {
                listRoomInfo[i].target.gameObject.SetActive(true);
            }
            roomsCanvas.gameObject.SetActive(true);
        }
    }

    public void TurnOnListInfo()
    {
        HideAllRoomInfo();
        if (!isShowingInfo)
        {
            isShowingInfo = true;
            listInfoCanvas.gameObject.SetActive(true);
            cameraPivot.transform.DOMove(Utilities.CameraMoveToSide(cameraPivot.transform.position), 0.5f);
            roomInfoCanvas.DOAnchorPosX(0, 0.5f)
            .OnComplete(() =>
            {
                infoIcon.transform.localScale = new Vector2(1, 1);
            });
        }
        else
        {
            isShowingInfo = false;
            // Camera.main.GetComponent<CameraController>().targetPoint.transform.DOMove(new Vector3(-9, 0, 10), 0.5f);
            cameraPivot.transform.DOMove(Utilities.CameraMoveBackToCurrent(cameraPivot.transform.position), 0.5f);
            roomInfoCanvas.DOAnchorPosX(-404, 0.5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                listInfoCanvas.gameObject.SetActive(false);
                infoIcon.transform.localScale = new Vector2(-1, 1);
            });

            ReappearAllRoomInfo();
        }
    }
}
