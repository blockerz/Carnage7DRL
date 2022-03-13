using lofi.RLCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeParticle : MonoBehaviour
{
    //public Region region;
    //public Unit unit;
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
            DestroyParticle();
        }
    }

    private void DestroyParticle()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
