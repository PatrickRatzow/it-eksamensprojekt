using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class HighscoreEntry
{
	public int number;
	public string name;
	public float highscore;
}

public class JsonHelper
{
	public static T[] getJsonArray<T>(string json)
	{
		string newJson = "{ \"array\": " + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
		return wrapper.array;
	}

	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] array;
	}
}

public class MainMenu : MonoBehaviour
{
    [Header("MainScreen")]
    public GameObject mainScreen;
    public Button mainSingleBut;
    public Button mainMultiBut;
    public Button mainHighScoreBut;
    public Button mainQuit;

    [Space]

    [Header("SingleplayerScreen")]
    public GameObject singleScreen;
    public Button singleLevel1;
    public Button singleBack;

    [Space]

    [Header("MultiplayerScreen")]
    public GameObject multiScreen;
    public Button multiJoinBut; 
    public Button multiHostBut;
    public Button mulBackBut;
    public GameObject multiJoinScreen;
    public InputField multiJoinIp;
    public InputField multiJoinPort;
    public Button multiJoinGameBut;
    public GameObject multiHostScreen;
    public InputField multiHostPort;
    public Button multiHostStartGameBut;

	[Space]

	[Header("HighscoreScreen")]

	public GameObject highScreen;
	public Button highBack;
	public RectTransform highEntry;
    

    // Start is called before the first frame update
    void Start()
    {

        //adding listeners
        mainSingleBut.onClick.AddListener(SingleScreen);
        mainMultiBut.onClick.AddListener(MultiScreen);
        multiJoinBut.onClick.AddListener(MultiJoinScreen);
        multiHostBut.onClick.AddListener(MultiHostScreen);
        mulBackBut.onClick.AddListener(BackBut);
        singleBack.onClick.AddListener(BackBut);
        multiJoinGameBut.onClick.AddListener(JoinServer);
        multiHostStartGameBut.onClick.AddListener(HostGame);
        singleLevel1.onClick.AddListener(SingleLevel1);
		highBack.onClick.AddListener(BackBut);
		mainHighScoreBut.onClick.AddListener(HighScreen);

		MainScreen();
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            MainScreen();
        }
    }

    void MainScreen() {
        ClearScreen();
        mainScreen.SetActive(true);
    }

    void SingleScreen() {
        ClearScreen();
        singleScreen.SetActive(true);
    }

    void MultiScreen() {
        ClearScreen();
        multiScreen.SetActive(true);
    }

	void HighScreen() {
		ClearScreen();
		highScreen.SetActive(true);

		StartCoroutine(GetHighscores());
	}

	IEnumerator GetHighscores() {
		UnityWebRequest req = UnityWebRequest.Get("http://lolihaven.org/it");
		yield return req.SendWebRequest();

		if (req.isNetworkError || req.isHttpError)
		{
			Debug.Log(req.error);
		} else
		{
			HighscoreEntry[] entries = JsonHelper.getJsonArray<HighscoreEntry>(req.downloadHandler.text);

			int i = 1;
			foreach (HighscoreEntry a in entries) {
				a.number = i;

				RectTransform entry = Instantiate(highEntry);
				entry.transform.SetParent(highScreen.transform);
				entry.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, -115, entry.rect.width);
				entry.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -253 + ((i - 1) * 125), entry.rect.height);

				RectTransform number = entry.GetChild(0) as RectTransform;
				number.GetComponent<Text>().text = "#" + i.ToString();

				RectTransform name = entry.GetChild(1) as RectTransform;
				name.GetComponent<Text>().text = a.name;

				RectTransform highscore = entry.GetChild(2) as RectTransform;
				highscore.GetComponent<Text>().text = a.highscore.ToString() + " seconds";

				i++;
			}
		}
	}

    void MultiJoinScreen() {
        ClearScreen();
        multiScreen.SetActive(true);
        multiJoinScreen.SetActive(true);
    }

    void MultiHostScreen() {
        ClearScreen();
        multiHostScreen.SetActive(true);
        multiScreen.SetActive(true);
    }

    void BackBut() {
        ClearScreen();
    }

    void SingleLevel1() {
        SinglePlayer(3);
    }


    void JoinServer() {
        NetworkLobbyManager manager = GameObject.Find("NetworkManager").GetComponent<NetworkLobbyManager>();
        manager.networkAddress = multiJoinIp.text;
        manager.networkPort = Convert.ToInt32(multiJoinPort.text);
        Debug.Log(manager.networkAddress);
        Debug.Log(manager.networkPort);
        manager.StartClient();

        manager.ServerChangeScene(manager.lobbyScene);
    }

    void HostGame() {
        NetworkLobbyManager manager = GameObject.Find("NetworkManager").GetComponent<NetworkLobbyManager>();
        manager.networkPort = Convert.ToInt32(multiHostPort.text);

        manager.StartHost();

        manager.ServerChangeScene(manager.lobbyScene);
    }

    void ClearScreen() {
        singleScreen.SetActive(false);
        multiScreen.SetActive(false);
        multiJoinScreen.SetActive(false);
        multiHostScreen.SetActive(false);
		highScreen.SetActive(false);
    }

    void SinglePlayer(int sceneInt) {
        SceneManager.LoadScene(sceneInt);
    }
}
