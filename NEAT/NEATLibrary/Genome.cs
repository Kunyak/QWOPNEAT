using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class Genome
    {

        protected List<NodeGene> Nodes;
        protected Dictionary<int, ConnectionGene> Connections;
        private Random random;
        protected GeneMarker Marker;
        public int nSensor { get; }
        public int nOutput { get; }
        //private bool RNN;

        public Genome(int sensor, int output, GeneMarker gmarker)
        {
            initBaseNodes(sensor, output);
            Marker = gmarker;
            random = new Random();
            nSensor = sensor;
            nOutput = output;
        }

        /// <summary>
        /// Crossover of two Genomes
        /// </summary>
        /// <param name="Pfittest"> Main structure will be similar to the fitter parents structure</param>
        /// <param name="Plessfit"></param>
        public Genome(Genome Pfittest, Genome Plessfit)
        {
            //if (Pfittest.nOutput != Plessfit.nOutput || Pfittest.nSensor != Plessfit.nSensor) throw new Exception("Parent genomes has different basenodes");
            initBaseNodes(0, 0);
            Marker = Pfittest.Marker;
            random = new Random();
            
           
            // get nodes from fittest parent
            foreach(NodeGene gene in Pfittest.Nodes)
            {
                Nodes.Add(new NodeGene(gene)); // clones the node genes of the fittest parent
            }


            foreach(var fitGene in Pfittest.Connections) // lining up the genes of each parent
            {

                var g1 = fitGene.Value;

                if (Plessfit.Connections.ContainsKey(fitGene.Key)) // matching genes
                {
                    var g2 = Plessfit.Connections[fitGene.Key];
                    var newGene = new ConnectionGene((random.NextDouble() < 0.5f)?g1:g2); // randomly choosing between parents genes
                    addConnectionGene(newGene);
                }
                else // unmatching genes -- adding gene from the fittest parents
                {
                    addConnectionGene(new ConnectionGene(g1));
                }

            }



        }


        private void initBaseNodes(int a, int b)
        {
            Nodes = new List<NodeGene>();
            Connections = new Dictionary<int, ConnectionGene>();
            for (int i=0; i< a+b; i++)
            {
                if (i < a)
                {
                    Nodes.Add(new NodeGene(NodeType.Sensor, i, 0));
                }
                else
                {
                    Nodes.Add(new NodeGene(NodeType.Output, i, 1));
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

            NodeGene oldIn = Nodes[oldConnoection.inNode];
            NodeGene oldOut = Nodes[oldConnoection.outNode];

            NodeGene newNode = new NodeGene(NodeType.Hidden,Nodes.Count,(oldIn.LayerQuotient + oldOut.LayerQuotient)/2);
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
            double weight = random.NextDouble() * 2f - 1f;
            // initial weight is between -1 and 1
            /* var inId = (connectable == 1) ? inNode.Id : outNode.Id; --> use for FF only ANN;
            var outId = (connectable == 1) ? outNode.Id : inNode.Id;*/

            var inId = inNode.Id;
            var outId = outNode.Id;


            foreach (ConnectionGene gene in Connections.Values) // Search for Same Connections
            {
                if ( gene.isEqualNodes(inId,outId,false) ){ // set true if you dont want 'bidirectional' connections
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
                   code +=  (gene.Id+1).ToString() + ";"; // add 1 so the output is similar to the one show in the original paper
                }
            }

            code += @"label = ""Sensor"";}
                    subgraph cluster_1 {
		                    node [style=filled];";
            foreach (NodeGene gene in Nodes)
            {
                if (gene.Type == NodeType.Output)
                {
                    code += (gene.Id+1).ToString() + ";";
                }
            }
            code += @"label =""Output""; color = blue }
                        subgraph cluster_2 {
		                    node [style=filled, color=lightgrey];";
            foreach (NodeGene gene in Nodes)
            {
                if (gene.Type == NodeType.Hidden)
                {
                    code += (gene.Id+1).ToString() + ";";
                }
            }
            code += @"label =""Hidden""; color = blue }";


            foreach (ConnectionGene gene in Connections.Values)
            {
                if (gene.isEnabled)
                {
                    code += (gene.inNode+1).ToString() + "->" + (gene.outNode+1).ToString() + @"[ label = "" " + gene.Weight.ToString("0.00") + @" "" ];";
                }
                else
                {
                    code += (gene.inNode+1).ToString() + "->" + (gene.outNode+1).ToString() + @"[color=""0.002 0.999 0.999""];";
                }
            }

            code += "}";

            return code;

        }

    }
}
