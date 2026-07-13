using System;
using System.Collections;
using UnityEngine;

public class HUDCanvas : MonoBehaviour
{
    [SerializeField] private Canvas hudCanvas;

    /// <summary> The Winning Panel that gets displayed after the game is over /// </summary>
    [SerializeField] private WinningPanel mWinningPanel;
    private void GetCanvas () => hudCanvas = gameObject.GetComponent<Canvas> ();

    private void Awake()
    {
        GetCanvas ();

        GridControllerEvents.OnCellSelected += HandleOnCellSelected;
        GridControllerEvents.OnGameWon += HandleGameWon;
        GridControllerEvents.OnGameDraw += HandleOnGameDraw;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hudCanvas.enabled = false;
    }

    private void HandleOnCellSelected(ICell cell)
    {
        Debug.Log($"<color=red> This event in not implemented yet.</color>");
    }

    private void HandleGameWon(CurrentStatus status)
    {
        StartCoroutine(DelayBeforeGameOverAfterWin(status));
    }
    private void HandleOnGameDraw()
    {
        StartCoroutine(DelayBeforeGameOverAfterDraw(true));
    }

    private void OnDestroy()
    {
        GridControllerEvents.OnCellSelected -= HandleOnCellSelected;
        GridControllerEvents.OnGameWon -= HandleGameWon;
        GridControllerEvents.OnGameDraw -= HandleOnGameDraw;
    }

    private IEnumerator DelayBeforeGameOverAfterWin(CurrentStatus _status)
    {
        yield return new WaitForSeconds(1.0f);

        Debug.Log($"<color=green>Player {_status} won the game.</color>");
        mWinningPanel.ShowWinningStatus(_status);
        hudCanvas.enabled = true;
    }
    
    private IEnumerator DelayBeforeGameOverAfterDraw(bool _draw)
    {
        yield return new WaitForSeconds(1.0f);

        if (_draw)
        {
            Debug.Log($"The game is drawn");
            mWinningPanel.ShowDrawStatus();
            hudCanvas.enabled = true;
        }
    }
}
