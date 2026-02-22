using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    public List<Customer> currentCustomers = new List<Customer>();
    public int maxCustomers = 3;

    // UI элементы
    public GameObject[] customerPanels;  // 3 панели
    public Button[] sellButtons;         // 3 кнопки
    public Text[] customerNameTexts;     // Тексты имён
    public Text[] customerTypeTexts;     // Тексты типа
    public Text[] customerBudgetTexts;   // Тексты бюджета
    public Text[] customerTimeTexts;     // Тексты времени
    public Image[] customerPortraits;    // Портреты (3 штуки)

    // ДОБАВЛЕНО - спрайты для разных типов
    public Sprite[] customerTypeSprites;   // 4 спрайта: студент, коллекционер, бизнесмен, миллионер

    public TradeManager tradeManager;
    public InventorySystem inventory;

    void Start()
    {
        GenerateCustomers();
    }

    void Update()
    {
        // Обновляем таймеры
        bool needUpdate = false;

        for (int i = currentCustomers.Count - 1; i >= 0; i--)
        {
            Customer c = currentCustomers[i];
            c.UpdatePatience(Time.deltaTime);

            if (!c.isInterested)
            {
                currentCustomers.RemoveAt(i);
                needUpdate = true;
                Debug.Log($"Покупатель {c.name} устал ждать и ушёл");
            }
        }

        if (needUpdate)
            UpdateCustomerUI();
    }

    public void GenerateCustomers()
    {
        currentCustomers.Clear();

        int count = Random.Range(2, maxCustomers + 1);
        for (int i = 0; i < count; i++)
        {
            currentCustomers.Add(Customer.GenerateRandom());
        }

        Debug.Log($"👥 Пришло покупателей: {count}");
        UpdateCustomerUI();
    }

    public bool CanAfford(Customer customer, ItemData item, float demandBonus)
    {
        int price = item.GetFinalPrice(demandBonus, GameManager.Instance.reputation);
        return customer.budget >= price;
    }

    public int MakeOffer(Customer customer, ItemData item)
    {
        float demand = 1.0f;
        PriceCalculator calculator = FindObjectOfType<PriceCalculator>();
        if (calculator != null)
            demand = calculator.GetDemandMultiplier(item.category);

        int realPrice = item.GetFinalPrice(demand, GameManager.Instance.reputation);

        float offerMultiplier = 1.0f;
        switch (customer.type)
        {
            case "student": offerMultiplier = 0.6f; break;
            case "collector": offerMultiplier = 1.2f; break;
            case "businessman": offerMultiplier = 0.9f; break;
            case "millionaire": offerMultiplier = 1.5f; break;
        }

        int offer = Mathf.RoundToInt(realPrice * offerMultiplier);
        return Mathf.Min(offer, customer.budget);
    }

    public void UpdateCustomerUI()
    {
        for (int i = 0; i < maxCustomers; i++)
        {
            if (i < currentCustomers.Count)
            {
                Customer c = currentCustomers[i];
                GameObject panel = customerPanels[i];
                panel.SetActive(true);

                // Имя
                if (customerNameTexts != null && i < customerNameTexts.Length && customerNameTexts[i] != null)
                    customerNameTexts[i].text = c.name;

                // Тип
                if (customerTypeTexts != null && i < customerTypeTexts.Length && customerTypeTexts[i] != null)
                    customerTypeTexts[i].text = GetRussianType(c.type);

                // Бюджет
                if (customerBudgetTexts != null && i < customerBudgetTexts.Length && customerBudgetTexts[i] != null)
                    customerBudgetTexts[i].text = $"💰 {c.budget}₽";

                // Время
                if (customerTimeTexts != null && i < customerTimeTexts.Length && customerTimeTexts[i] != null)
                {
                    int secondsLeft = Mathf.CeilToInt(c.timeLeft);
                    customerTimeTexts[i].text = $"⏱️ {secondsLeft} сек";

                    if (secondsLeft < 5)
                        customerTimeTexts[i].color = Color.red;
                    else
                        customerTimeTexts[i].color = Color.white;
                }

                // ПОРТРЕТ
                if (customerPortraits != null && i < customerPortraits.Length && customerPortraits[i] != null)
                {
                    Sprite portraitSprite = GetPortraitForType(c.type);

                    if (portraitSprite != null)
                    {
                        customerPortraits[i].sprite = portraitSprite;
                        customerPortraits[i].gameObject.SetActive(true);

                        // Если покупатель уходит - портрет серый
                        customerPortraits[i].color = c.isInterested ? Color.white : Color.gray;
                    }
                }

                // Кнопка
                if (sellButtons != null && i < sellButtons.Length && sellButtons[i] != null)
                {
                    sellButtons[i].gameObject.SetActive(true);
                    sellButtons[i].onClick.RemoveAllListeners();
                    int index = i;
                    sellButtons[i].onClick.AddListener(() => OnSellButtonClick(index));

                    // Активируем только если есть предметы в инвентаре
                    sellButtons[i].interactable = (inventory != null && inventory.items.Count > 0);
                }
            }
            else
            {
                if (customerPanels[i] != null)
                    customerPanels[i].SetActive(false);

                if (sellButtons != null && i < sellButtons.Length && sellButtons[i] != null)
                    sellButtons[i].gameObject.SetActive(false);

                // Скрываем портрет
                if (customerPortraits != null && i < customerPortraits.Length && customerPortraits[i] != null)
                    customerPortraits[i].gameObject.SetActive(false);
            }
        }
    }

    // Метод для получения спрайта по типу
    Sprite GetPortraitForType(string type)
    {
        if (customerTypeSprites == null || customerTypeSprites.Length < 4)
        {
            Debug.LogWarning("⚠️ Спрайты покупателей не назначены в Inspector!");
            return null;
        }

        switch (type)
        {
            case "student": return customerTypeSprites[0];
            case "collector": return customerTypeSprites[1];
            case "businessman": return customerTypeSprites[2];
            case "millionaire": return customerTypeSprites[3];
            default: return null;
        }
    }

    // ИСПРАВЛЕННЫЙ OnSellButtonClick
    void OnSellButtonClick(int index)
    {
        if (index >= currentCustomers.Count) return;

        Customer selectedCustomer = currentCustomers[index];
        Debug.Log($"👉 Выбран покупатель: {selectedCustomer.name}");

        if (tradeManager != null && inventory != null)
        {
            if (inventory.items.Count > 0)
            {
                // Автоматически выбираем первый предмет
                tradeManager.SelectItem(0);
                tradeManager.SelectCustomer(selectedCustomer);
                // Панель откроется сама в TryShowTradePanel
            }
            else
            {
                Debug.Log("❌ Нет предметов в инвентаре!");
            }
        }
    }

    string GetRussianType(string type)
    {
        switch (type)
        {
            case "student": return "Студент";
            case "collector": return "Коллекционер";
            case "businessman": return "Бизнесмен";
            case "millionaire": return "Миллионер";
            default: return type;
        }
    }
}
