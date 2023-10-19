using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominos;
    public Tilemap tilemap { get; private set; }
    public Piece activePiece{ get; private set; }
    List<TetrominoData> tetrominoData = new List<TetrominoData>();
    public Next nextPiece{ get; private set; }
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize); 
        } 
    }
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>(); 
        activePiece = GetComponentInChildren<Piece>(); 
        nextPiece = GetComponentInChildren<Next>(); 
        
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if (nextPiece.isInitialized)
        {
            ClearNext(nextPiece);
        }

        while (tetrominoData.Count < 4)
        {
            int random = Random.Range(0, tetrominos.Length);
            tetrominoData.Add(tetrominos[random]); //should be added to the back
        }
     
        activePiece.Initialize(this, spawnPosition, tetrominoData[0]);
        nextPiece.Initialize(this, tetrominoData[1]);
        //remove tetrominoData[0]
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
            tetrominoData.Remove(tetrominoData[0]);
            nextPiece.UpdateData(tetrominoData[0]);
            SetNext(nextPiece);  // Set the new next pie
        } else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        tilemap.ClearAllTiles();
        // extend logic 
    }
    
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    public void SetNext(Next piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }
    
    public void ClearNext(Next piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }


    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;
        
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if (tilemap.HasTile(tilePosition))
            {
                return false; 
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (isLineFull(row))
            {
                LineClear(row); 
            }
            else
            {
                row++; 
            }
        }
    }

    private bool isLineFull(int row)
    {
        RectInt bounds = Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true; 
    }

    private void LineClear(int  row)
    {
        RectInt bounds = Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            { 
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);
                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}
