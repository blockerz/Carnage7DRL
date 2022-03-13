using lofi.RLCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMesh : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Material material;
    Mesh mapMesh;
    List<Vector3> vertices;
    List<Vector2> uvs;
    List<int> triangles;
    List<Color> colors;

    [SerializeField]
    public Texture2D tileTexture;

    private Region region;

    private readonly float tileWidth = 64;
    private readonly float tileHeight = 64;
    private readonly float tilesheetWidth = 64*8;
    private readonly float tilesheetHeight = 64*1;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Initialize(Region region)
    {
        this.region = region;
        this.gameObject.layer = Constants.MAP_LAYER_ID;        

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = mapMesh = new Mesh();
        mapMesh.name = "Map Mesh";

        material = new Material(Shader.Find("Sprites/Default"));
        material.mainTexture = tileTexture;
        meshRenderer.material = material;

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    public void CreateMapMeshForRegion()
    {
        mapMesh.Clear();
        vertices.Clear();
        uvs.Clear();
        triangles.Clear();
        colors.Clear();

        for (int y = 0; y < region.Height; y++)
        {
            for (int x = 0; x < region.Width; x++)
            {
                //Color color = Color.red;

                //if (region.GetCell(x, y).IsWalkable)
                //{
                //    if (region.GetCell(x, y).IsTransparent)
                //    {
                //        color = Color.green;
                //    }
                //    else
                //    {
                //        color = Color.white;
                //    }
                //}
                AreaData area = region.GetAreaAtXY(x, y).Data;

                CreateMapQuad(region.Origin + new Vector3(x * region.AreaWorldSizeX, y * region.AreaWorldSizeY, 0), 
                                    new Vector3(region.AreaWorldSizeX, region.AreaWorldSizeY, 0), Color.white, area.spriteXIndex, area.spriteYIndex);

            }
        }

        mapMesh.vertices = vertices.ToArray();
        mapMesh.uv = uvs.ToArray();
        mapMesh.triangles = triangles.ToArray();
        mapMesh.colors = colors.ToArray();
        mapMesh.RecalculateNormals();

    }

    private void CreateMapQuad(Vector3 position, Vector3 size, Color color, int uvX, int uvY)
    {
        Vector3 topLeftCorner = position + new Vector3(0, size.y, 0);
        Vector3 width = new Vector3(size.x, 0, 0);
        Vector3 height = new Vector3(0, size.y, 0);


        AddTriangle(topLeftCorner, topLeftCorner + width, topLeftCorner - height);
        AddTriangleColor(color);
        AddTriangle(topLeftCorner + width, topLeftCorner + width - height, topLeftCorner - height);
        AddTriangleColor(color);
        
        AddUVs(uvX, uvY);
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    void AddUVs(int column, int row)
    {
        Vector2[] uv = GetUVs(column, row);

        foreach (var u in uv)
        {
            //Debug.Log("Adding UV: " + u.x + ", " + u.y);
            uvs.Add(u);
        }
    }

    public Vector2[] GetUVs(int column, int rowYup)
    {
        float xLeft = (((float)column) * tileWidth) / tilesheetWidth;
        float xRight = (((float)column + 1) * tileWidth) / tilesheetWidth;
        float yTop = (((float)rowYup + 1) * tileHeight) / tilesheetHeight;
        float yBottom = (((float)rowYup) * tileHeight) / tilesheetHeight;

        float bleedAdjust = 0.001f;
        xLeft += bleedAdjust;
        xRight -= bleedAdjust;
        yTop -= bleedAdjust;
        yBottom += bleedAdjust;

        return new Vector2[]
        {
            new Vector2 (xLeft,yTop),
            new Vector2 (xRight,yTop),
            new Vector2 (xLeft,yBottom),
            new Vector2 (xRight,yTop),
            new Vector2 (xRight,yBottom),
            new Vector2 (xLeft,yBottom)

        };
    }

}
