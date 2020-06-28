using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGeneration : MonoBehaviour
{

    public float scrollModifier = 1;

    public WorldGenData data;

    public List<GameObject> sections;

    private GameObject world;
    private int nextSafeZone;

    void Awake(){
        GenerateStart();
    }

    void GenerateStart(){
        world = transform.gameObject;
        

        int minSafeDistance = data.minimumDistanceBetweenSafeZones;
        int maxSafeDistance = data.safeZoneDistanceVariance + minSafeDistance;

        nextSafeZone = 0 + Random.Range(minSafeDistance, maxSafeDistance);



        for (int i = 0; i < data.lengthGenerated; i++){
            GameObject newSection = CreateNewSection();
            newSection.transform.parent = world.transform;

            float zOffset = i * 10;
            Vector3 spawnPos = new Vector3(0,0, zOffset);

            newSection.transform.localPosition = spawnPos;
            sections.Add(newSection);
        }
    }

    GameObject CreateNewSection(){
        int r;
        GameObject newSection = new GameObject();

        print(nextSafeZone);
        if(GetBlocksRun() == nextSafeZone)
        {
            int minSafeDistance = data.minimumDistanceBetweenSafeZones;
            int maxSafeDistance = data.safeZoneDistanceVariance + minSafeDistance;

            nextSafeZone = nextSafeZone + Random.Range(minSafeDistance, maxSafeDistance);

            SpawnPeice(data.SafeZones[0], newSection);
        }
        else {
            r = Random.Range(0, data.TopSidewalks.Count);
            SpawnPeice(data.TopSidewalks[r], newSection);
        }



        r = Random.Range(0, data.Buildings.Count);
        SpawnPeice(data.Buildings[r], newSection);

        float r2 = Random.value;
        if (r2 > .7)
        {
            r = 0;
        }
        else
        {
            r = Random.Range(0, data.Roads.Count);
        }
        SpawnPeice(data.Roads[r], newSection);
        SpawnPeice(data.BottomSidewalks[0], newSection);
        return newSection;
    }

    void SpawnPeice(GameObject piece, GameObject parent)
    {
        Transform newPiece = Instantiate(piece, parent.transform).transform;
        newPiece.localPosition = Vector3.zero;
    }

    private void Update()
    {
        float distance = Time.deltaTime * data.scrollSpeed * scrollModifier;

        Vector3 worldPos = world.transform.localPosition;
        world.transform.localPosition = new Vector3(worldPos.x, worldPos.y, worldPos.z - distance);

        int sectionNumber = GetBlocksRun();

        for (int i = 0; i < sections.Count; i++)
        {
            Vector3 sectionPos = sections[i].transform.localPosition;
            if(sectionPos.z < sectionNumber * 10)
            {//gen new section
                GameObject newSection = CreateNewSection();
                newSection.transform.parent = world.transform;

                float zOffset = (sections[i].transform.localPosition.z + data.lengthGenerated * 10);
                Vector3 spawnPos = new Vector3(0, 0, zOffset);

                newSection.transform.localPosition = spawnPos;
                sections.Add(newSection);

                Destroy(sections[i]);
                sections.RemoveAt(i);
                i--;

                //Add to ui
                GameManagerScript.Instance.distance.value = GetBlocksRun()/4;

                UpdateDifficulty();

            }
        }
    }

    void UpdateDifficulty()
    {
        scrollModifier = 1 + (1 * (((float)GetBlocksRun()) / 4) / 50);
        if (scrollModifier > 2) scrollModifier = 2;
    }


    public void SetScrollModifier(float amount)
    {
        scrollModifier = amount;
    }

    public int GetBlocksRun()
    {
        return -Mathf.FloorToInt(world.transform.localPosition.z / 10);
    }

}
