using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CellGrid : MonoBehaviour, ICell
{
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private bool isInitialized = false;

    [SerializeField] private TMP_Text markLabel; // assign in prefab, or auto-found below


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
        GridEvents.RaiseCellClickedUI(this.row, this.column);
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

    public void HighLight()
    {
        Color _color = new Color(0.541f, 0.168f, 0.886f); // Approximate blueViolet
        var button = this.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = _color;
        colors.selectedColor = _color;
        button.colors = colors;
    }

    public void NormalizeCell()
    {
        Color _color = new Color(1.0f, 1.0f, 1.0f); // Approximate White
        var button = this.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = _color;
        colors.selectedColor = _color;
        button.colors = colors;
    }
}