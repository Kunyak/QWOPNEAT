using System;
using System.Collections.Generic;
using System.Text;

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

        public NodeType Type { get;}
        public int Id { get; }
        public Double LayerQuotient { get; set; }


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
        /// 1 means its good, 0 means it has to be reversed, -1 means its never happenning
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public int canConnectTo(NodeGene destination)
        {

            if (destination.Type == NodeType.Sensor) // destination cant be sensor;
            {
                return -1;
            }

            if (Type == destination.Type && this.Type != NodeType.Hidden) // hidden and output (pretty sure you mean sensor and output) neurons can't connect to the same type of neuron
            {
                return -1; // 
            }

            /*if ( (Type > destination.Type) || (LayerQuotient > destination.LayerQuotient) ) // can join, but a reverse is needed --> only used if you want Feed Forward Only ANN
            {
                return 0;
            }*/

            return 1;
        }



    }
}
