using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace woodfrog
{
    class Board
    {
        // Bit board declarations
        ulong[] pieceBitBoards = new ulong[16]; // 1 -> 6  (0001 -> 0110) = White Pawn, Knight, Bishop, Rook, Queen and King respectively
                                                // 9 -> 14 (1001 -> 1110) = Black 
                                                // 7 = Combined White Pieces      15 = Combined Black Pieces
        ulong[] enPassantBoards = new ulong[2];

        int[] castlingRights = new int[4];      // White Kingside, White Queenside, Black Kingside, Black Queenside
        int whiteToPlay;
        int halfMoveCounter;

        // Reduntant piece positions, indexed by square. Makes it easier to find which pieces are one which squares
        int[] boardMailBox = new int[64]; 


        // If no board set up is given, initialise a starting board
        public Board()
        {
            pieceBitBoards[0] = 0;          pieceBitBoards[8] = 0;
            pieceBitBoards[1] = 0xff00;     pieceBitBoards[9] = 0xff000000000000;
            pieceBitBoards[2] = 0x42;       pieceBitBoards[10] = 0x4200000000000000;
            pieceBitBoards[3] = 0x24;       pieceBitBoards[11] = 0x2400000000000000;
            pieceBitBoards[4] = 0x81;       pieceBitBoards[12] = 0x8100000000000000;
            pieceBitBoards[5] = 0x8;        pieceBitBoards[13] = 0x800000000000000;
            pieceBitBoards[6] = 0x10;       pieceBitBoards[14] = 0x1000000000000000;
            pieceBitBoards[7] = 0xffff;     pieceBitBoards[15] = 0xffff000000000000;

            enPassantBoards[0] = 0;         enPassantBoards[1] = 0;
            castlingRights[0] = 1;          castlingRights[2] = 1;
            castlingRights[1] = 1;          castlingRights[3] = 1;

            whiteToPlay = 0;
            halfMoveCounter = 0;

            // Rows vertically mirrored here as board counts index 0 from A1
            boardMailBox = new int[] {
                 4,  2,  3,  5,  6,  3,  2,  4,
                 1,  1,  1,  1,  1,  1,  1,  1,
                 0,  0,  0,  0,  0,  0,  0,  0,
                 0,  0,  0,  0,  0,  0,  0,  0,
                 0,  0,  0,  0,  0,  0,  0,  0,
                 0,  0,  0,  0,  0,  0,  0,  0,
                 9,  9,  9,  9,  9,  9,  9,  9,
                 12, 10, 11, 13, 14, 11, 10, 12, };
        }

        // Move() moves the piece at the given origin square to the given destination square, as well as
        // performing the extra changes needed to moves that involve castling or en passant. It assumes that 
        // all moves inputted are valid, legal moves
        public void Move(Move inputmove)
        {

        }

        //
        public void unMove()
        {

        }

        public void printBoard()
        {
            // In order to print the board, we will convert all the individual piece bitboards 
            // into an array of strings which can be printed to the console

            // Initialise string array. Each index corresponds to a board square, with an empty board represented by a period
            string[] graphicBoard = new string[64];
            Array.Fill(graphicBoard, " . ");

            // Define the symbols to be used for representing each piece
            var pieceLookup = new Dictionary<int, string> { { 1, "P " }, { 2, "N " }, { 3, "B "}, { 4, "R " }, { 5, "Q " }, { 6, "K " } };

            // Serialize white boards
            for(int i = 1; i <= 6; i++)
            {
                // Checking each bit in the bitboard. If it is set, overwrite the corresponding square with a piece of 
                // the corresponding type and colour
                for (int x = 0; x < 64; x++)
                {
                    if ((pieceBitBoards[i] >> x & 1) == 1)
                    {
                        graphicBoard[x] = "w" + pieceLookup[i];
                    }
                }
            }

            // Serialize black boards
            for (int i = 9; i <= 14; i++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if ((pieceBitBoards[i] >> x & 1) == 1)
                    {
                        graphicBoard[x] = "b" + pieceLookup[i - 8];
                    }
                }
            }

            // Loop through each square on the string array and print it to the console, 
            // adding a new line between every row
            for (int row = 7; row >= 0; row--)
            {
                for(int col = 0; col < 8; col++)
                {
                    Console.Write(graphicBoard[row * 8 + col]);
                }
                Console.Write("\n");
            }
        }

    }
}
