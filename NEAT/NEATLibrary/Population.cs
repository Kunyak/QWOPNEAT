using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace NEATLibrary
{
    class Population
    {


        #region Population Definition
        public List<Genome> currentGeneration;
        public Dictionary<int, Genome> ProgressionHistory; //Hall of Fame
        public int SpeciesCount { get { return (species != null) ? species.Count : 0; } }

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

        public Population(XmlReader r)
        {
            ProgressionHistory = new Dictionary<int, Genome>();
            currentGeneration = new List<Genome>();
            species = new List<Species>();
            random = new Random();
            bestScore = 0;
            PopulationInproductivity = 0;

            ReadXml(r);

        }

        #endregion


        #region public methods

        public void setCommonMarker() // if the population is just serialized or new genome added then yous should call this function
        {
            if (currentGeneration.Count > 1)
            {
                GeneMarker marker = currentGeneration[0].Marker;

                var i = 1;
                while (i < currentGeneration.Count && currentGeneration[i].Marker == marker) i++;

                if (i == currentGeneration.Count || marker == null) // means there are different markers or ther arent any
                {

                    //get maximum innovation number
                    var max = currentGeneration[0].maxInnovation;
                    for (int j = 1; j < currentGeneration.Count; j++)
                    {
                        if (currentGeneration[j].maxInnovation > max) max = currentGeneration[j].maxInnovation;
                    }

                    GeneMarker COMMONMARKER = new GeneMarker(max + 1);

                    // set every marker reference to the common one

                    foreach (Genome g in currentGeneration)
                    {
                        g.Marker = COMMONMARKER;
                        //also its wise to use only one instance of random so i make it common too
                        g.random = random;
                    }

                }
            }
        }

        public void addGenome(Genome g)
        {
            if (currentGeneration.Count < popSize) currentGeneration.Add(g);
        }



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
            Debug.WriteLine("gen: " + Generation.ToString(), "GenerationReport");
            Debug.WriteLine("species: " + species.Count.ToString(), "GenerationReport");
            Debug.WriteLine("maxfitness: " + bestScore, "GenerationReport");

#endif      

        }
        #endregion


        #region private methods

        private double sumOFavgSpeciesFitness()
        {
            double sum = 0f;
            foreach (Species s in species)
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
                    s.ShareFitness();
                    s.sortGenomes(); // sort the genomes based on fitness
                    s.Split(); //kill the lowest half of each species
                               // s.ShareFitness(); //set the shared fitness and calculate the fitness of the species => calculating shared fitness after splitting caused some bugs...
                    s.setProductivity(); //set the inproductivity of a species
                }
            }

            // Sort the species
            species.Sort();

            //kill species that are empty or haven't progressed in 15 generations (if its not in the first x species beacuase we dont want to remove all species)
            var FitnessSum = sumOFavgSpeciesFitness();
            foreach (Species s in species.ToArray())
            {

                if (AvaibleSpace(s, FitnessSum) < 1 && species.Count > 1) //wouldn't get place in ngen anyways
                {
                    species.Remove(s);
                }
                else if (s.Inproductivity >= TuningParameters.INPRODUCTIVITY_THRESHOLD && species.Count > TuningParameters.SAFE_SPECIES)
                {
                    species.Remove(s);
                }


            }
        }

        private void RecordHistory() //saves the progression of the population
        {
            currentGeneration.Sort();
            var Champion = currentGeneration.Max();

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


            if (species.Count > 1)
            {
                var generationChampion = currentGeneration.Max();
                nextGen.Add(new Genome(generationChampion));
            }//there was  a bug that sometimes the overall champion doesn't get to the next generation,
             //it seems to be fixed but i leave it here so i make sure there is no declension in the populations


            species.Reverse(); // reverse so fittest gets the first places
            foreach (Species s in species)
            {
                nextGen.Add(new Genome(s.Champion()));

                var AS = AvaibleSpace(s, FitnessSum) - 1; // -1 because the champion is already added
                for (int i = 0; i < AS; i++) nextGen.Add(getOffspring(s));

            }

            // stg went really wrong
            if (nextGen.Count > popSize)
            {
                nextGen.RemoveRange(popSize, nextGen.Count - popSize);
                Debug.WriteLine("Offspringoverload");
            }


            // fill missing space 
            var StrongestSpecies = species[0];
            while (nextGen.Count < popSize)
            {
                nextGen.Add(getOffspring(StrongestSpecies));
            }

            currentGeneration = nextGen;
            Generation++;

        }

        #endregion


        #region Serializer
        private void ReadXml(XmlReader reader)
        {
            reader.Read(); // skip "Population"
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == GetType().ToString())
            {
                globalBestScore = double.Parse(reader["globalBestScore"]);
                popSize = int.Parse(reader["popSize"]);
                Generation = int.Parse(reader["Generation"]);

                reader.Read();


            }
        }

        public void WriteXml(XmlWriter writer)
        {

            writer.WriteStartElement(GetType().ToString()); // write out population
            writer.WriteAttributeString("globalBestScore", globalBestScore.ToString());
            writer.WriteAttributeString("popSize", popSize.ToString());
            writer.WriteAttributeString("Generation", Generation.ToString());

        }
        #endregion
    }
}
