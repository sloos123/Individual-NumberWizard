using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WizardController : MonoBehaviour
{

    private GameLogic gameLogic;
    public Text nbrText, introText;
    public Button btnStartEnd, LowerBtn, HigherBtn, OkBtn;
    public GameObject nbrGuess;

    // Use this for initialization
    void Start()
    {

        gameLogic = new GameLogic();
        nbrGuess.SetActive(false);
    }

    public void NumberHigher()
    {
        gameLogic.AdaptMin();
        nbrText.text = gameLogic.GetGuess();
    }

    public void NumberLower()
    {
        gameLogic.AdaptMax();
        nbrText.text = gameLogic.GetGuess();
    }

    public void NumberEqual()
    {
        nbrGuess.SetActive(false);
        introText.text = gameLogic.GetGuess() + " was the number, great! New Game?";
        introText.enabled = true;
        btnStartEnd.GetComponentInChildren<Text>().text = "Start Game";
        HigherBtn.enabled = false;
        LowerBtn.enabled = false;
        OkBtn.enabled = false;

    }

    public void StartGame()
    {
        gameLogic.StartGame();
        nbrText.text = gameLogic.GetGuess();
        introText.enabled = false;
        btnStartEnd.GetComponentInChildren<Text>().text = "Restart Game";
        btnStartEnd.GetComponent<Image>().color = Color.yellow;
        nbrGuess.SetActive(true);
        HigherBtn.enabled = true;
        LowerBtn.enabled = true;
        OkBtn.enabled = true;
    }

    public void EndGame()
    {
        nbrGuess.SetActive(false);
        introText.enabled = true;
        btnStartEnd.GetComponentInChildren<Text>().text = "Start Game";
    }
}