using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class Node
    {
        #region Node definition
        public NodeType Type { get; }
        public int Id { get; }
        public Dictionary<int, double> Outputs; //the int is the id of the node the output is going to, the double is the connection weight
        public double input = 0;
        public double LayerQuotient { get; set; }

        public Node(NodeType type, Dictionary<int, double> outConnections, double layer, int id)
        {
            Outputs = new Dictionary<int, double>();
            Type = type;
            Id = id;
            Outputs = outConnections;
            LayerQuotient = layer;
        }

        public Node(NodeType type, double layer, int id)
        {
            Outputs = new Dictionary<int, double>();
            Type = type;
            Id = id;
            LayerQuotient = layer;
        }
        #endregion

        #region Public Methods
        public double TransferFunction() //this might need to be changed
        {
            return Math.Tanh(input);
        }
        #endregion
    }
}
