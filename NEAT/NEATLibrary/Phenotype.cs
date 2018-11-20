using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NEATLibrary
{
    class Phenotype
    {
        #region Phenotype definition
        public List<Node> Nodes;
        public List<Node> NodesIdSorted;
        public List<double> Outputs;

        public Phenotype(Genome genome)
        {
            Nodes = new List<Node>();
            Outputs = new List<double>();


            foreach(NodeGene nodeGene in genome.Nodes)
            {
                var temp = new Node(nodeGene.Type, nodeGene.LayerQuotient, nodeGene.Id);
                Nodes.Add(temp);
            }

            Nodes = Nodes.OrderBy(node => node.LayerQuotient).ToList<Node>();

            NodesIdSorted = Nodes.OrderBy(node => node.Id).ToList<Node>();

            NodesIdSorted.OrderBy(node => node.Id);

            var x = NodesIdSorted.Count;
            for (int i = 0; i < x; i++)
            {
                if (NodesIdSorted[i].Id > i)
                {
                    NodesIdSorted.Insert(i, null);
                    x++;
                }
            }
            //now we can access nodes by their Id

            foreach (var connection in genome.Connections.Values)
            {
                if (connection.isEnabled)
                {
                    NodesIdSorted[connection.inNode].Outputs.Add(connection.outNode, connection.Weight);  //specifies what nodes the output needs to be transmitted to
                }
            }
        }
        #endregion

        #region Public Methods
        public void Run(List<double> sensorInputs)
        {
            foreach (Node node in Nodes)
            {
                if (node != null)
                {
                    if (node.Type == NodeType.Sensor)
                    {
                        node.input = sensorInputs[0];
                        sensorInputs.RemoveAt(0);
                    }
                    if (node.Type != NodeType.Output)
                    {
                        foreach (KeyValuePair<int, double> output in node.Outputs)
                        {
                            NodesIdSorted[output.Key].input += output.Value * node.TransferFunction();
                        }
                        node.input = 0;
                    }
                    if (node.Type == NodeType.Output)
                    {
                        Outputs.Add(node.TransferFunction());
                        node.input = 0;
                    }
                }
            }
        }

        public void clearOutputs()
        {
            Outputs.Clear();
        }
        #endregion
    }
}
