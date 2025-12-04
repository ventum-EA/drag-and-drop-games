using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // REQUIRED FOR UI
using UnityEngine.SceneManagement; // REQUIRED FOR SCENE LOADING

public class HanoiUltimate : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject winPanel;       // Drag your Panel GameObject here
    public Text winMovesText;         // Drag a Text object inside the panel to show score
    public Button retryButton;        // Drag your Retry Button (Image) here
    public Button menuButton;         // Drag your Menu Button (Image) here
    public string menuSceneName = "MainMenu"; // Exact name of your menu scene

    [Header("Visuals")]
    public Material baseMaterial;

    [Header("Settings")]
    public float diskAmount = 4f;
    public float spawnInterval = 0.3f;

    // SCALED UP SETTINGS
    public float poleGap = 350f;
    public float diskHeight = 50f;
    public float baseDiskRadius = 200f;
    public float poleWidth = 40f;
    public float poleHeight = 600f;

    private float floorY = -435f;
    private float skySpawnHeight = 600f;
    private PhysicsMaterial slipperyMat;

    [Header("Interaction")]
    public float snapDistance = 150f;

    // Game State
    private int moves = 0;
    private bool gameWon = false;
    private bool isSpawning = false;

    // Internal
    private List<Stack<GameObject>> poles;
    private List<Vector3> polePositions;
    private List<GameObject> poleVisuals; // Track visual poles to hide them

    // Dragging
    private GameObject currentDisk;
    private int originalPoleIndex;
    private Plane dragPlane;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        poles = new List<Stack<GameObject>>();
        polePositions = new List<Vector3>();
        poleVisuals = new List<GameObject>();
    }

    void Start()
    {
        // Setup UI
        if (winPanel != null) winPanel.SetActive(false);
        if (retryButton != null) retryButton.onClick.AddListener(ReloadScene);
        if (menuButton != null) menuButton.onClick.AddListener(GoToMenu);

        // Physics Setup
        Physics.gravity = new Vector3(0, -4000f, 0);
        Physics.defaultSolverIterations = 50;
        Physics.defaultSolverVelocityIterations = 50;

        CreateSlipperyMaterial();
        SetupCamera();
        CreateEnvironment();
        CreateWallsAndFloor();

        StartCoroutine(SpawnSequence());
    }

    // --- BUTTON FUNCTIONS ---
    void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
    // ------------------------

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
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 540f;
            cam.transform.position = new Vector3(0, 0, -1000);
            cam.transform.rotation = Quaternion.Euler(0, 0, 0);
            cam.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
        }

        if (FindFirstObjectByType<Light>() == null)
        {
            GameObject lightGO = new GameObject("Directional Light");
            Light light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
    }

    void CreateWallsAndFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.position = new Vector3(0, floorY - 20f, 0);
        floor.transform.localScale = new Vector3(3000f, 40f, 3000f);
        ApplyMaterial(floor, new Color(0.2f, 0.2f, 0.2f));
        floor.GetComponent<Collider>().material = slipperyMat;

        CreateWall(new Vector3(-1000f, 0, 0), new Vector3(50f, 2000f, 1000f));
        CreateWall(new Vector3(1000f, 0, 0), new Vector3(50f, 2000f, 1000f));
        CreateWall(new Vector3(0, 1200f, 0), new Vector3(2000f, 50f, 1000f));
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

            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole_" + i;
            pole.transform.position = poleBasePos + Vector3.up * (poleHeight / 2f);
            pole.transform.localScale = new Vector3(poleWidth, poleHeight / 2f, poleWidth);
            ApplyMaterial(pole, new Color(0.7f, 0.7f, 0.7f));
            Destroy(pole.GetComponent<Collider>());

            poles.Add(new Stack<GameObject>());

            // Add to visual list
            poleVisuals.Add(pole);
        }
    }

    IEnumerator SpawnSequence()
    {
        isSpawning = true;
        int count = Mathf.RoundToInt(diskAmount);

        for (int i = count; i > 0; i--)
        {
            SpawnSingleDisk(i, count);
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    void SpawnSingleDisk(int index, int totalCount)
    {
        Vector3 strictSpawnPos = new Vector3(-poleGap, skySpawnHeight, 0);

        GameObject disk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disk.name = "Disk_" + index;
        disk.transform.position = strictSpawnPos;

        float sizeRatio = (float)index / totalCount;
        float radius = poleWidth + 10f + (baseDiskRadius * sizeRatio);
        disk.transform.localScale = new Vector3(radius, diskHeight / 2f, radius);

        ApplyMaterial(disk, Color.HSVToRGB(sizeRatio * 0.15f, 0.9f, 0.9f));

        Destroy(disk.GetComponent<Collider>());
        BoxCollider boxCol = disk.AddComponent<BoxCollider>();
        boxCol.material = slipperyMat;
        boxCol.size = new Vector3(0.95f, 2.0f, 0.95f);

        Rigidbody rb = disk.AddComponent<Rigidbody>();
        rb.mass = 50f + (index * 10f);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        rb.position = strictSpawnPos;

        poles[0].Push(disk);
    }

    void ApplyMaterial(GameObject obj, Color color)
    {
        Renderer rend = obj.GetComponent<Renderer>();

        if (baseMaterial != null)
        {
            rend.material = new Material(baseMaterial);
        }
        else
        {
            Shader shader = Shader.Find("Unlit/Color");
            if (shader == null) shader = Shader.Find("Mobile/Diffuse");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader != null) rend.material = new Material(shader);
        }
        rend.material.color = color;
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
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
#endif

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
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
        rb.position = alignedPos;

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

    public void GrantReward_InstantWin()
    {
        if (gameWon) return;
        Debug.Log("Reward Granted: Instant Win!");
        StopAllCoroutines();
        isSpawning = false;
        if (currentDisk != null)
        {
            currentDisk.GetComponent<Collider>().enabled = true;
            currentDisk = null;
        }

        List<GameObject> allDisks = new List<GameObject>();
        for (int i = 0; i < poles.Count; i++)
        {
            while (poles[i].Count > 0)
            {
                allDisks.Add(poles[i].Pop());
            }
        }
        allDisks.Sort((a, b) => b.transform.localScale.x.CompareTo(a.transform.localScale.x));
        StartCoroutine(StackDisksRoutine(allDisks));
    }

    IEnumerator StackDisksRoutine(List<GameObject> sortedDisks)
    {
        int targetPole = 2;
        foreach (GameObject disk in sortedDisks)
        {
            Rigidbody rb = disk.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            DropDiskToPole(disk, targetPole);
            yield return new WaitForFixedUpdate();
        }
        CheckWinCondition();
    }

    void CheckWinCondition()
    {
        if (!gameWon && (poles[2].Count == diskAmount || poles[1].Count == diskAmount))
        {
            gameWon = true;
            Debug.Log("YOU WIN!");

            if (winPanel != null)
            {
                winPanel.SetActive(true);

                // 1. HIDE THE MIDDLE POLE
                //if (poleVisuals.Count > 1 && poleVisuals[1] != null)
                //{
                //    poleVisuals[1].SetActive(false);
                //}

                //// 2. NEW: HIDE ALL DISKS
                //foreach (Stack<GameObject> stack in poles)
                //{
                //    foreach (GameObject disk in stack)
                //    {
                //        if (disk != null)
                //        {
                //            disk.SetActive(false);
                //        }
                //    }
                //}

                if (winMovesText != null)
                {
                    winMovesText.text = "Moves: " + moves;
                }
            }
        }
    }

    void OnGUI()
    {
        // Only show HUD if game is NOT won
        if (!gameWon)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 40; // Size of the text
            style.normal.textColor = Color.black; // Color of the text

            // --- SETTINGS FOR POSITION ---
            float paddingLeft = 50f; // Distance from the Left border
            float paddingTop = 50f;  // Distance from the Top border
            // -----------------------------

            // Rect(X, Y, Width, Height)
            // X = paddingLeft, Y = paddingTop
            GUI.Label(new Rect(paddingLeft, paddingTop, 400, 100), "Moves: " + moves, style);
        }
    }
}