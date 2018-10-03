﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NEATLibrary
{
    class Population
    {


        #region Population Definition
        public List<Genome> currentGeneration;
        public Dictionary<int, Genome> ProgressionHistory; //Hall of Fame
        public int SpeciesCount { get { return (species != null )?species.Count:0; } }

        public Double bestScore { get; private set; }
        public Double globalBestScore { get; private set; }
        public int popSize { get; private set; }
        public int Generation { get; private set; }
        public int PopulationInproductivity { get; private set; }

        private List<Species> species;
        private Random random;

        public Population(int size, Genome starter)
        {
            ProgressionHistory = new Dictionary<int, Genome>();
            currentGeneration = new List<Genome>();
            species = new List<Species>();
            popSize = size;
            random = starter.random;
            Generation = 0;
            bestScore = 0;
            PopulationInproductivity = 0;
            globalBestScore = 0;

            for (int i = 0; i < size; i++)
            {
                currentGeneration.Add(new Genome(starter));
            }
           

                 
        }
        #endregion


        #region public methods
        public void Evaluate()
        {

            // put the genomes into species
            Speciate();

            // sets the bestscore and stores in the "hall of fame" if the progress is big enough
            RecordHistory();

            // if the population isnt going anywhere kill every species but the top five
            KillLongInproductivity();

            //Get the next generation of genomes
            getNextGenerationFromSpecies();

#if DEBUG
            Debug.WriteLine("gen: " + Generation.ToString() , "GenerationReport");
            Debug.WriteLine("species: " + species.Count.ToString() , "GenerationReport");
            Debug.WriteLine("maxfitness: " + bestScore , "GenerationReport");

#endif

        }
        #endregion


        #region private methods

        private double sumOFavgSpeciesFitness()
        {
            double sum = 0f;
            foreach( Species s in species)
            {
                sum += s.SharedFitnessSum;
            }
            return sum;
        }

        private Genome getOffspring(Species s) // gets an offspring from a species with crossover or cloning and then mutates it
        {
            Genome offspring;
            if (random.NextDouble() < TuningParameters.CROSSOWER_RATE)
            {
                // crossover two parents from the same species

                Genome parent1 = s.selectRandomGenome();
                Genome parent2 = s.selectRandomGenome();

                if (parent1.Fitness > parent2.Fitness)
                {
                    offspring = new Genome(parent1, parent2); // makes new offspring with crossower
                }
                else
                {
                    offspring = new Genome(parent2, parent1);
                }


            }
            else
            {
                offspring = new Genome(s.selectRandomGenome()); // clone from the species
            }

            // mutate the offspring and add to next generation

            offspring.Mutate();
            return offspring;
        }

        private int AvaibleSpace(Species s, double SfitnessSum) // returns how many offspring can a spieces give to the next generation
        {
            return (int)Math.Floor(s.SharedFitnessSum / SfitnessSum * popSize);
        }

       

        private void Speciate()
        {
            // clear all existing genomes
            foreach (Species s in species) 
            {
                s.Clear();
            }

             // fit each genome into species
            foreach (Genome genome in currentGeneration)
            {
                int i = 0;
                while (i < species.Count && !species[i].isCompatible(genome)) i++; // finds the first species that compatible with the genome

                if (i == species.Count) //didnt find apropiate species
                {
                    species.Add(new Species(genome));
                }
                else //found a good species
                {
                    species[i].Add(genome);
                }
            }


            // split the Species
            foreach (Species s in species.ToArray())
            {
                if (s.Count == 0)
                {
                    species.Remove(s);
                }
                else
                {
                    s.sortGenomes(); // sort the genomes based on fitness
                    s.Split(); //kill the lowest half of each species
                    s.ShareFitness(); //set the shared fitness and calculate the fitness of the species
                    s.setProductivity(); //set the inproductivity of a species
                }
            }

            // Sort the species
            species.Sort();

            //kill species that are empty or haven't progressed in 15 generations (if its not in the first x species beacuase we dont want to remove all species)
            var FitnessSum = sumOFavgSpeciesFitness();
            foreach (Species s in species.ToArray())
            {
                if (s.Inproductivity >= TuningParameters.INPRODUCTIVITY_THRESHOLD && species.Count > TuningParameters.SAFE_SPECIES)
                {
                    species.Remove(s);
                }
                else if (AvaibleSpace(s,FitnessSum) < 1 && species.Count > TuningParameters.SAFE_SPECIES) //wouldn't get place in ngen anyways
                {
                    species.Remove(s);
                }
            }
        }

        private void RecordHistory() //saves the progression of the population
        {
            currentGeneration.Sort(); 
            var Champion = currentGeneration[popSize - 1];

            if (Champion.Fitness - globalBestScore > TuningParameters.HISTORY_RECORD_TH)
            {
                ProgressionHistory.Add(Generation, Champion);
            }
            // get the best genome of the generation

            if (Champion.Fitness <= bestScore) // if the current champion is worse than the previous champ than the population is inproductive
            {
                PopulationInproductivity++;
            }
            else
            {
                PopulationInproductivity = 0;
            }

            bestScore = Champion.Fitness; // Best score of the current generation

            if (bestScore > globalBestScore) globalBestScore = bestScore; // best score of the population
        }

        //
        private void KillLongInproductivity()
        {
            if (PopulationInproductivity >= TuningParameters.GENERATION_INPRODUCTIVITY_THRESHOLD && species.Count > 5)
            {
                PopulationInproductivity = 0;
                var killCount = species.Count - 5;
                species.RemoveRange(0, killCount);
            }
        }

        private void getNextGenerationFromSpecies()
        {
            var nextGen = new List<Genome>();
            double FitnessSum = sumOFavgSpeciesFitness();
            // add champions to the next generation "unharmed"; and then get children
            foreach (Species s in species)
            {
                nextGen.Add(new Genome(s.Champion()));

                var AS = AvaibleSpace(s, FitnessSum) - 1; // -1 because the champion is already added
                for (int i = 0; i < AS; i++) nextGen.Add(getOffspring(s));

            }

            // stg went really wron
            if (nextGen.Count > popSize) throw new OverflowException("Too much offsprings");


            // fill missing space 
            var StrongestSpecies = species[species.Count - 1];
            while (nextGen.Count < popSize)
            {
                nextGen.Add(getOffspring(StrongestSpecies));
            }

            currentGeneration = nextGen;
            Generation++;

        }

        #endregion
    }
}