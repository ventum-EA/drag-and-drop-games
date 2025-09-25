using UnityEngine;

public class CarSpawnerScript : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject[] carHolders;
    public ScreenBoundriesScript screenBoundries;
    Vector3 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject car in cars)
        {
            Debug.Assert(car != null);
            Debug.Log(screenBoundries.maxX);
            Vector3 spawnPosition = new Vector3(Random.Range(screenBoundries.minX, screenBoundries.maxX), Random.Range(screenBoundries.minY, screenBoundries.maxY), carHolders[0].GetComponent<Transform>().position.z);
            Instantiate(car, spawnPosition, Random.rotation);
        }
    }
}


   
