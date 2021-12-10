using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace woodfrog
{
    class Move
    {
        public int target;
        public int origin;
        public int promotionPiece;        // 1 -> 6  (0001 -> 0110) = White Pawn, Knight, Bishop, Rook, Queen and King respectively
                                          // 9 -> 14 (1001 -> 1110) = Black 
                                          // Same as above, but records pawn promotion

        public Move(int origin, int target, int promotionPiece)
        {
            this.target = target;
            this.origin = origin;
            this.promotionPiece = promotionPiece;
        }

        // Takes a move in uci format, and converts it to a Move object
        public static Move UCItoMove(string uciString)
        {
            Move newMove = new Move(0, 0, 0);

            newMove.origin = ChessIO.algSquareToInt(uciString.Substring(0, 2));
            newMove.target = ChessIO.algSquareToInt(uciString.Substring(2, 2));

            // Pawn promotion adds a fifth letter to designate the piece promoted to
            if(uciString.Length == 5)
            {
                newMove.promotionPiece = ChessIO.pieceToInt[uciString[4]] ^ ((newMove.target / 48) << 3);
            } else
            {
                newMove.promotionPiece = 0;
            }

            return newMove;
        }
    }
}
