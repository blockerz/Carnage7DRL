
using System.Collections.Generic;
using UnityEngine;

namespace lofi.RLCore
{
    public class ItemFactory : MonoBehaviour
    {
        [SerializeField]
        public GameState gameState;

        [SerializeField]
        public Item itemPrefab;

        Region Level;

        public Dictionary<ItemType, ItemData> itemDatabase;


        public void Initialize()
        {
            Level = gameState.Level;

            ItemData[] allItemData;
            Resources.LoadAll<ItemData>("ScriptableObjects/Items");
            allItemData = Resources.FindObjectsOfTypeAll<ItemData>();

            itemDatabase = new Dictionary<ItemType, ItemData>();

            foreach (var data in allItemData)
            {
                itemDatabase.Add(data.Type, data);
            }

        }

        public Item SpawnItemOnLevel(ItemType itemType, int amount, int x, int y)
        {
            Item item = Instantiate(itemPrefab).GetComponent<Item>();
            item.itemData = itemDatabase[itemType];
            item.amount = amount;
            item.Initialize();
            item.transform.position = Level.GetAreaWorldPosition(x, y) + new Vector3(0,0,-1);
            Level.AddItemAtArea(item, x, y);
            return item;
        }
    }
}