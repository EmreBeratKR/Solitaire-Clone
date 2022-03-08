using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCard : MonoBehaviour
{
    public Transform drawArea;
    [SerializeField] CardController cardController;
    [SerializeField] MoveHistory moveHistory;
    [SerializeField] SceneController sceneController;
    public bool isDrawing = false;



    public void draw()
    {
        if (!isDrawing && !cardController.isWin && (drawArea.childCount != 0 || transform.childCount != 0))
        {
            isDrawing = true;
            sceneController.updateMoveCounter(1);
            StartCoroutine(drawCoroutine());
        }
    }
    IEnumerator drawCoroutine()
    {
        if (transform.childCount != 0)
        {
            int countOfDraw = 1;
            float offset = 0;
            List<Transform> dealedCards = new List<Transform>();
            if (cardController.gameMode == "Hard")
            {
                countOfDraw = 3;
                if (transform.childCount < 3)
                {
                    countOfDraw = transform.childCount;
                }
            }

            for (int i = 1; i <= countOfDraw; i++)
            {
                dealedCards.Add(transform.GetChild(transform.childCount-i));
            }
            moveHistory.addMove("deal card", dealedCards, transform, null);

            if (countOfDraw > transform.childCount)
            {
                countOfDraw = transform.childCount;
            }
            if (countOfDraw == 1)
            {   
                List<Transform> lastTwoCards = new List<Transform>();
                bool isOverFlow = false;
                if (drawArea.childCount < 3)
                {
                    offset = cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y * drawArea.childCount;
                }
                if (drawArea.childCount >= 3)
                {
                    isOverFlow = true;
                    lastTwoCards.Add(drawArea.GetChild(drawArea.childCount-2));
                    lastTwoCards.Add(drawArea.GetChild(drawArea.childCount-1));
                    offset = cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y * 2;
                }
                Transform temp = transform.GetChild(transform.childCount-1);
                Vector3 moveVector = drawArea.position + (Vector3.down * offset) - temp.position;
                cardController.openCard(temp);
                temp.SetParent(drawArea);
                for (int i = 0; i < 20; i++)
                {
                    temp.position += moveVector / 20;
                    yield return 0;
                }
                if (isOverFlow)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        lastTwoCards[0].position += Vector3.up * (cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10);
                        lastTwoCards[1].position += Vector3.up * (cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10);
                        yield return 0;
                    }
                }
            }
            else
            {
                List<Vector3> moveVectors = new List<Vector3>();
                List<Transform> newCards = new List<Transform>();
                List<Transform> overFlows = new List<Transform>();
                List<Vector3> overFlowsPos = new List<Vector3>();
                if (drawArea.childCount > 1)
                {
                    if (drawArea.childCount == 2)
                    {
                        overFlows.Add(drawArea.GetChild(drawArea.childCount-1));
                        overFlowsPos.Add(drawArea.GetChild(drawArea.childCount-1).position);
                    }
                    else if (drawArea.childCount > 2)
                    {
                        overFlows.Add(drawArea.GetChild(drawArea.childCount-1));
                        overFlowsPos.Add(drawArea.GetChild(drawArea.childCount-1).position);
                        overFlows.Add(drawArea.GetChild(drawArea.childCount-2));
                        overFlowsPos.Add(drawArea.GetChild(drawArea.childCount-2).position);
                        
                    }
                }
                float tempOffset = cardController.stackOffset * 2f * cardController.GetComponent<RectTransform>().lossyScale.y;
                for (int i = countOfDraw; i >= 1; i--)
                {
                    cardController.openCard(transform.GetChild(transform.childCount-i));
                    moveVectors.Add(drawArea.position - transform.GetChild(transform.childCount-i).position + Vector3.down * tempOffset);
                    newCards.Add(transform.GetChild(transform.childCount-i));
                    tempOffset -= cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y;
                }
                for (int i = countOfDraw - 1; i >= 0 ; i--)
                {
                    newCards[i].SetParent(drawArea);
                }
                for (int i = 0; i < 20; i++)
                {
                    for (int o = 0; o < overFlows.Count; o++)
                    {
                        overFlows[o].position += (drawArea.position - overFlowsPos[o]) / 20f;
                    }
                    for (int c = 0; c < countOfDraw; c++)
                    {
                        newCards[c].position += moveVectors[c] / 20f;
                    }
                    yield return 0;
                }
            }
        }
        else
        {
            float resetDuration = 0.25f;
            float offset = 0f;
            moveHistory.addMove("reset deck", new List<Transform>(), null, null);
            for (int i = drawArea.childCount-1; i >= 0; i--)
            {
                cardController.closeCard(drawArea.GetChild(i));
                drawArea.GetChild(i).SetParent(transform);
                transform.GetChild(transform.childCount-1).LeanMoveLocalY(offset, resetDuration);
                offset -= cardController.deckOffset;
            }
            yield return new WaitForSeconds(resetDuration);
        }
        isDrawing = false;
    }
}
