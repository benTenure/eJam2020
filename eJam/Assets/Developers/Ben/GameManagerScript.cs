using System.Collections;
using System.Collections.Generic;
// using UnityEditorInternal;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private static GameManagerScript _instance;

    public IntVariable distance;
    public IntVariable people;

    public GameUIManager ui;

    [SerializeField]
    GameObject flatPlayer;
    
    public static GameManagerScript Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        distance.value = 0;
        people.value = 0;
    }

    public void AddPeopleSaved(int amount)
    {
         people.value += amount;
    }

    public void AddDistanceTraveled(int amount)
    {
        distance.value += amount;
    }

    public void PlayerDied(Transform player)
    {
        if (flatPlayer)
        {
            GameObject newFlat = Instantiate(flatPlayer, null);
            newFlat.transform.position = player.position;
        }

        ui.SwitchToDeathScreen();
    }
}
