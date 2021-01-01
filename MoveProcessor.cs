using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveInRow
{
    class XY
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    class MoveProcessor
    {
        public static XY FindMove(List<Coordinates> allCoordinates)
        {
            List<Coordinates> ordered = allCoordinates.OrderBy(a => a.x).ToList();

            int x = ordered.Last().x + 1;

            if (x < 13) x = 13;

            return new XY { X = x, Y = 0 };
        }

    }
}
