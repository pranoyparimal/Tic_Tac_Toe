using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CellGrid : MonoBehaviour
{
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private bool isInitialized = false;

    [SerializeField] private TMP_Text markLabel; // assign in prefab, or auto-found below

    public enum CurrentStatus
    {
        Empty,
        O,
        X
    }
    private CurrentStatus status = CurrentStatus.Empty;
    public CurrentStatus Status => status;

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

        if (markLabel == null)
            markLabel = GetComponentInChildren<TMP_Text>();
    }

    private void OnGridSelected()
    {
        if (status != CurrentStatus.Empty) return;
        Debug.Log($"The Grid is selcted: <color=yellow> ({this.row}, {this.column})</color>");
        GridEvents.RaiseCellClicked(this);
    }

    /// <summary>Called externally (by TurnManager) once a move is accepted.</summary>
    public void SetMark(CurrentStatus mark)
    {
        status = mark;

        if (markLabel != null)
            markLabel.text = mark == CurrentStatus.Empty ? "" : mark.ToString();
        else
            Debug.LogWarning($"CellGrid ({row},{column}): no TMP_Text found to display mark.");
    }

    private void OnDestroy()
    {
       GetComponent<Button>().onClick?.RemoveListener(OnGridSelected);
    }
}