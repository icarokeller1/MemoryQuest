using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Text nameText;
    public Text descriptionText;
    public Text priceText;
    public Text rarityText;
    public Image iconImage;
    public Button actionButton;
    public Text buttonText; // Texto do bot�o

    public MysticRelic relic;
    private GameManager gameManager;
    private bool isInBackpack = false;
    private Transform originalParent;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetRelic(MysticRelic relic, GameManager gameManager, bool inBackpack)
    {
        this.relic = relic;
        this.gameManager = gameManager;
        this.isInBackpack = inBackpack;

        // Atualiza as informa��es da rel�quia
        nameText.text = relic.name;
        nameText.color = GetColorByRarity(relic.rarity);
        rarityText.text = relic.rarity.ToString();
        descriptionText.text = relic.description;
        priceText.text = $"${relic.price}";
        iconImage.sprite = relic.icon;

        // Configura o bot�o com base no estado (comprar ou vender)
        if (isInBackpack)
        {
            buttonText.text = "Vender";
            actionButton.onClick.AddListener(SellRelic);
        }
        else
        {
            buttonText.text = "Comprar";
            actionButton.onClick.AddListener(BuyRelic);
        }
    }

    
    public void BuyRelic()
    {
        if (gameManager.IsBackpackFull())
        {
            Debug.Log("Mochila cheia! N�o � poss�vel comprar mais rel�quias.");
            // Aqui voc� pode abrir a interface de venda, se necess�rio
            return;
        }

        if (gameManager.memoryEchoes >= relic.price)
        {
            gameManager.memoryEchoes -= relic.price;
            gameManager.AddRelicToBackpack(relic); // Adiciona � mochila do jogador
            gameManager.UpdateUI(); // Atualiza a interface da loja e do jogo
            Destroy(gameObject); // Remove a rel�quia da loja
        }
        else
        {
            Debug.Log("N�o h� ecos de mem�ria suficientes!");
        }
    }

    public void SellRelic()
    {
        gameManager.SellRelic(relic); // Vende a rel�quia
        Destroy(gameObject); // Remove da mochila
    }

    private Color GetColorByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return Color.gray;
            case Rarity.Uncommon:
                return Color.green;
            case Rarity.Rare:
                return Color.red;
            case Rarity.Legendary:
                return Color.magenta;
            default:
                return Color.white;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInBackpack) return;

        originalParent = transform.parent;
        canvasGroup.alpha = 0.6f; // Torna o objeto semitransparente
        canvasGroup.blocksRaycasts = false; // Permite o drop

        // Remove temporariamente do layout
        transform.SetParent(gameManager.backpackRelicContainer.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInBackpack) return;

        rectTransform.anchoredPosition += eventData.delta / gameManager.GetCanvasScaleFactor();

        // Detectar mudan�as de posi��o
        Transform closestSlot = GetClosestSlot();
        if (closestSlot != null && closestSlot != originalParent)
        {
            AdjustSlotOrder(closestSlot);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInBackpack) return;

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;

        // Volta ao layout
        Transform closestSlot = GetClosestSlot();
        if (closestSlot != null)
        {
            transform.SetParent(closestSlot);
            transform.SetSiblingIndex(closestSlot.GetSiblingIndex());
        }
        else
        {
            transform.SetParent(originalParent);
        }

        rectTransform.anchoredPosition = Vector2.zero;

        // Atualiza a ordem da lista no GameManager
        gameManager.UpdateBackpackRelicsOrder();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // N�o � necess�rio implementar comportamento aqui
    }

    private Transform GetClosestSlot()
    {
        float minDistance = float.MaxValue;
        Transform closestSlot = null;

        foreach (Transform slot in gameManager.backpackRelicContainer)
        {
            float distance = Vector2.Distance(rectTransform.position, slot.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestSlot = slot;
            }
        }

        return closestSlot;
    }

    private void AdjustSlotOrder(Transform newParent)
    {
        transform.SetParent(newParent.parent);
        transform.SetSiblingIndex(newParent.GetSiblingIndex());
        originalParent = newParent;
    }
}
