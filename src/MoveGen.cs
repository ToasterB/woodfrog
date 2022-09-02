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
       
        //  !!!!!
        //  Currently works on assumption that only 2 knights exist per colour, which isn't always true
        //  !!!!!
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

        // --- Sliding Pieces ---
        // Generate move bitboards for sliding pieces. Will run once at startup

        ulong[,] rookMoveboards = new ulong[64, 4096];
        ulong[,] bishopMoveboards = new ulong[64, 512];
        private void generateMoveboards()
        {
            
        }

        // --- Bishops ---

        ulong[] bishopBlockerMasks = new ulong[]
        {
            0x40201008040200  , 0x402010080400    , 0x4020100a00      , 0x40221400        , 0x2442800         , 0x204085000       , 0x20408102000     , 0x2040810204000   ,
            0x20100804020000  , 0x40201008040000  , 0x4020100a0000    , 0x4022140000      , 0x244280000       , 0x20408500000     , 0x2040810200000   , 0x4081020400000   ,
            0x10080402000200  , 0x20100804000400  , 0x4020100a000a00  , 0x402214001400    , 0x24428002800     , 0x2040850005000   , 0x4081020002000   , 0x8102040004000   ,
            0x8040200020400   , 0x10080400040800  , 0x20100a000a1000  , 0x40221400142200  , 0x2442800284400   , 0x4085000500800   , 0x8102000201000   , 0x10204000402000  ,
            0x4020002040800   , 0x8040004081000   , 0x100a000a102000  , 0x22140014224000  , 0x44280028440200  , 0x8500050080400   , 0x10200020100800  , 0x20400040201000  ,
            0x2000204081000   , 0x4000408102000   , 0xa000a10204000   , 0x14001422400000  , 0x28002844020000  , 0x50005008040200  , 0x20002010080400  , 0x40004020100800  ,
            0x20408102000     , 0x40810204000     , 0xa1020400000     , 0x142240000000    , 0x284402000000    , 0x500804020000    , 0x201008040200    , 0x402010080400    ,
            0x2040810204000   , 0x4081020400000   , 0xa102040000000   , 0x14224000000000  , 0x28440200000000  , 0x50080402000000  , 0x20100804020000  , 0x40201008040200
        };


        // --- Rooks ---

        ulong[] rookBlockerMasks = new ulong[]
        {
            0x101010101017e   , 0x202020202027c   , 0x404040404047a   , 0x8080808080876   , 0x1010101010106e  , 0x2020202020205e  , 0x4040404040403e  , 0x8080808080807e  ,
            0x1010101017e00   , 0x2020202027c00   , 0x4040404047a00   , 0x8080808087600   , 0x10101010106e00  , 0x20202020205e00  , 0x40404040403e00  , 0x80808080807e00  ,
            0x10101017e0100   , 0x20202027c0200   , 0x40404047a0400   , 0x8080808760800   , 0x101010106e1000  , 0x202020205e2000  , 0x404040403e4000  , 0x808080807e8000  ,
            0x101017e010100   , 0x202027c020200   , 0x404047a040400   , 0x8080876080800   , 0x1010106e101000  , 0x2020205e202000  , 0x4040403e404000  , 0x8080807e808000  ,
            0x1017e01010100   , 0x2027c02020200   , 0x4047a04040400   , 0x8087608080800   , 0x10106e10101000  , 0x20205e20202000  , 0x40403e40404000  , 0x80807e80808000  ,
            0x17e0101010100   , 0x27c0202020200   , 0x47a0404040400   , 0x8760808080800   , 0x106e1010101000  , 0x205e2020202000  , 0x403e4040404000  , 0x807e8080808000  ,
            0x7e010101010100  , 0x7c020202020200  , 0x7a040404040400  , 0x76080808080800  , 0x6e101010101000  , 0x5e202020202000  , 0x3e404040404000  , 0x7e808080808000  ,
            0x7e01010101010100, 0x7c02020202020200, 0x7a04040404040400, 0x7608080808080800, 0x6e10101010101000, 0x5e20202020202000, 0x3e40404040404000, 0x7e80808080808000
        };

    }


}
