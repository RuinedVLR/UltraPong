using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    //[SerializeField] private Player player;
    //[SerializeField] private BallManager ballManager;
    //[SerializeField] private Paddle paddle;

    [SerializeField] private List<RaritySettings> raritySettings;

    private UpgradeContext context;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //context = new UpgradeContext
        //{
        //    Player = player,
        //    BallManager = ballManager,
        //    Paddle = paddle
        //};
    }

    public RaritySettings GetRaritySettings(UpgradeRarity rarity)
    {
        foreach (var settings in raritySettings)
        {
            if (settings.Rarity == rarity)
            {
                return settings;
            }
        }

        return null;
    }

    public void AddUpgrade(UpgradeData upgrade)
    {
        foreach (var effect in upgrade.Effects)
        {
            effect.Apply(context);
        }
    }
}
