using UnityEngine;

public class CarSpawnerScript : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject[] carHolders;
    public ScreenBoundriesScript screenBoundries;
    public ObjectScript objScr;
    public Canvas canv;
    Vector3 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        foreach (GameObject car in cars)
        {
            Debug.Assert(car != null);
            Debug.Log(screenBoundries.maxX);
            Vector3 spawnPosition = new Vector3(Random.Range(screenBoundries.minX, screenBoundries.maxX), Random.Range(screenBoundries.minY, screenBoundries.maxY), carHolders[0].GetComponent<Transform>().position.z);
     
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


   
