using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptGameOver : MonoBehaviour
{
    void Start(){
        Cursor.visible = true;
    }

    public void JogarNovamente()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void MenuPrincipal()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
