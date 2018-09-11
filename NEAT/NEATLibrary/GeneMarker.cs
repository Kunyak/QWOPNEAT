using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class GeneMarker
    {
        private int counter;

        public GeneMarker()
        {
            counter = 0;
        }

        public GeneMarker(int basenum)
        {
            counter = basenum;
        }

        public int getMarker()
        {
            return counter++;
        }

    }
}
