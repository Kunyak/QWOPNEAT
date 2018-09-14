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

        public NodeGene(NodeType type, int id)
        {
            Type = type;
            Id = id;
        }
        /// <summary>
        /// Checks if there can be a connection originating from a node to the destination node
        /// 1 means its good, 0 means it has to be reversed, -1 means its never happenning
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public int canConnectTo(NodeGene destination)
        {
            if ( (Type == destination.Type && this.Type != NodeType.Hidden) || (Id == destination.Id) )
            {
                return -1; // absolutely can't connect
            }

            if (Type > destination.Type) // can join, but a reverse is needed
            {
                return 0;
            }

            return 1;
        }



    }
}
