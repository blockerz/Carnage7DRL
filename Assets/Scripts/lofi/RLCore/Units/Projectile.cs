using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using lofi.RLCore;

public class Projectile : MonoBehaviour
{
    //public SpriteRenderer renderer;

    public float speed;
    public Vector3 direction;
    public Region region;
    public Unit unit;
    private float time;

    void Start()
    {
        //renderer = GetComponent<SpriteRenderer>();
        gameObject.layer = Constants.MAP_LAYER_ID;
        speed = 300f;
        time = 0;
    }

    void Update()
    {

        time += Time.deltaTime;
        transform.position += direction * speed * Time.deltaTime;

        if (region != null)
        {
            var pos = region.GetXYFromWorldPosition(transform.position);

            var contact = region.GetUnitAtXY(pos.x, pos.y); 
            if (contact != null && contact != unit && !contact.unitData.bulletproof)
            {
                DestroyProjectile();
            }
        }

        if (time > 1f)
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
