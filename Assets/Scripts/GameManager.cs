using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour {

    private string GameDataFileName = "GameData.json";

    private PlayerScore PlayerScore_;
    private string FilePath;

    void Start ()
    {
        DontDestroyOnLoad(this);

        FilePath = Path.Combine(Application.streamingAssetsPath, GameDataFileName);

        LoadGameData();
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

    public void ChangeVolume(float Value)
    {
        AudioListener.volume = Value;
    }

    public void LoadGameData()
    {
        

        if (File.Exists(FilePath))
        {
            string DataJson = File.ReadAllText(FilePath);

            PlayerScore PlayerScore_ = JsonUtility.FromJson<PlayerScore>(DataJson);
        }

        else
        {
            Debug.Log("No saved file");
        }
    }

    public void SaveGameData()
    {
        string DataJson = JsonUtility.ToJson(PlayerScore_);
        File.WriteAllText(FilePath, DataJson);
    }
}
