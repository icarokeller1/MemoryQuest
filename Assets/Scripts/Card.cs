using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour
{
    public int cardID;  // ID único para identificar pares de cartas
    public Image cardImage;  // Imagem que será mostrada ao virar a carta (parte da frente)
    public GameObject backSide;  // Referência para a parte de trás da carta
    private Button button;
    public bool isMatched = false;  // Indica se a carta já foi combinada
    private bool isAnimating = false;  // Impede interações durante a animação

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);
        ShowBack(); // Inicializa a carta virada para baixo
    }

    public void SetCard(int id, Sprite image)
    {
        cardID = id;
        cardImage.sprite = image;  // Define a imagem da frente da carta
    }

    public void OnCardClicked()
    {
        if (!isMatched && !isAnimating)  // Apenas permite interação se a carta não foi combinada ou animada
        {
            FindObjectOfType<GameManager>().OnCardClicked(this);
        }
    }

    public void ShowCard()
    {
        if (isAnimating) return;
        StartCoroutine(AnimateCard(true));
    }

    public void HideCard()
    {
        StartCoroutine(AnimateCard(false));
    }

    private IEnumerator AnimateCard(bool showFront)
    {
        isAnimating = true;

        // Primeira metade da rotação (0 a 90 graus)
        float duration = 0.25f; // Duração da metade da rotação
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0, 0, 0),
                Quaternion.Euler(0, 90, 0),
                elapsed / duration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Atualiza o estado da carta no meio da rotação (após atingir 90 graus)
        backSide.SetActive(!showFront);
        cardImage.enabled = showFront;

        // Segunda metade da rotação (90 a 0 graus)
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(0, 90, 0),
                Quaternion.Euler(0, 0, 0),
                elapsed / duration
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Garante que a rotação finalize na posição correta
        transform.rotation = Quaternion.Euler(0, 0, 0);

        isAnimating = false;
    }

    public void ShowBack()
    {
        cardImage.enabled = false;  // Esconde a imagem da frente
        backSide.SetActive(true);  // Mostra a parte de trás
    }
}
