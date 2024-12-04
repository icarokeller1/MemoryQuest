using System.Collections;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab; // Prefab da nuvem
    public float spawnInterval = 15f; // Intervalo entre os spawns
    public float startPositionX = 2300f; // Posi��o inicial no eixo X
    public float randomYMin = -200f; // Posi��o m�nima no eixo Y
    public float randomYMax = 900f; // Posi��o m�xima no eixo Y
    public float minOpacity = 0.1f; // Opacidade m�nima
    public float maxOpacity = 0.3f; // Opacidade m�xima

    void Start()
    {
        StartCoroutine(SpawnClouds());
    }

    IEnumerator SpawnClouds()
    {
        while (true)
        {
            SpawnCloud();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnCloud()
    {
        // Calcula uma posi��o inicial para a nova nuvem
        float randomY = Random.Range(randomYMin, randomYMax);
        Vector3 spawnPosition = new Vector3(startPositionX, randomY, 0);

        // Instancia a nuvem como filha do CloudSpawner
        GameObject newCloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity, transform);

        // Configura a opacidade aleat�ria
        float randomOpacity = Random.Range(minOpacity, maxOpacity);

        // Se for um objeto UI (Image)
        var image = newCloud.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            Color color = image.color;
            color.a = randomOpacity;
            image.color = color;
        }

        // Se for um objeto SpriteRenderer
        var spriteRenderer = newCloud.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = randomOpacity;
            spriteRenderer.color = color;
        }
    }
}
