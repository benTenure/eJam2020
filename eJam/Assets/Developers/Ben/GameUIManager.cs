using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public IntVariable distance;
    public IntVariable people;

    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI peopleText;

    public TextMeshProUGUI distanceStats;
    public TextMeshProUGUI peopleStats;
    
    public GameObject inGamePanel;
    public GameObject deathPanel;

    private void Start()
    {
        inGamePanel.SetActive(true);
        deathPanel.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        distanceText.text = distance.value.ToString();

        peopleText.text = people.value.ToString();
    }

    public void RestartGame()
    {
        // We're just assuming we're reloading the game scene since yeah... idk
        print("WE did it boss!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SwitchToDeathScreen()
    {
        distanceStats.text = $"{distance.value} blocks";
        peopleStats.text = $"{people.value} people";
        
        inGamePanel.SetActive(false);
        deathPanel.SetActive(true);
    }
}
