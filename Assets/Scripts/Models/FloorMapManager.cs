using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FloorMapManager : MonoBehaviour
{
    public NavigationManager navigationManager;
    public List<RoomInfo> listRoomInfo;
    public GameObject rooms;
    private bool isShowingButtons = false;
    void Awake()
    {
        SetupRoomInfo();
    }

    void SetupRoomInfo()
    {
        for (int i = 0; i < listRoomInfo.Count; i++)
        {
            listRoomInfo[i].navigationManager = this.navigationManager;
        }
    }

    public void ToggleButtons()
    {
        if (!isShowingButtons)
        {
            isShowingButtons = true;
            for (int i = 0; i < listRoomInfo.Count; i++)
            {
                listRoomInfo[i].target.gameObject.SetActive(false);
            }
            rooms.gameObject.SetActive(false);
        }
        else if (isShowingButtons)
        {
            isShowingButtons = false;
            for (int i = 0; i < listRoomInfo.Count; i++)
            {
                listRoomInfo[i].target.gameObject.SetActive(true);
            }
            rooms.gameObject.SetActive(true);
        }
    }
}
