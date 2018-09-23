using NEATLibrary;
using System;

namespace NEAT // this is for testing the NEAT-implementation without the game
{
    class Program
    {
        static void Main(string[] args)
        {
             var m = new GeneMarker(10);
             Genome g1 = new Genome(3, 1, m);
             Genome g2 = new Genome(3, 1, m);

             // set genome 1 to look like in the paper
             g1.addNodeGene(new NodeGene(NodeType.Hidden,4,0.5f));
             g1.addConnectionGene(new ConnectionGene(0, 3, 1.0f, true, 0));
             g1.addConnectionGene(new ConnectionGene(1, 3, 1.0f, false, 1));
             g1.addConnectionGene(new ConnectionGene(2, 3, 1.0f, true, 2));
             g1.addConnectionGene(new ConnectionGene(1, 4, 1.0f, true, 3));
             g1.addConnectionGene(new ConnectionGene(4, 3, 1.0f, true, 4));
             g1.addConnectionGene(new ConnectionGene(0, 4, 1.0f, true, 7));
             // set genome 2 to  look like in the paper
             g2.addNodeGene(new NodeGene(NodeType.Hidden, 4, 0.5f));
             g2.addNodeGene(new NodeGene(NodeType.Hidden, 5, 0.75f));
             g2.addConnectionGene(new ConnectionGene(0, 3, 0.5f, true, 0));
             g2.addConnectionGene(new ConnectionGene(1, 3, 0.4f, false, 1));
             g2.addConnectionGene(new ConnectionGene(2, 3, 0.3f, true, 2));
             g2.addConnectionGene(new ConnectionGene(1, 4, 0.5f, true, 3));
             g2.addConnectionGene(new ConnectionGene(4, 3, 2.0f, false, 4));
             //g1.addConnectionGene(new ConnectionGene(1, 5, 1.0f, true, 7));
             g2.addConnectionGene(new ConnectionGene(4, 5, 2.0f, true, 5));
             g2.addConnectionGene(new ConnectionGene(5, 3, 1.0f, true, 6));
             g2.addConnectionGene(new ConnectionGene(2, 4, 0.8f, true, 8));
             g2.addConnectionGene(new ConnectionGene(0, 5, 0.25f, true, 9));


             Genome Child = new Genome(g1,g2);

             Console.WriteLine();
             Console.WriteLine(Child.toWebGraphviz());
             Console.WriteLine("asd");
             Console.ReadLine();


        }
    }
}
