﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace woodfrog
{
    class Board
    {
        // Bit board declarations
        public ulong[] pieces { get; private set; } = new ulong[16];  // 1 -> 6  (0001 -> 0110) = White Pawn, Knight, Bishop, Rook, Queen and King respectively
                                                                      // 9 -> 14 (1001 -> 1110) = Black 
                                                                      // 7 = Combined White Pieces      15 = Combined Black Pieces
        public ulong enPassant { get; private set; }

        ulong[] castlingRights = new ulong[4];      // White Kingside, White Queenside, Black Kingside, Black Queenside
        public int colourToPlay { get; private set; }
        ulong halfMoveCounter;

        public List<Move> attackingMoveList = new List<Move>();
        public List<Move> quietMoveList = new List<Move>();

        // Reduntant piece positions, indexed by square. Makes it easier to find which pieces are on which squares
        int[] boardMailBox = new int[64]; 

        public Board(string fen)
        {
            string[] fenFields = fen.Split(" ");
            
            // Loop through each rank in the fen string
            string[] rankPieces = fenFields[0].Split("/");
            char testChar;
            int charIndex;
            for(int i = 0; i < 8; i++)
            {
                charIndex = 0;
                // For each character, check if it is a letter (piece) or number (representing a number of blank squares)
                for(int j = 0; j < 8; j++)
                {
                    testChar = rankPieces[i][charIndex];
                    // If the character is a number, skip ahead the specified number of blank squares
                    if (Char.IsDigit(testChar))
                    {
                        j += (int)Char.GetNumericValue(testChar) - 1;
                    }
                    // Otherwise, add the specified piece to our board representation
                    else
                    {
                        boardMailBox[(56 - i * 8) + j] = ChessIO.pieceToInt[testChar];
                        pieces[ChessIO.pieceToInt[testChar]] |= (ulong)1 << (56 - i * 8) + j;
                    }
                    charIndex++;
                }
            }

            foreach(ulong bb in pieces[1..6])
            {
                pieces[7] |= bb;
            }
            foreach(ulong bb in pieces[9..14])
            {
                pieces[15] |= bb;
            }

            // Colour to play
            colourToPlay = (fenFields[1] == "w" ? 0 : 1);

            // Castling rights
            for(int i = 0; i < 4; i++)
            {
                char[] temp = new char[] { 'K', 'Q', 'k', 'q' };
                castlingRights[i] = (ulong)(fenFields[2].Contains(temp[i]) ? 1 : 0);
            }

            // En passant square
            if(fenFields[3] != "-")
            {
                enPassant |= (ulong)1 << ChessIO.algSquareToInt(fenFields[3]);
            }

            // Halfmove clock
            halfMoveCounter = (ulong)Convert.ToInt32(fenFields[4]);
        }

        // If no board set up is given, initialise a starting board
        public Board() : this("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") { }


        // Move() moves the piece at the given origin square to the given destination square, as well as
        // performing the extra changes needed to moves that involve castling or en passant. It assumes that 
        // all moves inputted are valid, legal moves

        ulong originBB = 0;
        ulong targetBB = 0;
        ulong originTargetBB = 0;
        int movedPiece = 0;
        int capturedPiece = 0;
        Stack<ulong> gamestateStack = new Stack<ulong>();

        public void Move(Move inputMove)
        {
            // Save the enPassantBB before the move, so that we can unmake the move later
            gamestateStack.Push(enPassant);
            foreach(ulong x in castlingRights) 
            { 
                gamestateStack.Push(x); 
            }
            gamestateStack.Push(halfMoveCounter);

            // Set up move bitboards
            originBB = (ulong)1 << inputMove.origin;
            targetBB = (ulong)1 << inputMove.target;
            originTargetBB = originBB | targetBB;

            // Set variables for the pieces being moved and captured (if applicable)
            movedPiece = boardMailBox[inputMove.origin];
            capturedPiece = boardMailBox[inputMove.target];
            

            // Increment halfmove counter
            if(capturedPiece == 0 && (movedPiece & ~(1 << 3)) != 1)
            {
                halfMoveCounter++;
            }
            else
            {
                halfMoveCounter = 0;
            }

            // Move target piece to target square, and removed captured piece
            pieces[movedPiece] ^= originTargetBB;
            pieces[boardMailBox[inputMove.target]] &= ~targetBB;
            boardMailBox[inputMove.target] = movedPiece;
            boardMailBox[inputMove.origin] = 0;

            // If move is an en passant capture
            if ((targetBB & enPassant) != 0)
            {
                pieces[9 ^ (colourToPlay << 3)] &= ~((ulong)1 << (inputMove.target - (colourToPlay == 0 ? 8 : -8)));
                boardMailBox[inputMove.target - (colourToPlay == 0 ? 8 : -8)] = 0;
                capturedPiece = 1 | ((colourToPlay ^ 1) << 3);
            }
            enPassant = 0;

            // If the move pushed a pawn two foward, update the en passant board
            if(movedPiece == 1 && inputMove.target - inputMove.origin == 16)
            {
                enPassant |= (ulong)1 << (inputMove.origin + 8);
            }
            else if(movedPiece == 9 && inputMove.origin - inputMove.target == 16)
            {
                enPassant |= (ulong)1 << (inputMove.origin - 8);
            }

            // If a rook moved, remove castling rights for that direction for that colour
            else if(castlingRights[0] == 1 && movedPiece == 4 && inputMove.origin == 0)
            {
                castlingRights[0] = 0;
            } 
            else if(castlingRights[1] == 1 && movedPiece == 4 && inputMove.origin == 7)
            {
                castlingRights[1] = 0;
            }
            else if (castlingRights[2] == 1 && movedPiece == 12 && inputMove.origin == 63)
            {
                castlingRights[2] = 0;
            }
            else if(castlingRights[3] == 1 && movedPiece == 12 && inputMove.origin == 56)
            {
                castlingRights[3] = 0;
            }

            // If king moved, remove castling rights for both directions for that colour
            else if ((castlingRights[0] == 1 || castlingRights[1] == 1) && movedPiece == 6)
            {
                castlingRights[0] = 0;
                castlingRights[1] = 0;

                // If the king was castling, then move the rook to the correct place as well
                if(inputMove.target == 2)
                {
                    boardMailBox[0] = 0;
                    boardMailBox[3] = 4;
                    pieces[4] ^= 0b1001;
                }
                else if(inputMove.target == 6)
                {
                    boardMailBox[7] = 0;
                    boardMailBox[5] = 4;
                    pieces[4] ^= 0b101 << 5;
                }
            } 
            // Similar to above, but for black
            else if((castlingRights[2] == 1 || castlingRights[3] == 1) && movedPiece == 14)
            {
                castlingRights[2] = 0;
                castlingRights[3] = 0;

                if (inputMove.target == 58)
                {
                    boardMailBox[56] = 0;
                    boardMailBox[58] = 12;
                    pieces[12] ^= (ulong)0b1001 << 56;
                }
                else if (inputMove.target == 62)
                {
                    boardMailBox[63] = 0;
                    boardMailBox[61] = 12;
                    pieces[12] ^= (ulong)0b101 << 61;
                }
            }

            // If promoting pawn, change it to the promotion piece
            else if(inputMove.promotionPiece != 0)
            {
                boardMailBox[inputMove.target] = inputMove.promotionPiece;
                pieces[movedPiece] &= ~((ulong)1 << inputMove.target);
                pieces[inputMove.promotionPiece] |= (ulong)1 << inputMove.target;
            }

            gamestateStack.Push((ulong)capturedPiece);
            colourToPlay ^= 1;
        }

        //
        public void unMove(Move inputMove)
        {
            colourToPlay ^= 1;

            capturedPiece = (int)gamestateStack.Pop();
            halfMoveCounter = gamestateStack.Pop();
            for(int x = 3; x >= 0; x--)
            {
                castlingRights[x] = gamestateStack.Pop();
            }
            enPassant = gamestateStack.Pop();
            

            // Set up move bitboards
            originBB = (ulong)1 << inputMove.origin;
            targetBB = (ulong)1 << inputMove.target;
            originTargetBB = originBB | targetBB;

            // If promoting pawn, change it back to a pawn
            if (inputMove.promotionPiece != 0)
            {
                boardMailBox[inputMove.target] = 1 | ((colourToPlay ^ 1) << 3);
                pieces[boardMailBox[inputMove.target]] |= (ulong)1 << inputMove.target;
                pieces[inputMove.promotionPiece] &= ~((ulong)1 << inputMove.target);
            }

            // Move piece back to original square
            pieces[boardMailBox[inputMove.target]] ^= originTargetBB;
            boardMailBox[inputMove.origin] = boardMailBox[inputMove.target];
            boardMailBox[inputMove.target] = 0;

            // If the move was an en passant capture, put the pawn back in the correct position
            if((enPassant & ((ulong)1 << inputMove.target)) > 1)
            {
                pieces[9 ^ (colourToPlay << 3)] |= (ulong)1 << (inputMove.target - (colourToPlay == 0 ? 8 : -8));
                boardMailBox[inputMove.target - (colourToPlay == 0 ? 8 : -8)] = capturedPiece;
            } 
            // Otherwise put captured piece back in the target square
            else
            {
                boardMailBox[inputMove.target] = capturedPiece;
                pieces[capturedPiece] |= (ulong)1 << inputMove.target;
            }

            // If the move was castling, put the rook back in the right place
            if ((castlingRights[0] == 1 || castlingRights[1] == 1) && boardMailBox[inputMove.origin] == 6)
            {
                if (inputMove.target == 2)
                {
                    boardMailBox[0] = 4;
                    boardMailBox[3] = 0;
                    pieces[4] ^= 0b1001;
                }
                else if (inputMove.target == 6)
                {
                    boardMailBox[7] = 4;
                    boardMailBox[5] = 0;
                    pieces[4] ^= 0b101 << 5;
                }
            }
            else if ((castlingRights[2] == 1 || castlingRights[3] == 1) && boardMailBox[inputMove.origin] == 14)
            {
                if (inputMove.target == 58)
                {
                    boardMailBox[56] = 12;
                    boardMailBox[58] = 0;
                    pieces[12] ^= (ulong)0b1001 << 56;
                }
                else if (inputMove.target == 62)
                {
                    boardMailBox[63] = 12;
                    boardMailBox[61] = 0;
                    pieces[12] ^= (ulong)0b101 << 61;
                }
            }

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
                    if ((pieces[i] >> x & 1) == 1)
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
                    if ((pieces[i] >> x & 1) == 1)
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

            Console.Write("\n");
            Console.WriteLine("EnPassantBoard: {0}", enPassant);

            Console.Write("Castling Rights: ");
            for (int i = 0; i < 4; i++)
            {
                char[] temp = new char[] { 'K', 'Q', 'k', 'q' };
                if(castlingRights[i] == 1) { Console.Write(temp[i]); }
            }
            
            Console.Write("\n");
            Console.WriteLine("Colour to play: " + (colourToPlay == 0 ? "w" : "b"));

            Console.WriteLine("Halfmove Clock: {0}", halfMoveCounter);
        }

    }
}
