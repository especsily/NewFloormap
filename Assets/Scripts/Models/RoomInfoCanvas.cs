using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomInfoCanvas : MonoBehaviour
{
    public string roomID;
    //icon panel
    public Image iconFrame, icon;
    //icon in info panel
    public Image iconFrame1, icon1;
    public Image infoFrame;
    public GameObject infoIconPin, frame;
    public Image infoTop, infoBot;
    public Text roomName;
    public Text roomInfo;
    public RectTransform canvasRect;
    public GameObject target;
    private RectTransform rectTransform;

    [Header("Options")]
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private Color iconColor;
    [SerializeField] private string nameText;
    [SerializeField] private string infoText;
    public Color iconFrameColor;
    private bool isShowing = false;

    //navigation manager
    public INavigation navigationManager;

    void Awake()
    {
        roomID = transform.name;
        rectTransform = GetComponent<RectTransform>();

        //set up
        icon.sprite = iconSprite;
        icon1.sprite = iconSprite;
        iconFrame.color = iconColor;
        iconFrame1.color = iconColor;

        roomName.text = nameText;
        roomInfo.text = infoText;
        roomName.color = Color.white;
        roomInfo.color = Color.white;

        iconFrameColor = iconFrame.color;
        infoFrame.color = iconFrameColor;
        infoTop.color = iconFrameColor;
        infoBot.color = iconFrameColor;
    }

    public void MouseOver()
    {
        if (!isShowing)
        {
            isShowing = true;
            DOTween.CompleteAll();
            navigationManager.ShowNavigation(roomID);
            navigationManager.ShowHighlightFloor(roomID, iconColor);

            var sequence = DOTween.Sequence();

            //turn off icon
            // iconFrame.gameObject.SetActive(false);
            iconFrame.color = Utilities.ChangeColorAlpha(iconFrame.color, 0);
            icon.color = Utilities.ChangeColorAlpha(iconFrame.color, 0);

            //turn on info
            // frame.gameObject.SetActive(true);
            infoIconPin.gameObject.SetActive(true);
            transform.SetAsLastSibling();

            sequence.Append(infoFrame.transform.DOScaleX(1f, 0.3f).SetEase(Ease.Linear));
            sequence.Join(infoFrame.transform.DOScaleY(1f, 0.3f).SetEase(Ease.Linear));
            sequence.Join(infoTop.transform.DOLocalMoveY(74, 0.3f).SetEase(Ease.Linear));
            sequence.Join(iconFrame1.transform.DOLocalMoveY(74, 0.3f).SetEase(Ease.Linear));
            // sequence.Join(infoTop.DOColor(Color.white, 0.7f));
            // sequence.Join(infoBot.DOColor(Color.white, 0.3f));
            sequence.Play();
            sequence.OnComplete(() => isShowing = false);
        }
    }

    public void MouseExit()
    {
        if (!isShowing)
        {
            isShowing = true;
            DOTween.CompleteAll();
            navigationManager.StopNavigation();
            navigationManager.StopHighlightFloor();

            var sequence = DOTween.Sequence();
            sequence.Append(infoFrame.transform.DOScaleY(0, 0.32f).SetEase(Ease.Linear));
            sequence.Join(infoFrame.transform.DOScaleX(0, 0.32f).SetEase(Ease.Linear));
            sequence.Join(infoTop.transform.DOLocalMoveY(22.8f, 0.3f).SetEase(Ease.Linear));
            // sequence.Join(infoTop.DOColor(iconFrameColor, 0.3f));
            // sequence.Join(infoBot.DOColor(iconFrameColor, 0.3f));
            sequence.Join(iconFrame1.transform.DOLocalMoveY(22.8f, 0.3f).SetEase(Ease.Linear));

            sequence.OnComplete(() =>
            {
                isShowing = false;
                // frame.gameObject.SetActive(false);
                iconFrame.color = Utilities.ChangeColorAlpha(iconFrame.color, 1);
                icon.color = Color.white;

                infoIconPin.gameObject.SetActive(false);
                iconFrame.gameObject.SetActive(true);
                transform.SetAsFirstSibling();
            });
            sequence.Play();
        }
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
