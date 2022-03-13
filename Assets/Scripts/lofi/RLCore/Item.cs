using System;
using UnityEngine;

namespace lofi.RLCore
{
    public class Item : MonoBehaviour
    {
        public ItemData itemData;
        public int amount;
        SpriteRenderer spriteRenderer;

        public void Initialize()
        {

            this.gameObject.layer = Constants.MAP_LAYER_ID;

            this.gameObject.name = itemData.Name;

            spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sprite = itemData.sprite;
        }

        public void DestroyItem()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
