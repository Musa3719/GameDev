using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject Menu;
    public GameObject LoadMenu;
    public GameObject OptionsMenu;

    public GameObject SavePrefab;
    bool isLoading;

    private void Awake()
    {
        Time.timeScale = 1;
    }
    public void StartNewGame()
    {
        if (!isLoading)
        {
            SceneManager.LoadScene(1);
            isLoading = true;
        }
        
    }
    public void LoadScene(int sceneIndex)
    {
        if (!isLoading)
        {
            SceneManager.LoadScene(sceneIndex);
            isLoading = true;
        }
        
    }
    public void ToMenu()
    {
        Menu.SetActive(true);
        LoadMenu.SetActive(false);
        OptionsMenu.SetActive(false);
    }
    public void ToLoadMenu()
    {
        Menu.SetActive(false);
        LoadMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }
    public void SpawnAllSaves()
    {
        //SpawnSave() for all save files
    }
    public void SpawnSave()
    {
        Instantiate(SavePrefab, LoadMenu.transform.Find("Saves"));
    }
    public void LoadSave()
    {

    }
    public void ToOptionsMenu()
    {
        Menu.SetActive(false);
        LoadMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
