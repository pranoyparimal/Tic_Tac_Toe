using System;
using TMPro;
using UnityEngine;

public class PlayerActivityIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Player_OneActivity;
    [SerializeField] private TextMeshProUGUI m_Player_TwoActivity;

    private void Awake()
    {
        TurnManagerEvents.OnTurnChanged += OnPlayerTurnChanged;
    }

    private void OnPlayerTurnChanged(TurnManager.PlayerId id, CurrentStatus status, bool _markerPlaced)
    {
        if (_markerPlaced)
        {
            if (m_Player_OneActivity != null && id == TurnManager.PlayerId.Player1) 
            {
                m_Player_OneActivity.text = $"{id} placed {status}";
            }
            else if (m_Player_TwoActivity != null && id == TurnManager.PlayerId.Player2) 
            {
                m_Player_TwoActivity.text = $"{id} placed {status}";
            }
        }
        else
        {
            m_Player_OneActivity.text = $"";
            m_Player_TwoActivity.text = $"";
        }
    }

    private void OnDestroy()
    {
        TurnManagerEvents.OnTurnChanged -= OnPlayerTurnChanged;
    }
}
