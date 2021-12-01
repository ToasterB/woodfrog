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
    }
}
