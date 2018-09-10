using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FloorMapManager : MonoBehaviour
{
    public NavigationManager navigationManager;
    public SpreadSheetReader spreadsheetManager;
    public UIManager uiManager;
    
    void Awake()
    {
        BindUIManager();
    }

    void BindUIManager()
    {
        uiManager.spreadsheetManager = spreadsheetManager;
        uiManager.navigationManager = navigationManager;
    }
}
