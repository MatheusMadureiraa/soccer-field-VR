using UnityEngine;
using UnityEngine.InputSystem;

public class ChuteBola : MonoBehaviour
{
    [Header("Configurações de Força")]
    // força do Chute
    public float forcaChute = 18f;

    private Rigidbody rb;
    private MeshRenderer meshRenderer;

    void Start()
    {
        // componentes de física e renderização da própria bola
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        // detecta o clique esquerdo do mouse
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // posição do ponteiro na tela
            Vector2 posicaoMouse = Mouse.current.position.ReadValue();

            // raio óptico da câmera do jogo em direção ao cenário 3D
            Ray raio = Camera.main.ScreenPointToRay(posicaoMouse);
            RaycastHit hit;

            if (Physics.Raycast(raio, out hit))
            {
                // Se o raio colidir c/ o colisor desta bola
                if (hit.collider.gameObject == this.gameObject)
                {
                    ExecutarChute();
                }
            }
        }
    }

    // INTERAÇÃO VIA LASER VIRTUAL (META XR INTERACTION SDK)
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Ray") ||
            other.gameObject.name.Contains("Hand") ||
            other.gameObject.name.Contains("Controller") ||
            other.gameObject.name.Contains("Interactor"))
        {
            ExecutarChute();
        }
    }

    // LOGICA DO IMPACTO
    public void ExecutarChute()
    {
        if (rb != null)
        {
            // desliga o modo Trigger para a bola voltar a ser sólida e quicar no chão
            Collider colisorBola = GetComponent<Collider>();
            if (colisorBola != null)
            {
                colisorBola.isTrigger = false;
            }

            // ativa a gravidade
            rb.useGravity = true;

            // Zera velocidades e rotações anteriores
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // define a direção -> Z para frente, Y levemente para cima
            Vector3 direcaoChute = new Vector3(0f, 0.4f, 1f).normalized;

            // aplica a força física instantânea (Modo Impulso)
            rb.AddForce(direcaoChute * forcaChute, ForceMode.Impulse);

            // altera a cor do material para vermelho ao interagir
            if (meshRenderer != null)
            {
                meshRenderer.material.color = Color.red;
            }

            Debug.Log("Bola ativada com gravidade e chutada");
        }
    }
}