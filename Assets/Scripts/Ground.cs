using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using lofi.RLCore;

public class Ground : MonoBehaviour
{
    Region region;
    Pathfinding pathfinding;

    // Start is called before the first frame update
    void Start()
    {
        //region = new Region(20, 20, new Vector3(0,0));
        //region = new Region(10, 10, new Vector3(100,100));
        //region = new Region(10, 10, new Vector3(200,200));
        //region = new Region(30, 30, new Vector3(0, 0));
        //pathfinding = new Pathfinding(region);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    var pos = UnityUtils.GetMouseWorldPosition();

        //    Vector2Int areaXY = pathfinding.Region.GetXYFromWorldPosition(pos);

        //    List<PathNode> path = pathfinding.FindPath(0, 0, areaXY.x, areaXY.y);

        //    if (path != null)
        //    {
        //        for(int i = 0; i < path.Count - 1; i++)
        //        {
        //            Debug.DrawLine(new Vector3(path[i].X, path[i].Y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].X, path[i + 1].Y) * 10f + Vector3.one * 5f, Color.green, 5f);
        //        }
        //    }
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    long id = region.GetIDAtWorldPosition(UnityUtils.GetMouseWorldPosition());
        //    Vector2Int coord = region.GetXYFromID(id);
        //    Debug.Log("x: " + coord.x + ", y: " + coord.y);
        //}
    }
}
