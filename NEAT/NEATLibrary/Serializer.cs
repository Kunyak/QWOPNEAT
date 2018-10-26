using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NEATLibrary
{
    class Serializer
    {
        private static string Gdirectory = "Genomes/";
        // class for serializing and deserializing Genomes and population

        public static void SerializeGenome(Genome g, string name)
        {

            Directory.CreateDirectory(Gdirectory); // if the Genomes dir doesnt exists create it

           
            var fileName = Gdirectory + name + ".genome";
            using (XmlWriter writer = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true, IndentChars = "\t" }))
            {
                writer.WriteStartDocument();
                g.WriteXml(writer);
                writer.WriteEndDocument();
            }
                          
        }

        /// <summary>
        /// IMPORTANT: After deserialization a new marker and a new random should be added!
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Genome DeserialiseGenome(string name)
        {
            var fileName = Gdirectory + name + ".genome";
            if (!File.Exists(fileName))
                throw new FileNotFoundException("N genome data found");

           
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                return new Genome(reader);
            }
        }

       public static void SerializePopulation(Population pop, string name)
        {
            //check if it already exists
            var dirName = Gdirectory + name+"/";

            if (Directory.Exists(dirName))
            {
                throw new IOException("Population with this name already exists");
            }
            else
            {
                Directory.CreateDirectory(dirName);
            }

            Dictionary<Genome, string> GenomeList = new Dictionary<Genome, string>();
            pop.currentGeneration.Sort(); // make sure its sorted;
            var nameIndex = 0; 
            //Save each genome individually with a unique name

            foreach(Genome g in pop.currentGeneration)
            {
                var filename = name + "_" + nameIndex++;
                GenomeList.Add(g,filename);
                SerializeGenome(g,name + "/" + filename);
            }
            // do the same with history

            foreach (Genome g in pop.ProgressionHistory.Values)
            {
                    var filename = name + "_h_" + nameIndex++;
                    GenomeList.Add(g, filename);
                    SerializeGenome(g, name + "/" + filename);
                
            }

            // Write the info.xml

            using (XmlWriter writer = XmlWriter.Create(dirName + "population.pinfo", new XmlWriterSettings() { Indent = true, IndentChars = "\t" }))
            {
                writer.WriteStartDocument();

                pop.WriteXml(writer);

                //Population
                writer.WriteStartElement("Generation");
                foreach (var g in pop.currentGeneration)
                {
                    writer.WriteStartElement("Genome");
                    writer.WriteAttributeString("path", GenomeList[g]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                //ProgresionHistory
                writer.WriteStartElement("ProgressionHistory");
                foreach(var g in pop.ProgressionHistory)
                {
                    writer.WriteStartElement("HistoricalProgression");
                    writer.WriteAttributeString("gen",g.Key.ToString());
                    writer.WriteAttributeString("fitness", g.Value.Fitness.ToString());
                    writer.WriteAttributeString("path", GenomeList[g.Value]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
        
        public static Population DeserialisePopulation(string name)
        {
            //return new Population(1,new Genome(1,1,new GeneMarker()));
            var dirname = Gdirectory + name + "/";
            if (!Directory.Exists(dirname)) throw new IOException("Population not found");


           using (XmlReader reader = XmlReader.Create(dirname + "population.pinfo"))
            {
                Population newpop = new Population(reader);

                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Generation")
                {
                    reader.Read();
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName.ToString() == "Genome")
                    {
                        Genome newGenome = DeserialiseGenome(name + "/" + reader["path"]);
                        newpop.addGenome(newGenome);
                        reader.Read(); // Skip to next genom
                    }

                }
                reader.Read();
                //Read progressionHistory
                if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ProgressionHistory")
                {
                    reader.Read();
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName.ToString() == "HistoricalProgression" )
                    {
                        Genome newGenome = DeserialiseGenome(name + "/" + reader["path"]);
                        newGenome.Fitness = double.Parse(reader["fitness"]);
                        newpop.ProgressionHistory.Add(int.Parse(reader["gen"]),newGenome);
                        reader.Read(); // Skip to next genome
                    }

                }
                
                // make a common historical marker
                newpop.setCommonMarker();

                return newpop;
            }

           
        }


    }
}
