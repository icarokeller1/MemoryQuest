using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public GameObject cardPrefab; // Prefab das cartas
    public GridLayoutGroup cardGrid; // Grid onde as cartas ser�o instanciadas (agora como GridLayoutGroup)
    public List<Sprite> cardImages; // Imagens para os pares de cartas
    public int initialPairs = 5; // N�mero inicial de pares
    public int maxRelics = 5; // Limite m�ximo de rel�quias na mochila
    public List<MysticRelic> availableRelics; // Lista de rel�quias dispon�veis para a loja
    public List<MysticRelic> playerRelics = new List<MysticRelic>(); // Rel�quias carregadas pelo jogador
    public Transform shopRelicContainer; // Cont�iner para as rel�quias na loja
    public Transform backpackRelicContainer; // Cont�iner para as rel�quias na mochila
    public GameObject relicPrefab; // Prefab para exibir as rel�quias

    [Header("UI Elements")]
    public Text attemptsText; // Texto para mostrar tentativas restantes
    public Text roundText; // Texto para mostrar a rodada atual
    public Text memoryEchoesText; // Texto para mostrar os pontos mentais
    public GameObject shopPanel; // Painel da loja (opcional)
    public GameObject debugPanel;
    public InputField inputField;

    // Vari�veis internas
    private List<Card> cards = new List<Card>();
    private Card firstSelectedCard, secondSelectedCard;
    public int attemptsRemaining;
    private int currentRound = 1;
    private int pairs;
    public int memoryEchoes = 0; // Vari�vel para armazenar os pontos mentais
    private float delayTimer = 0f;
    private bool isCheckingMatch = false;
    private bool isShopOpen = false; // Controla se a loja est� aberta

    void Start()
    {
        pairs = initialPairs;
        TriggerEffects(ActivationMoment.OnGameStart);
        availableRelics = MysticRelic.RegisterRelics();
        StartNewRound();
        UpdateUI();
    }

    void Update()
    {
        // Verifica se estamos aguardando o atraso para checar o par
        if (isCheckingMatch)
        {
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0)
            {
                CompleteCheckMatch();
            }
        }
    }

    public void StartNewRound()
    {
        if (isShopOpen) return; // N�o inicia nova rodada enquanto a loja estiver aberta

        ClearBoard();

        if (currentRound > 1)
        {
            pairs++;
        }

        memoryEchoes += 5 * currentRound;
        attemptsRemaining = pairs * 2;

        List<int> ids = GenerateCardIDs(pairs);

        foreach (int id in ids)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardGrid.transform);
            Card card = cardObject.GetComponent<Card>();

            int imageIndex = id % cardImages.Count;
            card.SetCard(id, cardImages[imageIndex]);
            cards.Add(card);
        }

        // Ajusta dinamicamente o n�mero de colunas no GridLayoutGroup
        AdjustGridColumns();

        // Mostra as cartas brevemente no in�cio da rodada
        foreach (Card card in cards)
        {
            card.ShowCard(); // Mostra a carta
        }

        // Esconde as cartas ap�s 0.5 segundos
        Invoke("HideAllCards", 0.5f);
        TriggerEffects(ActivationMoment.OnRoundStart);

        UpdateUI();
    }

    private void HideAllCards()
    {
        foreach (Card card in cards)
        {
            card.HideCard(); // Esconde a carta
        }
    }

    private void AdjustGridColumns()
    {
        // Calcula o n�mero de colunas com base no n�mero total de cartas
        int columns = Mathf.CeilToInt(Mathf.Sqrt(cards.Count));
        cardGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cardGrid.constraintCount = columns;
    }

    private List<int> GenerateCardIDs(int pairs)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < pairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }
        ids.Shuffle(); // Embaralha a lista de IDs
        return ids;
    }

    public void OnCardClicked(Card card)
    {
        TriggerEffects(ActivationMoment.OnCardFlip);
        if (firstSelectedCard == null)
        {
            firstSelectedCard = card;
            card.ShowCard();
        }
        else if (secondSelectedCard == null && card != firstSelectedCard)
        {
            secondSelectedCard = card;
            card.ShowCard();
            StartCheckMatch();
        }
    }

    private void StartCheckMatch()
    {
        isCheckingMatch = true;
        delayTimer = 1.0f; // Define o atraso de 1 segundo para visualizar as cartas antes de verificar
    }

    private void CompleteCheckMatch()
    {
        isCheckingMatch = false;

        if (firstSelectedCard.cardID == secondSelectedCard.cardID)
        {
            TriggerEffects(ActivationMoment.OnPairMatch);
            // Pares coincidem, marcam as cartas como encontradas
            firstSelectedCard.isMatched = true;
            secondSelectedCard.isMatched = true;

            firstSelectedCard = null;
            secondSelectedCard = null;

            // Verifica se todos os pares foram encontrados
            if (AllPairsFound())
            {
                TriggerEffects(ActivationMoment.OnRoundEnd);
                currentRound++;
                OpenShop(); // Abre a loja ao completar a rodada
            }
        }
        else
        {
            // Pares n�o coincidem, escondem as cartas novamente
            firstSelectedCard.HideCard();
            secondSelectedCard.HideCard();
            firstSelectedCard = null;
            secondSelectedCard = null;

            attemptsRemaining--;
            UpdateUI();

            if (attemptsRemaining <= 0)
            {
                EndGame();
            }
        }
    }

    private bool AllPairsFound()
    {
        foreach (Card card in cards)
        {
            if (!card.isMatched)
            {
                return false;
            }
        }
        return true;
    }

    private void ClearBoard()
    {
        foreach (Transform child in cardGrid.transform)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();
    }

    private void EndGame()
    {
        Debug.Log("Fim de jogo! Voc� chegou at� a rodada " + currentRound);
        RestartGame(); // Chama o m�todo de reiniciar o jogo automaticamente
    }

    public void UpdateUI()
    {
        attemptsText.text = "Tentativas: " + attemptsRemaining;
        roundText.text = "Rodada: " + currentRound;
        memoryEchoesText.text = "Ecos de Mem�ria: " + memoryEchoes; // Atualiza a exibi��o dos pontos mentais
        DisplayBackpackRelics(); // Atualiza a interface da mochila
    }

    public void OpenShop()
    {
        PopulateShop(); // Preenche a loja com rel�quias
        TriggerEffects(ActivationMoment.OnShopOpen);
        shopPanel.SetActive(true);
        isShopOpen = true;
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        isShopOpen = false;
        StartNewRound(); // Inicia a pr�xima rodada ao fechar a loja
    }

    public void PopulateShop()
    {
        // Limpa rel�quias anteriores
        foreach (Transform child in shopRelicContainer)
        {
            Destroy(child.gameObject);
        }

        // Obt�m os pesos de raridade para a rodada atual
        int[] rarityWeights = GetRarityWeights();

        List<MysticRelic> relicsToDisplay = new List<MysticRelic>();

        // Gera at� 5 rel�quias
        for (int i = 0; i < 5; i++)
        {
            Rarity selectedRarity = SelectRarity(rarityWeights);

            // Filtra rel�quias da raridade selecionada
            List<MysticRelic> filteredRelics = availableRelics.FindAll(relic => relic.rarity == selectedRarity);

            if (filteredRelics.Count > 0)
            {
                // Seleciona uma rel�quia aleat�ria da lista filtrada
                MysticRelic chosenRelic = filteredRelics[Random.Range(0, filteredRelics.Count)];

                // Adiciona a rel�quia � lista para exibir e remove da lista dispon�vel (para evitar repeti��o)
                relicsToDisplay.Add(chosenRelic);
                availableRelics.Remove(chosenRelic);
            }
        }

        // Adiciona cada rel�quia selecionada � loja
        foreach (MysticRelic relic in relicsToDisplay)
        {
            GameObject relicObject = Instantiate(relicPrefab, shopRelicContainer);
            RelicUI relicUI = relicObject.GetComponent<RelicUI>();

            relicUI.SetRelic(relic, this, false);
        }
    }

    private Rarity SelectRarity(int[] weights)
    {
        int totalWeight = 0;

        // Calcula o peso total
        foreach (int weight in weights)
        {
            totalWeight += weight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return (Rarity)i; // Retorna a raridade correspondente
            }
        }

        return Rarity.Common; // Caso n�o encontre, retorna o padr�o
    }


    private int[] GetRarityWeights()
    {
        if (currentRound >= 16)
        {
            return new int[] { 25, 40, 25, 10 }; // Comum, Incomum, Raro, Lend�rio
        }
        else if (currentRound >= 11)
        {
            return new int[] { 30, 45, 20, 5 };
        }
        else if (currentRound >= 6)
        {
            return new int[] { 50, 40, 10, 0 };
        }
        else
        {
            return new int[] { 75, 20, 5, 0 };
        }
    }

    public bool IsBackpackFull()
    {
        return playerRelics.Count >= maxRelics;
    }

    public void SellRelic(MysticRelic relic)
    {
        if (playerRelics.Contains(relic))
        {
            playerRelics.Remove(relic);
            memoryEchoes += relic.price / 2; // Retorna metade do valor como "ecos de mem�ria"
            UpdateUI();
            Debug.Log($"Rel�quia {relic.name} vendida! +{relic.price / 2} ecos de mem�ria.");
        }
    }

    public void AddRelicToBackpack(MysticRelic relic)
    {
        if (!IsBackpackFull())
        {
            playerRelics.Add(relic);
            TriggerEffects(ActivationMoment.OnRelicPurchase);
            Debug.Log($"Rel�quia {relic.name} adicionada � mochila!");
        }
        else
        {
            Debug.Log("Mochila cheia! Venda uma rel�quia para liberar espa�o.");
        }
    }

    public void DisplayBackpackRelics()
    {
        foreach (Transform child in backpackRelicContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (MysticRelic relic in playerRelics)
        {
            GameObject relicObject = Instantiate(relicPrefab, backpackRelicContainer);
            RelicUI relicUI = relicObject.GetComponent<RelicUI>();
            relicUI.SetRelic(relic, this, true); // `true` indica que est� na mochila
        }
    }

    public void UpdateBackpackRelicsOrder()
    {
        playerRelics.Clear();
        foreach (Transform child in backpackRelicContainer)
        {
            RelicUI relicUI = child.GetComponent<RelicUI>();
            if (relicUI != null)
            {
                playerRelics.Add(relicUI.relic);
            }
        }
        Debug.Log("Ordem das rel�quias atualizada dinamicamente!");
    }

    public void AddRelicEffect(MysticRelic relic)
    {
        Debug.Log($"Rel�quia {relic.name} comprada! Aplicando efeito: {relic.description}");

        // Exemplo de efeitos (voc� pode expandir isso):
        switch (relic.name)
        {
            case "Rel�quia da Fortuna":
                attemptsRemaining += 5; // Adiciona tentativas extras
                break;
            case "Rel�quia da Mem�ria":
                memoryEchoes += 10; // Adiciona ecos de mem�ria
                break;
            case "Rel�quia do Tempo":
                // Adicione efeitos customizados aqui
                break;
        }

        UpdateUI();
    }

    public void RestartGame()
    {
        // Redefine as vari�veis de controle
        currentRound = 1;
        pairs = initialPairs;
        attemptsRemaining = pairs * 2;
        memoryEchoes = 0; // Reinicia os pontos mentais

        // Redefine as cartas selecionadas
        firstSelectedCard = null;
        secondSelectedCard = null;
        isCheckingMatch = false;

        // Limpa o tabuleiro
        ClearBoard();

        // Inicia a primeira rodada novamente
        StartNewRound();
    }

    public void ToggleDebugPanel()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    public void SkipRound()
    {
        currentRound++;
        StartNewRound();
    }

    public void ApplyEchoesInput()
    {
        int newEchoes;
        if (int.TryParse(inputField.text, out newEchoes) && newEchoes > 0)
        {
            memoryEchoes = newEchoes;
        }
        else
        {
            Debug.LogWarning("N�mero inv�lido!");
        }

        UpdateUI();
    }

    public void ApplyAttemptsInput()
    {
        int newAttempts;
        if (int.TryParse(inputField.text, out newAttempts) && newAttempts > 0)
        {
            attemptsRemaining = newAttempts;
        }
        else
        {
            Debug.LogWarning("N�mero inv�lido!");
        }

        UpdateUI();
    }

    public void ApplyPairsInput()
    {
        int newPairs;
        if (int.TryParse(inputField.text, out newPairs) && newPairs > 0)
        {
            initialPairs = newPairs;
            RestartGame();
        }
        else
        {
            Debug.LogWarning("N�mero de pares inv�lido!");
        }
    }

    public void TriggerEffects(ActivationMoment moment)
    {
        foreach (var relic in playerRelics)
        {
            if (relic.activationMoments.Contains(moment))
            {
                relic.effect?.Invoke(this);
                Debug.Log($"Efeito da rel�quia {relic.name} ativado no momento {moment}.");
            }
        }
        UpdateUI();
    }

    public float GetCanvasScaleFactor()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            return canvas.scaleFactor;
        }
        else
        {
            Debug.LogError("Nenhum Canvas foi encontrado na cena!");
            return 1f; // Retorna 1 como padr�o para evitar erros adicionais
        }
    }
}
