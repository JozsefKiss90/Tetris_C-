using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public int width = 10; // Width of the grid
    public int height = 20; // Height of the grid
    public GameObject cellPrefab; // Prefab for a single grid cell
    float scale = 1f / 0.9f;

    void Start()
    {
        CreateGridContainer();
        DrawGrid();
    }

    void CreateGridContainer()
    {
        GameObject gridContainer = new GameObject("GridContainer");
        gridContainer.transform.position = Vector3.zero;
        gridContainer.transform.SetParent(transform); // Make it a child of the TetrisGrid object
    }

    void DrawGrid()
    {
        cellPrefab.transform.localScale = new Vector3(scale, scale, scale);

        Camera mainCamera = Camera.main;
        Vector3 cameraPosition = mainCamera.transform.position;
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;
        Vector3 gridPosition = new Vector3(cameraPosition.x - halfWidth, cameraPosition.y + halfHeight, 0);
        Transform gridContainer = transform.Find("GridContainer"); // Find the GridContainer we created

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(x, -y, 0) + gridPosition;
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
                cell.transform.SetParent(gridContainer); // Parent the cell to the GridContainer
            }
        }
    }
}