using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class Phenotype
    {
        #region Phenotype definition
        public Dictionary<int, Node> Nodes; //the int is the id of the node

        public Phenotype(Genome genome)
        {
            foreach(NodeGene nodeGene in genome.Nodes)
            {
                Nodes.Add(nodeGene.Id, Node(nodeGene.Type, nodeGene.LayerQuotient));
            }

            foreach(KeyValuePair<int, ConnectionGene> connection in genome.Connections)
            {
                Nodes[connection.Value.outNode].inputs.Add(connection.Value.inNode, connection.Value.Weight);
                Nodes[connection.Value.inNode].outputs.Add(connection.Value.outNode, connection.Value.Weight);
            }
        }
        #endregion
        //should the nodes get ordered by layerquotient?
    }
}
