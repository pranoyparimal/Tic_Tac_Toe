using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinningPanel : MonoBehaviour
{
    /// <summary> The Label that shows the winning player. /// </summary>
    [SerializeField] private TextMeshProUGUI m_TextMeshProUGUI;

    /// <summary>  Restart Btn - That restarts the Game. /// </summary>
    [SerializeField] private Button m_Restart_Btn;

    private void Awake()
    {
        m_Restart_Btn.onClick.AddListener(OnRestartBtnClicked);
    }

    private void OnRestartBtnClicked()
    {
        //Debug.Log("On Restart Btn Clicked Functionality yet to be implemented.");
        GridControllerEvents.RaiseOnGameReset();
    }

    public void ShowWinningStatus(CurrentStatus _status)
    {
        Debug.Log($"<color=green>The Player {_status} wins.</color>");
        m_TextMeshProUGUI.SetText($"The Player {_status} wins.");
    }
    
    public void ShowDrawStatus()
    {
        Debug.Log($"<color=green>The Game is Drawn. </color>");
        m_TextMeshProUGUI.SetText($"The Game is Drawn.");
    }

    private void OnDestroy()
    {
        m_Restart_Btn.onClick.RemoveListener(OnRestartBtnClicked);
    }
}
