using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class WinScript : NetworkBehaviour
{
    [Header("Win Options")]
    public string m_newSceneName = "";
    public int m_newSceneId = 0;

    private bool m_tonyOnPlatform = false;
    private bool m_jeffOnPlatform = false;
    private bool m_win = false;

    private void OnTriggerEnter(Collider other) {
        if(other.transform.tag == "Tony") {
            m_tonyOnPlatform = true;
        } else if (other.transform.tag == "Jeff") {
            m_jeffOnPlatform = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.transform.tag == "Tony") {
            m_tonyOnPlatform = false;
        } else if (other.transform.tag == "Jeff") {
            m_jeffOnPlatform = false;
        }
    }

    private void Update() {
        if(m_jeffOnPlatform && m_tonyOnPlatform && !m_win) {
            m_win = true;

            Win();
        }
    }

    private void Win() {
        DisplayWinUI();
        StartCoroutine(NextSceneTimer());
    }

    private void DisplayWinUI() {

    }

    private IEnumerator NextSceneTimer() {
        float time = Time.timeSinceLevelLoad;

        string secret_key = "ndYo3sntLCPNFyVqYcO5nzeKcc53IX8jFOrVl3Y8FD43QLEjJQFr1RskTIfF";
        string name = "Jesper";
        string hs = time.ToString();

        UnityWebRequest req = UnityWebRequest.Get("http://lolihaven.org/it/upload/" + secret_key + "/" + name + "/" + hs);
        yield return req.SendWebRequest();

        if(GameObject.Find("NetworkManager") != null) {
            NetworkLobbyManager manager = GameObject.Find("NetworkManager").GetComponent<NetworkLobbyManager>();

            if(m_newSceneName == "MainMenu") {
                if(isServer) {
                    manager.StopHost();
                } else {
                    manager.StopClient();
                }
            }
            manager.ServerChangeScene(m_newSceneName);
        } else {
            SceneManager.LoadScene(m_newSceneId);
        } 
    }

}
