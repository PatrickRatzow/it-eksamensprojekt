using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyUI : NetworkBehaviour
{

    [Header("Levels")]
    public Button level1But;

    public Scene[] scenes;
    private int selectedLevel = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer) {
            transform.Find("UI").gameObject.SetActive(false);
        }
        level1But.onClick.AddListener(SetLevel1);
    }

    // Update is called once per frame
    void Update()
    {
        if(isServer) {
            CmdSetLevel(selectedLevel);
        }
    }

    void SetLevel1() {
        selectedLevel = 0;
        transform.Find("UI").gameObject.SetActive(false);
    }

    [Command]
    private void CmdSetLevel(int i) {
        RpcSetLevel(i);
    }

    [ClientRpc]
    private void RpcSetLevel(int i) {
        NetworkLobbyManager manager = GetComponent<NetworkLobbyManager>();
        //manager.playScene = scenes[selectedLevel].name;
    }
}
