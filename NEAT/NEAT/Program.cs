using NEATLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NEAT // this is for testing the NEAT-implementation without the game
{
    class Program
    {
        /// <summary>
        /// Test data for XOR problem
        /// </summary>
        private static readonly double[] input1 = { 0, 0, 1, 1 };
        private static readonly double[] input2 = { 0, 1, 0, 1 };
        private static readonly double[] output = { 0, 1, 1, 0 };


        static void Main(string[] args)
        {

            GeneMarker marker = new GeneMarker();
            Genome starter = new Genome(3, 1, marker, true, true);
            Population pop = new Population(12000, starter);
            var finished = false;

            while (!finished && pop.Generation != 100)
            {
                Console.WriteLine("gen{0}", pop.Generation);
                foreach (Genome g in pop.currentGeneration)
                {
                    FitnessTest(g);
                }

                finished = FinalTest(pop.currentGeneration.Max());

                pop.Evaluate();



            }

            Console.WriteLine("\n\n Generation History: \n");

            foreach (var historyElement in pop.ProgressionHistory)
            {
                Console.WriteLine("Generation: {0} score: {1}", historyElement.Key, historyElement.Value.Fitness);
            }

            Genome Champion;
            if (pop.ProgressionHistory.Count > 0)
            {
                var max = pop.ProgressionHistory.Keys.Max();
                Champion = pop.ProgressionHistory[max];
                Console.WriteLine("\n{0}",Champion.toDOTstring());
            }
            else
            {
                Champion = pop.currentGeneration[new Random().Next(pop.popSize)];
                Console.WriteLine("Training failed.\n Random genome:\n{0}", Champion.toDOTstring());
            }


            Console.WriteLine("innovation counter: {0}", marker.counter);
            Console.WriteLine("done");



            Console.WriteLine("\n Press enter to test...");


            Console.ReadLine();


            Phenotype NN = new Phenotype(Champion);

            char c;
            do
            {
                Console.Clear();
                Console.Write("input1: ");
                int i1 = int.Parse(Console.ReadLine());
                Console.Write("input2: ");
                int i2 = int.Parse(Console.ReadLine());
                List<double> NNinput = new List<double>();
                NNinput.Add(1.0f); //BIAS
                NNinput.Add(i1);
                NNinput.Add(i2);
                NN.Run(NNinput);

                var sol = NN.Outputs.Dequeue();
                Console.WriteLine("Output is: {0}     (rel:{1})", Math.Round(sol), sol);

                c = Console.ReadKey().KeyChar;

            } while (c != '\x1B');


        }


        public static void FitnessTest(Genome g)
        {
            /*var f1 = g.getComplexity();
            var f2 = g.RC_Count;

            g.Fitness = f1 + f2 * f2 + 1;*/

            var NN = new Phenotype(g);
            double error = 0f;
            for (int i = 0; i < 4; i++)
            {
                List<double> NNinput = new List<double>();
                NNinput.Add(1.0f); //BIAS
                NNinput.Add(input1[i]);
                NNinput.Add(input2[i]);

                NN.Run(NNinput);
                error += Math.Abs(NN.Outputs.Dequeue() - output[i]);

            }

            g.Fitness = Math.Pow(1000 / (error + 1), 2);


        }


        public static bool FinalTest(Genome g)
        {
            double error = 0.0f;
            double solDiffSum = 0;
            if (g.Connections.Count > 1)
            {
                Phenotype NN = new Phenotype(g);

                for (int i = 3; i >= 0; i--)
                {
                    List<double> NNinput = new List<double>();
                    NNinput.Add(1.0f); //BIAS
                    NNinput.Add(input1[i]);
                    NNinput.Add(input2[i]);

                    NN.Run(NNinput);
                    var sol = NN.Outputs.Dequeue();
                    solDiffSum += Math.Abs(sol - output[i]);
                    error += Math.Abs(Math.Round(sol) - output[i]);
                    Debug.WriteLine(sol, "FinalTest");
                    
                }

                Console.WriteLine("Efficiency: {0}%", (-(100 / 6) * solDiffSum + 100).ToString("0.00")); //In XOR test the maximum error is 6

                return (error == 0f);

            }
            else
            {
                return false;
            }

        }


    }
}
