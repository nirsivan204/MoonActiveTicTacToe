using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ButtonTests
    {

        [Test]
        public void testHintButtonFullBoard()
        {
            gameMgr gm = new gameMgr();
            gm.IsPVP = false;
            gm.IsGameEnded = false;
            gm.N_turn = 9;
            gm.GameBoard = new int[9] { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            Assert.AreEqual(-1, gm.GetHint());
        }

        [Test]
        public void testHintButtonAlmostFullBoard()
        {
            gameMgr gm = new gameMgr();
            gm.IsPVP = false;
            gm.IsGameEnded = false;
            gm.N_turn = 8;
            gm.GameBoard = new int[9] { 1, 1, 1, 1, 1, 0, 1, 1, 1 };
            Assert.AreEqual(5, gm.GetHint());
            gm.GameBoard = new int[9] { 1, 1, 0, 1, 1, 1, 1, 1, 1 };
            Assert.AreEqual(2, gm.GetHint());
            gm.GameBoard = new int[9] { 0, 1, 1, 1, 1, 1, 1, 1, 1 };
            Assert.AreEqual(0, gm.GetHint());
            gm.GameBoard = new int[9] { 1, 1, 1, 1, 1, 1, 1, 1, 0 };
            Assert.AreEqual(8, gm.GetHint());
            gm.GameBoard = new int[9] { 1, 1, 1, 1, 1, 1, 1, 0, 1 };
            Assert.AreEqual(7, gm.GetHint());
        }

        [Test]
        public void testHintButtonAlmostEemptyBoard()
        {
            gameMgr gm = new gameMgr();
            gm.IsPVP = false;
            gm.IsGameEnded = false;
            gm.N_turn = 1;
            gm.GameBoard = new int[9] { 0, 0, 0, 0, 1, 0, 0, 0, 0 };
            for (int i = 0; i < 20; i++)
            {
                int hint = gm.GetHint();
                Assert.Less(hint, 9);
                Assert.GreaterOrEqual(hint, 0);
                Assert.AreNotEqual(hint, 4);
            }
            gm.GameBoard = new int[9] { 1, 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0; i < 20; i++)
            {
                int hint = gm.GetHint();
                Assert.Less(hint, 9);
                Assert.GreaterOrEqual(hint, 0);
                Assert.AreNotEqual(hint, 0);
            }

            gm.GameBoard = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 1 };

            for (int i = 0; i < 20; i++)
            {
                int hint = gm.GetHint();
                Assert.Less(hint, 9);
                Assert.GreaterOrEqual(hint, 0);
                Assert.AreNotEqual(hint, 8);
            }
        }

        [Test]
        public void testHintButtonOnlyInPVE()
        {
            gameMgr gm = new gameMgr();
            gm.IsPVP = true;
            gm.IsGameEnded = false;
            int[] board = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Assert.AreEqual(-1, gm.GetHint());
        }

        [Test]
        public void testUndoButtonOnlyInPVE()
        {
            gameMgr gm = new gameMgr();
            gm.IsPVP = true;
            gm.IsGameEnded = false;
            gm.GameBoard = new int[9] { 1, 2, 1, 2, 0, 0, 0, 0, 0 };
            int[] boardCopy = (int[]) gm.GameBoard.Clone();
            gm.Turns = new int[9] { 0, 1, 2, 3, -1, -1, -1, -1, -1 };
            int[] turnsCopy = (int[]) gm.Turns.Clone();
            gm.N_turn = 4;
            gm.Undo();
            Assert.AreEqual(boardCopy, gm.GameBoard);
            Assert.AreEqual(4, gm.N_turn);
            Assert.AreEqual(turnsCopy, gm.Turns);

        }

        [Test]
        public void testUndoButton()
        {
            gameMgr gm = new gameMgr();
            gm.IsPVP = false;
            gm.IsGameEnded = false;
            gm.GameBoard = new int[9] { 1, 2, 1, 2, 0, 0, 0, 0, 0 };
            gm.Turns = new int[9] { 0, 1, 2, 3, -1, -1, -1, -1, -1 };
            gm.N_turn = 4;
            gm.Undo();
            Assert.AreEqual(new int[]{ 1, 2, 0, 0, 0, 0, 0, 0, 0 }, gm.GameBoard);
            Assert.AreEqual(2, gm.N_turn);
            Assert.AreEqual(new int[] { 0, 1, -1, -1, -1, -1, -1, -1, -1 }, gm.Turns);


            gm.GameBoard = new int[9] { 1, 1, 0, 1, 0, 2, 0, 0, 2 };
            gm.Turns = new int[9] { 0, 8, 3, 5, 1, -1, -1, -1, -1 };
            gm.N_turn = 5;
            gm.Undo();
            Assert.AreEqual(new int[] { 1, 0, 0, 1, 0, 0, 0, 0, 2 }, gm.GameBoard);
            Assert.AreEqual(3, gm.N_turn);
            Assert.AreEqual(new int[] { 0, 8, 3, -1, -1, -1, -1, -1, -1 }, gm.Turns);
        }
    }
}
