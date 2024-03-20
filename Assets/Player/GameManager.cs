using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int coinsCounter = 0;
    public TextMeshProUGUI coinText;
    public Person2 player;

    void Update()
    {
        coinText.text = coinsCounter.ToString();
        /* if (player.RespawnPoint == true)
         {
             // Reload the scene after 3 seconds
             Invoke("ReloadLevel", 1);
        } */

    }
    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
