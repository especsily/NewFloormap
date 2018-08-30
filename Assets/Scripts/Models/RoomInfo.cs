using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoomInfo : MonoBehaviour{
    public string roomID;
    //icon panel
    public Image iconFrame, icon;
    //icon in info panel
    public Image iconFrame1, icon1;
    public Image infoFrame;
    public Text roomName;
    public Text roomInfo;
    public RectTransform canvasRect;
	public GameObject target;
    private RectTransform rectTransform;

    [Header("Options")]
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private Color iconColor;
    [SerializeField] private string name;
    [SerializeField] private string info;

    //navigation manager
    public INavigation navigationManager;
    private Color iconFrameColor;

    void Start()
    {
        // roomID = transform.name;
        rectTransform = GetComponent<RectTransform>();

        //set up
        icon.sprite = iconSprite;
        icon1.sprite= iconSprite;
        iconFrame.color = iconColor;
        iconFrame1.color = iconColor;

        roomName.text = name;
        roomInfo.text = info;

        iconFrameColor = iconFrame.color;
        target.GetComponentInChildren<MeshRenderer>().sharedMaterial.color = iconColor;
    }

    public void MouseOver()
    {
        DOTween.CompleteAll();
        navigationManager.ShowNavigation(roomID);
        navigationManager.ShowHighlightFloor(roomID, iconColor);

        var sequence = DOTween.Sequence();
        //turn off icon
        sequence.Append(iconFrame.DOColor(new Color(0, 0, 0, 0), 0.2f));
        sequence.Join(icon.DOColor(new Color(0, 0, 0, 0), 0.2f));
        // sequence.Join(roomIcon.DOColor(new Color(0, 0, 0, 0), 0.2f));

        //turn on info
        sequence.AppendCallback(() => {
            infoFrame.gameObject.SetActive(true);
            transform.SetAsLastSibling();
        });
        sequence.Append(infoFrame.transform.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack, 3));
        sequence.Join(infoFrame.transform.DOMoveY(infoFrame.transform.position.y - 50, 0.2f).From());
        sequence.Join(infoFrame.DOColor(new Color(0,0,0,0), 0.2f).From());
        sequence.Append(infoFrame.transform.DOScaleX(1f, 0.2f).SetEase(Ease.OutBack, 3));
        sequence.Play();
    }

    public void MouseExit()
    {   
        DOTween.CompleteAll();
        navigationManager.StopNavigation();
        navigationManager.StopHighlightFloor();

        var sequence = DOTween.Sequence();
        sequence.Append(infoFrame.transform.DOScaleX(0.01f, 0.3f));
        sequence.Append(infoFrame.transform.DOScaleY(0, 0.5f));
        sequence.Join(infoFrame.DOColor(new Color(0, 0, 0, 0), 0.4f));

        sequence.Append(iconFrame.DOColor(iconFrameColor, 0.2f));
        sequence.Join(icon.DOColor(Color.white, 0.2f));
        // sequence.Join(roomIcon.DOColor(Color.white, 0.2f));

        sequence.OnComplete(() =>
        {
            infoFrame.gameObject.SetActive(false);
            transform.SetAsFirstSibling();            
            infoFrame.color = Color.white;
            infoFrame.transform.localScale = new Vector2(0.01f, 0.1f);
        });
        sequence.Play();
    }
    
    void LateUpdate()
    {
		// Get the position on the canvas
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(target.transform.position);
        Vector2 proportionalPosition = new Vector2(ViewportPosition.x * canvasRect.sizeDelta.x, ViewportPosition.y * canvasRect.sizeDelta.y);
        Vector2 uiOffset = new Vector2((float)canvasRect.sizeDelta.x / 2f, (float)canvasRect.sizeDelta.y / 2f);
        
		rectTransform.anchoredPosition = proportionalPosition - uiOffset + Vector2.up * -20;
    }
}
