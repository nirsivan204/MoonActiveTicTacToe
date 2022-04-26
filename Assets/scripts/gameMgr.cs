using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class gameMgr : MonoBehaviour
{
    public const int N_PLACES = 9;
    private const int MAX_SECONDS = 5;
    public const int EMPTY_SYMBOL = 0;
    public const int PLAYER1_SYMBOL = 1;
    public const int PLAYER2_SYMBOL = 2;
    [SerializeField] Sprite[] imgs; //[XImg, OImg, BGImg]
    [SerializeField] GameObject hintButton;
    [SerializeField] GameObject undoButton;
    [SerializeField] GameObject board;
    [SerializeField] SpriteRenderer BGSprite;
    public Text winText;
    public Text secondsText;
    public Text hintText;
    int[] turns = new int[N_PLACES];
    int[] gameBoard = new int[N_PLACES]; // gameBoard will be represented by an array of 9 ints, that is mapped to the 3 by 3 board by: 0,1,2  when 0,1,2... are the indexes of the array, and each place is filled with the correct player's symbol
                                         //                                                                                             3,4,5
                                         //                                                                                             6,7,8
    int n_turn = 0;
    bool P1CurrentTurn = true; //true for player1, false for player2/Computer
    private bool isComTurn = false;
    [SerializeField] bool isPVP = true;
    private Sprite player1Img;
    private Sprite player2Img;
    private int secondsLeft;
    Coroutine timerCoroutine;
    private bool isGameEnded = false;
    [SerializeField] private BoardMgr boardMgr;
    public int COMDifficulty;

    public bool IsPVP { get => isPVP; set => isPVP = value; }
    public bool IsGameEnded { get => isGameEnded; set => isGameEnded = value; }
    public int N_turn { get => n_turn; set => n_turn = value; }
    public int[] GameBoard { get => gameBoard; set => gameBoard = value; }
    public int[] Turns { get => turns; set => turns = value; }
    public Sprite[] Imgs { get => imgs; set => imgs = value; }

    /*This function is called every time the board gets an input from the player (clicking a place in the board).
     */
    public void ChooseBoardPlace(int id)
    {
        if (id > N_PLACES || id < 0)
        {
            Debug.Log("error in chooseBoardPlace");
        }
        else
        {
            if (!isComTurn && gameBoard[id] == EMPTY_SYMBOL && !isGameEnded)
            {
                if (P1CurrentTurn)
                {
                    if (boardMgr)
                    {
                        boardMgr.DrawSpriteInPlace(id, player1Img);
                    }
                    gameBoard[id] = PLAYER1_SYMBOL;
                }
                else
                {
                    if (boardMgr)
                    {
                        boardMgr.DrawSpriteInPlace(id, player2Img);
                    }
                    gameBoard[id] = PLAYER2_SYMBOL;
                }
                turns[n_turn] = id;
                FinishTurn();
            }
        }
    }
/* This function is being called after each turn. It checks for winners or draw. If this is the case, finishes the game. 
 * If not, changes the turn to the other player, raising the turn counts and restarts the timer.
 */
    private void FinishTurn()
    {
        if (hintText)
        {
            hintText.enabled = false;
        }
        int winner = IsWin(gameBoard);
        if (winner != NO_WIN)
        {
            if (winText)
            {
                winner = WinnerCodeToNum(winner);
                winText.text = "Player " + winner + " Wins";
                winText.enabled = true;
            }
            EndGame();
        }
        else
        {
            P1CurrentTurn = !P1CurrentTurn;
            n_turn++;
            if (n_turn == N_PLACES)
            {
                if (winText)
                {
                    winText.text = "Draw";
                    winText.enabled = true;
                }
                EndGame();
            }
            else
            {
                if (!isPVP)
                {
                    isComTurn = !isComTurn;
                    if (isComTurn)
                    {
                        MakeComMove();
                    }
                }
                StopCoroutine(timerCoroutine);
                if (!isComTurn)
                {
                    timerCoroutine = StartCoroutine(Timer());
                }
            }
        }
    }

    private int WinnerCodeToNum(int winner) //see AnalizeThreePlaces to understand the conversion
    {
        return winner-N_PLACES+1;
    }

    IEnumerator Timer()
    {
        secondsLeft = MAX_SECONDS;
        secondsText.text = secondsLeft.ToString();
        while (true)
        {
            yield return new WaitForSeconds(1);
            secondsLeft--;
            secondsText.text = secondsLeft.ToString();

            if (secondsLeft == 0)
            {
                int winner;
                if (P1CurrentTurn)
                {
                    winner = 2;
                }
                else
                {
                    winner = 1;
                }
                winText.text = "Player " + winner + " Wins";
                winText.enabled = true;
                EndGame();
                break;
            }
        }
    }

    public void StartGame()
    {
        hintButton.SetActive(!isPVP);
        undoButton.SetActive(!isPVP);
        BGSprite.sprite = imgs[2];
        board.SetActive(true);
        boardMgr.Init();
        Restart();
    }

    public void Restart()
    {
        n_turn = 0;
        isComTurn = false;
        isGameEnded = false;
        InitTurnsArray();
        InitGameBoard();
        RemoveSpritesFromEmptyPlaces(gameBoard);
        winText.enabled = false;
        hintText.enabled = false;
        secondsLeft = MAX_SECONDS;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(Timer());
        int randImgId = Random.Range(0, 2);
        player1Img = imgs[randImgId];
        player2Img = imgs[1 - randImgId];
        if (randImgId == 0)//if player1 is X
        {
            P1CurrentTurn = true;
        }
        else
        {
            P1CurrentTurn = false;
            if (!isPVP)
            {
                isComTurn = true;
                MakeComMove();
            }
        }
    }

    private void InitTurnsArray()
    {
        for (int i = 0; i < N_PLACES; i++)
        {
            turns[i] = -1; //-1 means no one played this turn yet
        }
    }

    private void InitGameBoard()
    {
        for (int i = 0; i < N_PLACES; i++)
        {
            gameBoard[i] = EMPTY_SYMBOL;
        }
    }

/* Undo the 2 last moves. delete 2 turns from the turn array, and updating the board to delete the imgs from this places
 */
    public void Undo()
    {
        if (!isPVP && !isGameEnded)
        {
            if (n_turn >= 2)
            {
                gameBoard[turns[n_turn - 1]] = EMPTY_SYMBOL;
                gameBoard[turns[n_turn - 2]] = EMPTY_SYMBOL;
                turns[n_turn - 1] = turns[n_turn - 2] = -1;
                n_turn -= 2;
                if (hintText)
                {
                    hintText.enabled = false;
                }
                RemoveSpritesFromEmptyPlaces(gameBoard);
            }
        }
    }

    private void RemoveSpritesFromEmptyPlaces(int[] board)
    {
        for (int i = 0; i < N_PLACES; i++)
        {
            if (board[i] == EMPTY_SYMBOL)
            {
                if (boardMgr)
                {
                    boardMgr.DrawSpriteInPlace(i, null);
                }
            }
        }
    }

    public static int IsWin(int[] board)
    {


        int winner = HorizontalWin(board);
        if (winner != NO_WIN)
        {
            return winner;
        }
        winner = VerticalWin(board);
        if (winner != NO_WIN)
        {
            return winner;
        }
        return DiagonalWin(board);

    }

    public const int P1_WIN = 9;
    public const int P2_WIN = 10;
    public const int NO_WIN = -1;

 /* This function gets a board, and 3 indexes. these indexes corresponds to a line in the board needed to be checked (row,col,or diagonal).
  * It counts the number of O,X or empty spaces, and returns a resault:
  * If it is almost a win (2 out of 3 are the same player's symbol), it returns the index of the empty place.
  * If it is a win, returns the winner (because 0-8 are already taken, 9 means P1 is winning, 10 means P2 is winning
  * else returns no win.
  * 
  */
    public static int AnalizeThreePlaces(int[] board,int[] indexes)
    {
        int p1Counter = 0;
        int p2Counter = 0;
        int OCounter = 0;
        int OFirstLoc = -1;
        for (int i = 0;i < 3; i++)
        {
            switch (board[indexes[i]])
            {
                case EMPTY_SYMBOL:
                    OCounter++;
                    if (OCounter == 1)
                    {
                        OFirstLoc = indexes[i];
                    }
                    break;
                case PLAYER1_SYMBOL:
                    p1Counter++;
                    break;
                case PLAYER2_SYMBOL:
                    p2Counter++;
                    break;
            }
        }
        if(p1Counter == 3)
        {
            return P1_WIN;
        }
        if(p2Counter == 3)
        {
            return P2_WIN;
        }
        if(OCounter == 1 && p1Counter!= p2Counter)
        {
            return OFirstLoc;
        }
        return NO_WIN;

    }

    private static int VerticalWin(int[] board)
    {
        for (int i = 0; i < 3; i++)
        {
            int res = AnalizeThreePlaces(board, new int[] { i, 3 + i, 6 + i });
            if (res == P1_WIN||res == P2_WIN)
            {
                return res;
            }

        }
        return NO_WIN;
    }

    private static int HorizontalWin(int[] board)
    {
        for (int i = 0; i < 3; i++)
        {
            int res = AnalizeThreePlaces(board, new int[] { i * 3, i * 3 + 1, i * 3 + 2 });
            if (res == P1_WIN || res == P2_WIN)
            {
                return res;
            }

        }
        return NO_WIN;
    }

    private static int DiagonalWin(int[] board)
    {
        int res = AnalizeThreePlaces(board, new int[] { 0, 4, 8 });
        if (res == P1_WIN || res == P2_WIN)
        {
            return res;
        }
        res = AnalizeThreePlaces(board, new int[] { 2, 4, 6 });
        if (res == P1_WIN || res == P2_WIN)
        {
            return res;
        }
        return NO_WIN;
    }

    public void OnGetHintButtonPress()
    {
        GetHint();
    }

    //search for an empty random place and tell the player
    public int GetHint()
    {
        if (!isPVP && !isGameEnded)
        {
            int hint;

            if (n_turn < N_PLACES)
            {
                int tries = 0;
                do
                {
                    hint = Random.Range(0, N_PLACES);
                    tries++;
                    if (tries > 100)
                    {
                        Debug.Log("error in hint");
                        return -1;
                    }
                } while (gameBoard[hint] != EMPTY_SYMBOL);
                if (hintText)
                {
                    hintText.text = hint.ToString();
                    hintText.enabled = true;
                }
                return hint;
            }
            return -1;
        }
        return -1;
    }


    private void MakeComMove()
    {
        int move = AIMgr.GetNextComMove(COMDifficulty, gameBoard);
        boardMgr.DrawSpriteInPlace(move,player2Img);
        gameBoard[move] = PLAYER2_SYMBOL;
        turns[n_turn] = move;
        FinishTurn();
    }

    private void EndGame()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        isGameEnded = true;
    }

}
