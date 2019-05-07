using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : MonoBehaviour
{
    [Header("Options")]
    public bool m_playingMultiplayer = false;
    private GameObject[] m_players;

    private void Update() {
        m_players = GameObject.FindGameObjectsWithTag("Player");

        if(m_players.Length > 1) {
            m_playingMultiplayer = true;
        } else {
            m_playingMultiplayer = false;
        }
    }

    public int GetPlayerCount() {
        return m_players.Length;
    }
}
