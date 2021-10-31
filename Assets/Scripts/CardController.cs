using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{
    string[] shapes = {"H", "D", "C", "S"};
    public List<string> types;
    List<string> deck = new List<string>();
    [SerializeField] GameObject cardPrefab;
    [SerializeField] RectTransform deckStart;
    [SerializeField] RectTransform[] slots;
    [SerializeField] RectTransform[] stacks;
    [SerializeField] RectTransform drawedCards;
    [SerializeField] Sprite[] shapeSprites;
    [SerializeField] Sprite[] jokerSprites;
    [SerializeField] SceneController sceneController;
    [SerializeField] MoveHistory moveHistory;
    [SerializeField] DrawCard drawCard;
    [SerializeField] WinScreen winScreen;
    [SerializeField] AudioSystem audioSystem;
    [SerializeField] List<string> stackShapes;
    public string gameMode = "Easy";
    public float stackOffset;
    public float deckOffset = 0.25f;
    public bool gameAvailable = false;
    public bool isWin = false;
    public int moveCount;
    int openCardsCount;

    void Start()
    {
        createDeck();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(winCoroutine());
        }
    }

    public void initGame(string mode)
    {
        gameMode = mode;
        StartCoroutine(initGameCoroutine());
    }

    IEnumerator initGameCoroutine()
    {
        float cleanDuration = 0.5f;
        openCardsCount = 0;
        moveHistory.moves.Clear();
        moveHistory.deactivateUndo();
        sceneController.closeNewGamePanel();
        isWin = false;
        gameAvailable = true; //kazanınca false yap
        moveCount = 0;
        sceneController.updateMoveCounter();
        cleanBoard(cleanDuration);
        yield return new WaitForSeconds(cleanDuration);
        for (int i = 0; i < deckStart.childCount; i++)
        {
            Destroy(deckStart.GetChild(i).gameObject);
        }
        winScreen.gameObject.SetActive(false);
        sceneController.startTime = Time.time;
        yield return 0;
        updateStacks();
        shuffleDeck();
        putDeck();
        StartCoroutine(dealDeck());
    }

    void cleanBoard(float duration)
    {
        int tempChildCount = drawCard.drawArea.childCount;
        for (int i = 0; i < tempChildCount; i++)
        {
            Transform temp = drawCard.drawArea.GetChild(0);
            temp.SetParent(deckStart);
            closeCard(temp);
            temp.LeanMoveLocal(new Vector3(0, -deckOffset * deckStart.childCount, 0), duration).setEaseOutQuint();
        }
        for (int i = 0; i < slots.Length; i++)
        {
            tempChildCount = slots[i].childCount;
            for (int c = 0; c < tempChildCount; c++)
            {
                Transform temp = slots[i].GetChild(0);
                temp.SetParent(deckStart);
                closeCard(temp);
                temp.LeanMoveLocal(new Vector3(0, -deckOffset * deckStart.childCount, 0), duration).setEaseOutQuint();
            }
        }
        for (int i = 0; i < stacks.Length; i++)
        {
            tempChildCount = stacks[i].childCount;
            for (int c = 0; c < tempChildCount; c++)
            {
                Transform temp = stacks[i].GetChild(0);
                temp.SetParent(deckStart);
                closeCard(temp);
                temp.LeanMoveLocal(new Vector3(0, -deckOffset * deckStart.childCount, 0), duration).setEaseOutQuint();
            }
        }
    }


    void createDeck()
    {
        foreach (var shape in shapes)
        {
            foreach (var type in types)
            {
                deck.Add(shape + type);
            }
        }
    }

    void shuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            string temp = deck[i];
            int randomIndex = Random.Range(0, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    IEnumerator dealDeck()
    {
        float dealDuration = 0.5f;
        audioSystem.dealDeckSound.Play();
        for (int i = 0; i < slots.Length; i++)
        {
            for (int c = 0; c < i+1; c++)
            {
                deckStart.GetChild(deckStart.childCount-1).SetParent(slots[i]);
                slots[i].GetChild(c).LeanMoveLocal(Vector3.down * c * stackOffset, dealDuration).setEaseOutQuint();
                if (c == i)
                {
                    openCard(slots[i].GetChild(c));
                }
            }
        }
        yield return new WaitForSeconds(dealDuration);
    }

    void putDeck()
    {
        float offset = 0f;
        foreach (var card in deck)
        {
            GameObject newCard = Instantiate(cardPrefab, deckStart.position + new Vector3(0, offset, 0), Quaternion.identity, deckStart);
            offset -= deckOffset * gameObject.GetComponent<RectTransform>().lossyScale.y;
            newCard.name = card;
            newCard.GetComponent<Card>().shape = card.Substring(0, 1);
            newCard.GetComponent<Card>().type = card.Substring(1, card.Length-1);
            newCard.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = card.Substring(1, card.Length-1);
            if (card.Substring(0, 1) == "H")
            {
                newCard.GetComponent<Card>().color = "red";
                newCard.transform.Find("Type").GetComponent<TextMeshProUGUI>().color = Color.red;
                newCard.transform.Find("Shape Small").GetComponent<Image>().sprite = shapeSprites[0];
                if (card.Substring(1, 1) == "J")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[0];
                }
                else if (card.Substring(1, 1) == "Q")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[1];
                }
                else if (card.Substring(1, 1) == "K")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[2];
                }
                else
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = shapeSprites[0];
                }
            }
            else if (card.Substring(0, 1) == "D")
            {
                newCard.GetComponent<Card>().color = "red";
                newCard.transform.Find("Type").GetComponent<TextMeshProUGUI>().color = Color.red;
                newCard.transform.Find("Shape Small").GetComponent<Image>().sprite = shapeSprites[1];
                if (card.Substring(1, 1) == "J")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[0];
                }
                else if (card.Substring(1, 1) == "Q")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[1];
                }
                else if (card.Substring(1, 1) == "K")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[2];
                }
                else
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = shapeSprites[1];
                }
            }
            else if (card.Substring(0, 1) == "C")
            {
                newCard.GetComponent<Card>().color = "black";
                newCard.transform.Find("Type").GetComponent<TextMeshProUGUI>().color = Color.black;
                newCard.transform.Find("Shape Small").GetComponent<Image>().sprite = shapeSprites[2];
                if (card.Substring(1, 1) == "J")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[3];
                }
                else if (card.Substring(1, 1) == "Q")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[4];
                }
                else if (card.Substring(1, 1) == "K")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[5];
                }
                else
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = shapeSprites[2];
                }
            }
            else if (card.Substring(0, 1) == "S")
            {
                newCard.GetComponent<Card>().color = "black";
                newCard.transform.Find("Type").GetComponent<TextMeshProUGUI>().color = Color.black;
                newCard.transform.Find("Shape Small").GetComponent<Image>().sprite = shapeSprites[3];
                if (card.Substring(1, 1) == "J")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[3];
                }
                else if (card.Substring(1, 1) == "Q")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[4];
                }
                else if (card.Substring(1, 1) == "K")
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = jokerSprites[5];
                }
                else
                {
                    newCard.transform.Find("Shape Large").GetComponent<Image>().sprite = shapeSprites[3];
                }
            }
        }
    }

    public void openCard(Transform card)
    {
        card.Find("Back Face").gameObject.SetActive(false);
        card.gameObject.GetComponent<Card>().setDragEvent();
        card.gameObject.GetComponent<Card>().isOpen = true;
        openCardsCount++;
        if (openCardsCount == shapes.Length * types.Count)
        {
            StartCoroutine(winCoroutine());
        }
    }

    public void closeCard(Transform card)
    {
        card.Find("Back Face").gameObject.SetActive(true);
        Destroy(card.gameObject.GetComponent<EventTrigger>());
        card.gameObject.GetComponent<Card>().isOpen = false;
        openCardsCount--;
    }

    public void updateStacks()
    {
        for (int i = 0; i < stacks.Length; i++)
        {
            if (stacks[i].childCount == 0)
            {
                stackShapes[i] = "";
            }
            else
            {
                stackShapes[i] = stacks[i].GetChild(0).GetComponent<Card>().shape;
            }
        }
    }

    IEnumerator winCoroutine()
    {
        isWin = true;
        yield return new WaitForSeconds(0.5f);

        List<Transform> lastCards = new List<Transform>();
        List<Vector3> moveVectors = new List<Vector3>();
        int temp = drawedCards.childCount;
        for (int c = 0; c < temp; c++)
        {
            for (int i = 0; i < stacks.Length; i++)
            {
                if (stackShapes.Contains(drawedCards.GetChild(0).GetComponent<Card>().shape))
                {
                    lastCards.Add(drawedCards.GetChild(0));
                    moveVectors.Add(stacks[stackShapes.IndexOf(drawedCards.GetChild(0).GetComponent<Card>().shape)].position - drawedCards.GetChild(0).position);
                    drawedCards.GetChild(0).SetParent(stacks[stackShapes.IndexOf(drawedCards.GetChild(0).GetComponent<Card>().shape)]);
                    break;
                }
                else if (stackShapes[i] == "")
                {
                    stackShapes[i] = drawedCards.GetChild(0).GetComponent<Card>().shape;
                    lastCards.Add(drawedCards.GetChild(0));
                    moveVectors.Add(stacks[i].position - drawedCards.GetChild(0).position);
                    drawedCards.GetChild(0).SetParent(stacks[i]);
                    break;
                }
            }
        }

        for (int s = 0; s < slots.Length; s++)
        {
            temp = slots[s].childCount;
            for (int c = 0; c < temp; c++)
            {
                for (int i = 0; i < stacks.Length; i++)
                {
                    if (stackShapes.Contains(slots[s].GetChild(0).GetComponent<Card>().shape))
                    {
                        lastCards.Add(slots[s].GetChild(0));
                        moveVectors.Add(stacks[stackShapes.IndexOf(slots[s].GetChild(0).GetComponent<Card>().shape)].position - slots[s].GetChild(0).position);
                        slots[s].GetChild(0).SetParent(stacks[stackShapes.IndexOf(slots[s].GetChild(0).GetComponent<Card>().shape)]);
                        break;
                    }
                    else if (stackShapes[i] == "")
                    {
                        stackShapes[i] = slots[s].GetChild(0).GetComponent<Card>().shape;
                        lastCards.Add(slots[s].GetChild(0));
                        moveVectors.Add(stacks[i].position - slots[s].GetChild(0).position);
                        slots[s].GetChild(0).SetParent(stacks[i]);
                        break;
                    }
                }
            }
        }
        
        foreach (var stack in stacks)
        {
            for (int t = 0; t < types.Count; t++)
            {
                for (int c = 0; c < stack.childCount; c++)
                {
                    if (types[t] == stack.GetChild(c).GetComponent<Card>().type)
                    {
                        stack.GetChild(c).SetSiblingIndex(t);
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < 30; i++)
        {
            for (int c = 0; c < lastCards.Count; c++)
            {
                lastCards[c].position += moveVectors[c] / 30;
            }
            yield return 0;
        }
        
        audioSystem.winSounds[0].Play();
        yield return new WaitForSeconds(0.5f);

        winScreen.transform.GetChild(0).localPosition = Vector3.zero;
        winScreen.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1f;
        winScreen.banner.localScale = new Vector3(0f, 1f, 1f);
        winScreen.folds.alpha = 0f;
        Vector3 textsMoveVector = Vector3.up * winScreen.texts[0].parent.GetComponent<RectTransform>().sizeDelta.y * gameObject.GetComponent<RectTransform>().lossyScale.y;
        foreach (var text in winScreen.texts)
        {
            text.position -= textsMoveVector;
        }
        List<RectTransform> aces = new List<RectTransform>();
        foreach (var shape in stackShapes)
        {
            foreach (var ace in winScreen.aces)
            {
                if (ace.gameObject.name == shape)
                {
                    aces.Add(ace);
                    break;
                }
            }
        }
        int tempColm = -3;
        foreach (var ace in aces)
        {
            ace.localPosition = new Vector3(tempColm * 90, -300, ace.localPosition.z);
            ace.GetComponent<CanvasGroup>().alpha = 0f;
            tempColm += 2;
        }
        winScreen.gameObject.SetActive(true);
        
        winScreen.bannerScale();
        audioSystem.winSounds[1].Play();
        yield return new WaitForSeconds(winScreen.bannerEaseTime);

        for (int i = 0; i < 20; i++)
        {
            winScreen.folds.alpha += 1f/20f;
            yield return 0;
        }

        foreach (var text in winScreen.texts)
        {
            yield return new WaitForSeconds(0.25f);
            StartCoroutine(moveCoroutine(text, textsMoveVector));
        }
        
        audioSystem.winSounds[2].Play();
        foreach (var ace in aces)
        {
            winScreen.aceMove(ace.gameObject);
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 80; i++)
        {
            winScreen.transform.GetChild(0).position += (Vector3.up * GetComponent<RectTransform>().sizeDelta.y * GetComponent<RectTransform>().localScale.y) / 80f;
            winScreen.transform.GetChild(0).GetComponent<CanvasGroup>().alpha -= 1f / 80f;
            yield return 0;
        }
        gameAvailable = false;
        sceneController.openNewGamePanel(true);
    }

    IEnumerator moveCoroutine(RectTransform item, Vector3 moveVector)
    {
        for (int i = 0; i < 20; i++)
        {
            item.position += moveVector / 20f;
            yield return 0;
        }
    }
}
