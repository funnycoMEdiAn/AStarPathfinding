using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public bool walkable; // Indicates whether the node is walkable or blocked (e.g., by an obstacle)
    public Vector3 worldPosition; // The world position of the node in 3D space

    // The X index and Y index of the node in the grid
    public int gridX;
    public int gridY;

    public int gCost; // The cost of moving from the starting node to this node
    public int hCost; // The estimated cost of moving from this node to the target node (heuristic)

    public Node parent; // Reference to the parent node in the pathfinding process (used to trace the path)

    int heapIndex; // Index of the node in the heap (used for efficient priority queue operations)

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) // Constructor to initialize the node with its properties
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost // Property to calculate the total cost (F-cost) of the node
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex // Property to get or set the heap index of the node (required by the heap implementation)
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    // Compares this node to another node based on their F-costs. If F-costs are equal, compares based on H-costs
    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost); // Compare based on F-cost

        // If F-costs are equal, compare based on H-cost (tie-breaker)
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        
        return -compare; // Return negative to prioritize lower F-costs in the heap
    }
}