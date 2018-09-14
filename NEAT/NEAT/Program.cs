using NEATLibrary;
using System;

namespace NEAT // this is for testing the NEAT-implementation without the game
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var m = new GeneMarker();
            Genome g1 = new Genome(5,4,m);
            Genome g2 = new Genome(5, 4, m);


            for (int i =0; i< 20; i++)
            {
                g1.Mutate();
                g2.Mutate();
            }
            Console.WriteLine();
            Console.WriteLine(g1.toWebGraphviz());
            Console.WriteLine();
            Console.WriteLine(g2.toWebGraphviz());
            Console.WriteLine("asd");
            Console.ReadLine();
        }
    }
}
