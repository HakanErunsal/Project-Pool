using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Loader : MonoBehaviour {

    public GameObject GameManager_;
    GameObject GameManagerRef;

    private string FilePath;
    private string GameDataFileName = "/StreamingAssets/GameData.json";
    public Text ScoreText, TimeText, HitCountText;

    private PlayerScore PlayerScore_;

    void Awake()
    {
        //Spawn Game Manager if it doesn't exist in the current scene, if does, get the reference
        if (GameManager.GameInstance == null)
        {
            GameManagerRef = Instantiate(GameManager_);
        }
        else
        {
            GameManagerRef = GameManager.GameInstance.gameObject;
        }

        FilePath = Application.dataPath + GameDataFileName;

        LoadGameData();
    }

    public void LoadGameData()
    {
        //Load if exists, create if doesn't
        if (File.Exists(FilePath))
        {
            string DataJson = File.ReadAllText(FilePath);

            PlayerScore PlayerScore_ = JsonUtility.FromJson<PlayerScore>(DataJson);
            SetLoadedInfoOnScreen(PlayerScore_);

            Debug.Log("Game data loaded");
        }

        else
        {
            PlayerScore_ = new PlayerScore
            {
                Score_ = 0,
                Time_ = 0,
                HitCount_ = 0
            };

            SaveGameData(PlayerScore_);
            SetLoadedInfoOnScreen(PlayerScore_);
            Debug.Log("Game data created");
        }
    }

    public void SaveGameData(PlayerScore PlayerScore_)
    {
        string DataJson = JsonUtility.ToJson(PlayerScore_);
        File.WriteAllText(FilePath, DataJson);
        Debug.Log("Game data saved");
    }

    public void LoadScene()
    {
        GameManagerRef.GetComponent<GameManager>().LoadScene();
    }

    public void ChangeVolume(float Value)
    {
        AudioListener.volume = Value;
    }

    void SetLoadedInfoOnScreen(PlayerScore PlayerScore_)
    {
        ScoreText.text = "Score: " + PlayerScore_.Score_.ToString();

        string Minutes = Mathf.Floor(PlayerScore_.Time_ / 60).ToString("00");
        string Seconds = Mathf.RoundToInt(PlayerScore_.Time_ % 60).ToString("00");
        TimeText.text = "Time: " + Minutes + ":" + Seconds;


        HitCountText.text = "HitCount: " + PlayerScore_.HitCount_.ToString();
    }
}
