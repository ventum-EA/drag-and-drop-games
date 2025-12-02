using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HanoiUltimate : MonoBehaviour
{
    [Header("Settings")]
    public float diskAmount = 4f;
    public float spawnInterval = 0.3f;

    // SCALED UP SETTINGS
    public float poleGap = 350f;
    public float diskHeight = 50f;
    public float baseDiskRadius = 200f;
    public float poleWidth = 40f;
    public float poleHeight = 600f;

    // Physics Config
    private float floorY = -400f;
    private float skySpawnHeight = 600f;
    private PhysicsMaterial slipperyMat;

    [Header("Interaction")]
    public float snapDistance = 150f;

    // Game State
    private int moves = 0;
    private bool gameWon = false;
    private bool isSpawning = false;

    // Internal
    private List<Stack<GameObject>> poles; // Logic Stacks
    private List<Vector3> polePositions;   // Pole Base Positions

    // Dragging
    private GameObject currentDisk;
    private int originalPoleIndex;
    private Plane dragPlane;

    void Awake()
    {
        // 1. Initialize Lists in Awake to ensure they are empty before anything else happens
        poles = new List<Stack<GameObject>>();
        polePositions = new List<Vector3>();
    }

    void Start()
    {
        // 2. Physics Engine Overdrive (Prevents sinking)
        Physics.gravity = new Vector3(0, -4000f, 0);
        Physics.defaultSolverIterations = 50;
        Physics.defaultSolverVelocityIterations = 50;

        CreateSlipperyMaterial();
        SetupCamera();
        CreateEnvironment();
        CreateWallsAndFloor();

        StartCoroutine(SpawnSequence());
    }

    void CreateSlipperyMaterial()
    {
        slipperyMat = new PhysicsMaterial("ZeroFriction");
        slipperyMat.dynamicFriction = 0f;
        slipperyMat.staticFriction = 0f;
        slipperyMat.bounciness = 0f;
        slipperyMat.frictionCombine = PhysicsMaterialCombine.Minimum;
        slipperyMat.bounceCombine = PhysicsMaterialCombine.Minimum;
    }

    void SetupCamera()
    {
        Camera cam = Camera.main;
        cam.orthographic = true;
        cam.orthographicSize = 540f;
        cam.transform.position = new Vector3(0, 0, -1000);
        cam.transform.rotation = Quaternion.Euler(0, 0, 0);
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
    }

    void CreateWallsAndFloor()
    {
        // FLOOR
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.position = new Vector3(0, floorY - 20f, 0);
        floor.transform.localScale = new Vector3(3000f, 40f, 3000f);
        floor.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
        floor.GetComponent<Collider>().material = slipperyMat;

        // CAGE WALLS
        CreateWall(new Vector3(-1000f, 0, 0), new Vector3(50f, 2000f, 1000f)); // Left
        CreateWall(new Vector3(1000f, 0, 0), new Vector3(50f, 2000f, 1000f));  // Right
        CreateWall(new Vector3(0, 1200f, 0), new Vector3(2000f, 50f, 1000f));  // Ceiling
    }

    void CreateWall(Vector3 pos, Vector3 size)
    {
        GameObject wall = new GameObject("InvisibleWall");
        wall.transform.position = pos;
        BoxCollider bc = wall.AddComponent<BoxCollider>();
        bc.size = size;
        bc.material = slipperyMat;
    }

    void CreateEnvironment()
    {
        for (int i = 0; i < 3; i++)
        {
            float xPos = (i - 1) * poleGap;
            Vector3 poleBasePos = new Vector3(xPos, floorY, 0);
            polePositions.Add(poleBasePos);

            // Pole Visual
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole_" + i;
            pole.transform.position = poleBasePos + Vector3.up * (poleHeight / 2f);
            pole.transform.localScale = new Vector3(poleWidth, poleHeight / 2f, poleWidth);
            pole.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0.6f);
            Destroy(pole.GetComponent<Collider>()); // No pole colliders

            poles.Add(new Stack<GameObject>());
        }
    }

    IEnumerator SpawnSequence()
    {
        isSpawning = true;
        int count = Mathf.RoundToInt(diskAmount);

        // Loop: Largest (count) -> Smallest (1)
        for (int i = count; i > 0; i--)
        {
            SpawnSingleDisk(i, count);
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    void SpawnSingleDisk(int index, int totalCount)
    {
        // 1. CALCULATE POSITION FIRST
        // Explicitly calculate Pole 0 X coordinate (-poleGap)
        // We do NOT use the list here to avoid any index errors.
        Vector3 strictSpawnPos = new Vector3(-poleGap, skySpawnHeight, 0);

        // 2. CREATE OBJECT
        GameObject disk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disk.name = "Disk_" + index;

        // 3. IMMEDIATE TELEPORT (CRITICAL FIX)
        // Move it to the correct position BEFORE adding Physics components.
        // This prevents the "default spawn at 0,0,0" issue.
        disk.transform.position = strictSpawnPos;

        // 4. Visuals
        float sizeRatio = (float)index / totalCount;
        float radius = poleWidth + 10f + (baseDiskRadius * sizeRatio);
        disk.transform.localScale = new Vector3(radius, diskHeight / 2f, radius);
        disk.GetComponent<Renderer>().material.color = Color.HSVToRGB(sizeRatio * 0.15f, 1f, 1f);

        // 5. Physics Setup
        Destroy(disk.GetComponent<Collider>());
        BoxCollider boxCol = disk.AddComponent<BoxCollider>();
        boxCol.material = slipperyMat;
        boxCol.size = new Vector3(0.95f, 2.0f, 0.95f);

        Rigidbody rb = disk.AddComponent<Rigidbody>();
        rb.mass = 50f + (index * 10f);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        // Double check position in physics engine
        rb.position = strictSpawnPos;

        // 6. Add to Logic
        poles[0].Push(disk);
    }

    void Update()
    {
        if (gameWon || isSpawning) return;
        HandleInput();
        CheckWinCondition();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // FILTER: Ignore walls/floors
                if (!hit.collider.name.Contains("Disk")) return;

                GameObject clickedObject = hit.collider.gameObject;
                int poleIndex = GetPoleIndexOfDisk(clickedObject);

                if (poleIndex != -1)
                {
                    if (poles[poleIndex].Count > 0 && poles[poleIndex].Peek() == clickedObject)
                    {
                        currentDisk = poles[poleIndex].Pop();
                        originalPoleIndex = poleIndex;

                        Rigidbody rb = currentDisk.GetComponent<Rigidbody>();
                        rb.isKinematic = true;

                        dragPlane = new Plane(Vector3.back, currentDisk.transform.position);
                        currentDisk.GetComponent<Collider>().enabled = false;
                    }
                }
            }
        }

        if (Input.GetMouseButton(0) && currentDisk != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (dragPlane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                hitPoint.z = 0;
                currentDisk.transform.position = Vector3.Lerp(currentDisk.transform.position, hitPoint, 30 * Time.deltaTime);
            }
        }

        if (Input.GetMouseButtonUp(0) && currentDisk != null)
        {
            int targetPoleIndex = GetClosestPoleIndex(currentDisk.transform.position);
            bool validMove = false;

            if (targetPoleIndex != -1)
            {
                if (poles[targetPoleIndex].Count == 0) validMove = true;
                else
                {
                    GameObject targetTop = poles[targetPoleIndex].Peek();
                    if (targetTop.transform.localScale.x > currentDisk.transform.localScale.x) validMove = true;
                }
            }

            if (validMove)
            {
                if (targetPoleIndex != originalPoleIndex) moves++;
                DropDiskToPole(currentDisk, targetPoleIndex);
            }
            else
            {
                DropDiskToPole(currentDisk, originalPoleIndex);
            }

            currentDisk.GetComponent<Collider>().enabled = true;
            currentDisk = null;
        }
    }

    void DropDiskToPole(GameObject disk, int index)
    {
        poles[index].Push(disk);

        Rigidbody rb = disk.GetComponent<Rigidbody>();

        Vector3 alignedPos = disk.transform.position;
        alignedPos.x = polePositions[index].x;
        alignedPos.z = 0;

        float stackHeight = floorY + (poles[index].Count * diskHeight);
        if (alignedPos.y < stackHeight) alignedPos.y = stackHeight + 10f;

        disk.transform.position = alignedPos;
        rb.position = alignedPos; // Sync physics immediately

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.WakeUp();
    }

    int GetPoleIndexOfDisk(GameObject disk)
    {
        for (int i = 0; i < poles.Count; i++)
        {
            if (poles[i].Contains(disk)) return i;
        }
        return -1;
    }

    int GetClosestPoleIndex(Vector3 point)
    {
        int closest = -1;
        float minDst = float.MaxValue;
        for (int i = 0; i < polePositions.Count; i++)
        {
            float dst = Mathf.Abs(polePositions[i].x - point.x);
            if (dst < snapDistance && dst < minDst)
            {
                minDst = dst;
                closest = i;
            }
        }
        return closest;
    }

    void CheckWinCondition()
    {
        if (poles[2].Count == diskAmount || poles[1].Count == diskAmount)
        {
            gameWon = true;
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 40;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 400, 100), "Moves: " + moves, style);

        if (gameWon)
        {
            style.fontSize = 80;
            style.normal.textColor = Color.green;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 200), "YOU WIN!", style);
        }
    }
}