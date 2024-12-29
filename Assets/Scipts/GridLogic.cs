using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLogic : MonoBehaviour
{
    public bool onlyDisplayPathGizmos; // Option to display only the path gizmos in the editor
    public LayerMask unwalkableMask; // Layer mask to define which objects are unwalkable on the grid
    public Vector2 gridWorldSize; // The size of the grid in world space
    public float nodeRadius; // Radius of each node in the grid
    Node[,] grid; // 2D array representing the grid of nodes

    // Calculated properties for node dimensions
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        // Calculate the diameter of a node and determine the grid dimensions
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid(); // Generate the grid
    }

    public int MaxSize // Property to get the total number of nodes in the grid
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid() // Creates the grid of nodes based on the specified dimensions and properties
    {
        grid = new Node[gridSizeX, gridSizeY];
        // Calculate the bottom-left corner of the grid in world space
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Loop through each position in the grid and create a node
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Determine the world position of the node
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                // Check if the position is walkable by using the unwalkable mask
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                // Create a node at this position
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node) // Gets the neighboring nodes of a given node
    {
        List<Node> neighbours = new List<Node>();

        // Check all 8 possible directions around the node
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {    
                if (x == 0 && y == 0) // Skip the node itself
                    continue;

                // Calculate the grid position of the neighbor
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Ensure the neighbor is within the grid bounds
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) // Converts a world position into the corresponding node in the grid
    {
        // Normalize the world position to a percentage of the grid size
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Convert the percentage into grid coordinates
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }
  
    public List<Node> path; // List of nodes representing the current path, used for visualization
    void OnDrawGizmos() // Draws gizmos in the Unity Editor to visualize the grid and path
    {
        // Draw the outline of the grid
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            // If only displaying the path, draw it as black cubes
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else
        {
            // Otherwise, draw all nodes and highlight the path
            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    // Draw walkable nodes as white and unwalkable nodes as red
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    // Highlight the path nodes as black
                    if (path != null && path.Contains(n))
                        Gizmos.color = Color.black;

                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }

}
