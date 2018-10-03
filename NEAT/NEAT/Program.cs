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
            Genome starter = new Genome(5,10,marker);
            Population pop = new Population(100, starter);

            for (int i= 0; i< 100; i++)
            {
                Console.WriteLine("gen{0}",i);
                foreach (Genome g in pop.currentGeneration)
                {
                    FitnessTest(g);
                }

                pop.Evaluate();

            }

            var max = -1;
            foreach(var historyElement in pop.ProgressionHistory)
            {
                Console.WriteLine("Generation: {0} score: {1}",historyElement.Key,historyElement.Value.Fitness);
                if (historyElement.Key > max) max = historyElement.Key;
            }

            Console.WriteLine();
            if (max != -1)
            {
                Console.WriteLine(pop.ProgressionHistory[max].toWebGraphviz());
            }
            else
            {
                Console.WriteLine(pop.currentGeneration[new Random().Next(pop.popSize)].toWebGraphviz());
            }


            Console.WriteLine("innovation counter: {0}",marker.counter);
            Console.WriteLine("done");
            Console.ReadLine();
             

        }


        public static void FitnessTest(Genome g)
        {
            var f1 = g.getComplexity();
            g.Fitness =  f1;
        }
    }
}
