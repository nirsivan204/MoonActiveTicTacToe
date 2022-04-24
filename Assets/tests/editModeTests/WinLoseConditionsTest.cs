using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class WinLoseConditionsTest
    {
        int x = gameMgr.PLAYER1_SYMBOL;
        int o = gameMgr.PLAYER2_SYMBOL;
        int n = gameMgr.EMPTY_SYMBOL;
        [Test]
        public void WinLoseTest()
        {
            Assert.AreEqual(gameMgr.P1_WIN, gameMgr.IsWin(new int[9] { x, x, x, n, o, o, n, n, n })); //horizontal p1 win
            Assert.AreEqual(gameMgr.P1_WIN, gameMgr.IsWin(new int[9] { x, n, o, x, o, n, x, n, n })); //vertical p1 win
            Assert.AreEqual(gameMgr.P2_WIN, gameMgr.IsWin(new int[9] { o, x, n, n, o, x, x, n, o })); //diagonal p1 lose
            Assert.AreEqual(gameMgr.P2_WIN, gameMgr.IsWin(new int[9] { n, x, o, n, o, x, o, n, x })); //diagonal p1 lose
            Assert.AreEqual(gameMgr.NO_WIN, gameMgr.IsWin(new int[9] { n, x, x, n, o, o, n, n, n })); //no win or lose
            Assert.AreEqual(gameMgr.NO_WIN, gameMgr.IsWin(new int[9] { x, x, n, n, o, o, n, o, x })); //no win or lose
        }
        [Test]
        public void DrawTest()
        {
            gameMgr gm = new gameMgr();
            gm.IsPVP = true;
            gm.IsGameEnded = false;
            gm.GameBoard = new int[9] { x, o, x, o, o, x, n, x, o };
            gm.Turns = new int[9] { 0, 1, 2, 3, 5, 4, 7, 8, -1 };
            gm.N_turn = 8;
            gm.ChooseBoardPlace(6);
            Assert.AreEqual(gameMgr.NO_WIN, gameMgr.IsWin(gm.GameBoard)); //no winner
            Assert.IsTrue(gm.IsGameEnded); //game ended with no winner
        }
    }
}
