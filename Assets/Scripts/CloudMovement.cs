using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 200.0f; // Velocidade da nuvem
    public float resetPositionX = -500f; // Posição no eixo X onde a nuvem será destruída

    void Start()
    {
        // Configura uma velocidade inicial aleatória entre 70 e 200
        speed = Random.Range(70f, 200f);
    }

    void Update()
    {
        // Move a nuvem para a esquerda
        transform.position = new Vector3(
            transform.position.x - speed * Time.deltaTime,
            transform.position.y,
            transform.position.z
        );

        // Destrói a nuvem quando ela sai da tela
        if (transform.position.x < resetPositionX)
        {
            Destroy(gameObject);
        }
    }
}
