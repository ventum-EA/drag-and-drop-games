using UnityEngine;

public class FlyingObjectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DestroyAllFlyingObjects()
    {
        ObstacleControllerScript[] flyingObjects = Object.FindObjectsByType<ObstacleControllerScript>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach(ObstacleControllerScript obj in flyingObjects)
        {
            if (obj == null)
            {
                continue;
            }
            if(obj.CompareTag("CloudBomb"))
                obj.TriggerExplosion();
            else
                obj.StartToDestroy(Color.cyan);
        }
    }
}
