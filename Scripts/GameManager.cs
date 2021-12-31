using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject[] ThrowablePrefabs;

    public static bool isPaused;
    //public AudioClip clip;
    public GameObject HealthBack;
    public GameObject Health;
    public GameObject ArmorBack;
    public GameObject Armor;
    public GameObject StaminaBack;
    public GameObject Stamina;

    public GameObject InGameUI;
    public GameObject StopScreen;
    public GameObject GameOverScreen;


    public Dictionary<string, float> AnimationSpeedMultipliers;


    private bool isLoading;
    

    private void Awake()
    {
        instance = this;
        AnimationSpeedMultipliers = new Dictionary<string, float>();
        AnimationSpeedMultipliers.Add("Dodge", 4);
        //AnimationSpeedMultipliers.Add("key",value)
        ///////////////////////   buraya ekle   ///////////////////////
    }
    private void Start()
    {
        //SoundPlay.instance.PlayClip(clip, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
        CheckStopping();
    }
    private void CheckStopping()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (InGameUI.activeInHierarchy && Time.timeScale > 0)
            {
                StopGame();
            }
            else if (StopScreen.activeInHierarchy && Time.timeScale == 0)
            {
                ResumeGame();
            }
        }
        
    }
    public void UpdateUI()
    {
        Stamina.GetComponent<SlicedFilledImage>().fillAmount = Player.instance.Stamina / 100f;
        Health.GetComponent<SlicedFilledImage>().fillAmount = Player.instance.Health / Player.instance.MaxHealth;
        Armor.GetComponent<SlicedFilledImage>().fillAmount = Player.instance.Armor / Player.instance.MaxArmor;
        Health.GetComponent<RectTransform>().sizeDelta = new Vector2(Player.instance.MaxHealth * 5, Health.GetComponent<RectTransform>().sizeDelta.y);
        Armor.GetComponent<RectTransform>().sizeDelta = new Vector2(Player.instance.MaxArmor * 5, Armor.GetComponent<RectTransform>().sizeDelta.y);
    }
    public void ToMenu()
    {
        if (!isLoading)
        {
            SceneManager.LoadScene(0);
            isLoading = true;
        }
        
    }
    public void StopGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        InGameUI.SetActive(false);
        StopScreen.SetActive(true);
        GameOverScreen.SetActive(false);

        var sounds = GameObject.FindGameObjectsWithTag("Sound");
        foreach (var item in sounds)
        {
            item.GetComponent<DestroySound>().isPaused = true;
            item.GetComponent<AudioSource>().Pause();
        }
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        InGameUI.SetActive(true);
        StopScreen.SetActive(false);
        GameOverScreen.SetActive(false);

        var sounds = GameObject.FindGameObjectsWithTag("Sound");
        foreach (var item in sounds)
        {
            item.GetComponent<DestroySound>().isPaused = false;
            item.GetComponent<AudioSource>().UnPause();
        }
    }
    public void GameOver()
    {
        isPaused = true;
        Time.timeScale = 1;
        InGameUI.SetActive(false);
        StopScreen.SetActive(false);
        GameOverScreen.SetActive(true);

    }
    public void ReLoadWorld()
    {
        //respawn like dark souls
    }
}

