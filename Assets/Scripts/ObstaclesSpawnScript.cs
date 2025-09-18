using UnityEngine;

public class ObstaclesSpawnScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] cloudsPrefabs;
    public GameObject[] obstaclesPrefabs;
    public Transform spawnPoint;
    public float cloudSpawnInterval = 3f;
    public float obstacleSpawnInterval = 2f;
    public float minY = -540f;
    public float maxY = 540f;
    public float cloudMinSpeed = 1.5f;
    public float cloudMaxSpeed = 150f;

    public float obstacleMinSpeed = 2f;
    public float obstacleMaxSpeed = 200f;
    void Start()
    {
        InvokeRepeating(nameof(SpawnCloud), 0f, cloudSpawnInterval);
        InvokeRepeating(nameof(SpawnObstacle), 0f, obstacleSpawnInterval);
    }
    void SpawnCloud()
    {
        if (cloudsPrefabs.Length == 0)
        {
            return;
        }
        GameObject cloudPrefab = cloudsPrefabs[Random.Range(0, cloudsPrefabs.Length - 1)];
        float y = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, y, spawnPoint.position.z);
        GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity, spawnPoint);
        float movementSpeed = Random.Range(cloudMinSpeed, cloudMaxSpeed);
        ObstacleControllerScript controller = cloud.GetComponent<ObstacleControllerScript>();
        controller.speed = movementSpeed;



    }
    void SpawnObstacle()
    {

        if (obstaclesPrefabs.Length == 0)
        {
            return;
        }
        GameObject obstaclePrefab = obstaclesPrefabs[Random.Range(0, obstaclesPrefabs.Length - 1)];
        float y = Random.Range(minY, maxY);
        Vector3 spawnPosition = new Vector3(-spawnPoint.position.x, y, spawnPoint.position.z);
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, spawnPoint);
        float movementSpeed = Random.Range(cloudMinSpeed, cloudMaxSpeed);
        ObstacleControllerScript controller = obstacle.GetComponent<ObstacleControllerScript>();
        controller.speed = -movementSpeed;



    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
