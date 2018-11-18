using System;
using System.Xml;
using System.Xml.Schema;


namespace NEATLibrary
{

    public enum NodeType
    {
        Sensor,
        Hidden,
        Output
    }

    class NodeGene
    {

        public NodeType Type { get; private set; }
        public int Id { get; private set; }
        public Double LayerQuotient { get; private set; }


        public NodeGene(XmlReader r) // read from xml
        {
            ReadXml(r);
        }

        public NodeGene(NodeType type, int id, Double layer)
        {
            Type = type;
            Id = id;
            LayerQuotient = layer;
        }

        public NodeGene(NodeGene gene)
        {
            Type = gene.Type;
            Id = gene.Id;
            LayerQuotient = gene.LayerQuotient;
        }

        /// <summary>
        /// Checks if there can be a connection originating from a node to the destination node
        /// </summary>
        /// <param name="destination"></param>
        /// <returns>1 means its good, 0 means it has to be reversed, -1 means its never happenning</returns>
        public int canConnectTo(NodeGene destination)
        {

            if (destination.Type == NodeType.Sensor) // destination cant be sensor;
            {
                return -1;
            }

            if (Type == destination.Type && this.Type != NodeType.Hidden) // sensor and output neurons can't connect to the same type of neuron
            {
                return -1; // 
            }

           if (LayerQuotient > destination.LayerQuotient)
            {
                return 0;
            }

            return 1;
        }
        #region Serialization


        private void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == GetType().ToString())
            {
                Type = (NodeType)Enum.Parse(typeof(NodeType), reader["NodeType"]);
                Id = int.Parse(reader["Id"]);
                LayerQuotient = double.Parse(reader["LayerQuotient"]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {

            writer.WriteStartElement(GetType().ToString());
            writer.WriteAttributeString("NodeType", Type.ToString());
            writer.WriteAttributeString("Id", Id.ToString());
            writer.WriteAttributeString("LayerQuotient", LayerQuotient.ToString());
            writer.WriteEndElement();
        }
        #endregion
    }
}
