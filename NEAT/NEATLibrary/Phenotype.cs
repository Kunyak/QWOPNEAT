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
        public Dictionary<int, Node> NodesById; //this exists to make connecting nodes easier, I'll figure out which method is most efficient when I get home (idk if in the case of the IDs being the keys vs the nodes being put in the list in ascending order by ID would be the same)
        public List<double> Outputs;

        public Phenotype(Genome genome)
        {
            Nodes = new List<Node>();
            NodesById = new Dictionary<int, Node>();
            Outputs = new List<double>();

            foreach(NodeGene nodeGene in genome.Nodes)
            {
                var temp = new Node(nodeGene.Type, nodeGene.LayerQuotient, nodeGene.Id);
                Nodes.Add(temp);
                NodesById.Add(temp.Id, temp);
            }

            foreach(var connection in genome.Connections.Values)
            {
                NodesById[connection.inNode].Outputs.Add(connection.outNode, connection.Weight);  //specifies what nodes the output needs to be transmitted to
            }

            // Nodes = Nodes.OrderBy(node => node.LayerQuotient);  //this is done so that running the phenotype is faster, it doesn't have to always search for the next node in the net
            Nodes.OrderBy(node => node.LayerQuotient); //
        }
        #endregion

        #region Public Methods
        public void Run(double[] sensorInputs)
        {
            foreach(Node node in Nodes)
            {
                foreach(KeyValuePair<int, double> output in node.Outputs)
                {
                    NodesById[output.Key].input += output.Value * node.TransferFunction();
                }
                if(node.Type == NodeType.Sensor)
                {
                    node.input = sensorInputs[0];
                }
                if(node.Type != NodeType.Sensor)
                {
                    node.input = 0;
                }
                else if(node.Type == NodeType.Output)
                {
                    Outputs.Add(node.TransferFunction());  //a different Transfer Function can be considered here
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
