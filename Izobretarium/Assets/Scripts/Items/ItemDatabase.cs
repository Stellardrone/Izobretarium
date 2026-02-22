using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<ItemData> allItems = new List<ItemData>();

    // ПО ОДНОМУ СПРАЙТУ НА КАТЕГОРИЮ
    public Sprite robotSprite;      // Один спрайт для всех роботов
    public Sprite techSprite;       // Один спрайт для всех технологий
    public Sprite retroSprite;      // Один спрайт для всех ретро

    private void Start()
    {
        CreateAllItems();
    }

    private void CreateAllItems()
    {
        allItems.Clear();

        // ===== РОБОТЫ (все с одним спрайтом) =====
        allItems.Add(CreateItem("robot_001", "Робот-Уборщик", 300, "robot", "new", "common", robotSprite));
        allItems.Add(CreateItem("robot_002", "Робот-Повар", 500, "robot", "new", "common", robotSprite));
        allItems.Add(CreateItem("robot_003", "Робот-Садовник", 400, "robot", "used", "common", robotSprite));
        allItems.Add(CreateItem("robot_004", "Робот-Собака", 800, "robot", "new", "rare", robotSprite));
        allItems.Add(CreateItem("robot_005", "Робот-Секретарь", 1200, "robot", "new", "rare", robotSprite));

        // ===== ТЕХНОЛОГИИ (все с одним спрайтом) =====
        allItems.Add(CreateItem("tech_001", "Карманный телепорт", 1000, "tech", "new", "rare", techSprite));
        allItems.Add(CreateItem("tech_002", "Голографический проектор", 600, "tech", "used", "common", techSprite));
        allItems.Add(CreateItem("tech_003", "Машина времени (мини)", 5000, "tech", "broken", "legendary", techSprite));
        allItems.Add(CreateItem("tech_004", "Левитационные кроссовки", 800, "tech", "new", "rare", techSprite));

        // ===== РЕТРО (все с одним спрайтом) =====
        allItems.Add(CreateItem("retro_001", "Паровой двигатель", 200, "retro", "used", "common", retroSprite));
        allItems.Add(CreateItem("retro_002", "Механический калькулятор", 150, "retro", "broken", "common", retroSprite));
        allItems.Add(CreateItem("retro_003", "Граммофон", 400, "retro", "used", "rare", retroSprite));
        allItems.Add(CreateItem("retro_004", "Печатная машинка", 250, "retro", "new", "common", retroSprite));
        allItems.Add(CreateItem("retro_005", "Часы с кукушкой", 600, "retro", "used", "rare", retroSprite));

        Debug.Log($"✅ Создано {allItems.Count} изобретений");
        Debug.Log($"   • Роботы: используют спрайт {(robotSprite != null ? robotSprite.name : "null")}");
        Debug.Log($"   • Технологии: используют спрайт {(techSprite != null ? techSprite.name : "null")}");
        Debug.Log($"   • Ретро: используют спрайт {(retroSprite != null ? retroSprite.name : "null")}");
    }

    // Метод создания одного предмета
    private ItemData CreateItem(string id, string name, int price, string cat, string cond, string rare, Sprite icon)
    {
        ItemData item = new ItemData();
        item.id = id;
        item.name = name;
        item.basePrice = price;
        item.category = cat;
        item.condition = cond;
        item.rarity = rare;
        item.icon = icon;

        return item;
    }

    // Метод для получения случайного предмета
    public ItemData GetRandomItem()
    {
        if (allItems.Count == 0)
            return null;

        int randomIndex = Random.Range(0, allItems.Count);
        return allItems[randomIndex];
    }
}
