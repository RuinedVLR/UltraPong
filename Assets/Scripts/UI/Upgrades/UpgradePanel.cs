using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private UpgradeSelector selector;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private int upgradesCount = 3;

    private readonly List<UpgradeCardUI> spawnedCards = new();

    public void Show()
    {
        Clear();

        var upgrades = selector.GetRandomUpgrades(upgradesCount);

        foreach (var upgrade in upgrades)
        {
            var cardObj = Instantiate(cardPrefab, cardsContainer);
            var card = cardObj.GetComponent<UpgradeCardUI>();
            card.Setup(upgrade, HandleUpgradeSelected);
            spawnedCards.Add(card);
        }

        gameObject.SetActive(true);
    }

    private void HandleUpgradeSelected()
    {
        Hide();
        GameManager.Instance.OnUpgradeChosenContinue();
    }

    public void Hide()
    {
        Clear();
        gameObject.SetActive(false);
    }

    private void Clear()
    {
        foreach (var card in spawnedCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        spawnedCards.Clear();
    }
}