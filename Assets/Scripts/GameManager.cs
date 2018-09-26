using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager GameInstance;

    public int WinningScore = 5;

    void Awake()
    {
        if (GameInstance == null)
            GameInstance = this;

        else if (GameInstance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            SceneManager.LoadScene("PoolScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    public void SaveGame(PlayerScore PlayerScore_)
    {
        string DataJson = JsonUtility.ToJson(PlayerScore_);
        File.WriteAllText(Application.dataPath + "/StreamingAssets/GameData.json", DataJson);
        Debug.Log("Game data saved");
    }
}
