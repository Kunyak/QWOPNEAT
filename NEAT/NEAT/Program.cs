using NEATLibrary;
using System;


namespace NEAT // this is for testing the NEAT-implementation without the game
{
    class Program
    {
        static void Main(string[] args)
        {

            GeneMarker marker = new GeneMarker();
            Genome starter = new Genome(5,10,marker,false);
            Population pop = new Population(200, starter);

            for (int i= 0; i< 25; i++)
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
                Console.WriteLine(pop.ProgressionHistory[max].RC_Count);
            }
            else
            {
                Console.WriteLine(pop.currentGeneration[new Random().Next(pop.popSize)].toWebGraphviz());
            }


            Console.WriteLine("innovation counter: {0}",marker.counter);
            Console.WriteLine("done");
            var name = "Population_" + new Random().Next(1, 1000);
            try
            {
                Serializer.SerializePopulation(pop, name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var p2 = Serializer.DeserialisePopulation(name);


            Console.ReadLine();
             

        }


        public static void FitnessTest(Genome g)
        {
            var f1 = g.getComplexity();
            var f2 = g.RC_Count;

            g.Fitness = f1 + f2 * f2 + 1;
        }
    }
}
