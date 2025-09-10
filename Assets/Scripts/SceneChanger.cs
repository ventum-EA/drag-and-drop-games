using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }
    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);


    }
    public void ToCityScene()
    {
        SceneManager.LoadScene("CityScene", LoadSceneMode.Single);
    }
    
}