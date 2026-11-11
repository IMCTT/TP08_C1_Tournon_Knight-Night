using UnityEngine;

public static class SaveData
{
    private const string CoinsKey = "Coins";
    private const string HighScoreKey = "HighScore";

    public static int Coins
    {
        get
        {
            return PlayerPrefs.GetInt(CoinsKey, 0);
        }
        set
        {
            PlayerPrefs.SetInt(CoinsKey, value); PlayerPrefs.Save();
        }
    }

    public static int HighScore
    {
        get
        {
            return PlayerPrefs.GetInt(HighScoreKey, 0); 
            
        }
        set
        {
            PlayerPrefs.SetInt(HighScoreKey, value); PlayerPrefs.Save();
        }
    }
}