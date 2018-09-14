using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class ConnectionGene
    {
        public int inNode { get; }
        public int outNode { get; }
        public Double Weight { get; set; }
        public bool isEnabled { get; private set; }
        public int Innovation { get; }


        public ConnectionGene(int _in, int _out, double _weight, bool _expressed, GeneMarker marker)
        {
            inNode = _in;
            outNode = _out;
            Weight = _weight;
            isEnabled = _expressed;
            Innovation = marker.getMarker();
        }

        public void Disable() 
        {
            isEnabled = false;
        }

        public bool isEqualNodes(ConnectionGene gene)
        {
            return( (inNode == gene.inNode && outNode == gene.outNode) || (inNode == gene.outNode && outNode == gene.inNode) );
        }

        public bool isEqualNodes(int _in, int _out)
        {
            return ((inNode == _in && outNode == _out) || (inNode == _out && outNode == _in));
        }

    }
}
