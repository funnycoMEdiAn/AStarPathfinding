using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target; // Transforms representing the seeker (start position) and the target (end position)

    GridLogic grid; // Reference to the grid logic script that manages the grid and nodes

    void Awake() // Initializes the grid reference when the script is loaded
    {
        grid = GetComponent<GridLogic>();
    }

    void Update()  // Find a path when spacebar is pressed
    {
        if (Input.GetButtonDown("Jump"))
        {
            FindPath(seeker.position, target.position);
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos) // The main pathfinding method that implements the A* algorithm
    {
        Stopwatch sw = new Stopwatch(); // Stopwatch to measure the time taken to find the path
        sw.Start();

        // Get the start and target nodes based on their world positions
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize); // Open set (priority queue) for nodes to be evaluated
        HashSet<Node> closedSet = new HashSet<Node>(); // Closed set (hash set) for nodes that have already been evaluated
        openSet.Add(startNode);

        while (openSet.Count > 0) // Process nodes in the open set until it's empty or the target is found
        {            
            Node currentNode = openSet.RemoveFirst(); // Get the node with the lowest F cost from the open set
            closedSet.Add(currentNode);

            if (currentNode == targetNode) // If the current node is the target node, pathfinding is complete
            {
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, targetNode); // Generate the path
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) // Evaluate all neighboring nodes of the current node
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) // Skip the neighbor if it's not walkable or has already been evaluated
                {
                    continue;
                }
           
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour); // Calculate the new movement cost to the neighbor
                
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) // If the new cost is lower or the neighbor is not in the open set, update it
                { 
                    // Update the neighbor's costs and set its parent to the current node
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    // Add the neighbor to the open set if it's not already there
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                    {
                        // openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode) // Retraces the path from the target node back to the start node and stores it
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
     
        while (currentNode != startNode) // Follow the parent nodes from the end node to the start node
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse(); // Reverse the path to get it from start to end

        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB) // Calculates the distance between two nodes using a weighted Manhattan distance
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}