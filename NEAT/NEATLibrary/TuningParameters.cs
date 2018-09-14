using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class TuningParameters // Constants for tuning the evolution process
    {
        // compatibilty distance
        public static readonly double C1 = 1.0f;
        public static readonly double C2 = 1.0f;
        public static readonly double C3 = 0.4f;
        public static readonly double DIST = 10.0f;

        //mutation rates
        public static readonly double WEIGHT_MUTATION_RATE = 0.5f;
        public static readonly double NEW_CONNECTION_RATE = 0.05f;
        public static readonly double NEW_NODE_RATE = 0.1f;
        public static readonly double NEW_WEIGHT_RATE = 0.1f;

    }
}
