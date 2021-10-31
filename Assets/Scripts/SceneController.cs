using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject gameModeSelection;
    [SerializeField] TextMeshProUGUI timeCounter;
    [SerializeField] TextMeshProUGUI moveCounter;
    [SerializeField] Image gameModeImage;
    [SerializeField] CardController cardController;
    [SerializeField] AudioSystem audioSystem;
    [SerializeField] Sprite[] gameModeSprites;
    public float startTime;


    void Update()
    {
        updateTimeCounter();
    }

    public void openNewGamePanel(bool forceOpen = default)
    {
        if (!cardController.isWin || forceOpen)
        {
            if (cardController.gameAvailable)
            {
                gameModeSelection.transform.GetChild(0).Find("Title").GetComponent<TextMeshProUGUI>().text = "Start New Game?";
            }
            else
            {
                gameModeSelection.transform.GetChild(0).Find("Title").GetComponent<TextMeshProUGUI>().text = "Play Again?";
            }
            gameModeSelection.transform.GetChild(0).Find("Close Button").gameObject.SetActive(cardController.gameAvailable);
            gameModeSelection.transform.GetChild(0).localScale = Vector3.zero;
            gameModeSelection.transform.GetChild(0).LeanScale(Vector3.one, 0.5f).setEaseOutBack();
            gameModeSelection.SetActive(true);
        }
    }

    public void closeNewGamePanel()
    {
        if (cardController.gameMode == "Easy")
        {
            gameModeImage.sprite = gameModeSprites[0];
        }
        else if (cardController.gameMode == "Hard")
        {
            gameModeImage.sprite = gameModeSprites[1];
        }
        gameModeSelection.transform.GetChild(0).Find("Close Button").gameObject.SetActive(true);
        gameModeSelection.SetActive(false);
    }

    void updateTimeCounter()
    {
        if (!cardController.isWin && cardController.gameAvailable)
        {
            List<string> tempList = new List<string>();
            float elapsedTime = Time.time - startTime;
            float hour = elapsedTime / 3600;
            float minute = (elapsedTime % 3600) / 60;
            float second = (elapsedTime % 3600) % 60;
            if (hour >= 10)
            {
                tempList.Add(hour.ToString().Substring(0, 2));
            }
            else
            {
                tempList.Add("0" + hour.ToString().Substring(0, 1));
            }
            if (minute >= 10)
            {
                tempList.Add(minute.ToString().Substring(0, 2));
            }
            else
            {
                tempList.Add("0" + minute.ToString().Substring(0, 1));
            }
            if (second >= 10)
            {
                tempList.Add(second.ToString().Substring(0, 2));
            }
            else
            {
                tempList.Add("0" + second.ToString().Substring(0, 1));
            }
            timeCounter.text = tempList[0] + ":" + tempList[1] + ":" + tempList[2];
        }
    }

    public void updateMoveCounter(int delta = default)
    {
        if (cardController.gameAvailable)
        {
            if (delta == 1)
            {
                audioSystem.moveSound.Play();
            }
            cardController.moveCount += delta;
            moveCounter.text = cardController.moveCount.ToString();
        }
    }







    public void quitGame()
    {
        Application.Quit();
    }
}
