using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class gameMgr : MonoBehaviour
{
    //    [SerializeField] SpriteRenderer[] boardPlacesSprites;
    public const int N_PLACES = 9;
    private const int MAX_SECONDS = 5;
    public const int EMPTY_SYMBOL = 0;
    public const int PLAYER1_SYMBOL = 1;
    public const int PLAYER2_SYMBOL = 4;
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

    public bool IsPVP { get => isPVP; set => isPVP = value; }
    public bool IsGameEnded { get => isGameEnded; set => isGameEnded = value; }
    public int N_turn { get => n_turn; set => n_turn = value; }
    public int[] GameBoard { get => gameBoard; set => gameBoard = value; }
    public int[] Turns { get => turns; set => turns = value; }
    public Sprite[] Imgs { get => imgs; set => imgs = value; }

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
                        boardMgr.drawSpriteInPlace(id, player1Img);
                    }
                    gameBoard[id] = PLAYER1_SYMBOL;
                }
                else
                {
                    if (boardMgr)
                    {
                        boardMgr.drawSpriteInPlace(id, player2Img);
                    }
                    gameBoard[id] = PLAYER2_SYMBOL;
                }
                turns[n_turn] = id;
                FinishTurn();
            }
        }
    }

    private void FinishTurn()
    {
        if (hintText)
        {
            hintText.enabled = false;
        }
        int winner = IsWin(gameBoard);
        if (winner != 0)
        {
            if (winText)
            {
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
        boardMgr.init();
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
        player2Img = imgs[1- randImgId];
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
            if(board[i] == EMPTY_SYMBOL)
            {
                if (boardMgr)
                {
                    boardMgr.drawSpriteInPlace(i, null);
                }
            }
        }
    }

    public static int IsWin(int[] board)
    {
        int winner = HorizontalWin(board);
        if (winner != 0)
        {
            return winner;
        }
        winner = VerticalWin(board);
        if (winner != 0)
        {
            return winner;
        }
        return CrossWin(board);

    }

/*    private void func()
    {
        int p1Counter = 0;
        int p2Counter = 0;
        int OCounter = 0;
        int OFirstLoc = -1;


    }*/

/*    private static int verticalWin(int[] board)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j < 3; j++)
                if (board[i] == board[i + 3] && board[i + 3] == board[i + 6] && board[i + 6] == j)
                {
                    return j;
                }
        }
        return 0;
    }*/
    private static int VerticalWin(int[] board)
    {
        int sum;
        int winner;
        for (int i = 0; i < 3; i++)
        {
            sum = board[i] + board[i + 3] + board[i + 6];
            winner = CheckSumWin(sum);
            if (winner != 0)
            {
                return winner;
            }

        }
        return 0;
    }
    private static int CrossWin(int[] board)
    {
        int sum = board[0] + board[4] + board[8];
        int winner = CheckSumWin(sum);
        if (winner != 0)
        {
            return winner;
        }
        sum = board[2] + board[4] + board[6];
        return CheckSumWin(sum);
    }

    private static int HorizontalWin(int[] board)
    {
        int sum;
        int winner;
        for (int i = 0; i< 3; i++)
        {
            sum = board[i*3] + board[i*3+1] + board[i*3 + 2];
            winner = CheckSumWin(sum);
            if (winner != 0)
            {
                return winner;
            }

        }
        return 0;
    }

    private static int CheckSumWin(int sum, bool checkAlmostWin = false)
    {
        int multiplier = 3;
        if (checkAlmostWin)
        {
            multiplier = 2;
        }
        if(sum == multiplier * PLAYER1_SYMBOL)
        {
            return 1;
        }
        if(sum == multiplier * PLAYER2_SYMBOL)
        {
            return 2;
        }
        return 0;
    }

/*    private static int crossWin(int[] board)
    {
        for (int j = 1; j < 3; j++)
        {
            if (board[0] == board[4] && board[4] == board[8] && board[8] == j)
            {
                return j;
            }
            if (board[2] == board[4] && board[4] == board[6] && board[6] == j)
            {
                return j;
            }
        }
        return 0;
    }*/

    /*    private static int horizontalWin(int[] board)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 1; j < 3; j++)
                {
                    if (board[i * 3] == board[1 + i * 3] && board[1 + i * 3] == board[2 + i * 3] && board[2 + i * 3] == j)
                    {
                        return j;
                    }
                }

            }
            return 0;
        }*/

    public void OnGetHintButtonPress()
    {
        GetHint();
    }


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

    private int EasyCom()
    {
        int move;
        int tries = 0;
        do
        {
            move = Random.Range(0, N_PLACES);
            tries++;
            if (tries > 100)
            {
                Debug.Log("error in com move");
                break;
            }
        } while (gameBoard[move] != EMPTY_SYMBOL);
        return move;
    }

    private static int FindEmptyPlaceInRow(int[] board ,int row)
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[3 * row + i] == 0)
            {
                return 3 * row + i;
            }
        }
        return -1; // error
    }

    private static int FindEmptyPlaceInCol(int[] board, int col)
    {
        for (int i = 0; i < 3; i++)
        {
            if (board[3*i + col] == 0)
            {
                return 3 * i + col;
            }
        }
        return -1; // error
    }

    /*    private int tryWin()
        {
            int counter = 0;
            for(int i = 0; i < 3; i++)
            {

            }
        }*/

    private int MediumCom()
    {
        int sum;
        for(int i=0; i<3; i++)
        {
            sum = gameBoard[i * 3] + gameBoard[i * 3 + 1] + gameBoard[i * 3 + 2]; // check row i
            if (CheckSumWin(sum, true) != 0)
            {
                return FindEmptyPlaceInRow(GameBoard,i);
            }


            sum = gameBoard[i] + gameBoard[i + 3] + gameBoard[i + 6]; //check col i
            if (CheckSumWin(sum, true) != 0)
            {
                return FindEmptyPlaceInCol(GameBoard,i);
            }

        }

        sum = gameBoard[0] + gameBoard[4] + gameBoard[8]; // check \ diagonal
        if (CheckSumWin(sum, true) != 0)
        {
            if(gameBoard[0] == 0)
            {
                return 0;
            }
            if(gameBoard[4] == 0)
            {
                return 4;
            }
            return 8;
        }
        sum = gameBoard[2] + gameBoard[4] + gameBoard[6];
        if(CheckSumWin(sum, true) != 0)
        {
            if (gameBoard[2] == 0)
            {
                return 2;
            }
            if (gameBoard[4] == 0)
            {
                return 4;
            }
            return 6;
        }
        return EasyCom();
    }

    private void MakeComMove()
    {
        int move = MediumCom();
        boardMgr.drawSpriteInPlace(move,player2Img);
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
