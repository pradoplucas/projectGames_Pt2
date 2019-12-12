using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptMenu : MonoBehaviour
{
    void Start(){
        Cursor.visible = true;
        GetComponent<AudioSource>().Play();
    }

    public void Jogar()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void Sobre()
    {

    }

    public void Sair()
    {
        Application.Quit();
    }
}
