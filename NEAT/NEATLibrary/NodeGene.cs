using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{

    public enum NodeType
    {
        Sensor,
        Hidden,
        Output,
        Bias,
    }

    class NodeGene
    {

        public NodeType Type { get;}
        public int Id { get; }

        public NodeGene(NodeType type, int id)
        {
            Type = type;
            Id = id;
        }

    }
}
