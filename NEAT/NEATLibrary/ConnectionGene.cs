using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class ConnectionGene
    {
        
        public int inNode { get; }
        public int outNode { get; }
        public Double Weight;
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


        public ConnectionGene(ConnectionGene gene) // Cloning
        {
            inNode = gene.inNode;
            outNode = gene.outNode;
            Weight = gene.Weight;
            isEnabled = gene.isEnabled;
            Innovation = gene.Innovation;
        }

        public void Disable() 
        {
            isEnabled = false;
        }

        public bool isEqualNodes(ConnectionGene gene, bool isTwoWay)
        {
            if (isTwoWay)
            {
                return ((inNode == gene.inNode && outNode == gene.outNode) || (inNode == gene.outNode && outNode == gene.inNode));
            }
            else
            {
                return (inNode == gene.inNode && outNode == gene.outNode);
            }
            
        }

        public bool isEqualNodes(int _in, int _out, bool isTwoWay)
        {
            if (isTwoWay)
            {
                return ((inNode == _in && outNode == _out) || (inNode == _out && outNode == _in));
            }
            else
            {
                return (inNode == _in && outNode == _out);
            }
        }

    }
}
