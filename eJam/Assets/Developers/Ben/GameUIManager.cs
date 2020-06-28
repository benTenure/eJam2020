using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public IntVariable distance;
    public IntVariable people;

    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI peopleText;

    // Update is called once per frame
    void Update()
    {
        distanceText.text = distance.value.ToString();

        peopleText.text = people.value.ToString();
    }
}
