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
    public Hold holdPiece{ get; private set; }
    public bool isHeld = false;
    public bool retrieveEnabled = true;
    public Ghost ghostPiece;
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public int score = 0;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI gameOverText;
    public TMPro.TextMeshProUGUI restartText;
    public TMPro.TextMeshProUGUI holdText;
    public bool gameOver { get; private set; }
    private float timeSinceGameOver = 0f;

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
        ghostPiece = GetComponentInChildren<Ghost>(); 
        holdPiece = null;
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Update()
    {
        HoldListener();
        if (gameOver)
        {
            timeSinceGameOver += Time.deltaTime;

            if (timeSinceGameOver > 1f && Input.GetKeyDown(KeyCode.Space))
            {
                RestartGame();
            }
            return;
        }
    }

    private void Start()
    {
        SpawnPiece();
    }
    
    private void RestartGame()
    {
        gameOver = false;
        score = 0;
        scoreText.text = "0";

        tilemap.ClearAllTiles();

        gameOverText.text = "";
        restartText.text = "";

        SpawnPiece();
    }

    public void HoldListener()
    {
        if (Input.GetKeyDown(KeyCode.C) && isHeld && retrieveEnabled)
        {
            RetrieveHoldPiece();
            retrieveEnabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isHeld == false)
        {
            if (holdPiece is not null)
            {
                ClearHold(holdPiece);
            }
            HoldPiece(activePiece.data);
            isHeld = true;
        }
    }

    public void HoldPiece(TetrominoData tetromino)
    {
        tetrominoData.Remove(tetromino);
        if (holdPiece is null)
        {
            holdPiece = GetComponentInChildren<Hold>();
        }
        holdPiece.Initialize(this, tetromino);
        Clear(activePiece);
        ClearNext(nextPiece);
        SpawnPiece();
        SetHold(holdPiece);
    }

    public void RetrieveHoldPiece()
    {
        TetrominoData tempData = activePiece.data;
        Clear(activePiece);
        activePiece.Initialize(this, spawnPosition, holdPiece.data);
        ClearHold(holdPiece);
        holdPiece.Initialize(this, tempData);
        SetHold(holdPiece);
        // SpawnPiece
    }

    public void SpawnPiece()
    {
        if (gameOver)
        {   
            tilemap.ClearAllTiles();
            return;
        };
        if (nextPiece.isInitialized)
        {
            ClearNext(nextPiece);
        }

        while (tetrominoData.Count < 4)
        {
            int random = Random.Range(0, tetrominos.Length);
            tetrominoData.Add(tetrominos[random]);
        }

        activePiece.Initialize(this, spawnPosition, tetrominoData[0]);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            tetrominoData.Remove(tetrominoData[0]);
            nextPiece.Initialize(this, tetrominoData[1]);
            nextPiece.UpdateData(tetrominoData[0]);
        
            if (!gameOver)
            {
                Set(activePiece);
                SetNext(nextPiece);
            }
        }
        else
        {
            GameOver();
            timeSinceGameOver = 0f;
            tilemap.ClearAllTiles();
        }
    }

    private void GameOver()
    {
        gameOver = true;
        tilemap.ClearAllTiles();
        gameOverText.text = "GAME OVER";
        restartText.text = "press space\nto restart";
        ClearNext(nextPiece); 
    }
    
    public void Set(Piece piece)
    {
        if (gameOver)
        {   
            tilemap.ClearAllTiles();
            return;
        };
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    public void SetNext(Next piece)
    {   
        if (gameOver)
        {   
            tilemap.ClearAllTiles();
            return;
        };
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    public void SetHold(Hold piece)
    {   
        if (gameOver)
        {   
            tilemap.ClearAllTiles();
            return;
        };
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text ="" + score;
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
    
    public void ClearHold(Hold piece)
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
        int linescleared = 0;
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (isLineFull(row))
            {
                LineClear(row);
                linescleared++;
            }
            else
            {
                row++; 
            }
        }

        switch (linescleared)
        {
            case 1: AddScore(100); break;
            case 2: AddScore(200); break;
            case 3: AddScore(500); break;
            case 4: AddScore(1000); break;
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
