using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class JpsNode
    {
        public Vector2Int pos; // The position of this node
        public Vector2Int[] direactions; // Directions for further exploration
        public int cost; // The cost associated with reaching this node
        public List<Vector2Int> parents; // A list of parent positions for this node

        // Constructor
        public JpsNode(Vector2Int parentPos, Vector2Int pos, Vector2Int[] direactions, int cost)
        {
            this.pos = pos;
            this.direactions = direactions;
            this.cost = cost;
            this.parents = new List<Vector2Int>();
            parents.Add(parentPos);
        }
        
        //To string method
        public override string ToString()
        {
            return pos + "";
        }
    }

}