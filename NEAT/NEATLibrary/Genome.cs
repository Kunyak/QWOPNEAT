using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class Genome : IComparable
    {
        // fields + constructor
        #region Class definition
        public List<NodeGene> Nodes;                         //changed from protected
        public Dictionary<int, ConnectionGene> Connections;  //changed from protected
        protected GeneMarker Marker;
        public Random random;
        public int nSensor { get; }
        public int nOutput { get; }
        public double Fitness;
        public double AdjustedFitness;

        public Genome(int sensor, int output, GeneMarker gmarker)
        {
            initBaseNodes(sensor, output);
            Marker = gmarker;
            random = new Random();
            nSensor = sensor;
            nOutput = output;
        }


        public Genome(Genome g)
        {
            initBaseNodes(0, 0);
            Marker = g.Marker;
            random = g.random;
            nSensor = g.nSensor;
            nOutput = g.nOutput;

            foreach (NodeGene gene in g.Nodes)
            {
                Nodes.Add(new NodeGene(gene));
            }

            foreach (ConnectionGene gene in g.Connections.Values)
            {
                addConnectionGene(new ConnectionGene(gene));
            }

        }
        #region crossover
        public Genome(Genome Pfittest, Genome Plessfit) // CROSSOVER
        {
            initBaseNodes(0, 0);
            Marker = Pfittest.Marker;
            random = Pfittest.random;


            // get nodes from fittest parent
            foreach (NodeGene gene in Pfittest.Nodes)
            {
                Nodes.Add(new NodeGene(gene)); // clones the node genes of the fittest parent
            }

            foreach (var fitGene in Pfittest.Connections) // lining up the genes of each parent
            {
                var g1 = fitGene.Value;

                if (Plessfit.Connections.ContainsKey(fitGene.Key)) // matching genes
                {
                    var g2 = Plessfit.Connections[fitGene.Key];
                    var newGene = new ConnectionGene((random.NextDouble() < 0.5f) ? g1 : g2); // randomly choosing between parents genes
                    addConnectionGene(newGene);
                }
                else // unmatching genes -- adding gene from the fittest parents
                {
                    addConnectionGene(new ConnectionGene(g1));
                }
            }
        }
        #endregion
        #endregion

        
        #region public methods
        public void addNodeGene(NodeGene gene)
        {
            Nodes.Add(gene);
        }

        public void addConnectionGene(ConnectionGene gene)
        {
            Connections.Add(gene.Innovation, gene);
        } 

        public void Mutate() // Mutates the genome with 3 different mutations
        {

            if (random.NextDouble() < TuningParameters.WEIGHT_MUTATION_RATE)
            {
                MutateWeights();
            }
            if (random.NextDouble() < TuningParameters.NEW_CONNECTION_RATE && Nodes.Count > 1)
            {
                ConnectionMutation();
            }
            if (random.NextDouble() < TuningParameters.NEW_NODE_RATE && Connections.Count > 0)
            {
                NodeMutation();
            }

        }

        public static double CompatibilityDistance(Genome g1, Genome g2)
        {
            int matching = 0;
            int disjoint = 0;
            int excess = 0;
            int N;
            double sumWeightDiff = 0f;

            var conn1 = g1.Connections;
            var conn2 = g2.Connections;

            N = Math.Max(conn1.Count, conn2.Count);
            if (N == 0) return 0; // if neither of the genomes has genes then its useless to count distance; 

            var maxKey1 = (conn1.Keys.Count != 0) ? conn1.Keys.Max() : 0; // max gives an exeption if Count is 0;
            var maxKey2 = (conn2.Keys.Count != 0) ? conn2.Keys.Max() : 0;



            int excessLine = Math.Min(maxKey1, maxKey2);
            int markerLine = Math.Max(maxKey1, maxKey2);

            for (int i = 0; i < markerLine; i++)
            {
                if (conn1.ContainsKey(i) && conn2.ContainsKey(i)) // matching gene
                {
                    sumWeightDiff += Math.Abs(conn1[i].Weight - conn2[i].Weight);
                    matching++;
                }
                else if (conn1.ContainsKey(i) ^ conn2.ContainsKey(i)) // excess or disjoint genes
                {
                    if (i <= excessLine) // disjoint gene
                    {
                        disjoint++;
                    }
                    else //excess gene
                    {
                        excess++;
                    }
                }
            }

            var avgWeightDiff = (matching > 0) ? (sumWeightDiff / matching) : 100;

            return (TuningParameters.C1 * excess / N) + (TuningParameters.C2 * disjoint / N) + (TuningParameters.C3 * avgWeightDiff);

        }

        public Double getComplexity() // only for testing purpose
        {
            return Math.Sqrt(Math.Pow(Nodes.Count, 2) + Math.Pow(Connections.Count, 2));
        }


        public int CompareTo(object obj) // when a list of genome is sorted you sort it with the fitness.
        {
            if (obj is Genome)
            {
                Genome g = (Genome)obj;
                return Fitness.CompareTo(g.Fitness);
            }
            else throw new ArgumentException("Object is not a genome");
        }


        #region Graph
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
                    code += (gene.Id + 1).ToString() + ";"; // add 1 so the output is similar to the one show in the original paper
                }
            }

            code += @"label = ""Sensor"";}
                    subgraph cluster_1 {
		                    node [style=filled];";
            foreach (NodeGene gene in Nodes)
            {
                if (gene.Type == NodeType.Output)
                {
                    code += (gene.Id + 1).ToString() + ";";
                }
            }
            code += @"label =""Output""; color = blue }
                        subgraph cluster_2 {
		                    node [style=filled, color=lightgrey];";
            foreach (NodeGene gene in Nodes)
            {
                if (gene.Type == NodeType.Hidden)
                {
                    code += (gene.Id + 1).ToString() + ";";
                }
            }
            code += @"label =""Hidden""; color = blue }";


            foreach (ConnectionGene gene in Connections.Values)
            {
                if (gene.isEnabled)
                {
                    code += (gene.inNode + 1).ToString() + "->" + (gene.outNode + 1).ToString() + @"[ label = "" " + gene.Weight.ToString("0.00") + @" "" ];";
                }
                else
                {
                    code += (gene.inNode + 1).ToString() + "->" + (gene.outNode + 1).ToString() + @"[color=""0.002 0.999 0.999""];";
                }
            }

            code += "}";

            return code;

        }
        #endregion // Returns a string that can be viewed by a grpahviz application as a graph


        #endregion


        #region private methods
        private void initBaseNodes(int a, int b)
        {
            Fitness = 0;
            AdjustedFitness = 0;
            Nodes = new List<NodeGene>();
            Connections = new Dictionary<int, ConnectionGene>();
            for (int i = 0; i < a + b; i++)
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


        #region mutation
        private void NodeMutation() // Add a new node in the middle of an existing connection
        {
            ConnectionGene oldConnoection = Connections.ElementAt(random.Next(0, Connections.Count)).Value; // take a random connection     
            oldConnoection.Disable();

            NodeGene oldIn = Nodes[oldConnoection.inNode];
            NodeGene oldOut = Nodes[oldConnoection.outNode];

            NodeGene newNode = new NodeGene(NodeType.Hidden, Nodes.Count, (oldIn.LayerQuotient + oldOut.LayerQuotient) / 2);
            ConnectionGene newConn1 = new ConnectionGene(oldConnoection.inNode, newNode.Id, oldConnoection.Weight, true, Marker);
            ConnectionGene newConn2 = new ConnectionGene(newNode.Id, oldConnoection.outNode, 1f, true, Marker);

            Nodes.Add(newNode);
            Connections.Add(newConn1.Innovation, newConn1);
            Connections.Add(newConn2.Innovation, newConn2);
        }

        private void ConnectionMutation() // add a new connection between two nodes
        {
            NodeGene inNode = Nodes[random.Next(0, Nodes.Count - 1)];
            NodeGene outNode = Nodes[random.Next(0, Nodes.Count - 1)];

            var connectable = inNode.canConnectTo(outNode);
            if (connectable == -1)
            {
                return;
            }
            double weight = random.NextDouble() * 2f - 1f;

            var inId = inNode.Id;
            var outId = outNode.Id;

            foreach (ConnectionGene gene in Connections.Values) // Search for Same Connections
            {
                if (gene.isEqualNodes(inId, outId, false))
                { //
                    return;
                }
            }

            ConnectionGene newGene = new ConnectionGene(inId, outId, weight, true, Marker);
            addConnectionGene(newGene);
        }

        private void MutateWeights() // adjust weights
        {
            foreach (ConnectionGene gene in Connections.Values)
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
        #endregion
        #endregion

        
    }
}
