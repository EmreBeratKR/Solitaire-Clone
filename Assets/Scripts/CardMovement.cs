using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    
    [SerializeField] RectTransform[] snappingAreas;
    public Transform[] potentialSnaps;
    public Transform currentSnap;
    public Transform draggingCard;
    public List<Transform> draggingCards;
    [SerializeField] CardController cardController;
    public bool coroutineRunning;
    public Transform revealedCard;

    void Update()
    {
        if (draggingCard != null)
        {
            checkCardPos(draggingCard.position);
        }
        if (draggingCards.Count != 0)
        {
            checkCardPos(draggingCards[0].position);
        }
    }

    public void checkCardPos(Vector3 cardPos)
    {
        int illegalSnap = 0;
        if (draggingCards.Count > 1)
        {
            illegalSnap = 4;
        }
        currentSnap = null;
        for (int i = 0; i < snappingAreas.Length - illegalSnap; i++)
        {
            if (cardPos.x < snappingAreas[i].position.x + (snappingAreas[i].sizeDelta.x / 2f) * cardController.GetComponent<RectTransform>().lossyScale.x
            && cardPos.x > snappingAreas[i].position.x - (snappingAreas[i].sizeDelta.x / 2f) * cardController.GetComponent<RectTransform>().lossyScale.x)
            {
                if (cardPos.y < snappingAreas[i].position.y + (snappingAreas[i].sizeDelta.y / 2f * cardController.GetComponent<RectTransform>().lossyScale.y)
                && cardPos.y > snappingAreas[i].position.y - (snappingAreas[i].sizeDelta.y / 2f) * cardController.GetComponent<RectTransform>().lossyScale.y)
                {
                    currentSnap = potentialSnaps[i];
                    break;
                }
            }
        }
    }

    public void revealCard()
    {
        for (int i = 0; i < potentialSnaps.Length-4; i++)
        {
            if (potentialSnaps[i].childCount > 0)
            {
                Transform lastCard_OfSlot = potentialSnaps[i].GetChild(potentialSnaps[i].childCount-1);
                if (!lastCard_OfSlot.GetComponent<Card>().isOpen)
                {
                    cardController.openCard(lastCard_OfSlot);
                    revealedCard = lastCard_OfSlot;
                }
            }
        }
    }

    public List<string> canSnapTo(Transform card)
    {
        string shape = card.GetComponent<Card>().shape;
        string type = card.GetComponent<Card>().type;
        List<string> snappableShapes = new List<string>();
        List<string> tempList = new List<string>();
        if (shape == "H" || shape == "D")
        {
            snappableShapes.Add("C");
            snappableShapes.Add("S");
        }
        else if (shape == "C" || shape == "S")
        {
            snappableShapes.Add("H");
            snappableShapes.Add("D");
        }
        if (type == "K")
        {
            tempList.Add("Slot");
            return tempList;
        }
        else if (type == "A")
        {
            tempList.Add("");
            return tempList;
        }
        else
        {
            foreach (var shp in snappableShapes)
            {
                tempList.Add(shp + nextType(type));
            }
            return tempList;
        }
    }

    public string canStackTo(Transform card)
    {
        string shape = card.GetComponent<Card>().shape;
        string type = card.GetComponent<Card>().type;
        if (type == "A")
        {
            return "Stack";
        }
        else
        {
            return shape + previousType(type);
        }
    }

    string nextType(string type)
    {
        for (int i = 0; i < cardController.types.Count; i++)
        {
            if (cardController.types[i] == type)
            {
                return cardController.types[i+1];
            }
        }
        return "";
    }

    string previousType(string type)
    {
        for (int i = 0; i < cardController.types.Count; i++)
        {
            if (cardController.types[i] == type)
            {
                return cardController.types[i-1];
            }
        }
        return "";
    }

}
