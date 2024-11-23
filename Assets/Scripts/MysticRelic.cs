using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MysticRelic
{
    public string name;          // Nome da Rel�quia
    public string description;   // Descri��o da Rel�quia
    public int price;            // Pre�o em Ecos de Mem�ria
    public Rarity rarity;        // Tier de Raridade
    public Sprite icon;          // �cone que ser� exibido na loja
    public List<ActivationMoment> activationMoments; // Momentos em que a rel�quia pode ser ativada
    public System.Action<GameManager> effect; // A��o que define o efeito da rel�quia

    public static List<MysticRelic> RegisterRelics()
    {
        return new List<MysticRelic>() {
            new MysticRelic
            {
                name = "Fortuna",
                description = "Adiciona tentativas extras no in�cio da rodada.",
                price = 1,
                rarity = Rarity.Common,
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.attemptsRemaining += 3; }
            },
            new MysticRelic
            {
                name = "Mem�ria",
                description = "Adiciona 10 ecos de mem�ria ao completar um par.",
                price = 1,
                rarity = Rarity.Common,
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnPairMatch },
                effect = (gameManager) => { gameManager.memoryEchoes += 10; }
            },
            new MysticRelic
            {
                name = "Infinito",
                description = "Adiciona 20 ecos de mem�ria ao completar um par.",
                price = 1,
                rarity = Rarity.Common,
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnPairMatch },
                effect = (gameManager) => { gameManager.memoryEchoes += 20; }
            },
            new MysticRelic
            {
                name = "Tematica",
                description = "Adiciona 40 ecos de mem�ria ao completar um par.",
                price = 1,
                rarity = Rarity.Common,
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnPairMatch },
                effect = (gameManager) => { gameManager.memoryEchoes += 40; }
            },
            new MysticRelic
            {
                name = "Debug",
                description = "Adiciona 40 ecos de mem�ria ao completar um par.",
                price = 1,
                rarity = Rarity.Common,
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnPairMatch },
                effect = (gameManager) => { gameManager.memoryEchoes += 40; }
            },
            new MysticRelic
            {
                name = "Teste",
                description = "Adiciona 40 ecos de mem�ria ao completar um par.",
                price = 1,
                rarity = Rarity.Uncommon,
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnPairMatch },
                effect = (gameManager) => { gameManager.memoryEchoes += 40; }
            }
        };
    }
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}

public enum ActivationMoment
{
    OnGameStart,
    OnRoundStart,
    OnCardFlip,
    OnPairMatch,
    OnRoundEnd,
    OnShopOpen,
    OnRelicPurchase
}

