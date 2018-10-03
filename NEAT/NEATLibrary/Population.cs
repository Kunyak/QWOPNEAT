using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NEATLibrary
{
    class Population
    {



        public List<Genome> currentGeneration;
        public Genome bestGenome;
        private List<Species> species;
        public Dictionary<int, Genome> ProgressionHistory;
        public int popSize { get; private set; }
        public int Generation { get; private set; }
        private GeneMarker Marker;
        private Random random;

        public Population(int size, GeneMarker marker, Genome starter)
        {
            ProgressionHistory = new Dictionary<int, Genome>();
            currentGeneration = new List<Genome>();
            species = new List<Species>();
            popSize = size;
            Marker = marker;
            random = starter.random;
            Generation = 0;

            for (int i = 0; i < size; i++)
            {
                currentGeneration.Add(new Genome(starter));
            }
           

                 
        }

        private void Init(int s, GeneMarker m)
        {

        }

        public bool addGenome(Genome g)
        {
            if (currentGeneration.Count < popSize)
            {
                currentGeneration.Add(g);
                return true;
            }
            else return false;
        }


        public void nextGeneration()
        {

            var nextGen = new List<Genome>();

            //TODO evaluate fitness

            currentGeneration.Sort();
            // if (currentGeneration[currentGeneration.Count-1].Fitness > ProgressionHistory[ProgressionHistory.Keys[ProgressionHistory.Keys.Count - 1]].Fitness) ProgressionHistory.Add(Generation,currentGeneration[currentGeneration.Count-1]);

            bestGenome = currentGeneration[currentGeneration.Count - 1];

            Debug.WriteLine("Generation: " + Generation.ToString() + ". maxFitness: " + currentGeneration[popSize-1].Fitness,"EvalTest");


            // put the genomes into species
            Speciate();
            Debug.WriteLine("Generation: " + Generation.ToString() + ". SpeciesCount: " + species.Count.ToString(), "EvalTest");
            double SumavgFitness = sumOFavgSpeciesFitness();


            // add champions to the next generation "unharmed"; and then get children
            foreach (Species s in species)
            {
                nextGen.Add(new Genome(s.Champion()));

                var AvaibleSpace = Math.Floor(s.avgFitness/SumavgFitness*popSize) - 1; // -1 because the champion is already added

                for (int i = 0; i < AvaibleSpace; i++)
                {
                    Genome offspring;
                    if (random.NextDouble() < TuningParameters.CROSSOWER_RATE)
                    {
                        // crossover two parents from the same species

                        Genome parent1 = s.selectRandomGenome();
                        Genome parent2 = s.selectRandomGenome();

                        if (parent1.Fitness > parent2.Fitness)
                        {
                            offspring = new Genome(parent1,parent2); // makes new offspring with crossower
                        }
                        else
                        {
                            offspring = new Genome(parent2, parent1);
                        }


                    }
                    else
                    {
                        offspring = new Genome (s.selectRandomGenome()); // clone from the species
                    }

                    // mutate the offspring and add to next generation

                    offspring.Mutate();

                    nextGen.Add(offspring);
                }

            }


            while (nextGen.Count < popSize)
            {
                Genome offspring = species[species.Count - 1].selectRandomGenome();
                offspring.Mutate();
                nextGen.Add(offspring);
            }

            currentGeneration = nextGen;
            Generation++;


        }

        private double sumOFavgSpeciesFitness()
        {
            double sum = 0f;
            foreach( Species s in species)
            {
                sum += s.avgFitness;
            }
            return sum;
        }

        private void Speciate()
        {
            foreach (Species s in species) // clear all previous me
            {
                s.Clear();
            }
             // fit each genome into species
            foreach (Genome genome in currentGeneration)
            {
                int i = 0;
                while (i < species.Count && !species[i].Compatible(genome)) i++; // finds the first species that compatible with the genome

                if (i == species.Count) //didnt find apropiate species
                {
                    species.Add(new Species(genome));
                }
                else //found a good species
                {
                    species[i].Add(genome);
                }
            }

            // Calculate adjusted fitness and fitness sum

            foreach (Species s in species)
            {
                s.AdjustFitness();
            }

            species.Sort();
            //kill species that are empty or haven't progressed in 15 generations (if its not in the first x species beacuase we dont want to remove all species)


            for (int i = 0; i < species.Count; i++) 
            {
                if (species[i].Count == 0 || (species[i].Inproductivity >= TuningParameters.INPRODUCTIVITY_THRESHOLD && i > TuningParameters.SAFE_SPECIES)) 
                {
                    species.RemoveAt(i);
                    i--;
                }
            }

            // split the Species

            foreach (Species s in species)
            {
                s.Split();
            }

            // kill species that aren't gone give offspring anyway

        }
    }
}
