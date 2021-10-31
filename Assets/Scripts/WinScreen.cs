using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    public RectTransform banner;
    public CanvasGroup folds;
    public RectTransform[] texts;
    public RectTransform[] aces;
    public float bannerEaseTime;
    [SerializeField] float aceTargetY;
    [SerializeField] float aceEaseTime;


    public void bannerScale()
    {
        LeanTween.scaleX(banner.gameObject, 1f, bannerEaseTime);
    }

    public void aceMove(GameObject ace)
    {
        LeanTween.alphaCanvas(ace.GetComponent<CanvasGroup>(), 1f, aceEaseTime);
        LeanTween.moveLocalY(ace, aceTargetY, aceEaseTime).setEaseOutBack();
    }
}
