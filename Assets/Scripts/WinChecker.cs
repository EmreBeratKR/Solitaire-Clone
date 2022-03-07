using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinChecker : MonoBehaviour
{
    public static WinChecker instance;

    [SerializeField] private Transform deck;
    [SerializeField] private Transform drawPile;
    [SerializeField] private Transform[] slots;

    public bool isWin
    {
        get
        {
            if (deck.childCount != 0) return false;

            if (drawPile.childCount != 0) return false;

            foreach (Transform slot in slots)
            {
                foreach (Transform child in slot)
                {
                    var card = child.GetComponent<Card>();
                    if (!card.isOpen) return false;
                }
            }
            return true;
        }
    }

    private void Awake() => instance = this;
}
