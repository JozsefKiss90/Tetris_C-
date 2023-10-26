using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class Piece : MonoBehaviour
{
    public Board board { get; private set;}
    public Vector3Int position { get; private set;}
    public Vector3Int[] cells{ get; private set;}
    public TetrominoData data { get; private set;}
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
        
    private float stepTime;
    private float lockTime;
    

    public void Initialize(Board newBoard, Vector3Int newPosition, TetrominoData newData)
    {
        board = newBoard;
        position = newPosition;
        data = newData;
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        lockTime = 0f;
        
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length]; 
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private bool isHolding = false;
    public float consecutiveMoveInterval = 0.1f; // time in seconds between consecutive moves
    private float nextConsecutiveMoveTime = 0f;  // internal timer for the next consecutive move

    private void Update()
    {
        if (board.gameOver) return;
        
        lockTime += Time.deltaTime;
    
        board.Clear(this);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Rotate(-1);
        } 
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Rotate(1);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (isHolding)
            {
                if (Time.time > nextConsecutiveMoveTime)
                {
                    Move(Vector2Int.left);
                    nextConsecutiveMoveTime = Time.time + consecutiveMoveInterval;
                }
            }
            else
            {
                Move(Vector2Int.left);
                isHolding = true;
                nextConsecutiveMoveTime = Time.time + consecutiveMoveInterval;
            }
        } 
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (isHolding)
            {
                if (Time.time > nextConsecutiveMoveTime)
                {
                    Move(Vector2Int.right);
                    nextConsecutiveMoveTime = Time.time + consecutiveMoveInterval;
                }
            }
            else
            {
                Move(Vector2Int.right);
                isHolding = true;
                nextConsecutiveMoveTime = Time.time + consecutiveMoveInterval;
            }
        } 
        else
        {
            isHolding = false;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }
    
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        if (Time.time >= stepTime)
        {
            Step();
        }
        board.Set(this);
    }
    
    private void Step()
    {
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);
        if (lockTime >= lockDelay)
        {
            if (!board.gameOver)
            {
                Lock();
            }
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock();
    }

    private void Lock()
    {
        if (board.gameOver) return; 
        board.Set(this);
        board.ClearLines();
        if (!board.gameOver)
        {
            board.SpawnPiece();
        }
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        
        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
            lockTime = 0f;
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];
            float x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                    float centerX = cell.x - 0.5f;
                    float centerY = cell.y - 0.5f;
                    
                    x = (centerX * Data.RotationMatrix[0] * direction) + (centerY * Data.RotationMatrix[1] * direction);
                    y = (centerX * Data.RotationMatrix[2] * direction) + (centerY * Data.RotationMatrix[3] * direction);
                    
                    x += 0.5f;
                    y += 0.5f;
                    cells[i] = new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y), 0);
                    break;
                case Tetromino.O:
                    break;
                default:
                    x = (cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction);
                    y = (cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction);

                    cells[i] = new Vector3Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y), 0);
                    break;
            }
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;
        
        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0)); 
    }
    
    private int Wrap(int input, int min, int max)
    {
        if (input < min) {
            return max - (min - input) % (max - min);
        } else {
            return min + (input - min) % (max - min);
        }
    }
}
