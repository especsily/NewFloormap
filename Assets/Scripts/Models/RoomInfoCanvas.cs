using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;

public class RoomInfoCanvas : MonoBehaviour
{
  [Header("Components")]
  [HideInInspector]
  public string roomID;
  public Text roomName;
  public Text roomInfo;
  public RectTransform canvasRect;
  public GameObject target;
  private RectTransform rectTransform;
  [Header("Icon pin")]
  public Image iconFrame;
  public Image icon;
  public Text iconText;

  [Header("Icon in frame")]
  public Text iconText1;
  public Image iconFrame1, icon1;
  public Image infoFrame;
  public GameObject infoIconPin, frame;
  public Image infoTop, infoBot;

  [Header("Options")]
  [SerializeField] private Sprite iconSprite;
  [SerializeField] private Color iconColor;
  [SerializeField] private string nameText;
  public string infoText;

  [Header("Info text")]
  public string area;
  public string numberOfPeople;
  public string equipments;

  [HideInInspector]
  public Color iconFrameColor;
  private bool isShowing = false;

  //navigation manager
  public INavigation navigationManager;
  public IAnimationUI UIManager;

  //sequence
  private Sequence showInfoSequence;
  private Sequence hideInfoSequence;

  void Awake()
  {
    DOTween.SetTweensCapacity(500, 125);
    roomID = transform.name;
    rectTransform = GetComponent<RectTransform>();

    //setup sequence
    showInfoSequence = DOTween.Sequence();
    showInfoSequence.SetAutoKill(false);
    float posY;
    if (roomID.Contains("Room") && roomID != "RestRoom")
      posY = 122f;
    else
      posY = 74f;

    showInfoSequence.Append((infoTop.transform as RectTransform).DOAnchorPosY(posY, 0.15f));
    showInfoSequence.Join((iconFrame1.transform as RectTransform).DOAnchorPosY(posY, 0.15f));
    showInfoSequence.Join(infoFrame.transform.DOScaleY(1f, 0.15f));
    showInfoSequence.Join(infoFrame.transform.DOScaleX(1f, 0.15f));

    hideInfoSequence = DOTween.Sequence();
    hideInfoSequence.SetAutoKill(false);
    hideInfoSequence.Append(infoFrame.transform.DOScaleY(0, 0.15f));
    hideInfoSequence.Join(infoFrame.transform.DOScaleX(0, 0.15f));
    hideInfoSequence.Join((infoTop.transform as RectTransform).DOAnchorPosY(22.8f, 0.15f));
    hideInfoSequence.Join((iconFrame1.transform as RectTransform).DOAnchorPosY(22.8f, 0.15f));
    hideInfoSequence.OnComplete(() =>
    {
      iconFrame.gameObject.SetActive(true);
      frame.gameObject.SetActive(false);
      transform.SetAsFirstSibling();
    });

    //set up
    if (icon != null)
    {
      icon.sprite = iconSprite;
      icon1.sprite = iconSprite;
    }
    else
    {
      int number = int.Parse(transform.name.Substring(4));
      iconText.text = string.Format("{0:00}", number);
      iconText1.text = string.Format("{0:00}", number);
    }
    iconFrame.color = iconColor;
    iconFrame1.color = iconColor;

    if (roomID.Contains("Room") && roomID != "RestRoom")
      infoText = "- " + area + "\n"
      + "- " + numberOfPeople + "\n"
      + "- " + equipments;
    roomName.text = nameText;
    roomInfo.text = infoText;

    iconFrameColor = iconFrame.color;
    infoFrame.color = iconFrameColor;
    infoTop.color = iconFrameColor;
    infoBot.color = iconFrameColor;

  }

  void SetupButtonColor()
  {
    var button = GetComponentInChildren<Button>();
    ColorBlock colorVar = button.colors;
    colorVar.normalColor = iconFrameColor;
    colorVar.highlightedColor = Utilities.ChangeHighlightColor(colorVar.normalColor);
    button.colors = colorVar;
  }

  public bool isShowingInfoFrame()
  {
    return isShowing;
  }

  public void ShowInfoFrame()
  {
    DOTween.CompleteAll();
    // UIManager.HideAllRoomInfo();

    // isShowing = true;


    // //turn off icon
    // iconFrame.gameObject.SetActive(false);

    // // turn on info
    // frame.gameObject.SetActive(true);
    // transform.SetAsLastSibling();

    // showInfoSequence.Restart();

    navigationManager.ShowNavigation(roomID);
    navigationManager.ShowHighlightFloor(roomID, iconColor);
  }

  public void OnClick()
  {
    UIManager.HideAllRoomInfo();
    UIManager.ClickRoomInfo(roomID);
  }

  public void HideInfoFrame()
  {
    DOTween.CompleteAll();
    // isShowing = false;
    navigationManager.StopNavigation();
    navigationManager.StopHighlightFloor();

    // hideInfoSequence.Restart();
  }

  void LateUpdate()
  {
    // Get the position on the canvas
    Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.transform.position);
    Vector2 proportionalPosition = new Vector2(ViewportPosition.x * canvasRect.sizeDelta.x, ViewportPosition.y * canvasRect.sizeDelta.y);
    Vector2 uiOffset = new Vector2((float)canvasRect.sizeDelta.x / 2f, (float)canvasRect.sizeDelta.y / 2f);

    rectTransform.anchoredPosition = proportionalPosition - uiOffset + Vector2.up * 30;
  }
}
