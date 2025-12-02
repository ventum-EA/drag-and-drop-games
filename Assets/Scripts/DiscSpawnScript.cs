using UnityEngine;

public class DiscSpawnScript : MonoBehaviour
{
    [Tooltip("Prefab used for each disc. Prefab should have a SpriteRenderer or MeshRenderer.")]
    public GameObject discPrefab;

    [Tooltip("Number of discs to spawn. (stored as float per request; will be converted to an int)")]
    [Min(1f)]
    public float amount = 3f;

    [Tooltip("Transform used as the peg / base to stack discs on. If null, this GameObject is used.")]
    public Transform spawnPeg;

    [Tooltip("Parent transform for spawned discs. If null, spawned discs will be parented to the spawnPeg (or this).")]
    public Transform discsParent;

    [Tooltip("Largest scale for the bottom disc.")]
    public float maxScale = 1.5f;

    [Tooltip("Smallest scale for the top disc.")]
    public float minScale = 0.6f;

    [Tooltip("Color for the bottom disc.")]
    public Color bottomColor = Color.red;

    [Tooltip("Color for the top disc.")]
    public Color topColor = Color.yellow;

    [Tooltip("If true pick random colors per disc instead of gradient between bottom/top.")]
    public bool randomizeColors = false;

    // Small additional gap between stacked discs (multiplied by prefab height)
    [Range(0f, 0.5f)]
    public float gapFactor = 0.05f;

    void Start()
    {
        SpawnDiscs();
    }

    public void SpawnDiscs()
    {
        if (discPrefab == null)
        {
            Debug.LogError("DiscSpawnScript: discPrefab is null. Assign a prefab before spawning.");
            return;
        }

        int count = Mathf.Max(1, Mathf.FloorToInt(amount));

        Transform baseTransform = spawnPeg != null ? spawnPeg : this.transform;
        Transform parent = discsParent != null ? discsParent : baseTransform;

        // Try to determine prefab height so we can stack properly.
        float prefabHeight = 0.1f;
        var prefabSprite = discPrefab.GetComponentInChildren<SpriteRenderer>();
        if (prefabSprite != null)
        {
            // sprite.bounds is in local sprite space; account for prefab scale
            prefabHeight = prefabSprite.sprite.bounds.size.y * discPrefab.transform.localScale.y;
        }
        else
        {
            var prefabMesh = discPrefab.GetComponentInChildren<MeshRenderer>();
            if (prefabMesh != null)
            {
                prefabHeight = prefabMesh.bounds.size.y * discPrefab.transform.localScale.y;
            }
        }
        // fallback safety
        prefabHeight = Mathf.Max(0.01f, prefabHeight);

        float verticalStep = prefabHeight * (1f + gapFactor);

        // Start placing discs from bottom (i = 0 -> bottom) to top (i = count-1)
        float currentHeight = 0f;
        Vector3 basePos = baseTransform.position;

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0f : (float)i / (count - 1); // 0..1 from bottom to top
            // bottom should be largest => lerp from maxScale (t=0) to minScale (t=1)
            float scale = Mathf.Lerp(maxScale, minScale, t);

            // Compute spawn position: place center of disc at basePos + currentHeight + half height of disc
            Vector3 spawnPos = new Vector3(basePos.x, basePos.y + currentHeight + (verticalStep * 0.5f), basePos.z);

            GameObject disc = Instantiate(discPrefab, spawnPos, Quaternion.identity, parent);
            disc.transform.localScale = new Vector3(scale, scale, scale);

            // Try to color the disc (support SpriteRenderer and MeshRenderer)
            if (randomizeColors)
            {
                Color rand = Random.ColorHSV(0f, 1f, 0.6f, 1f, 0.6f, 1f);
                ApplyColorToDisc(disc, rand);
            }
            else
            {
                // Use gradient from bottomColor (i=0) to topColor (i=count-1)
                Color col = Color.Lerp(bottomColor, topColor, t);
                ApplyColorToDisc(disc, col);
            }

            // Move up for the next disc
            currentHeight += verticalStep * scale;
        }
    }

    private void ApplyColorToDisc(GameObject disc, Color color)
    {
        var sprite = disc.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = color;
            return;
        }

        var mr = disc.GetComponentInChildren<MeshRenderer>();
        if (mr != null)
        {
            // avoid modifying a shared material instance
            Material mat = new Material(mr.sharedMaterial);
            mat.color = color;
            mr.material = mat;
            return;
        }

        // If the prefab uses UI Image (unlikely for world discs), try that too
#if UNITY_UI
        var img = disc.GetComponentInChildren<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.color = color;
            return;
        }
#endif
    }
}
