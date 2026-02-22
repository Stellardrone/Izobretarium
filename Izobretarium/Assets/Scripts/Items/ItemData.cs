using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData 
{
    public string id;
    public string name;
    public int basePrice;
    public string category; // "robot", "tech", "retro"
    public string condition; // "new", "used", "broken"
    public string rarity; // "common", "rare", "legendary"

    public Sprite icon;

    // Множители цены
    public float ConditionMultiplier()
    {
        switch (condition)
        {
            case "new": return 1.0f;
            case "used": return 0.7f;
            case "broken": return 0.3f;
            default: return 1.0f;
        }
    }

    public float RarityMultiplier()
    {
        switch (rarity)
        {
            case "common": return 1.0f;
            case "rare": return 2.0f;
            case "legendary": return 5.0f;
            default: return 1.0f;
        }
    }

    // Финальная цена с учётом всех множителей
    public int GetFinalPrice(float demandMultiplier = 1.0f, float reputationBonus = 1.0f)
    {
        float price = basePrice;
        price *= ConditionMultiplier();
        price *= RarityMultiplier();
        price *= demandMultiplier;
        price *= reputationBonus;

        return Mathf.RoundToInt(price);
    }

 //public ItemData(string id, string name, int basePrice, string category, string condition, string rarity)
//  {
// this.id = id;
// this.name = name;
 //this.basePrice = basePrice;
 //this.category = category;
 //this.condition = condition;
 //this.rarity = rarity;
//}

}
