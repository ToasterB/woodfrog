using System;
using System.Threading;

namespace woodfrog
{
    class Program
    {
        static void Main(string[] args)
        {
            testScript();
        }

        static void testScript()
        {
            Board gameBoard = new Board("4k2r/P3p3/8/5P2/5p2/8/p3P3/4K2R w Kk - 0 1");
            gameBoard.printBoard();
            Move inputMove;

            string[] testMoves = new string[] {"e2e4", "f4e3", "e1g1", "e7e5", "f5e6", "e8g8", "a7a8q", "a2a1n" };

            for(int x = 0; x < testMoves.Length; x++)
            {
                Console.Write("\n");
                /*if(x >= 0)
                {
                    _ = Console.ReadLine();
                }*/
                inputMove = Move.UCItoMove(testMoves[x]);
                gameBoard.Move(inputMove);
                gameBoard.printBoard();

            }

            Console.WriteLine("Moves finished. Unmoving: \n\n");

            for (int x = testMoves.Length - 1; x >= 0; x--)
            {
                Console.Write("\n");

                if (x <= testMoves.Length - 4)
                {
                    _ = Console.ReadLine();
                }
                inputMove = Move.UCItoMove(testMoves[x]);
                gameBoard.unMove(inputMove);
                gameBoard.printBoard();
            }
        }
    }
}
