using UnityEngine;

public class CarSpawnerScript : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject[] carHolders;
    public ScreenBoundriesScript screenBoundries;
    public ObjectScript objScr;
    public GameObject canv;
    Vector3 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        screenBoundries.RecalculateBounds();
        foreach (GameObject car in cars)
        {
            Debug.Assert(car != null);
            Debug.Log(screenBoundries.maxX);
            Vector3 spawnPosition = new Vector3(Random.Range(screenBoundries.worldBounds.min.x*0.9f, screenBoundries.worldBounds.max.x*0.9f), Random.Range(screenBoundries.worldBounds.min.y*0.9f, screenBoundries.worldBounds.max.y*0.9f), carHolders[0].GetComponent<Transform>().position.z);
     
            GameObject newCar = Instantiate(car, spawnPosition, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
            newCar.transform.SetParent(canv.transform);
            newCar.GetComponent<DragAndDropScript>().objectScr = objScr;
            newCar.GetComponent<DragAndDropScript>().screenBou = screenBoundries;
        }
    }
    void Start()
    {
       
    }
}


   
