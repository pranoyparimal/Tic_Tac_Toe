using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CellGrid : MonoBehaviour
{
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private bool isInitialized = false;

    public enum CurrentStatus
    {
        Empty,
        O,
        X
    }
    public CurrentStatus status;

    public int Row
    {
        get { return row; }
    }

    public int Column
    {
        get { return column; }
    }

    public void Initialize(int _row, int _column)
    {
        if (isInitialized)
        {
            Debug.Log($"Grid Cell already Initialized: ({this.row}, {this.column})");
            return;
        }

        this.row = _row;
        this.column = _column;
        status = CurrentStatus.Empty;
        isInitialized = true;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnGridSelected);
    }

    private void OnGridSelected()
    {
        if (status != CurrentStatus.Empty) return;
        Debug.Log($"The Grid is selcted: <color=yellow> ({this.row}, {this.column})</color>");
        GridEvents.RaiseCellClicked(this);
    }

    private void OnDestroy()
    {
       GetComponent<Button>().onClick?.RemoveListener(OnGridSelected);
    }
}