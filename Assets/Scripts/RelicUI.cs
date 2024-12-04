using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text nameText;
    public Text descriptionText;
    public Text priceText;
    public Text rarityText;
    public Image iconImage;
    public Button actionButton;
    public Text buttonText; // Texto do bot�o
    public GameObject tooltip; // Caixa flutuante para exibir os atributos

    public MysticRelic relic;
    private GameManager gameManager;
    private bool isInBackpack = false;
    private Transform originalParent;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private bool isMouseOver = false; // Indica se o mouse est� sobre a rel�quia

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (isMouseOver && tooltip != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            UpdateTooltipPosition(mousePosition);
        }
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
            return;
        }

        if (gameManager.memoryEchoes >= relic.price)
        {
            gameManager.memoryEchoes -= relic.price;
            gameManager.memoryEchoes += (int)(relic.price * gameManager.cashback);
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

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f; // Torna o objeto semitransparente
            canvasGroup.blocksRaycasts = false; // Permite intera��es de drop
        }

        // Mant�m o objeto no mesmo n�vel visual
        transform.SetParent(gameManager.backpackRelicContainer.parent, true);
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!isInBackpack) return;

        // Atualiza a posi��o global diretamente
        rectTransform.position = Input.mousePosition;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInBackpack) return;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1.0f; // Restaura a opacidade
            canvasGroup.blocksRaycasts = true;
        }

        // Retorna para o layout do slot mais pr�ximo
        Transform closestSlot = GetClosestSlot();
        if (closestSlot != null)
        {
            transform.SetParent(closestSlot.parent);
            transform.SetSiblingIndex(closestSlot.GetSiblingIndex());
        }
        else
        {
            transform.SetParent(originalParent); // Volta ao local original
        }

        // Restaura a posi��o global para alinhar com o layout
        rectTransform.anchoredPosition = Vector2.zero;

        // Atualiza a ordem no GameManager
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            isMouseOver = true; // Ativa o rastreamento
            tooltip.SetActive(true);
            UpdateTooltipContent();  
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            isMouseOver = false; // Desativa o rastreamento
            tooltip.SetActive(false);
        }
    }

    private void UpdateTooltipContent()
    {
        // Atualiza o conte�do do tooltip com os atributos da rel�quia
        tooltip.transform.Find("TextRelicName").GetComponent<Text>().text = relic.name;
        tooltip.transform.Find("TextRelicRarity").GetComponent<Text>().text = relic.rarity.ToString();
        tooltip.transform.Find("TextRelicDescription").GetComponent<Text>().text = relic.description;
        tooltip.transform.Find("TextRelicPrice").GetComponent<Text>().text = $"${relic.price}";
    }

    private void UpdateTooltipPosition(Vector3 mousePosition)
    {
        // Ajusta a posi��o do tooltip para seguir o mouse
        tooltip.transform.position = mousePosition + new Vector3(10, -10, 2);
    }
}
