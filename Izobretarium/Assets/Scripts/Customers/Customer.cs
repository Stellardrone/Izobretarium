using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Customer  // Убрал MonoBehaviour!
{
    public string name;
    public string type; // "student", "collector", "businessman", "millionaire"
    public int budget;
    public float patience; // в секундах
    public float interestLevel; // 0-1
    public string preferredCategory; // "robot", "tech", "retro", "any"

    // Для торговли
    public int currentOffer;
    public float timeLeft;
    public bool isInterested;

    public static Customer GenerateRandom()
    {
        Customer newCustomer = new Customer();

        string[] types = { "student", "collector", "businessman", "millionaire" };
        string[] categories = { "robot", "tech", "retro", "any" };

        newCustomer.type = types[Random.Range(0, types.Length)];
        newCustomer.preferredCategory = categories[Random.Range(0, categories.Length)];
        newCustomer.interestLevel = Random.Range(0.3f, 1.0f);

        switch (newCustomer.type)
        {
            case "student":
                newCustomer.budget = Random.Range(100, 500);
                newCustomer.patience = 10f;
                break;
            case "collector":
                newCustomer.budget = Random.Range(1000, 5000);
                newCustomer.patience = 30f;
                break;
            case "businessman":
                newCustomer.budget = Random.Range(500, 3000);
                newCustomer.patience = 20f;
                break;
            case "millionaire":
                newCustomer.budget = Random.Range(5000, 20000);
                newCustomer.patience = 15f;
                break;
        }

        newCustomer.name = GenerateName();
        newCustomer.timeLeft = newCustomer.patience;
        newCustomer.isInterested = true;
        newCustomer.currentOffer = 0;

        return newCustomer;
    }

    static string GenerateName()
    {
        string[] first = { "Вася", "Петя", "Маша", "Катя", "Жора", "Иннокентий", "Прохор", "РокетБосс" };
        string[] last = { "Студент", "Коллекционер", "Бизнесмен", "Миллионер", "Чудак", "Инвестор" };
        return $"{first[Random.Range(0, first.Length)]} {last[Random.Range(0, last.Length)]}";
    }

    public bool CanAfford(int price)
    {
        return budget >= price;
    }

    public int MakeOffer(ItemData item, float demandMultiplier, float reputationBonus)
    {
        int realPrice = item.GetFinalPrice(demandMultiplier, reputationBonus);

        float offerMultiplier = 1.0f;

        switch (type)
        {
            case "student": offerMultiplier = 0.5f; break;
            case "collector":
                if (item.rarity == "rare") offerMultiplier = 1.3f;
                else if (item.rarity == "legendary") offerMultiplier = 1.5f;
                else offerMultiplier = 0.9f;
                break;
            case "businessman": offerMultiplier = 0.8f; break;
            case "millionaire": offerMultiplier = 1.2f; break;
        }

        offerMultiplier *= interestLevel;

        if (preferredCategory != "any" && preferredCategory == item.category)
        {
            offerMultiplier *= 1.3f;
        }

        int offer = Mathf.RoundToInt(realPrice * offerMultiplier);
        offer = Mathf.Min(offer, budget);

        currentOffer = offer;
        return offer;
    }

    public void UpdatePatience(float deltaTime)
    {
        if (!isInterested) return;

        timeLeft -= deltaTime;

        if (timeLeft <= 0)
        {
            isInterested = false;
        }
    }

    public string GetDescription()
    {
        string typeName = "";
        switch (type)
        {
            case "student": typeName = "Студент"; break;
            case "collector": typeName = "Коллекционер"; break;
            case "businessman": typeName = "Бизнесмен"; break;
            case "millionaire": typeName = "Миллионер"; break;
        }

        return $"{name}\nТип: {typeName}\nБюджет: {budget}₽\nИнтерес: {Mathf.RoundToInt(interestLevel * 100)}%";
    }
}
