using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;


public class RobotOnMoon
{
    private struct TPosition
    {
        public TPosition(int x, int y)
        {
            this.X_ = x;
            this.Y_ = y;
        }

        public TPosition SimulateMove(char move)
        {
            switch (move)
            {
                case 'U': return new TPosition(X_ - 1, Y_);
                case 'D': return new TPosition(X_ + 1, Y_);
                case 'L': return new TPosition(X_, Y_ - 1);
                case 'R': return new TPosition(X_, Y_ + 1);
                default: return this;
            }
        }

        public int X_, Y_;
    }

    private struct TBoard
    {

        public enum ECode
        {
            Success = 0,
            Death = 1,
            IsWall = 2,
        };

        public TBoard(string[] board)
        {
            this.Board = board;
            this.Height = board.Length;
            this.Width = board[0].Length;
        }

        public ECode ValidatePosition(TPosition pos)
        {

            if (pos.X_ < 0 || pos.X_ >= Height ||
                pos.Y_ < 0 || pos.Y_ >= Width)
                return ECode.Death;

            if (Board[pos.X_][pos.Y_] == '#')
                return ECode.IsWall;

            return ECode.Success;
        }

        public TPosition FindStart()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    if (Board[i][j] == 'S')
                        return new TPosition(i, j);

            throw new Exception("The start was not found!");
        }

        private int Height, Width;
        private string[] Board;
    }

    public string isSafeCommand(string[] board, string moves)
    {
        TBoard b = new TBoard(board);

        TPosition current = b.FindStart();

        foreach (char move in moves)
        {

            TPosition next = current.SimulateMove(move);
            TBoard.ECode result = b.ValidatePosition(next);

            if (result == TBoard.ECode.Death)
                return "Dead";

            if (result == TBoard.ECode.Success)
                current = next;
        }

        return "Alive";
    }

    #region Testing code

    [STAThread]
    private static Boolean KawigiEdit_RunTest(int testNum, string[] p0, string p1, Boolean hasAnswer, string p2)
    {
        Console.Write("Test " + testNum + ": [" + "{");
        for (int i = 0; p0.Length > i; ++i)
        {
            if (i > 0)
            {
                Console.Write(",");
            }
            Console.Write("\"" + p0[i] + "\"");
        }
        Console.Write("}" + "," + "\"" + p1 + "\"");
        Console.WriteLine("]");
        RobotOnMoon obj;
        string answer;
        obj = new RobotOnMoon();
        DateTime startTime = DateTime.Now;
        answer = obj.isSafeCommand(p0, p1);
        DateTime endTime = DateTime.Now;
        Boolean res;
        res = true;
        Console.WriteLine("Time: " + (endTime - startTime).TotalSeconds + " seconds");
        if (hasAnswer)
        {
            Console.WriteLine("Desired answer:");
            Console.WriteLine("\t" + "\"" + p2 + "\"");
        }
        Console.WriteLine("Your answer:");
        Console.WriteLine("\t" + "\"" + answer + "\"");
        if (hasAnswer)
        {
            res = answer == p2;
        }
        if (!res)
        {
            Console.WriteLine("DOESN'T MATCH!!!!");
        }
        else if ((endTime - startTime).TotalSeconds >= 2)
        {
            Console.WriteLine("FAIL the timeout");
            res = false;
        }
        else if (hasAnswer)
        {
            Console.WriteLine("Match :-)");
        }
        else
        {
            Console.WriteLine("OK, but is it right?");
        }
        Console.WriteLine("");
        return res;
    }

    public static void Run()
    {
        Boolean all_right;
        all_right = true;

        string[] p0;
        string p1;
        string p2;

        // ----- test 0 -----
        p0 = new string[] { ".....", ".###.", "..S#.", "...#." };
        p1 = "URURURURUR";
        p2 = "Alive";
        all_right = KawigiEdit_RunTest(0, p0, p1, true, p2) && all_right;
        // ------------------

        // ----- test 1 -----
        p0 = new string[] { ".....", ".###.", "..S..", "...#." };
        p1 = "URURURURUR";
        p2 = "Dead";
        all_right = KawigiEdit_RunTest(1, p0, p1, true, p2) && all_right;
        // ------------------

        // ----- test 2 -----
        p0 = new string[] { ".....", ".###.", "..S..", "...#." };
        p1 = "URURU";
        p2 = "Alive";
        all_right = KawigiEdit_RunTest(2, p0, p1, true, p2) && all_right;
        // ------------------

        // ----- test 3 -----
        p0 = new string[] { "#####", "#...#", "#.S.#", "#...#", "#####" };
        p1 = "DRULURLDRULRUDLRULDLRULDRLURLUUUURRRRDDLLDD";
        p2 = "Alive";
        all_right = KawigiEdit_RunTest(3, p0, p1, true, p2) && all_right;
        // ------------------

        // ----- test 4 -----
        p0 = new string[] { "#####", "#...#", "#.S.#", "#...#", "#.###" };
        p1 = "DRULURLDRULRUDLRULDLRULDRLURLUUUURRRRDDLLDD";
        p2 = "Dead";
        all_right = KawigiEdit_RunTest(4, p0, p1, true, p2) && all_right;
        // ------------------

        // ----- test 5 -----
        p0 = new string[] { "S" };
        p1 = "R";
        p2 = "Dead";
        all_right = KawigiEdit_RunTest(5, p0, p1, true, p2) && all_right;
        // ------------------

        if (all_right)
        {
            Console.WriteLine("You're a stud (at least on the example cases)!");
        }
        else
        {
            Console.WriteLine("Some of the test cases had errors.");
        }
    }

    #endregion
}