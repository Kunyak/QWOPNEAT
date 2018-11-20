using NEATLibrary;
using System;
using System.Collections.Generic;


namespace NEAT // this is for testing the NEAT-implementation without the game
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneMarker marker = new GeneMarker();
            Genome genome = new Genome(1, 1, marker);
            NodeGene node1 = new NodeGene(NodeType.Hidden, 3, 0.5);
            genome.addNodeGene(node1);
            NodeGene node2 = new NodeGene(NodeType.Hidden, 4, 0.5);
            genome.addNodeGene(node2);
            NodeGene node3 = new NodeGene(NodeType.Hidden, 5, 0.75);
            genome.addNodeGene(node3);
            ConnectionGene conn1 = new ConnectionGene(1, 3, 0.5, true, marker);
            genome.addConnectionGene(conn1);
            ConnectionGene conn2 = new ConnectionGene(1, 4, 0.5, true, marker);
            genome.addConnectionGene(conn2);
            ConnectionGene conn3 = new ConnectionGene(4, 5, 0.5, true, marker);
            genome.addConnectionGene(conn3);
            ConnectionGene conn4 = new ConnectionGene(5, 2, 0.5, true, marker);
            genome.addConnectionGene(conn4);
            ConnectionGene conn5 = new ConnectionGene(3, 2, 0.5, true, marker);
            genome.addConnectionGene(conn5);
            ConnectionGene conn6 = new ConnectionGene(5, 3, 0.5, true, marker);
            genome.addConnectionGene(conn6);
            Phenotype phenotype = new Phenotype(genome);
            List<double> sensorInputs = new List<double>();
            sensorInputs.Add(1);
            phenotype.Run(sensorInputs);
            Console.WriteLine(phenotype.Outputs);
            phenotype.clearOutputs();
            phenotype.Run(sensorInputs);
            Console.WriteLine(phenotype.Outputs);
            phenotype.clearOutputs();
            Console.ReadLine();
        }
    }
}
