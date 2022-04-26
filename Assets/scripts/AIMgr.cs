using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AIMgr
{
    private static int FindIndex(bool isRow, int i, int j)
    {
        if (isRow)
            return 3 * j + i;
        else
            return 3 * i + j;
    }


    //The easy computer startegy, is just random behavior.
    public static int EasyCom(int[] board)
    {
        int move;
        int tries = 0;
        do
        {
            move = Random.Range(0, gameMgr.N_PLACES);
            tries++;
            if (tries > 100)
            {
                Debug.Log("error in com move");
                break;
            }
        } while (board[move] != gameMgr.EMPTY_SYMBOL);
        return move;
    }
    /*  The medium computer startegy, is checking if there is almost a win, and if so,  choosing the last place in the line (block the opponent, or win)
     *  If no line of almost win, do a random choise
    */
    public static int MediumCom(int[] board)
    {
        int res;
        bool[] isRowOrCol = { true, false };
        foreach (bool b in isRowOrCol)
        {
            for (int j = 0; j < 3; j++)
            {
                res = gameMgr.AnalizeThreePlaces(board, new int[] { FindIndex(b, 0, j), FindIndex(b, 1, j), FindIndex(b, 2, j) });
                if (0 <= res && res < 9)
                {
                    return res;
                }
            }
        }
        res = gameMgr.AnalizeThreePlaces(board, new int[] { 0, 4, 8 });
        if (0 <= res && res < 9)
        {
            return res;
        }
        res = gameMgr.AnalizeThreePlaces(board, new int[] { 2, 4, 6 });
        if (0 <= res && res < 9)
        {
            return res;
        }
        return EasyCom(board);
    }
    public static int HardCom(int[] board)
    {
        int placeToChoose;
        MinMax(board, FindNumEmptyPlaces(board), true, out placeToChoose);
        return placeToChoose;
    }

    private static int FindNumEmptyPlaces(int[] board)
    {
        int counter = 0;
        for (int i = 0; i < gameMgr.N_PLACES; i++)
        {
            if (board[i] != gameMgr.EMPTY_SYMBOL)
            {
                counter++;
            }
        }
        return counter;
    }
    /* The MinMax algorithm. it is recursive function that keeps scores for every move, while trying to maximize the score for the computer, and minimizing it for the player.
     * If a terminal state is found return the score. 1 for win, 0 for draw, -1 for lose.
     * Go through empty places on the board, and try putting the symbol in this place, and call the function again to calculate the scores.
     * If com's turn, take the option with the best score. If player's turn, take the worst score.
     * Return the score, and the move lead to this score.
     */
    private static int MinMax(int[] board, int moves, bool isComTurn, out int placeToChoose)
    {
        int winner = gameMgr.IsWin(board);
        placeToChoose = -1;
        if (winner == gameMgr.P1_WIN)
        {
            return -1;
        }
        if (winner == gameMgr.P2_WIN)
        {
            return 1;
        }
        if (moves == gameMgr.N_PLACES)
        {
            return 0;
        }
        int[] scores = new int[gameMgr.N_PLACES];
        if (isComTurn)
        {
            for (int i = 0; i < gameMgr.N_PLACES; i++)
            {
                scores[i] = -100;//really small number so every other score will be better.
            }
        }
        else
        {
            for (int i = 0; i < gameMgr.N_PLACES; i++)
            {
                scores[i] = 100; //really large number so every other score will be worse.
            }
        }
        for (int i = 0; i < gameMgr.N_PLACES; i++)
        {
            if (board[i] == gameMgr.EMPTY_SYMBOL)
            {
                int[] testBoard = (int[])board.Clone();
                if (isComTurn)
                {
                    testBoard[i] = gameMgr.PLAYER2_SYMBOL;
                }
                else
                {
                    testBoard[i] = gameMgr.PLAYER1_SYMBOL;
                }

                scores[i] = MinMax(testBoard, moves + 1, !isComTurn, out placeToChoose);
            }
        }
        if (isComTurn)
        {
            int maxScore = -100;
            int bestScorePlace = -1;
            for (int i = 0; i < gameMgr.N_PLACES; i++)
            {
                if (maxScore < scores[i])
                {
                    bestScorePlace = i;
                    maxScore = scores[i];
                }
            }
            placeToChoose = bestScorePlace;
            return maxScore;
        }
        else
        {
            int minScore = 100;
            int worstScorePlace = -1;
            for (int i = 0; i < gameMgr.N_PLACES; i++)
            {
                if (minScore > scores[i])
                {
                    worstScorePlace = i;
                    minScore = scores[i];
                }
            }
            placeToChoose = worstScorePlace;
            return minScore;
        }

    }

    public static int GetNextComMove(int Difficulty, int[] board)
    {
        switch (Difficulty)
        {
            case 0:
                return EasyCom(board);
            case 1:
                return MediumCom(board);
            case 2:
                return HardCom(board);
            default:
                return EasyCom(board);
        }
    }

}
