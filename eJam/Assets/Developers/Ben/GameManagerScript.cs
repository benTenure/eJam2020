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
        StartCoroutine(LoseGame(player));
    }

    IEnumerator LoseGame(Transform player)
    {
        Camera cam = Camera.main;
        float zoomIn = 3;
        yield return new WaitForSeconds(2);
        while(cam.orthographicSize > zoomIn+.1)
        {
            yield return new WaitForEndOfFrame();
            float newAmount = Mathf.Lerp(cam.orthographicSize, zoomIn, 2 * Time.deltaTime);
            cam.orthographicSize = newAmount;

            Vector3 targetPos =cam.transform.position + Vector3.ProjectOnPlane((player.position - cam.transform.position), cam.transform.forward);

            Vector3 newPos = Vector3.Lerp(cam.transform.position, targetPos, 2 * Time.deltaTime);
            cam.transform.position = newPos;

        }
        Camera.main.orthographicSize = zoomIn;
        ui.SwitchToDeathScreen();
    }
}
