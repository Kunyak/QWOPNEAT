using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class Genome
    {

        private List<NodeGene> Nodes;
        private Dictionary<int, ConnectionGene> Connections;
        private Random random;
        private GeneMarker Marker;

        public Genome(int sensor, int output, GeneMarker gmarker)
        {
            initBaseNodes(sensor, output);
            Marker = gmarker;
            random = new Random();
        }

        public Genome(int sensor, int output, GeneMarker gmarker, Random rnd)
        {
            initBaseNodes(sensor, output);
            Marker = gmarker;
            random = rnd;
        }


        private void initBaseNodes(int a, int b)
        {
            Nodes = new List<NodeGene>();
            Connections = new Dictionary<int, ConnectionGene>();
            Nodes.Add(new NodeGene(NodeType.Bias, 0));
            for (int i=1; i<= a+b; i++)
            {
                if (i < a)
                {
                    Nodes.Add(new NodeGene(NodeType.Sensor,i));
                }
                else
                {
                    Nodes.Add( new NodeGene(NodeType.Output, i));
                }
            }
        }


        public void Mutate()
        {
               
            if (random.NextDouble() < TuningParameters.WEIGHT_MUTATION_RATE)
            {
                MutateWeights();
            }
            if (random.NextDouble() < TuningParameters.NEW_CONNECTION_RATE)
            {
                ConnectionMutation();
            }
            if (random.NextDouble() < TuningParameters.NEW_NODE_RATE)
            {
                NodeMutation();
            }
            
        }

        private void NodeMutation()
        {
            ConnectionGene oldConnoection = Connections.ElementAt(random.Next(0, Connections.Count)).Value; // take a random connection     
            oldConnoection.Disable();

            NodeGene newNode = new NodeGene(NodeType.Hidden,Nodes.Count);
            ConnectionGene newConn1 = new ConnectionGene(oldConnoection.inNode, newNode.Id, oldConnoection.Weight, true, Marker);
            ConnectionGene newConn2 = new ConnectionGene(newNode.Id, oldConnoection.outNode, 1f , true, Marker);

            Nodes.Add(newNode);
            Connections.Add(newConn1.Innovation,newConn1);
            Connections.Add(newConn2.Innovation,newConn2);


        }

        private void ConnectionMutation()
        {
            throw new NotImplementedException();
        }

        private void MutateWeights()
        {
            foreach(ConnectionGene gene in Connections.Values)
            {
                if (random.NextDouble() < TuningParameters.NEW_WEIGHT_RATE)
                {
                    // get completly new value for weight between -2;2
                    gene.Weight = random.NextDouble() * 4f - 2f;
                }
                else
                {
                    //addjust weight 
                    gene.Weight *= random.NextDouble(); 
                }
            }
        }

        public void addNodeGene(NodeGene gene)
        {
            Nodes.Add(gene.Id,gene);
        }

        public void addConnectionGene(ConnectionGene gene)
        {
            Connections.Add(gene.Innovation,gene);
        }

        

    }
}
