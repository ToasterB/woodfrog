using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace woodfrog
{
    static class ChessIO
    {
        public static Dictionary<char, int> pieceToInt = new Dictionary<char, int> { { 'P', 1 }, { 'N', 2 }, { 'B', 3 }, { 'R', 4 }, { 'Q', 5 }, { 'K', 6 },
                                                                                     { 'p', 9 }, { 'n', 10 }, { 'b', 11 }, { 'r', 12 }, { 'q', 13 }, { 'k', 14 },};
        public static Dictionary<int, char> intToPiece = new Dictionary<int, char> { { 1, 'P'}, { 2, 'N' }, { 3, 'B'}, { 4, 'R' }, { 5, 'Q'}, { 6, 'K'},
                                                                                     { 9,'p'}, { 10, 'n'}, { 11, 'b' }, { 12, 'r' }, { 13, 'q' }, { 14, 'k' },};

        public static Dictionary<char, int> fileToInt = new Dictionary<char, int> { { 'a', 0 }, { 'b', 1 }, { 'c', 2 }, { 'd', 3 }, { 'e', 4 }, { 'f', 5 }, { 'g', 6 }, { 'h', 7 }};
        public static Dictionary<int, char> intToFile = new Dictionary<int, char> { { 0 ,'a' }, { 1, 'b' }, { 2, 'c' }, { 3, 'd' }, { 4, 'e' }, { 5, 'f' }, { 6, 'g' }, { 7, 'h' }};


        public static int algSquareToInt(string square)
        {
            return fileToInt[square[0]] + (int.Parse(square[1].ToString()) - 1) * 8;
        }
    }
}
