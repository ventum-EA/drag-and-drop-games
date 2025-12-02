
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarSpawnerScript : MonoBehaviour
{
    public GameObject[] cars;
    public GameObject[] carHolders;
    public ScreenBoundriesScript screenBoundries;
    public ObjectScript objScr;
    public WinningScript winScr;
    public GameObject carHolerHolder;
    public GameObject canv;
    Vector3 spawnPosition;
    public bool[] carYesNo;
    public Vector3[] forbiddenPos;

    private const float POSITION_TOLERANCE = 0.25f;
    private const int MAX_ATTEMPTS = 50;

    private void Awake()
    {
        
            foreach (GameObject car in cars)
            {
                Debug.Assert(car != null);
                Debug.Log(screenBoundries.maxX);
                Vector3 spawnPosition = new Vector3(Random.Range(screenBoundries.worldBounds.min.x, screenBoundries.worldBounds.max.x), Random.Range(screenBoundries.worldBounds.min.y, screenBoundries.worldBounds.max.y),canv.GetComponent<Transform>().position.z);

                GameObject newCar = Instantiate(car, spawnPosition, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
                newCar.transform.SetParent(canv.transform);
                newCar.GetComponent<DragAndDropScript>().objectScr = objScr;
                newCar.GetComponent<DragAndDropScript>().screenBou = screenBoundries;
            }
        
        
            // Keep track of positions we've already used so car holders don't spawn on top of each other
            List<Vector3> usedPositions = new List<Vector3>();

            foreach (GameObject carHolder in carHolders)
                {
                Debug.Assert(carHolder != null);

                // Generate a random position for this car holder within screen boundaries,
                // keep the original Z from the carHolder, and avoid forbidden or already used positions.
                Vector3 baseZ = carHolder.GetComponent<Transform>().position;
                int attempts = 0;
                Vector3 candidate;
                do
                {
                    // Random position across the map (you can adjust to be a local offset if desired)
                    candidate = new Vector3(
                        Random.Range(screenBoundries.minX, screenBoundries.maxX),
                        Random.Range(screenBoundries.minY, screenBoundries.maxY),
                        baseZ.z
                    );
                    attempts++;
                }
                while ((IsForbidden(candidate) || ExistsApproximatelyInList(candidate, usedPositions)) && attempts < MAX_ATTEMPTS);

                // If we exhausted attempts, fall back to the holder's own position
                if (attempts >= MAX_ATTEMPTS)
                {
                    candidate = new Vector3(baseZ.x, baseZ.y, baseZ.z);
                }

                // Remember this position to prevent overlaps with subsequent spawns
                usedPositions.Add(candidate);

                spawnPosition = candidate;

                GameObject newCar = Instantiate(carHolder, spawnPosition, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
            newCar.GetComponent<DropPlaceScript>().objScript = objScr;
            newCar.GetComponent<DropPlaceScript>().winScript = winScr;
            newCar.GetComponent<DropPlaceScript>().carsSpace = carHolerHolder;
            newCar.transform.SetParent(carHolerHolder.transform);
           
            }
        }
    

    void Start()
    {

    }

    private bool IsForbidden(Vector3 pos)
    {
        if (forbiddenPos == null || forbiddenPos.Length == 0) return false;
        return System.Array.Exists(forbiddenPos, element => ApproximatelyEq(element, pos, POSITION_TOLERANCE));
    }

    private bool ExistsApproximatelyInList(Vector3 pos, List<Vector3> list)
    {
        foreach (var v in list)
        {
            if (ApproximatelyEq(v, pos, POSITION_TOLERANCE)) return true;
        }
        return false;
    }

    private bool ApproximatelyEq(Vector3 a, Vector3 b, float tolerance)
    {
        return Vector3.Distance(a, b) <= tolerance;
    }
}