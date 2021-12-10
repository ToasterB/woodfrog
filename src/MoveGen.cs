using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace woodfrog
{
    class MoveGen
    {
        public void generateMoves(Board board)
        {
            if(board.colourToPlay == 0)
            {
                whitePawnMoves(board.pieces[15], board.pieces[7], board.pieces[1], board.enPassant, board);
            } 
            else
            {
                blackPawnMoves(board.pieces[15], board.pieces[7], board.pieces[9], board.enPassant, board);
            }
            
        }


        ulong pawnLeftAttack;
        ulong pawnRightAttack;
        ulong pawnAdvance;
        ulong pawnDoubleAdvance;

        private void whitePawnMoves(ulong blackPieces, ulong whitePieces, ulong whitePawns, ulong enPassants, Board board)
        {
            // The hex constants represent a board of 1's with either the H or A column set to 0's, effectively preventing pawns on the opposite outside
            // file trying to wrap around and target a square on the other side of the board
            pawnLeftAttack = (whitePawns << 7) & (blackPieces | enPassants) & 0x7f7f7f7f7f7f7f7f;
            pawnRightAttack = (whitePawns << 9) & (blackPieces | enPassants) & 0xfefefefefefefefe;

            // Pawns can move foward provided nothing is in the way
            pawnAdvance = (whitePawns << 8) & ~(whitePieces | blackPieces);

            // Pawns on the 2nd rank can move foward two squares provided nothing is in the way
            pawnDoubleAdvance = ((((whitePawns & 0xff00) << 8) & ~(whitePieces | blackPieces)) << 8) & ~(whitePieces | blackPieces);


            // Shift through the pawn board, square by square. For each square, if it has a pawn sitting on it, and 
            // a valid corresponding target, add it to the move list
            for(int i = 0; i < 64; i++)
            {
                if((1 & whitePawns & (pawnLeftAttack >> 7)) == 1)
                {
                    board.attackingMoveList.Add(new Move(i, i + 7, 0));
                }
                if((1 & whitePawns & (pawnRightAttack >> 9)) == 1)
                {
                    board.attackingMoveList.Add(new Move(i, i + 9, 0));
                }
                if((1 & whitePawns & (pawnAdvance >> 8)) == 1)
                {
                    board.quietMoveList.Add(new Move(i, i + 8, 0));
                }
                if((1 & whitePawns & (pawnDoubleAdvance >> 16)) == 1)
                {
                    board.quietMoveList.Add(new Move(i, i + 16, 0));
                }

                pawnLeftAttack >>= 1;
                pawnRightAttack >>= 1;
                pawnAdvance >>= 1;
                pawnDoubleAdvance >>= 1;
                whitePawns >>= 1;
            }
        }

        // Performs the same logic as the white generator, but flipped to work for the black pawns
        private void blackPawnMoves(ulong blackPieces, ulong whitePieces, ulong blackPawns, ulong enPassants, Board board)
        {
            pawnLeftAttack = (blackPawns >> 9) & (whitePieces | enPassants) & 0xfefefefefefefefe;
            pawnRightAttack = (blackPawns >> 7) & (blackPieces | enPassants) & 0x7f7f7f7f7f7f7f7f;
            pawnAdvance = (blackPawns >> 8) & ~(whitePieces | blackPieces);
            pawnDoubleAdvance = ((((blackPawns & 0xff000000000000) >> 8) & ~(whitePieces | blackPieces)) >> 8) & ~(whitePieces | blackPieces);

            for (int i = 0; i < 64; i++)
            {
                if ((1 & blackPawns & (pawnLeftAttack << 7)) == 1)
                {
                    board.attackingMoveList.Add(new Move(i, i + 7, 0));
                }
                if ((1 & blackPawns & (pawnRightAttack << 9)) == 1)
                {
                    board.attackingMoveList.Add(new Move(i, i + 9, 0));
                }
                if ((1 & blackPawns & (pawnAdvance << 8)) == 1)
                {
                    board.quietMoveList.Add(new Move(i, i + 8, 0));
                }
                if ((1 & blackPawns & (pawnDoubleAdvance << 16)) == 1)
                {
                    board.quietMoveList.Add(new Move(i, i + 16, 0));
                }

                pawnLeftAttack >>= 1;
                pawnRightAttack >>= 1;
                pawnAdvance >>= 1;
                pawnDoubleAdvance >>= 1;
                blackPawns >>= 1;
            }
        }
    }
}
