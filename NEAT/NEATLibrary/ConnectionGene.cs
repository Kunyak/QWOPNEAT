using System;
using System.Collections.Generic;
using System.Xml;


namespace NEATLibrary
{
    class ConnectionGene
    {
        
        public int inNode { get; private set; }
        public int outNode { get; private set; }
        public Double Weight;
        public bool isEnabled { get; set; }
        public int Innovation { get; private set; }


        public ConnectionGene(int _in, int _out, double _weight, bool _expressed, GeneMarker marker)
        {
            inNode = _in;
            outNode = _out;
            Weight = _weight;
            isEnabled = _expressed;
            Innovation = marker.getMarker();
        }

        public ConnectionGene(XmlReader r)
        {
            ReadXml(r);
        }

        public ConnectionGene(ConnectionGene gene) // Cloning
        {
            inNode = gene.inNode;
            outNode = gene.outNode;
            Weight = gene.Weight;
            isEnabled = gene.isEnabled;
            Innovation = gene.Innovation;
        }

        public void Disable() 
        {
            isEnabled = false;
        }

        public bool isEqualNodes(ConnectionGene gene, bool isTwoWay)
        {
            if (isTwoWay)
            {
                return ((inNode == gene.inNode && outNode == gene.outNode) || (inNode == gene.outNode && outNode == gene.inNode));
            }
            else
            {
                return (inNode == gene.inNode && outNode == gene.outNode);
            }
            
        }

        public bool isEqualNodes(int _in, int _out, bool isTwoWay)
        {
            if (isTwoWay)
            {
                return ((inNode == _in && outNode == _out) || (inNode == _out && outNode == _in));
            }
            else
            {
                return (inNode == _in && outNode == _out);
            }
        }
        #region SERIALIZATION

        private void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == GetType().ToString())
            {
                inNode = int.Parse(reader["inNode"]);
                outNode = int.Parse(reader["outNode"]);
                Weight = double.Parse(reader["Weight"]);
                isEnabled = bool.Parse(reader["isEnabled"]);
                Innovation = int.Parse(reader["Innovation"]);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().ToString());
            writer.WriteAttributeString("inNode", inNode.ToString());
            writer.WriteAttributeString("outNode", outNode.ToString());
            writer.WriteAttributeString("Weight", Weight.ToString());
            writer.WriteAttributeString("isEnabled", isEnabled.ToString());
            writer.WriteAttributeString("Innovation", Innovation.ToString());
            writer.WriteEndElement();
        }
        #endregion
    }
}
