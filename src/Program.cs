using System;

namespace woodfrog
{
    class Program
    {
        static void Main(string[] args)
        {
            Board gameBoard = new Board("k7/4P3/8/8/8/8/4p3/K7 w - - 0 1");

            while (true)
            {
                gameBoard.printBoard();
                Console.Write("\n");

                Console.Write("Input Move: ");
                Move inputMove = Move.UCItoMove(Console.ReadLine());
                gameBoard.Move(inputMove);
            }
        }
    }
}
