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
        public static readonly double COMPATIBILITY_THRESHOLD = 4.0f;

        //mutation ratesS
        public static readonly double WEIGHT_MUTATION_RATE = 0.5f;
        public static readonly double NEW_CONNECTION_RATE = 0.1f;
        public static readonly double NEW_NODE_RATE = 0.1f;
        public static readonly double NEW_WEIGHT_RATE = 0.1f;

        // evaluation
        public static readonly int INPRODUCTIVITY_THRESHOLD = 15;
        public static readonly int GENERATION_INPRODUCTIVITY_THRESHOLD = 20;
        public static readonly int SAFE_SPECIES = 2;
        public static readonly double CROSSOWER_RATE = 0.8f;
        public static readonly int HISTORY_RECORD_TH = 2;

    }
}
