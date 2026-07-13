using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Purely responsible for building the VISUAL grid: creates a World Space
/// canvas and spawns rows x columns UI Images sized to fit the camera's
/// frustum, then hands the resulting cell GameObjects off to a
/// GridController for data tracking. Holds no game state itself.
///
/// [ExecuteAlways] + the custom Editor below let you punch in rows/columns
/// and click "Generate Grid" in the Inspector without entering Play mode.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(GridController))]
public class Grid_Creator : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camera the grid is built relative to. Defaults to Camera.main.")]
    [SerializeField] private Camera targetCamera;

    [Tooltip("Prefab with a RectTransform + Image component.")]
    [SerializeField] private GameObject imagePrefab;

    [Header("Grid Settings")]
    [Min(1)] public int rows = 3;
    [Min(1)] public int columns = 3;

    [Tooltip("Distance from the camera along its forward vector.")]
    [SerializeField] private float distanceFromCamera = 5f;

    [Range(0.1f, 1f)]
    [Tooltip("Fraction of the visible frustum the grid should occupy (< 1 leaves a safety margin so nothing clips at the edges).")]
    [SerializeField] private float fillPercent = 0.8f;

    [Tooltip("Gap between cells, in world units.")]
    [SerializeField] private float cellSpacing = 0.2f;

    [Header("Behaviour")]
    [Tooltip("Automatically (re)build the grid when entering Play mode. Editor-mode builds are triggered manually via the Generate Grid button.")]
    [SerializeField] private bool buildOnStart = true;

    private Canvas worldCanvas;
    private GridController gridController;

    void Awake()
    {
        gridController = GetComponent<GridController>();
    }

    void Start()
    {
        /*if (!Application.isPlaying) return;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (buildOnStart)
            GenerateGrid();*/
    }

    /// <summary>
    /// Builds (or rebuilds) the grid. Safe to call in edit mode or play mode —
    /// also exposed as a right-click "Generate Grid" context menu entry and
    /// as an Inspector button via GridCreatorEditor below.
    /// </summary>
    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        if (gridController == null)
            gridController = GetComponent<GridController>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null || imagePrefab == null)
        {
            Debug.LogWarning("GridCreator: missing targetCamera or imagePrefab.");
            return;
        }

        CreateWorldCanvas();

        // --- 1. Position the canvas in front of the camera ---
        Vector3 centerPos = targetCamera.transform.position +
                             targetCamera.transform.forward * distanceFromCamera;
        worldCanvas.transform.SetPositionAndRotation(centerPos, targetCamera.transform.rotation);

        // --- 2. Work out how big the camera's view is ---
        // Orthographic cameras have no perspective falloff: the visible area
        // is constant regardless of distance, defined by orthographicSize and
        // aspect. distanceFromCamera only controls Z-depth placement.
        float frustumHeight, frustumWidth;
        if (targetCamera.orthographic)
        {
            frustumHeight = 2f * targetCamera.orthographicSize;
            frustumWidth = frustumHeight * targetCamera.aspect;
        }
        else
        {
            frustumHeight = 2f * distanceFromCamera *
                             Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            frustumWidth = frustumHeight * targetCamera.aspect;
        }

        float usableWidth = frustumWidth * fillPercent;
        float usableHeight = frustumHeight * fillPercent;

        // --- 3. Solve cell size so the whole grid fits inside that usable area ---
        float cellWidth = (usableWidth - (columns - 1) * cellSpacing) / columns;
        float cellHeight = (usableHeight - (rows - 1) * cellSpacing) / rows;
        float cellSize = Mathf.Min(cellWidth, cellHeight); // keep cells square

        RectTransform canvasRect = worldCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(frustumWidth, frustumHeight);

        // --- 4. Clear previous cells and reset the data model ---
        ClearExistingCells();
        gridController.Initialize(rows, columns);

        // --- 5. Lay out the grid, centered on (0,0) of the canvas ---
        float totalWidth = columns * cellSize + (columns - 1) * cellSpacing;
        float totalHeight = rows * cellSize + (rows - 1) * cellSpacing;

        float startX = -totalWidth / 2f + cellSize / 2f;
        float startY = totalHeight / 2f - cellSize / 2f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                GameObject cell = InstantiateCell(r, c);

                ICell _grid = cell.GetComponent<ICell>();
                if (_grid == null)
                {
                    _grid = cell.AddComponent<CellGrid>();
                }
                _grid.Initialize(r, c);

                RectTransform rt = cell.GetComponent<RectTransform>();
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                rt.sizeDelta = new Vector2(cellSize, cellSize);

                float xPos = startX + c * (cellSize + cellSpacing);
                float yPos = startY - r * (cellSize + cellSpacing);
                rt.anchoredPosition3D = new Vector3(xPos, yPos, 0f);

                gridController.RegisterCellObject(r, c, cell);
            }
        }
    }

    private GameObject InstantiateCell(int row, int col)
    {
        GameObject cell;
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Keeps the prefab connection alive for edit-mode-created instances.
            cell = (GameObject)PrefabUtility.InstantiatePrefab(imagePrefab, worldCanvas.transform);
        }
        else
#endif
        {
            cell = Instantiate(imagePrefab, worldCanvas.transform);
        }

        cell.name = $"Cell_{row}_{col}";
        return cell;
    }

    /// <summary>Creates the World Space canvas the grid cells live on (idempotent).</summary>
    private void CreateWorldCanvas()
    {
        if (worldCanvas != null) return;

        Transform existing = transform.Find("GridCanvas");
        if (existing != null && existing.TryGetComponent(out worldCanvas))
            return;

        GameObject canvasGO = new("GridCanvas");
        canvasGO.transform.SetParent(transform, false);

        worldCanvas = canvasGO.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.WorldSpace;
        worldCanvas.worldCamera = targetCamera;
        worldCanvas.vertexColorAlwaysGammaSpace = true;
        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = worldCanvas.GetComponent<RectTransform>();
        // Keep scale at 1 so 1 RectTransform unit == 1 world unit.
        canvasRect.localScale = Vector3.one;
    }

    private void ClearExistingCells()
    {
        if (worldCanvas == null) return;

        for (int i = worldCanvas.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = worldCanvas.transform.GetChild(i).gameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(child);
                continue;
            }
#endif
            Destroy(child);
        }
    }

#if UNITY_EDITOR
    // Draws the target frustum area in the Scene view so you can preview
    // placement before hitting Play.
    void OnDrawGizmosSelected()
    {
        if (targetCamera == null) return;

        Vector3 centerPos = targetCamera.transform.position +
                             targetCamera.transform.forward * distanceFromCamera;

        float frustumHeight, frustumWidth;
        if (targetCamera.orthographic)
        {
            frustumHeight = 2f * targetCamera.orthographicSize;
            frustumWidth = frustumHeight * targetCamera.aspect;
        }
        else
        {
            frustumHeight = 2f * distanceFromCamera *
                             Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            frustumWidth = frustumHeight * targetCamera.aspect;
        }

        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(centerPos, targetCamera.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(frustumWidth, frustumHeight, 0f));
    }
#endif
}

#if UNITY_EDITOR
/// <summary>Adds a one-click "Generate Grid" button to the GridCreator Inspector.</summary>
[CustomEditor(typeof(Grid_Creator))]
public class Grid_CreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Grid_Creator creator = (Grid_Creator)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Grid"))
        {
            creator.GenerateGrid();
        }
    }
}
#endif
