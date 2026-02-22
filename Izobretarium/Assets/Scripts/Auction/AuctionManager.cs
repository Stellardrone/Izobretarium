using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AuctionManager : MonoBehaviour
{
    // Ссылки на компоненты
    public ItemDatabase itemDatabase;
    public InventorySystem inventory;

    // Данные аукциона
    public List<ItemData> currentLots = new List<ItemData>();
    public int[] currentBids = new int[3];

    // UI элементы
    public Button[] lotButtons; // Массив из 3 кнопок
    public Image[] lotIcons;     // Массив из 3 иконок
    public Text[] lotNameTexts;  // Массив из 3 текстов названий
    public Text[] lotPriceTexts; // Массив из 3 текстов цен

    void Start()
    {
        // Найти компоненты, если не назначены в Inspector
        if (itemDatabase == null)
            itemDatabase = FindObjectOfType<ItemDatabase>();

        if (inventory == null)
            inventory = FindObjectOfType<InventorySystem>();

        // Сгенерировать первые лоты
        GenerateDailyLots();

        // Назначить слушатели на кнопки
        SetupButtons();
    }

    public void OnLotButtonClick(int lotIndex)
    {
        if (lotIndex >= currentLots.Count) return;

        ItemData lot = currentLots[lotIndex];
        int price = currentBids[lotIndex];

        if (GameManager.Instance.money >= price && inventory.HasSpace())
        {
            GameManager.Instance.money -= price;
            inventory.AddItem(lot);

            // Убираем лот
            currentLots.RemoveAt(lotIndex);

            // Сдвигаем оставшиеся лоты
            for (int i = lotIndex; i < 2; i++)
            {
                currentBids[i] = currentBids[i + 1];
            }

            // Обновить UI
            GameManager.Instance.UpdateUI();
            UpdateAuctionUI();

            //  обновляем UI покупателей, чтобы кнопки стали активными
            CustomerManager cm = FindObjectOfType<CustomerManager>();
            if (cm != null)
            {
                cm.UpdateCustomerUI();
                Debug.Log("🔄 UI покупателей обновлён после покупки");
            }

            Debug.Log($"✅ Куплен {lot.name} за {price}₽");
        }
        else
        {
            Debug.Log("❌ Недостаточно денег или места в инвентаре!");
        }
    }

    void SetupButtons()
    {
        for (int i = 0; i < lotButtons.Length; i++)
        {
            int index = i;
            if (lotButtons[i] != null)
            {
                lotButtons[i].onClick.RemoveAllListeners();
                lotButtons[i].onClick.AddListener(() => OnLotButtonClick(index));
            }
        }
    }

    public void GenerateDailyLots()
    {
        currentLots.Clear();

        // Берём 3 случайных предмета из базы
        for (int i = 0; i < 3; i++)
        {
            if (itemDatabase != null && itemDatabase.allItems.Count > 0)
            {
                ItemData randomItem = itemDatabase.GetRandomItem();
                currentLots.Add(randomItem);

                // Цена со случайной вариацией
                float variation = Random.Range(0.8f, 1.2f);
                currentBids[i] = Mathf.RoundToInt(randomItem.basePrice * variation);
            }
        }

        UpdateAuctionUI();
    }

    void UpdateAuctionUI()
    {
        for (int i = 0; i < lotButtons.Length; i++)
        {
            if (i < currentLots.Count)
            {
                ItemData item = currentLots[i];

                // Название
                if (lotNameTexts != null && i < lotNameTexts.Length && lotNameTexts[i] != null)
                {
                    lotNameTexts[i].text = item.name;
                }

                // Цена
                if (lotPriceTexts != null && i < lotPriceTexts.Length && lotPriceTexts[i] != null)
                {
                    lotPriceTexts[i].text = $"{currentBids[i]}₽";
                }

                // Иконка
                if (lotIcons != null && i < lotIcons.Length && lotIcons[i] != null)
                {
                    if (item.icon != null)
                    {
                        lotIcons[i].sprite = item.icon;
                        lotIcons[i].gameObject.SetActive(true);

                        // Цвет в зависимости от состояния
                        switch (item.condition)
                        {
                            case "new": lotIcons[i].color = Color.white; break;
                            case "used": lotIcons[i].color = new Color(0.7f, 0.7f, 0.7f); break;
                            case "broken": lotIcons[i].color = new Color(0.5f, 0.3f, 0.3f); break;
                        }
                    }
                }

                // Кнопка активна?
                lotButtons[i].interactable =
                    GameManager.Instance.money >= currentBids[i] &&
                    inventory.HasSpace();
            }
            else
            {
                // Пустой лот - скрываем иконку
                if (lotIcons != null && i < lotIcons.Length && lotIcons[i] != null)
                {
                    lotIcons[i].gameObject.SetActive(false);
                }

                if (lotButtons != null && i < lotButtons.Length && lotButtons[i] != null)
                {
                    lotButtons[i].interactable = false;
                }
            }
        }
    }
}