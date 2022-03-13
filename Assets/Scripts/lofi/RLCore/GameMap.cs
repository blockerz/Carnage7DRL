using Cinemachine;
using lofi.RLCore;
using System;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public event EventHandler<OnRegionChangedEventArgs> OnRegionChanged;
    public class OnRegionChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    [SerializeField]
    public AreaRuntimeSet areaRuntimeSet;
    

    [SerializeField]
    public MapMesh mapMeshPrefab;

    [SerializeField]
    public CinemachineConfiner2D gameCamera;

    [SerializeField]
    public PolygonCollider2D cameraConfiner;

    public Region Level { get; private set; }

    private MapMesh mapMesh;
    private float uiPadding;
    //private Pathfinding pathfinding;

    private TextMesh[,] debugText;

    public void CreateMapMesh(GameState gameState)
    {
        Level = gameState.Level;

        if (mapMesh != null)
        {
            GameObject.Destroy(mapMesh);
        }

        mapMesh = Instantiate(mapMeshPrefab).GetComponent<MapMesh>();
        mapMesh.Initialize(Level);
        mapMesh.CreateMapMeshForRegion();
        uiPadding = 4 * Level.AreaWorldSizeX;

        Vector2[] cameraColliderPath = new Vector2[] { 
            new Vector2(Level.Origin.x - uiPadding, Level.Origin.y),
            new Vector2(Level.Origin.x + uiPadding + (Level.AreaWorldSizeX * Level.Width), Level.Origin.y),
            new Vector2(Level.Origin.x + uiPadding + (Level.AreaWorldSizeX * Level.Width), Level.Origin.y + (Level.AreaWorldSizeY * Level.Height)),
            new Vector2(Level.Origin.x - uiPadding, Level.Origin.y + (Level.AreaWorldSizeY * Level.Height))
        };

        cameraConfiner.SetPath(0, cameraColliderPath);
        gameCamera.m_BoundingShape2D = cameraConfiner;
        gameCamera.InvalidateCache();

        DrawDebugView(Level.Width, Level.Height, Level.AreaWorldSizeX, Level.AreaWorldSizeY);

        OnRegionChanged += (object sender, OnRegionChangedEventArgs eventArgs) => {
            if (debugText?[eventArgs.x, eventArgs.y] != null)
                debugText[eventArgs.x, eventArgs.y].text = Level.GetAreaAtXY(eventArgs.x, eventArgs.y)?.ID.ToString();
        };
    }

    void Update()
    {
        
    }

    public void TriggerRegionChanged(int x, int y)
    {
        if (OnRegionChanged != null)
        {
            OnRegionChanged(this, new OnRegionChangedEventArgs { x = x, y = y });
        }
    }

    private void DrawDebugView(int width, int height, float areaWorldSizeX, float areaWorldSizeY)
    {
        if (Debug.isDebugBuild && Constants.DEBUG_ENABLED)
        {
            GameObject emptyParent = new GameObject("Debug Text");
            emptyParent.layer = Constants.MAP_LAYER_ID;

            if (debugText == null)
                debugText = new TextMesh[width, height];

            for (int y = 0; y < Level.Height; y++)
            {
                for (int x = 0; x < Level.Width; x++)
                {
                    if (debugText[x, y] != null)
                    {
                        debugText[x, y].text = "(" + x + "," + y + ")";
                        //debugText[x, y].text = region[x, y].ID.ToString();
                    }
                    else
                    {
                        debugText[x, y] = UnityUtils.CreateWorldText("(" + x + "," + y + ")", null, Level.GetAreaWorldPosition(x, y) + new Vector3(areaWorldSizeX, areaWorldSizeY) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter);
                        debugText[x, y].transform.parent = emptyParent.transform;
                        debugText[x, y].gameObject.layer = Constants.MAP_LAYER_ID;
                    }

                    Debug.DrawLine(Level.GetAreaWorldPosition(x, y), Level.GetAreaWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(Level.GetAreaWorldPosition(x, y), Level.GetAreaWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(Level.GetAreaWorldPosition(0, height), Level.GetAreaWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(Level.GetAreaWorldPosition(width, 0), Level.GetAreaWorldPosition(width, height), Color.white, 100f);
        }
    }

}
