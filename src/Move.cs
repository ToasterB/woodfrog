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
        public int capturedPiece;   // 1 -> 6  (0001 -> 0110) = White Pawn, Knight, Bishop, Rook, Queen and King respectively
                                    // 9 -> 14 (1001 -> 1110) = Black 
        public int promotionPiece;  // Same as above, but records pawn promotion

        public Move(int target, int origin, int capturedPiece,  int promotionPiece)
        {
            this.target = target;
            this.origin = origin;
            this.capturedPiece = capturedPiece;
            this.promotionPiece = promotionPiece;
        }


        

        static Dictionary<char, int> fileLookup = new() { { 'a', 0 }, { 'b', 1 }, { 'c', 2 }, { 'd', 3 }, 
                                                     { 'e', 4 }, { 'f', 5 }, { 'g', 6 }, { 'h', 7 }};
        static Dictionary<char, int> promotionLookup = new() { { 'n', 2 }, { 'b', 3 }, { 'r', 4 }, { 'q', 5 } };

        // Takes a move in uci format, and converts it to a Move object
        public static Move UCItoMove(string uciString)
        {
            Move formattedMove = new Move(0, 0, 0, 0);

            formattedMove.origin = fileLookup[uciString[0]] + (int.Parse(uciString[1].ToString()) - 1) * 8;
            formattedMove.target = fileLookup[uciString[2]] + (int.Parse(uciString[3].ToString()) - 1) * 8;

            // Pawn promotion adds a fifth letter to  designate the piece promoted to
            if(uciString.Length == 5)
            {
                formattedMove.promotionPiece = promotionLookup[uciString[4]] | ((formattedMove.target / 56) << 3);
            }

            return formattedMove;
        }
    }
}
