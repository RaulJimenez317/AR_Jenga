using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void IniciarPartida()
    {
        SceneManager.LoadScene("JengaAR");
    }

    public void Salir()
    {
        Application.Quit();
    }
}