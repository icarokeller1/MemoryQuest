using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour
{
    public int cardID;  // ID �nico para identificar pares de cartas
    public Image cardImage;  // Imagem que ser� mostrada ao virar a carta (parte da frente)
    public GameObject backSide;  // Refer�ncia para a parte de tr�s da carta
    private Button button;
    public bool isMatched = false;  // Indica se a carta j� foi combinada
    private bool isAnimating = false;  // Impede intera��es durante a anima��o

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
        if (!isMatched && !isAnimating)  // Apenas permite intera��o se a carta n�o foi combinada ou animada
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

        // Primeira metade da rota��o (0 a 90 graus)
        float duration = 0.25f; // Dura��o da metade da rota��o
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

        // Atualiza o estado da carta no meio da rota��o (ap�s atingir 90 graus)
        backSide.SetActive(!showFront);
        cardImage.enabled = showFront;

        // Segunda metade da rota��o (90 a 0 graus)
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

        // Garante que a rota��o finalize na posi��o correta
        transform.rotation = Quaternion.Euler(0, 0, 0);

        isAnimating = false;
    }

    public void ShowBack()
    {
        cardImage.enabled = false;  // Esconde a imagem da frente
        backSide.SetActive(true);  // Mostra a parte de tr�s
    }
}
