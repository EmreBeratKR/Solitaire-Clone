using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveHistory : MonoBehaviour
{
    public List<CardMove> moves = new List<CardMove>();
    [SerializeField] GameObject undoButton;
    [SerializeField] CardController cardController;
    [SerializeField] CardMovement cardMovement;
    [SerializeField] SceneController sceneController;
    [SerializeField] DrawCard drawCard;

    public void addMove(string moveType, List<Transform> cards, Transform target, Transform revCard)
    {
        CardMove temp = new CardMove();
        List<Transform> tempList = new List<Transform>();
        foreach (var card in cards)
        {
            tempList.Add(card);
        }
        temp.type = moveType;
        temp.cards = tempList;
        temp.target = target;
        temp.revealedCard = revCard;
        moves.Add(temp);
        if (moves.Count == 1)
        {
            activateUndo();
        }

        if (WinChecker.instance.isWin)
        {
            cardController.PlayWinAnimation();
        }
    }

    public void undoMove()
    {
        if (!cardMovement.coroutineRunning && !cardController.isWin)
        {
            cardMovement.coroutineRunning = true;
            CardMove lastMove = moves[moves.Count-1];
            if (lastMove.type == "drag" || lastMove.type == "new card" || lastMove.type == "deal card")
            {
                StartCoroutine(undoCoroutine(lastMove.type, lastMove.cards, lastMove.target, lastMove.revealedCard));
            }
            else if (lastMove.type == "reset deck")
            {
                StartCoroutine(undoResetDeck());
            }

            sceneController.updateMoveCounter(-1);
            moves.Remove(lastMove);
            if (moves.Count == 0)
            {
                deactivateUndo();
            }
        }
    }

    IEnumerator undoCoroutine(string moveType, List<Transform> cards, Transform target, Transform revCard)
    {
        if (revCard != null)
        {
            cardController.closeCard(revCard);
        }
        Vector3 offset = Vector3.down * target.childCount * cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y;
        if (target.name.Substring(0,5) == "Stack")
        {
            offset = Vector3.zero;
        }
        if (target.name == "Drawed Cards" && target.childCount >= 3)
        {
            offset = Vector3.down * 2 * cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y;
        }
        if (moveType == "deal card")
        {
            offset = Vector3.down * target.childCount * cardController.deckOffset * cardController.GetComponent<RectTransform>().lossyScale.y;
            foreach (var card in cards)
            {
                cardController.closeCard(card);
            }
        }
        Vector3 moveVector = target.position - cards[0].position + offset;
        foreach (var card in cards)
        {
            card.SetParent(card.GetComponent<Card>().draggingCanvas); 
        }
        List<Vector3> moveVectors = new List<Vector3>();
        for (int i = 0; i < 20; i++)
        {
            if (moveType == "deal card" && cards.Count == 3)
            {
                if (i == 0)
                {
                    foreach (var card in cards)
                    {
                        moveVectors.Add(target.position - card.position + Vector3.down * target.childCount * cardController.deckOffset * cardController.GetComponent<RectTransform>().lossyScale.y);
                    }
                }
                for (int c = 0; c < cards.Count; c++)
                {
                    cards[c].position += moveVectors[c] / 20;
                }
            }
            else
            {  
                foreach (var card in cards)
                {
                    card.position += moveVector / 20;  
                }
            }
            yield return 0;
        }
        if (moveType == "deal card")
        {
            for (int i = cards.Count - 1; i >= 0 ; i--)
            {
                cards[i].SetParent(target);
            }
        }
        else
        {
            foreach (var card in cards)
            {
                card.SetParent(target);   
            }
        }
        cardController.updateStacks();
        if (moveType == "new card")
        {
            if (target.childCount - cards.Count >= 3)
            {
                for (int i = 0; i < 10; i++)
                {
                    target.GetChild(target.childCount-2).position += Vector3.up * (cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10);
                    target.GetChild(target.childCount-3).position += Vector3.up * (cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10);
                    yield return 0;
                }
            }
        }
        if (moveType == "deal card")
        {
            if (drawCard.drawArea.childCount >= 3)
            {
                int count = 1;
                if (cards.Count == 3)
                {
                    count = 2;
                }
                for (int i = 0; i < 10; i++)
                {
                    drawCard.drawArea.GetChild(drawCard.drawArea.childCount-1).position += Vector3.down * count * (cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10);
                    drawCard.drawArea.GetChild(drawCard.drawArea.childCount-2).position += Vector3.down * (cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10);
                    yield return 0;
                }
            }
            else if (drawCard.drawArea.childCount == 2 && cards.Count > 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    drawCard.drawArea.GetChild(drawCard.drawArea.childCount-1).position += Vector3.down * (cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10);
                    yield return 0;
                }
            }
        }
        cardMovement.coroutineRunning = false;
    }

    IEnumerator undoResetDeck()
    {
        float resetDuration = 0.25f;
        int temp = drawCard.transform.childCount;
        if (temp == 1 || temp == 2)
        {
            for (int i = 0; i < temp; i++)
            {    
                cardController.openCard(drawCard.transform.GetChild(drawCard.transform.childCount-1));
                drawCard.transform.GetChild(drawCard.transform.childCount-1).SetParent(drawCard.drawArea);
                drawCard.drawArea.GetChild(drawCard.drawArea.childCount-1).LeanMoveLocalY(-cardController.stackOffset * i, resetDuration);
            }
        }
        else
        {    
            for (int i = 0; i < temp; i++)
            {
                float offset = 0f;
                if (i == temp - 1)
                {
                    offset = -2 * cardController.stackOffset;
                }
                else if (i == temp - 2)
                {
                    offset = -1 * cardController.stackOffset;
                }
                cardController.openCard(drawCard.transform.GetChild(drawCard.transform.childCount-1));
                drawCard.transform.GetChild(drawCard.transform.childCount-1).SetParent(drawCard.drawArea);
                drawCard.drawArea.GetChild(drawCard.drawArea.childCount-1).LeanMoveLocalY(offset, resetDuration);
            }
        }
        yield return new WaitForSeconds(resetDuration);
        cardMovement.coroutineRunning = false;
    }

    void activateUndo()
    {
        undoButton.GetComponent<Button>().enabled = true;
        undoButton.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void deactivateUndo()
    {
        undoButton.GetComponent<Button>().enabled = false;
        undoButton.GetComponent<CanvasGroup>().alpha = 0.5f;
    }
}