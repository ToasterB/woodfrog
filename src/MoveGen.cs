using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace woodfrog
{
    class MoveGen
    {
        public void generateMoves(Board board)
        {
            if(board.colourToPlay == 0)
            {
                whitePawnMoves(board.pieces[15], board.pieces[7], board.pieces[1], board.enPassant, board);
                knightMoves(board.pieces[15], board.pieces[7], board.pieces[2], board);
            } 
            else
            {
                blackPawnMoves(board.pieces[15], board.pieces[7], board.pieces[9], board.enPassant, board);
                knightMoves(board.pieces[7], board.pieces[15], board.pieces[10], board);
            }
            
        }

        // --- Pawns ---

        private ulong pawnLeftAttack;
        private ulong pawnRightAttack;
        private ulong pawnAdvance;
        private ulong pawnDoubleAdvance;


        // Pawns moving in only one direction means it's easier to give each colour it's own function, rather than using
        // one togglable one
        private void whitePawnMoves(ulong blackPieces, ulong whitePieces, ulong whitePawns, ulong enPassants, Board board)
        {
            // The hex constants represent a board of 1's with either the H or A column set to 0's, effectively preventing pawns on the opposite outside
            // file trying to wrap around and target a square on the other side of the board
            pawnLeftAttack = ((whitePawns << 7) & (blackPieces | enPassants) & 0xfefefefefefefefe) << 1;
            pawnRightAttack = ((whitePawns << 9) & (blackPieces | enPassants) & 0x7f7f7f7f7f7f7f7f) >> 1;
            
            // Pawns can move foward provided nothing is in the way
            pawnAdvance = ((whitePawns << 8) & ~(whitePieces | blackPieces));

            // Pawns on the 2nd rank can move foward two squares provided nothing is in the way
            pawnDoubleAdvance = (((((whitePawns & 0xff00) << 8) & ~(whitePieces | blackPieces)) << 8) & ~(whitePieces | blackPieces)) >> 8;

            whitePawns <<= 8;

            // Shift through the pawn board, square by square. For each square, if it has a pawn sitting on it, and 
            // a valid corresponding target, add it to the move list
            for (int i = 63; i >= 56; i--)
            {
                if ((0x8000000000000000 & whitePawns & pawnLeftAttack) == 0x8000000000000000)
                {
                    board.attackingMoveList.Add(new Move(i - 8, i - 1, 13));
                    board.attackingMoveList.Add(new Move(i - 8, i - 1, 12));
                    board.attackingMoveList.Add(new Move(i - 8, i - 1, 11));
                    board.attackingMoveList.Add(new Move(i - 8, i - 1, 10));
                }
                if ((0x8000000000000000 & whitePawns & pawnRightAttack) == 0x8000000000000000)
                {
                    board.attackingMoveList.Add(new Move(i - 8, i + 1, 13));
                    board.attackingMoveList.Add(new Move(i - 8, i + 1, 12));
                    board.attackingMoveList.Add(new Move(i - 8, i + 1, 11));
                    board.attackingMoveList.Add(new Move(i - 8, i + 1, 10));
                }
                if ((0x8000000000000000 & whitePawns & pawnAdvance) == 0x8000000000000000)
                {
                    board.quietMoveList.Add(new Move(i - 8, i, 13));
                    board.quietMoveList.Add(new Move(i - 8, i, 12));
                    board.quietMoveList.Add(new Move(i - 8, i, 11));
                    board.quietMoveList.Add(new Move(i - 8, i, 10));
                }

                pawnLeftAttack <<= 1;
                pawnRightAttack <<= 1;
                pawnAdvance <<= 1;
                pawnDoubleAdvance <<= 1;
                whitePawns <<= 1;
            }

            for (int i = 55; i >= 24; i--)
            {
                if((0x8000000000000000 & whitePawns & pawnLeftAttack) == 0x8000000000000000)
                {
                    board.attackingMoveList.Add(new Move(i - 8, i - 1, 0));
                }
                if((0x8000000000000000 & whitePawns & pawnRightAttack) == 0x8000000000000000)
                {
                    board.attackingMoveList.Add(new Move(i - 8, i + 1, 0));
                }
                if((0x8000000000000000 & whitePawns & pawnAdvance) == 0x8000000000000000)
                {
                    board.quietMoveList.Add(new Move(i - 8, i, 0));
                }

                pawnLeftAttack <<= 1;
                pawnRightAttack <<= 1;
                pawnAdvance <<= 1;
                pawnDoubleAdvance <<= 1;
                whitePawns <<= 1;
            }

            for (int i = 23; i >= 16; i--)
            {
                if ((0x8000000000000000 & whitePawns & pawnLeftAttack) == 0x8000000000000000)
                {
                    board.attackingMoveList.Add(new Move(i - 8, i - 1, 0));
                }
                if ((0x8000000000000000 & whitePawns & pawnRightAttack) == 0x8000000000000000)
                {
                    board.attackingMoveList.Add(new Move(i - 8, i + 1, 0));
                }
                if ((0x8000000000000000 & whitePawns & pawnAdvance) == 0x8000000000000000)
                {
                    board.quietMoveList.Add(new Move(i - 8, i, 0));
                }
                if ((0x8000000000000000 & whitePawns & pawnDoubleAdvance) == 0x8000000000000000)
                {
                    board.quietMoveList.Add(new Move(i - 8, i + 8, 0));
                }

                pawnLeftAttack <<= 1;
                pawnRightAttack <<= 1;
                pawnAdvance <<= 1;
                pawnDoubleAdvance <<= 1;
                whitePawns <<= 1;
            }

        }

        // Performs the same logic as the white generator, but flipped to work for the black pawns
        private void blackPawnMoves(ulong blackPieces, ulong whitePieces, ulong blackPawns, ulong enPassants, Board board)
        {
            pawnLeftAttack = ((blackPawns >> 7) & (whitePieces | enPassants) & 0x7f7f7f7f7f7f7f7f) >> 1;
            pawnRightAttack = ((blackPawns >> 9) & (whitePieces | enPassants) & 0xfefefefefefefefe) << 1;
            pawnAdvance = ((blackPawns >> 8) & ~(whitePieces | blackPieces));
            pawnDoubleAdvance = (((((blackPawns & 0xff000000000000) >> 8) & ~(whitePieces | blackPieces)) >> 8) & ~(whitePieces | blackPieces)) << 8;
            
            blackPawns >>= 8;

            for(int i = 0; i < 8; i++)
            {
                if ((1 & blackPawns & pawnLeftAttack) == 1)
                {
                    board.attackingMoveList.Add(new Move(i + 8, i + 1, 13));
                    board.attackingMoveList.Add(new Move(i + 8, i + 1, 12));
                    board.attackingMoveList.Add(new Move(i + 8, i + 1, 11));
                    board.attackingMoveList.Add(new Move(i + 8, i + 1, 10));
                }
                if ((1 & blackPawns & pawnRightAttack) == 1)
                {
                    board.attackingMoveList.Add(new Move(i + 8, i - 1, 13));
                    board.attackingMoveList.Add(new Move(i + 8, i - 1, 12));
                    board.attackingMoveList.Add(new Move(i + 8, i - 1, 11));
                    board.attackingMoveList.Add(new Move(i + 8, i - 1, 10));
                }
                if ((1 & blackPawns & pawnAdvance) == 1)
                {
                    board.quietMoveList.Add(new Move(i + 8, i, 13));
                    board.quietMoveList.Add(new Move(i + 8, i, 12));
                    board.quietMoveList.Add(new Move(i + 8, i, 11));
                    board.quietMoveList.Add(new Move(i + 8, i, 10));
                }

                pawnLeftAttack >>= 1;
                pawnRightAttack >>= 1;
                pawnAdvance >>= 1;
                pawnDoubleAdvance >>= 1;
                blackPawns >>= 1;
            }
            for (int i = 8; i < 40; i++)
            {
                if ((1 & blackPawns & pawnLeftAttack) == 1)
                {
                    board.attackingMoveList.Add(new Move(i + 8, i + 1, 0));
                }
                if ((1 & blackPawns & pawnRightAttack) == 1)
                {
                    board.attackingMoveList.Add(new Move(i + 8, i - 1, 0));
                }
                if ((1 & blackPawns & pawnAdvance) == 1)
                {
                    board.quietMoveList.Add(new Move(i + 8, i, 0));
                }

                pawnLeftAttack >>= 1;
                pawnRightAttack >>= 1;
                pawnAdvance >>= 1;
                pawnDoubleAdvance >>= 1;
                blackPawns >>= 1;
            }
            for (int i = 40; i < 48; i++)
            {
                if ((1 & blackPawns & pawnLeftAttack) == 1)
                {
                    board.attackingMoveList.Add(new Move(i + 8, i + 1, 0));
                }
                if ((1 & blackPawns & pawnRightAttack) == 1)
                {
                    board.attackingMoveList.Add(new Move(i + 8, i - 1, 0));
                }
                if ((1 & blackPawns & pawnAdvance) == 1)
                {
                    board.quietMoveList.Add(new Move(i + 8, i, 0));
                }
                if ((1 & blackPawns & pawnDoubleAdvance) == 1)
                {
                    board.quietMoveList.Add(new Move(i + 8, i - 8, 0));
                }

                pawnLeftAttack >>= 1;
                pawnRightAttack >>= 1;
                pawnAdvance >>= 1;
                pawnDoubleAdvance >>= 1;
                blackPawns >>= 1;
            }
        }

       


        // --- Knights ---

        // Bitboard mask of the knight attack pattern for every square on the board
        ulong[] knightPatterns = new ulong[]
        {
            0x20400           , 0x50800           , 0xa1100           , 0x142200          , 0x284400          , 0x508800          , 0xa01000          , 0x402000          ,
            0x2040004         , 0x5080008         , 0xa110011         , 0x14220022        , 0x28440044        , 0x50880088        , 0xa0100010        , 0x40200020        ,
            0x204000402       , 0x508000805       , 0xa1100110a       , 0x1422002214      , 0x2844004428      , 0x5088008850      , 0xa0100010a0      , 0x4020002040      ,
            0x20400040200     , 0x50800080500     , 0xa1100110a00     , 0x142200221400    , 0x284400442800    , 0x508800885000    , 0xa0100010a000    , 0x402000204000    ,
            0x2040004020000   , 0x5080008050000   , 0xa1100110a0000   , 0x14220022140000  , 0x28440044280000  , 0x50880088500000  , 0xa0100010a00000  , 0x40200020400000  ,
            0x204000402000000 , 0x508000805000000 , 0xa1100110a000000 , 0x1422002214000000, 0x2844004428000000, 0x5088008850000000, 0xa0100010a0000000, 0x4020002040000000,
            0x400040200000000 , 0x800080500000000 , 0x1100110a00000000, 0x2200221400000000, 0x4400442800000000, 0x8800885000000000, 0x100010a000000000, 0x2000204000000000,
            0x4020000000000   , 0x8050000000000   , 0x110a0000000000  , 0x22140000000000  , 0x44280000000000  , 0x88500000000000  , 0x10a00000000000  , 0x20400000000000
        };
        int[] knight = new int[2];
        ulong[] knightAttacks = new ulong[2];

        private void knightMoves(ulong friendlyPieces, ulong hostilePieces, ulong friendlyKnights, Board board)
        {
            if (friendlyKnights != 0)
            {
                // By counting the number of 0's before the most significant bit, and before the least significant bit, 
                // we can find the square addresses of the two knights
                Array.Clear(knight, 0, 2);
                knight[0] = BitOperations.TrailingZeroCount(friendlyKnights);
                knight[1] = 63 - BitOperations.LeadingZeroCount(friendlyKnights);

                // If there is at least one knight on the board generate its moves
                if ((knight[0] | knight[1]) >= 0)
                {
                    knightAttacks[0] = knightPatterns[knight[0]] & ~friendlyPieces;
                    knightAttacks[1] = knightPatterns[knight[1]] & ~friendlyPieces;
                    for (int x = 0; x < 2; x++)
                    {
                        for (int i = 0; i < 64; i++)
                        {
                            if ((1 & (hostilePieces >> i) & knightAttacks[x]) == 1)
                            {
                                board.attackingMoveList.Add(new Move(knight[x], i, 0));
                            }
                            else if ((1 & knightAttacks[x]) == 1)
                            {
                                board.quietMoveList.Add(new Move(knight[x], i, 0));
                            }


                            knightAttacks[x] >>= 1;
                        }

                        // Don't repeat the move generation if there's only one knight on the board
                        if (knight[0] == knight[1]) break;
                    }
                }
            }
        }


    }


}
