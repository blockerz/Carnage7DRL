using lofi.RLCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = Constants.MAP_LAYER_ID;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        
        if (time > 1f)
        {
            DestroyExplosion();
        }
    }

    private void DestroyExplosion()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
