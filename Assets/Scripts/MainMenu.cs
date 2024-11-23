using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Carrega a cena principal do jogo
        SceneManager.LoadScene("MainScene");
    }

    public void ExitGame()
    {
        // Sai do jogo (só funciona em uma build)
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}
