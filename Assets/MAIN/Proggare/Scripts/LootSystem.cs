using System.Collections.Generic;
using UnityEngine;

//Alexander

public class LootSystem : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject Prefab;
        [Range(0, 100)] public float DropChance; //Drop chance 1-100%
    }

    [System.Serializable]
    public class LootTable
    {
        public List<LootItem> LootItems = new List<LootItem>();

        public LootItem GetRandomLoot()
        {
            float randomValue = Random.Range(1, 101);
            float cumulativeProbability = 0f;

            foreach (var item in LootItems)
            {
                cumulativeProbability += item.DropChance;
                if (randomValue <= cumulativeProbability)
                {
                    return item;
                }
            }
            return null; //If no match found
        }
    }

    public LootTable lootTable;
    private bool lootDropped = false;

    public void DropLoot()
    {
        if (lootDropped) return;
        lootDropped = true;

        LootItem loot = lootTable.GetRandomLoot();
        if (loot != null && loot.Prefab != null)
        {
            Instantiate(loot.Prefab, transform.position, Quaternion.identity);
        }
    }
}