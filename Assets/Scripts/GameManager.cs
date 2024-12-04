using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
    public GridLayoutGroup cardGrid; // Grid onde as cartas serão instanciadas (agora como GridLayoutGroup)
    public List<Sprite> cardImages; // Imagens para os pares de cartas
    public int initialPairs = 5; // Número inicial de pares
    public int maxRelics = 5; // Limite máximo de relíquias na mochila
    public List<MysticRelic> availableRelics; // Lista de relíquias disponíveis para a loja
    public List<MysticRelic> playerRelics = new List<MysticRelic>(); // Relíquias carregadas pelo jogador
    public Transform shopRelicContainer; // Contêiner para as relíquias na loja
    public Transform backpackRelicContainer; // Contêiner para as relíquias na mochila
    public GameObject relicPrefab; // Prefab para exibir as relíquias

    [Header("UI Elements")]
    public Text attemptsText; // Texto para mostrar tentativas restantes
    public Text roundText; // Texto para mostrar a rodada atual
    public Text memoryEchoesText; // Texto para mostrar os pontos mentais
    public GameObject shopPanel; // Painel da loja
    public GameObject debugPanel;
    public InputField inputField; // Input do debug
    public Image fundoAzul; // Adicione uma referência ao fundo azul

    // Variáveis internas
    private List<Card> cards = new List<Card>();
    private Card firstSelectedCard, secondSelectedCard;
    public int attemptsRemaining;
    private int currentRound = 1;
    public int pairs;
    public int memoryEchoes = 0; // Variável para armazenar os pontos mentais
    private float delayTimer = 0f;
    private bool isCheckingMatch = false;
    private bool isShopOpen = false; // Controla se a loja está aberta
    public bool isBossRound = false; // Indica se é a rodada do chefe
    private Coroutine colorTransitionCoroutine;
    public float basePeekingTime = 0.5f;
    public float totalPeekingTime;
    public float discount = 0;
    public float cashback = 0;
    public int itemsOnShop = 2;
    public float bossShuffleChance = 0.25f;

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
        if (isShopOpen) return; // Não inicia nova rodada enquanto a loja estiver aberta

        ClearBoard();
        isBossRound = (currentRound == 6);

        if (currentRound > 1)
        {
            initialPairs++;
        }

        pairs = initialPairs;
        attemptsRemaining = initialPairs * 2;
        totalPeekingTime = basePeekingTime;
        discount = 0;
        cashback = 0;
        bossShuffleChance = 0.25f;
        TriggerEffects(ActivationMoment.OnRoundStart);

        List<int> ids = GenerateCardIDs(pairs);

        foreach (int id in ids)
        {
            GameObject cardObject = Instantiate(cardPrefab, cardGrid.transform);
            Card card = cardObject.GetComponent<Card>();

            int imageIndex = id % cardImages.Count;
            card.SetCard(id, cardImages[imageIndex]);
            cards.Add(card);
        }

        // Ajusta dinamicamente o número de colunas no GridLayoutGroup
        AdjustGridColumns();

        // Mostra as cartas brevemente no início da rodada
        foreach (Card card in cards)
        {
            card.ShowCard(); // Mostra a carta
        }

        // Esconde as cartas após 0.5 segundos
        Invoke("HideAllCards", totalPeekingTime);

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
        // Calcula o número de colunas com base no número total de cartas
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
                memoryEchoes += 70;
                currentRound++;

                if (isBossRound)
                {
                    isBossRound = false; // Reseta a rodada do chefe
                }

                if(currentRound > 6)
                {
                    Debug.Log("Parabens!!");
                    EndGame();
                }
                else
                {
                    OpenShop(); // Abre a loja ao completar a rodada
                }                
            }
        }
        else
        {
            // Pares não coincidem, escondem as cartas novamente
            firstSelectedCard.HideCard();
            secondSelectedCard.HideCard();
            firstSelectedCard = null;
            secondSelectedCard = null;
            attemptsRemaining--;

            if (isBossRound && Random.value < bossShuffleChance)
            {
                ShuffleCards(); // Embaralha as cartas no tabuleiro
            }

            UpdateUI();

            if (attemptsRemaining <= 0)
            {
                EndGame();
            }
        }
    }

    private void ShuffleCards()
    {
        // Pega todas as posições atuais das cartas
        List<Transform> cardTransforms = new List<Transform>();
        foreach (Transform card in cardGrid.transform)
        {
            cardTransforms.Add(card);
        }

        // Embaralha a lista de posições
        cardTransforms.Shuffle();

        // Reorganiza as cartas no Grid
        for (int i = 0; i < cardTransforms.Count; i++)
        {
            cardTransforms[i].SetSiblingIndex(i);
        }

        Debug.Log("As cartas foram embaralhadas!");
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
        Debug.Log("Fim de jogo! Você chegou até a rodada " + currentRound);
        RestartGame(); // Chama o método de reiniciar o jogo automaticamente
    }

    public void UpdateUI()
    {
        attemptsText.text = attemptsRemaining.ToString();
        memoryEchoesText.text = memoryEchoes.ToString(); // Atualiza a exibição dos pontos mentais

        if (isBossRound)
        {
            roundText.text = "BOSS";
            roundText.color = Color.red; // Destaca o texto da rodada

            // Inicia a transição para o azul escuro
            Color targetColor = new Color(20f / 255f, 50f / 255f, 100f / 255f);
            StartColorTransition(targetColor);
        }
        else
        {
            roundText.text = currentRound.ToString() + "/6";
            roundText.color = Color.black; // Cor padrão

            // Inicia a transição para o azul padrão
            Color targetColor = new Color(127f / 255f, 178f / 255f, 255f / 255f);
            StartColorTransition(targetColor);
        }

        DisplayBackpackRelics(); // Atualiza a interface da mochila
    }

    private void StartColorTransition(Color targetColor)
    {
        // Interrompe a transição anterior, se houver
        if (colorTransitionCoroutine != null)
        {
            StopCoroutine(colorTransitionCoroutine);
        }

        // Inicia uma nova transição
        colorTransitionCoroutine = StartCoroutine(TransitionBackgroundColor(fundoAzul.color, targetColor, 1f)); // 1 segundo
    }

    private IEnumerator TransitionBackgroundColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            fundoAzul.color = Color.Lerp(startColor, endColor, elapsedTime / duration); // Interpola entre as cores
            elapsedTime += Time.deltaTime;
            yield return null; // Espera até o próximo frame
        }

        fundoAzul.color = endColor; // Garante que a cor final seja definida
    }

    public void OpenShop()
    {
        itemsOnShop = 2;
        TriggerEffects(ActivationMoment.OnShopOpen);
        PopulateShop(); // Preenche a loja com relíquias
        shopPanel.SetActive(true);
        isShopOpen = true;
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        isShopOpen = false;
        StartNewRound(); // Inicia a próxima rodada ao fechar a loja
    }

    public void PopulateShop()
    {
        // Limpa relíquias anteriores
        foreach (Transform child in shopRelicContainer)
        {
            Destroy(child.gameObject);
        }

        // Obtém os pesos de raridade para a rodada atual
        int[] rarityWeights = GetRarityWeights();

        List<MysticRelic> relicsToDisplay = new List<MysticRelic>();

        // Gera até 5 relíquias
        for (int i = 0; i < itemsOnShop; i++)
        {
            Rarity selectedRarity = SelectRarity(rarityWeights);

            // Filtra relíquias da raridade selecionada
            List<MysticRelic> filteredRelics = availableRelics.FindAll(relic => relic.rarity == selectedRarity);

            if (filteredRelics.Count > 0)
            {
                // Seleciona uma relíquia aleatória da lista filtrada
                MysticRelic chosenRelic = filteredRelics[Random.Range(0, filteredRelics.Count)];

                // Adiciona a relíquia à lista para exibir e remove da lista disponível (para evitar repetição)
                relicsToDisplay.Add(chosenRelic);
            }
        }

        // Adiciona cada relíquia selecionada à loja
        foreach (MysticRelic relic in relicsToDisplay)
        {
            relic.price = (int)(relic.price * (1f - discount));
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

        return Rarity.Common; // Caso não encontre, retorna o padrão
    }

    private int[] GetRarityWeights()
    {
        if (currentRound >= 16)
        {
            return new int[] { 25, 40, 25, 10 }; // Comum, Incomum, Raro, Lendário
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
            memoryEchoes += relic.price / 2; // Retorna metade do valor como "ecos de memória"
            UpdateUI();
            Debug.Log($"Relíquia {relic.name} vendida! +{relic.price / 2} ecos de memória.");
        }
    }

    public void AddRelicToBackpack(MysticRelic relic)
    {
        if (!IsBackpackFull())
        {
            playerRelics.Add(relic);
            cashback = 0;
            TriggerEffects(ActivationMoment.OnRelicPurchase);
            Debug.Log($"Relíquia {relic.name} adicionada à mochila!");
        }
        else
        {
            Debug.Log("Mochila cheia! Venda uma relíquia para liberar espaço.");
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
            relicUI.SetRelic(relic, this, true); // `true` indica que está na mochila
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
        Debug.Log("Ordem das relíquias atualizada dinamicamente!");
    }

    public void RestartGame()
    {
        // Redefine as variáveis de controle
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
            Debug.LogWarning("Número inválido!");
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
            Debug.LogWarning("Número inválido!");
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
            Debug.LogWarning("Número de pares inválido!");
        }
    }

    public void TriggerEffects(ActivationMoment moment)
    {
        foreach (var relic in playerRelics)
        {
            if (relic.activationMoments.Contains(moment))
            {
                relic.effect?.Invoke(this);
                Debug.Log($"Efeito da relíquia {relic.name} ativado no momento {moment}.");
            }
        }
        UpdateUI();
    }

    public float GetCanvasScaleFactor()
    {
        return FindObjectOfType<Canvas>().scaleFactor;
    }
}
