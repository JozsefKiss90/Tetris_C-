using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    public Board board { get; private set;}
    
    public Vector3Int position  = new Vector3Int(-2, 4, 0);
    public Vector3Int[] cells{ get; private set;}
    public TetrominoData data { get; private set;}
    public bool isInitialized { get; private set; } = false;

    public void Initialize(Board newBoard, TetrominoData newData)
    {
        if (newBoard.gameOver) return;
        board = newBoard;
        data = newData;
        isInitialized = true;
        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length]; 
        }
        
        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }
    
    /*public void UpdateData(TetrominoData newData)
    {
        if (board.gameOver)
        {
            return;
        }
        data = newData;

        if (cells == null || cells.Length != data.cells.Length)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }*/

}