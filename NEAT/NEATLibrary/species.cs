using System;
using System.Collections.Generic;
using System.Text;

namespace NEATLibrary
{
    class Species : IComparable
    {
        private Genome reference;
        private List<Genome> specimen;
        private Random rand;
        public double avgFitness { get; private set; }
        public int Inproductivity { get; private set; }
        public int Count { get { return specimen.Count; } }

        private bool sorted;

        public Species (Genome referenceGenome)
        {
            sorted = false;
            specimen = new List<Genome>();
            reference = new Genome(referenceGenome);
            rand = reference.random;
            specimen.Add(referenceGenome);
            
        }

        public void Add(Genome g)
        {
            specimen.Add(g);
        }

        public void Clear()
        {
            // get new random reference
            if (specimen.Count > 1)
            {
                reference = new Genome(specimen[new Random().Next(specimen.Count)]); // gets a random specimen to represent the species
            }
            specimen.Clear();

        }

        public void AdjustFitness()
        {
            var N = specimen.Count;
            double sum = 0f;
            foreach (Genome g in specimen)
            {
                g.AdjustedFitness = g.Fitness / N;
                sum += g.AdjustedFitness; 

            }

            specimen.Sort();
            sorted = true;

            //get productivity of the species

            if (sum > avgFitness)
            {
                Inproductivity = 0;
            }
            else
            {
                Inproductivity++;
            }

            avgFitness = sum;

            
        }

        public void Split()
        {
            if (!sorted)
            {
                specimen.Sort();
            }

            if (specimen.Count > 2)
            {
                specimen.RemoveRange(0, specimen.Count / 2);
            }

            Fitness(); // the avg fitness and sum fitness is changed

        }

        public Genome Champion()
        {
            if (!sorted)
            {
                specimen.Sort();
                sorted = true;
            }

            return (specimen.Count > 0)?specimen[specimen.Count -1 ]: null;
        }

        public Genome selectRandomGenome()
        {
            double rnd = rand.Next(0, (int)Math.Ceiling(avgFitness));

            Double c = 0.0f;
            var i = 0;
            while (i < specimen.Count -1 && c <= rnd)
            {
                c += specimen[i].AdjustedFitness;
                i++;
            }

 //           if (i == specimen.Count) i--;

            return specimen[i];

        }

      
        public bool Compatible(Genome g)
        {
            return (Genome.CompatibilityDistance(reference, g) < TuningParameters.COMPATIBILITY_THRESHOLD);
        }

        public int CompareTo(object obj)
        {
            if (obj is Species)
            {
                var s = (Species)obj;
                return avgFitness.CompareTo(s.avgFitness);
            }
            else throw new ArgumentException("Object is not Species");
        }


        private void Fitness()
        {
            double sum = 0f;
            foreach (Genome g in specimen)
            {
                sum += g.AdjustedFitness;

            }
            avgFitness = sum;
        }

    }
}
