using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MysticRelic
{
    public string name;          // Nome da Relíquia
    public string description;   // Descrição da Relíquia
    public int price;            // Preço em Ecos de Memória
    public Rarity rarity;        // Tier de Raridade
    public Sprite icon;          // Ícone que será exibido na loja
    public List<ActivationMoment> activationMoments; // Momentos em que a relíquia pode ser ativada
    public System.Action<GameManager> effect; // Ação que define o efeito da relíquia

    public static List<MysticRelic> RegisterRelics()
    {
        return new List<MysticRelic>() {
            new MysticRelic
            {
                name = "Chave do Esquecimento",
                description = "Concede +1 tentativa por rodada.",
                price = 50,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Chave_Comum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.attemptsRemaining += 1; }
            },
            new MysticRelic
            {
                name = "Ampulheta de Areia",
                description = "Ganha 10% do dinheiro gasto de volta.",
                price = 50,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Ampulheta_Comum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart, ActivationMoment.OnRelicPurchase },
                effect = (gameManager) => { gameManager.cashback += 0.1f; }
            },
            new MysticRelic
            {
                name = "Cristal do Conhecimento",
                description = "Reduz o custo das Relíquias na loja em 10%.",
                icon = Resources.Load<Sprite>("Relics/Cristal_Comum"),
                price = 50,
                rarity = Rarity.Common,
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnShopOpen },
                effect = (gameManager) => { gameManager.discount += 0.1f; }
            },
            new MysticRelic
            {
                name = "Pluma da Sabedoria",
                description = "Concede 50 ecos de memória por rodada.",
                price = 70,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Pluma_Comum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.memoryEchoes += 50; }
            },
            new MysticRelic
            {
                name = "Lanterna do Caminho",
                description = "Adiciona 10 ecos de memória ao completar um par.",
                price = 70,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Lanterna_Comum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnPairMatch },
                effect = (gameManager) => { gameManager.memoryEchoes += 10; }
            },
            new MysticRelic
            {
                name = "Fragmento do Labirinto",
                description = "Concede -1 par por rodada.",
                price = 50,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Fragmento_Incomum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.pairs--; }
            },
            new MysticRelic
            {
                name = "Fragmento do Labirinto",
                description = "Adiciona 10 ecos de memória para cada tentativa restante ao ganhar.",
                price = 50,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Fragmento_Incomum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundEnd },
                effect = (gameManager) => { gameManager.memoryEchoes += (gameManager.attemptsRemaining * 10); }
            },
            new MysticRelic
            {
                name = "Fragmento do Labirinto",
                description = "Adiciona 0.5s de observação no inicio da rodada.",
                price = 50,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Fragmento_Incomum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.totalPeekingTime += 0.5f; }
            },
            new MysticRelic
            {
                name = "Fragmento do Labirinto",
                description = "Ganha uma relíquia extra na loja.",
                price = 50,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Fragmento_Incomum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.itemsOnShop += 1; }
            },
            new MysticRelic
            {
                name = "Fragmento do Labirinto",
                description = "Reduz em 5% a chance do chefe embaralhar ao errar.",
                price = 50,
                rarity = Rarity.Common,
                icon = Resources.Load<Sprite>("Relics/Fragmento_Incomum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.bossShuffleChance -= 0.05f; }
            },
            new MysticRelic
            {
                name = "Chave do Esquecimento 3",
                description = "Concede +3 tentativa por rodada.",
                price = 100,
                rarity = Rarity.Uncommon,
                icon = Resources.Load<Sprite>("Relics/Chave_Comum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.attemptsRemaining += 3; }
            },
            new MysticRelic
            {
                name = "Ampulheta do Cashback",
                description = "Ganha 20% do dinheiro gasto de volta.",
                price = 100,
                rarity = Rarity.Uncommon,
                icon = Resources.Load<Sprite>("Relics/Ampulheta_Comum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart, ActivationMoment.OnRelicPurchase },
                effect = (gameManager) => { gameManager.cashback += 0.2f; }
            },
            new MysticRelic
            {
                name = "Cristal do Conhecimento Incomum",
                description = "Reduz o custo das Relíquias na loja em 20%.",
                icon = Resources.Load<Sprite>("Relics/Cristal_Comum"),
                price = 100,
                rarity = Rarity.Uncommon,
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnShopOpen },
                effect = (gameManager) => { gameManager.discount += 0.2f; }
            },
            new MysticRelic
            {
                name = "Lanterna do Caminho Raro",
                description = "Adiciona 30 ecos de memória ao completar um par.",
                price = 200,
                rarity = Rarity.Rare,
                icon = Resources.Load<Sprite>("Relics/Lanterna_Comum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnPairMatch },
                effect = (gameManager) => { gameManager.memoryEchoes += 30; }
            },
            new MysticRelic
            {
                name = "Fragmento da Visão",
                description = "Adiciona 1.5s de observação no inicio da rodada.",
                price = 200,
                rarity = Rarity.Rare,
                icon = Resources.Load<Sprite>("Relics/Fragmento_Incomum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.totalPeekingTime += 1.5f; }
            },
            new MysticRelic
            {
                name = "Fragmento dos Perdidos",
                description = "Concede -3 par por rodada.",
                price = 1,
                rarity = Rarity.Rare,
                icon = Resources.Load<Sprite>("Relics/Fragmento_Incomum"),
                activationMoments = new List<ActivationMoment> { ActivationMoment.OnRoundStart },
                effect = (gameManager) => { gameManager.pairs -= 3; }
            },
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

