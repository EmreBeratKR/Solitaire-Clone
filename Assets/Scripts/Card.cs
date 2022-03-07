using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
    public string shape;
    public string type;
    public string color;
    public bool isOpen = false;
    Transform lastParent;
    Vector3 Pointer_BeginDragPosition;
    Vector3 Card_BeginDragPosition;
    public Transform draggingCanvas;
    CardMovement cardMovement;
    CardController cardController;
    SceneController sceneController;
    MoveHistory moveHistory;
    DrawCard drawCard;
    Transform drawedCardDeck;
    bool isLegalMove;
    bool isNewCard;
    bool moveSuccess;
    bool isDragging = false;

    void Start()
    {
        draggingCanvas = GameObject.FindWithTag("Dragging Canvas").transform;
        cardMovement = GameObject.FindWithTag("Main Canvas").GetComponent<CardMovement>();
        cardController = GameObject.FindWithTag("Main Canvas").GetComponent<CardController>();
        sceneController = GameObject.FindWithTag("Main Canvas").GetComponent<SceneController>();
        moveHistory = GameObject.FindWithTag("Main Canvas").GetComponent<MoveHistory>();
        drawCard = GameObject.FindWithTag("Draw Deck").GetComponent<DrawCard>();
    }


    public void setDragEvent()
    {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry beginDrag = new EventTrigger.Entry();
        beginDrag.eventID = EventTriggerType.BeginDrag;
        beginDrag.callback.AddListener((data) => { onBeginDrag((PointerEventData)data); });
        trigger.triggers.Add(beginDrag);

        EventTrigger.Entry dragging = new EventTrigger.Entry();
        dragging.eventID = EventTriggerType.Drag;
        dragging.callback.AddListener((data) => { onDragging((PointerEventData)data); });
        trigger.triggers.Add(dragging);

        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener((data) => { onEndDrag((PointerEventData)data); });
        trigger.triggers.Add(endDrag);

        EventTrigger.Entry click = new EventTrigger.Entry();
        click.eventID = EventTriggerType.PointerClick;
        click.callback.AddListener((data) => { autoPair((PointerEventData)data); });
        trigger.triggers.Add(click);
    }

    public void autoPair(PointerEventData data)
    {
        if (!isDragging && !cardController.isWin && !drawCard.isDrawing && !cardMovement.coroutineRunning && cardController.gameMode == "Easy")
        {
            List<string> snappableList = cardMovement.canSnapTo(transform);
            string stackable = cardMovement.canStackTo(transform);
            List<Transform> tempList = new List<Transform>();
            isNewCard = false;
            bool stackSuccess = false;
            lastParent = transform.parent;
            if (stackable == "Stack")
            {
                for (int i = 7; i < 11; i++)
                {
                    if (cardMovement.potentialSnaps[i].childCount == 0)
                    {
                        if (transform.parent.name == "Drawed Cards")
                        {
                            if (transform.GetSiblingIndex() == (transform.parent.childCount - 1))
                            {
                                drawedCardDeck = transform.parent;
                                isNewCard = true;
                                transform.SetParent(draggingCanvas);
                                tempList.Add(transform);
                                stackSuccess = true;
                                StartCoroutine(snapCard(tempList, cardMovement.potentialSnaps[i]));
                                break;
                            }
                        }
                        else
                        {
                            transform.SetParent(draggingCanvas);
                            tempList.Add(transform);
                            stackSuccess = true;
                            StartCoroutine(snapCard(tempList, cardMovement.potentialSnaps[i]));
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 7; i < 11; i++)
                {
                    if (cardMovement.potentialSnaps[i].childCount != 0 && stackable == cardMovement.potentialSnaps[i].GetChild(cardMovement.potentialSnaps[i].childCount-1).name
                    && transform.GetSiblingIndex() == (transform.parent.childCount-1))
                    {
                        if (transform.parent.name == "Drawed Cards")
                        {
                            if (transform.GetSiblingIndex() == (transform.parent.childCount - 1))
                            {
                                drawedCardDeck = transform.parent;
                                isNewCard = true;
                                transform.SetParent(draggingCanvas);
                                tempList.Add(transform);
                                stackSuccess = true;
                                StartCoroutine(snapCard(tempList, cardMovement.potentialSnaps[i]));
                                break;
                            }
                        }
                        else
                        {
                            transform.SetParent(draggingCanvas);
                            tempList.Add(transform);
                            stackSuccess = true;
                            StartCoroutine(snapCard(tempList, cardMovement.potentialSnaps[i]));
                            break;
                        }
                    }
                }
            }
            if (!stackSuccess)
            {
                if (snappableList[0] == "Slot")
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (cardMovement.potentialSnaps[i].childCount == 0)
                        {
                            if (transform.parent.name == "Drawed Cards")
                            {
                                if (transform.GetSiblingIndex() == (transform.parent.childCount - 1))
                                {
                                    drawedCardDeck = transform.parent;
                                    isNewCard = true;
                                    transform.SetParent(draggingCanvas);
                                    tempList.Add(transform);
                                    stackSuccess = true;
                                    StartCoroutine(snapCard(tempList, cardMovement.potentialSnaps[i]));
                                    break;
                                }
                            }
                            else
                            {
                                if (transform.parent.name.Substring(0, 4) == "Slot")
                                {
                                    for (int c = transform.GetSiblingIndex(); c < transform.parent.childCount; c++)
                                    {
                                        tempList.Add(transform.parent.GetChild(c));
                                    }
                                    foreach (var card in tempList)
                                    {
                                        card.SetParent(draggingCanvas);
                                    }
                                }
                                else
                                {
                                    transform.SetParent(draggingCanvas);
                                    tempList.Add(transform); 
                                }
                                stackSuccess = true;
                                StartCoroutine(snapCard(tempList, cardMovement.potentialSnaps[i]));
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (cardMovement.potentialSnaps[i].childCount != 0 && snappableList.Contains(cardMovement.potentialSnaps[i].GetChild(cardMovement.potentialSnaps[i].childCount-1).name))
                        {
                            if (transform.parent.name == "Drawed Cards")
                            {
                                if (transform.GetSiblingIndex() == (transform.parent.childCount - 1))
                                {
                                    drawedCardDeck = transform.parent;
                                    isNewCard = true;
                                    transform.SetParent(draggingCanvas);
                                    tempList.Add(transform);
                                    stackSuccess = true;
                                    StartCoroutine(snapCard(tempList, cardMovement.potentialSnaps[i]));
                                    break;
                                }
                            }
                            else
                            {
                                if (transform.parent.name.Substring(0, 4) == "Slot")
                                {
                                    for (int c = transform.GetSiblingIndex(); c < transform.parent.childCount; c++)
                                    {
                                        tempList.Add(transform.parent.GetChild(c));
                                    }
                                    foreach (var card in tempList)
                                    {
                                        card.SetParent(draggingCanvas);
                                    }
                                }
                                else
                                {
                                    transform.SetParent(draggingCanvas);
                                    tempList.Add(transform); 
                                }
                                stackSuccess = true;
                                StartCoroutine(snapCard(tempList, cardMovement.potentialSnaps[i]));
                                break; 
                            }
                        }
                    }
                }
            }

            if (isNewCard && drawedCardDeck.childCount >= 3)
            {
                StartCoroutine(separateDrawedCards());
            }
        }
    }

    public void onBeginDrag(PointerEventData data)
    {
        if (!cardController.isWin && !drawCard.isDrawing && !cardMovement.coroutineRunning)
        {
            isDragging = true;
            isLegalMove = true;
            isNewCard = false;
            if (transform.parent.name == "Drawed Cards")
            {
                drawedCardDeck = transform.parent;
                isLegalMove = ((transform.GetSiblingIndex() + 1) == transform.parent.childCount);
                isNewCard = isLegalMove;
            }
            if (isLegalMove)
            {
                if (transform.parent.name.Substring(0,4) == "Slot")
                {
                    Pointer_BeginDragPosition = data.position;
                    Card_BeginDragPosition = transform.position;
                    lastParent = transform.parent;
                    for (int i = transform.GetSiblingIndex(); i < transform.parent.childCount; i++)
                    {
                        cardMovement.draggingCards.Add(transform.parent.GetChild(i));
                    }
                    foreach (var card in cardMovement.draggingCards)
                    {
                        card.SetParent(draggingCanvas);
                    }
                }
                else
                {
                    cardMovement.draggingCard = transform;
                    Pointer_BeginDragPosition = data.position;
                    Card_BeginDragPosition = transform.position;
                    lastParent = transform.parent;
                    transform.SetParent(draggingCanvas);   
                }
            }
        }
    }

    public void onDragging(PointerEventData data)
    {
        if (!cardController.isWin && !drawCard.isDrawing && !cardMovement.coroutineRunning && isDragging)
        {
            if (cardMovement.draggingCards.Count > 0)
            {
                for (int i = 0; i < cardMovement.draggingCards.Count; i++)
                {
                    cardMovement.draggingCards[i].position = Card_BeginDragPosition + new Vector3(data.position.x, data.position.y, 0) - Pointer_BeginDragPosition
                    + Vector3.down * cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y * i;
                }
                if (cardMovement.draggingCards[0].localPosition.y > 240)
                {
                    for (int i = 0; i < cardMovement.draggingCards.Count; i++)
                    {
                        cardMovement.draggingCards[i].localPosition = new Vector3(cardMovement.draggingCards[i].localPosition.x, 240 - cardController.stackOffset * i, 0);
                    }
                }
                else if (cardMovement.draggingCards[0].localPosition.y < -440)
                {
                    for (int i = 0; i < cardMovement.draggingCards.Count; i++)
                    {
                        cardMovement.draggingCards[i].localPosition = new Vector3(cardMovement.draggingCards[i].localPosition.x, -440 - cardController.stackOffset * i, 0);
                    }
                }
                /*
                if (cardMovement.draggingCards[0].localPosition.x < -885)
                {
                    for (int i = 0; i < cardMovement.draggingCards.Count; i++)
                    {
                        cardMovement.draggingCards[i].localPosition = new Vector3(-885, cardMovement.draggingCards[i].localPosition.y, 0);
                    }
                }
                else if (cardMovement.draggingCards[0].localPosition.x > 885)
                {
                    for (int i = 0; i < cardMovement.draggingCards.Count; i++)
                    {
                        cardMovement.draggingCards[i].localPosition = new Vector3(885, cardMovement.draggingCards[i].localPosition.y, 0);
                    }
                }
                */
            }
            else
            {
                if (isLegalMove)
                {
                    transform.position = Card_BeginDragPosition + new Vector3(data.position.x, data.position.y, 0) - Pointer_BeginDragPosition;
                    if (transform.localPosition.y > 240)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, 240, 0);
                    }
                    else if (transform.localPosition.y < -440)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, -440, 0);
                    }
                    /*
                    if (transform.localPosition.x < -885)
                    {
                        transform.localPosition = new Vector3(-885, transform.localPosition.y, 0);
                    }
                    else if (transform.localPosition.x > 885)
                    {
                        transform.localPosition = new Vector3(885, transform.localPosition.y, 0);
                    }
                    */
                }   
            }
        }
    }

    public void onEndDrag(PointerEventData data)
    {
        isDragging = false;
        if (!cardController.isWin && !drawCard.isDrawing && !cardMovement.coroutineRunning && isLegalMove)
        {
            if (cardMovement.draggingCards.Count > 0)
            {
                if (cardMovement.currentSnap == null)
                {
                    StartCoroutine(returnCard(cardMovement.draggingCards));
                }
                else
                {
                    List<string> snappableList = cardMovement.canSnapTo(transform);
                    string stackable = cardMovement.canStackTo(transform);
                    Transform crntSnap = cardMovement.currentSnap;
                    if (crntSnap.name.Substring(0, crntSnap.name.Length-2) == "Slot")
                    {
                        if (snappableList[0] == "")
                        {
                            StartCoroutine(returnCard(cardMovement.draggingCards));
                        }
                        else if (snappableList[0] == "Slot")
                        {
                            if (crntSnap.childCount == 0)
                            {
                                if (crntSnap.name != lastParent.name)
                                {
                                    StartCoroutine(snapCard(cardMovement.draggingCards, crntSnap));
                                }
                                else
                                {
                                    StartCoroutine(returnCard(cardMovement.draggingCards));
                                }
                            }
                            else
                            {
                                StartCoroutine(returnCard(cardMovement.draggingCards));
                            }
                        }
                        else
                        {
                            if (crntSnap.childCount != 0 && snappableList.Contains(crntSnap.GetChild(crntSnap.childCount-1).name))
                            {
                                if (crntSnap.name != lastParent.name)
                                {
                                    StartCoroutine(snapCard(cardMovement.draggingCards, crntSnap));
                                }
                                else
                                {
                                    StartCoroutine(returnCard(cardMovement.draggingCards));
                                }
                            }
                            else
                            {
                                StartCoroutine(returnCard(cardMovement.draggingCards));
                            }
                        }
                    }
                    else if (crntSnap.name.Substring(0, crntSnap.name.Length-2) == "Stack")
                    {
                        if (stackable == "Stack")
                        {
                            if (crntSnap.childCount == 0)
                            {
                                if (crntSnap.name != lastParent.name)
                                {
                                    StartCoroutine(snapCard(cardMovement.draggingCards, crntSnap));
                                }
                                else
                                {
                                    StartCoroutine(returnCard(cardMovement.draggingCards));
                                }
                            }
                            else
                            {
                                StartCoroutine(returnCard(cardMovement.draggingCards));
                            }
                        }
                        else if (crntSnap.childCount != 0 && stackable == crntSnap.GetChild(crntSnap.childCount-1).name)
                        {
                            if (crntSnap.name != lastParent.name)
                            {
                                StartCoroutine(snapCard(cardMovement.draggingCards, crntSnap));
                            }
                            else
                            {
                                StartCoroutine(returnCard(cardMovement.draggingCards));
                            }
                        }
                        else
                        {
                            StartCoroutine(returnCard(cardMovement.draggingCards));
                        }
                    }
                } 
            }
            else
            {
                cardMovement.draggingCard = null;
                List<Transform> tempList = new List<Transform>();
                tempList.Add(transform);
                if (cardMovement.currentSnap == null)
                {
                    StartCoroutine(returnCard(tempList));
                }
                else
                {
                    List<string> snappableList = cardMovement.canSnapTo(transform);
                    string stackable = cardMovement.canStackTo(transform);
                    Transform crntSnap = cardMovement.currentSnap;
                    if (crntSnap.name.Substring(0, crntSnap.name.Length-2) == "Slot")
                    {
                        if (snappableList[0] == "")
                        {
                            StartCoroutine(returnCard(tempList));
                        }
                        else if (snappableList[0] == "Slot")
                        {
                            if (crntSnap.childCount == 0)
                            {
                                if (crntSnap.name != lastParent.name)
                                {
                                    StartCoroutine(snapCard(tempList, crntSnap));
                                }
                                else
                                {
                                    StartCoroutine(returnCard(tempList));
                                }
                            }
                            else
                            {
                                StartCoroutine(returnCard(tempList));
                            }
                        }
                        else
                        {
                            if (crntSnap.childCount != 0 && snappableList.Contains(crntSnap.GetChild(crntSnap.childCount-1).name))
                            {
                                if (crntSnap.name != lastParent.name)
                                {
                                    StartCoroutine(snapCard(tempList, crntSnap));
                                }
                                else
                                {
                                    StartCoroutine(returnCard(tempList));
                                }
                            }
                            else
                            {
                                StartCoroutine(returnCard(tempList));
                            }
                        }
                    }
                    else if (crntSnap.name.Substring(0, crntSnap.name.Length-2) == "Stack")
                    {
                        if (stackable == "Stack")
                        {
                            if (crntSnap.childCount == 0)
                            {
                                if (crntSnap.name != lastParent.name)
                                {
                                    StartCoroutine(snapCard(tempList, crntSnap));
                                }
                                else
                                {
                                    StartCoroutine(returnCard(tempList));
                                }
                            }
                            else
                            {
                                StartCoroutine(returnCard(tempList));
                            }
                        }
                        else if (crntSnap.childCount != 0 && stackable == crntSnap.GetChild(crntSnap.childCount-1).name)
                        {
                            if (crntSnap.name != lastParent.name)
                                {
                                    StartCoroutine(snapCard(tempList, crntSnap));
                                }
                                else
                                {
                                    StartCoroutine(returnCard(tempList));
                                }
                        }
                        else
                        {
                            StartCoroutine(returnCard(tempList));
                        }
                    }
                    if (isNewCard && moveSuccess && drawedCardDeck.childCount >= 3)
                    {
                        StartCoroutine(separateDrawedCards());
                    }
                }   
            }
        }
    }

    IEnumerator returnCard(List<Transform> cards)
    {
        cardMovement.coroutineRunning = true;
        moveSuccess = false;
        Vector3 moveVector = Card_BeginDragPosition - transform.position;
        for (int i = 0; i < 20; i++)
        {
            foreach (var card in cards)
            {
                card.position += moveVector / 20;
            }
            yield return 0;
        }
        foreach (var card in cards)
        {
            card.SetParent(lastParent);   
        }
        cardMovement.draggingCard = null;
        cardMovement.draggingCards.Clear();
        cardMovement.revealCard();
        cardMovement.coroutineRunning = false;
    }

    IEnumerator snapCard(List<Transform> cards, Transform target)
    {
        cardMovement.coroutineRunning = true;
        sceneController.updateMoveCounter(1);
        cardMovement.revealedCard = null;
        moveSuccess = true;
        Vector3 offset = Vector3.down * target.childCount * cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y;
        if (target.name.Substring(0,5) == "Stack")
        {
            offset = Vector3.zero;
        }
        Vector3 moveVector = target.position - transform.position + offset;
        for (int i = 0; i < 20; i++)
        {
            foreach (var card in cards)
            {
                card.position += moveVector / 20;  
            }
            yield return 0;
        }
        foreach (var card in cards)
        {
            card.SetParent(target);   
        }
        cardMovement.revealCard();
        if (isNewCard)
        {
            moveHistory.addMove("new card", cards, lastParent, cardMovement.revealedCard);
        }
        else
        {
            moveHistory.addMove("drag", cards, lastParent, cardMovement.revealedCard);
        }
        cardMovement.draggingCard = null;
        cardMovement.draggingCards.Clear();
        cardController.updateStacks();
        cardMovement.coroutineRunning = false;
    }


    IEnumerator separateDrawedCards()
    {
        for (int i = 0; i < 10; i++)
        {
            drawedCardDeck.GetChild(drawedCardDeck.childCount-1).position += Vector3.down * cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10;   
            drawedCardDeck.GetChild(drawedCardDeck.childCount-2).position += Vector3.down * cardController.stackOffset * cardController.GetComponent<RectTransform>().lossyScale.y / 10;
            yield return 0; 
        }
    }
}
