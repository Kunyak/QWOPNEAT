using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class Node
    {
        #region Node definition
        public NodeType Type { get; }
        //public int Id { get; }
        public Dictionary<int, double> inputs;  //the int is the id of the node the input is coming from, the double is the connection weight
        public Dictionary<int, double> outputs; //same here
        public double[] inputValues;
        public double LayerQuotient { get; set; }

        public Node(NodeType type, Dictionary<int, double> inConnections, Dictionary<int, double> outConnections, double layer)
        {
            Type = type;
            //Id = id;
            inputs = inConnections;
            outputs = outConnections;
            LayerQuotient = layer;

            inputValues = new double[inputs.Count];
        }

        public Node(NodeType type, double layer)
        {
            Type = type;
            //Id = id;
            LayerQuotient = layer;
        }
        #endregion

        #region Public Methods
        public void InitializeInputArray(Dictionary<int, double> inputsDict)
        {
            inputValues = new double[inputs.Count];
        }
        #endregion
    }
}
