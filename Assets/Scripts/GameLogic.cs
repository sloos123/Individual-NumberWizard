using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic
{
    //private static readonly string TAG = "GameLogic"; //Konstante

    //Instanzvariablen
    private int min, max, guess;
    private bool mydebug = false;

    //Konstruktor
    public GameLogic()
    {
        InitGame();
    }

    //Methoden deklaration von AdaptMax(), von Aussen erreichbar
    public void AdaptMax()
    {
        max = guess;
        CalcGuess();
    }

    //Methoden deklaration von AdaptMin(), von Aussen erreichbar
    public void AdaptMin()
    {
        min = guess;
        CalcGuess();
    }

    //Methoden deklaration von GetGuess(), von Aussen erreichbar
    public string GetGuess()
    {
        return guess.ToString();
    }

    public void StartGame()
    {
        InitGame();
    }

    //Methoden deklaration von InitGame()
    private void InitGame()
    {
        //Initialisierung
        min = 1;
        max = 1000;
        guess = 500;
    }

    //Methoden deklaration von CalcGuess() 
    private void CalcGuess()
    {
        guess = (min + max) / 2;
    }

    //Methoden deklaration von MyDebug() 
    private void MyDebug(string msg)
    {
        if (mydebug)
        {
            Debug.Log(msg);
        }
    }
}
