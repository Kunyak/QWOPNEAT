using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NEATLibrary
{
    class Species : IComparable
    {

        #region Species Definition

        public double SharedFitnessSum { get; private set; }
        public int Inproductivity { get; private set; }
        public int Count { get { return specimen.Count; } }

        private Genome reference;
        private List<Genome> specimen;
        private Random rand;
        private double GlobalBestFitness;
        private double CurrentBestFitness;


    


        public Species (Genome referenceGenome)
        {
            specimen = new List<Genome>();
            reference = new Genome(referenceGenome);

            rand = reference.random; // to use only one instance of random if possible
            specimen.Add(referenceGenome);
            GlobalBestFitness = referenceGenome.Fitness;
            CurrentBestFitness = referenceGenome.Fitness;

        }
        #endregion

        #region public methods

        public void Add(Genome g)
        {
            specimen.Add(g);
            if (g.Fitness > CurrentBestFitness) CurrentBestFitness = g.Fitness;
        }

        public void Clear()
        {
            specimen.Clear();
            SharedFitnessSum = 0;
            CurrentBestFitness = 0;
        }

        public void sortGenomes()
        {
            specimen.Sort();
        }

        // sees if there any porgress of the species
        public void setProductivity()
        {
            var newBest = CurrentBestFitness;
            if (newBest <= GlobalBestFitness)
            {
                Inproductivity++;
            }
            else
            {
                Inproductivity = 0;
                GlobalBestFitness = newBest;
            }
        }

        //sets the shared fitness of the species
        public void ShareFitness()
        {
            var N = specimen.Count;
            double sum = 0f;
            foreach (Genome g in specimen)
            {
                g.AdjustedFitness = g.Fitness / N;
                sum += g.AdjustedFitness;
            }

            SharedFitnessSum = sum;
            
        }
        //kills the lower part of the species
        public void Split()
        {
            if (specimen.Count > 2)
            {
                specimen.RemoveRange(0, specimen.Count / 2);
            }
        }

        // returns the champion of the speceis
        public Genome Champion()
        {
            return specimen[specimen.Count - 1];
        }

        //selects a genome from the species based on their fitness
        public Genome selectRandomGenome()
        {
            double randomFitness = rand.NextDouble() * SharedFitnessSum;

            Double fitness = 0.0f;
            var i = -1;
            do
            {
                i++;
                fitness += specimen[i].AdjustedFitness;
            } while (i < specimen.Count  && fitness <= randomFitness);

            if (i >= specimen.Count) throw new Exception("Something went really really wrong");

            return specimen[i];
        }

        public bool isCompatible(Genome g)
        {
            return (Genome.CompatibilityDistance(reference, g) < TuningParameters.COMPATIBILITY_THRESHOLD);
        }

        public int CompareTo(object obj)
        {
            if (obj is Species)
            {
                var s = (Species)obj;
                return CurrentBestFitness.CompareTo(s.CurrentBestFitness);
            }
            else throw new ArgumentException("Object is not Species");
        }

        #endregion
    }
}
