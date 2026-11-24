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
            // Suggestion: PlayerPrefs.Save() en cada moneda escribe a disco cada vez (hay 29 monedas en Gameplay). Alcanza con guardar al terminar la partida.
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