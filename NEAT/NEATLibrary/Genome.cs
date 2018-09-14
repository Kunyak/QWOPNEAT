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
            for (int i=0; i< a+b; i++)
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
            if (random.NextDouble() < TuningParameters.NEW_CONNECTION_RATE && Nodes.Count > 1);
            {
                ConnectionMutation();
            }
            if (random.NextDouble() < TuningParameters.NEW_NODE_RATE && Connections.Count > 0)
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

            NodeGene inNode = Nodes[random.Next(0, Nodes.Count - 1)];
            NodeGene outNode = Nodes[random.Next(0, Nodes.Count - 1)];

            var connectable = inNode.canConnectTo(outNode);

            if (connectable == -1)
            {
                return ;
            }
            double weight = random.NextDouble() * 2f - 1f; // initial weight is between -1 and 1
            var inId = (connectable == 1) ? inNode.Id : outNode.Id;
            var outId = (connectable == 1) ? outNode.Id : inNode.Id;
            

            foreach (ConnectionGene gene in Connections.Values)
            {
                if ( gene.isEqualNodes(inId,outId) ){
                    return;
                }
            }


            ConnectionGene newGene = new ConnectionGene(inId, outId, weight, true, Marker);
            addConnectionGene(newGene);
            return;


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
            Nodes.Add(gene);
        }

        public void addConnectionGene(ConnectionGene gene)
        {
            Connections.Add(gene.Innovation,gene);
        }

        public string toWebGraphviz()
        {
            string code = @"digraph GenomeMap {
	                        node [shape = circle];
                            subgraph cluster_0 {
		                            style=filled;
		                            color=lightgrey;
		                            node [style=filled,color=white];";

            // make sensoor type subgroup
            foreach (NodeGene gene in Nodes)
            {
                if (gene.Type == NodeType.Sensor)
                {
                   code +=  gene.Id.ToString() + ";";
                }
            }

            code += @"label = ""Sensor"";}
                    subgraph cluster_1 {
		                    node [style=filled];";
            foreach (NodeGene gene in Nodes)
            {
                if (gene.Type == NodeType.Output)
                {
                    code += gene.Id.ToString() + ";";
                }
            }
            code += @"label =""Output""; color = blue }
                        subgraph cluster_2 {
		                    node [style=filled, color=lightgrey];";
            foreach (NodeGene gene in Nodes)
            {
                if (gene.Type == NodeType.Hidden)
                {
                    code += gene.Id.ToString() + ";";
                }
            }
            code += @"label =""Hidden""; color = blue }";


            foreach (ConnectionGene gene in Connections.Values)
            {
                if (gene.isEnabled)
                {
                    code += gene.inNode.ToString() + "->" + gene.outNode.ToString() + @"[ label = "" " + gene.Weight.ToString("0.00") + @" "" ];";
                }
                else
                {
                    code += gene.inNode.ToString() + "->" + gene.outNode.ToString() + @"[color=""0.002 0.999 0.999""];";
                }
            }

            code += "}";

            return code;

        }

    }
}
