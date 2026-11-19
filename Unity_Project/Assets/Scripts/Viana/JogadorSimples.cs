// ============================================================================
// JogadorSimples.cs
// Controlador de primeira pessoa provisório para andar pelo edifício.
//
// Como se liga ao Unity:
//   - Este script vai num GameObject chamado "Jogador".
//   - O "Jogador" tem um CharacterController (a "cápsula" que anda e bate nas paredes).
//   - A Main Camera é FILHA do "Jogador", a 1,6 m de altura (altura dos olhos).
//
// Ideia geral (o que acontece em cada frame, no Update):
//   1. Lê o rato  -> roda o corpo para os lados e inclina a câmara para cima/baixo.
//   2. Lê o teclado (W A S D) -> calcula para onde o jogador quer andar.
//   3. Aplica gravidade para o jogador ficar no chão.
//   4. Move o CharacterController, que respeita as colisões do modelo.
// ============================================================================

// "using" importa espaços de nomes: dizem ao C# onde procurar as classes que usamos.
using UnityEngine;              // classes base do Unity: MonoBehaviour, Vector3, Transform, Time, Cursor...
using UnityEngine.InputSystem;  // novo sistema de input do Unity: Keyboard, Mouse (o projeto usa só este)

// Um "namespace" agrupa as nossas classes para não se confundirem com outras do mesmo nome.
namespace Viana
{
    /// <summary>
    /// Controlador de primeira pessoa PROVISÓRIO (até se importar o Starter Assets - First Person).
    /// Desktop: W A S D para andar, rato para olhar, Esc liberta o cursor, clique volta a prendê-lo.
    /// Não tem joystick virtual para Android (fica para depois).
    /// Requer um CharacterController e uma câmara filha (olhos a 1,6 m).
    /// </summary>
    // Este atributo obriga o Unity a garantir que existe um CharacterController no mesmo objeto.
    // Se o adicionares a um objeto sem ele, o Unity cria-o automaticamente.
    [RequireComponent(typeof(CharacterController))]
    // MonoBehaviour é a classe base de qualquer script que se põe num GameObject.
    // Dá-nos acesso aos métodos especiais Awake, OnEnable, Update, etc.
    public class JogadorSimples : MonoBehaviour
    {
        // ---------------- Campos públicos (aparecem no Inspector do Unity) ----------------

        // [Header] cria um título no Inspector; [Tooltip] mostra um texto de ajuda ao passar o rato.
        [Header("Movimento")]
        [Tooltip("Velocidade de caminhada em m/s (especificação: 1,5)")]
        public float velocidade = 7.5f;   // metros por segundo. O "f" indica que o número é float.
        public float gravidade = -9.81f;  // aceleração da gravidade (m/s²), negativa porque puxa para baixo (eixo Y)

        [Header("Olhar")]
        [Tooltip("Graus por pixel de movimento do rato")]
        public float sensibilidade = 0.1f;  // quanto mais alto, mais depressa a câmara roda
        public float limiteVertical = 85f;  // impede de olhar para cima/baixo além de 85° (evita "virar de cabeça para baixo")

        [Tooltip("Câmara do jogador (filha deste objeto)")]
        public Transform camara;  // Transform = posição/rotação/escala de um objeto. Aqui guardamos o da câmara.

        // ---------------- Campos privados (só este script os usa) ----------------

        CharacterController controlador;  // referência ao componente que move o jogador e deteta colisões
        float inclinacao;      // rotação vertical atual da câmara, em graus (positivo = olhar para baixo)
        float velocidadeQueda; // velocidade vertical acumulada pela gravidade (m/s); negativa = a cair

        // ---------------- Métodos do Unity ----------------

        // Awake corre UMA vez, quando o objeto é criado (antes de tudo o resto).
        // Serve para preparar referências.
        void Awake()
        {
            // Vai buscar o CharacterController que está neste mesmo GameObject.
            controlador = GetComponent<CharacterController>();

            // Se ninguém arrastou a câmara no Inspector, usa a câmara principal da cena (tag MainCamera).
            if (camara == null && Camera.main != null)
                camara = Camera.main.transform;
        }

        // OnEnable corre sempre que o script é ativado (inclui o início do Play).
        // Prende o cursor ao centro do ecrã para o rato poder rodar a câmara à vontade.
        void OnEnable()
        {
            PrenderCursor(true);
        }

        // OnDisable corre quando o script é desativado ou o Play termina.
        // Devolve o cursor ao utilizador, para ele poder clicar noutras janelas.
        void OnDisable()
        {
            PrenderCursor(false);
        }

        // Update corre UMA vez por frame (por exemplo 60 vezes por segundo).
        void Update()
        {
            // "current" é o teclado e o rato ligados ao computador. Podem ser null (ex.: num telemóvel não há teclado).
            Keyboard teclado = Keyboard.current;
            Mouse rato = Mouse.current;
            if (teclado == null) return;  // sem teclado não há nada para fazer; sai do Update

            // --- Cursor: Esc liberta, clique volta a prender ---
            // wasPressedThisFrame é verdadeiro só no frame em que a tecla foi carregada (um "clique", não "mantido").
            if (teclado.escapeKey.wasPressedThisFrame) PrenderCursor(false);
            if (rato != null && rato.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
                PrenderCursor(true);

            // Guardamos se o cursor está preso. Só se olha com o rato quando está preso.
            bool cursorPreso = Cursor.lockState == CursorLockMode.Locked;

            // --- Olhar (rato) ---
            if (cursorPreso && rato != null)
            {
                // delta = quanto o rato se moveu neste frame, em pixels (x = direita/esquerda, y = cima/baixo).
                // Multiplicamos pela sensibilidade para converter pixels em graus.
                Vector2 delta = rato.delta.ReadValue() * sensibilidade;

                // Rodar o CORPO do jogador à volta do eixo Y (vertical): vira para a esquerda/direita.
                transform.Rotate(0f, delta.x, 0f);

                // Inclinar só a CÂMARA para cima/baixo. Subtrai-se porque mover o rato para cima
                // deve fazer olhar para cima (rotação negativa em X no Unity).
                // Mathf.Clamp limita o valor entre -limiteVertical e +limiteVertical.
                inclinacao = Mathf.Clamp(inclinacao - delta.y, -limiteVertical, limiteVertical);

                if (camara != null)
                    camara.localRotation = Quaternion.Euler(inclinacao, 0f, 0f);  // rotação local só no eixo X
            }

            // --- Andar (teclado) ---
            // "isPressed" é verdadeiro enquanto a tecla está carregada.
            // (cond ? 1f : 0f) devolve 1 se a tecla está carregada e 0 se não.
            // frente: W dá +1, S dá -1. lado: D dá +1, A dá -1.
            float frente = (teclado.wKey.isPressed ? 1f : 0f) - (teclado.sKey.isPressed ? 1f : 0f);
            float lado = (teclado.dKey.isPressed ? 1f : 0f) - (teclado.aKey.isPressed ? 1f : 0f);

            // transform.forward e transform.right são as direções "para a frente" e "para a direita" do jogador.
            // Somamo-las para obter a direção do movimento no mundo.
            Vector3 direcao = transform.forward * frente + transform.right * lado;

            // Em diagonal (ex.: W + D) o comprimento do vetor seria 1,41. Normalizar põe-no a 1,
            // para andar na diagonal não ser mais rápido do que andar em frente.
            if (direcao.sqrMagnitude > 1f) direcao.Normalize();

            // --- Gravidade simples ---
            // Se está no chão e a "queda" ainda é negativa, fixa-a num valor pequeno (-2).
            // Isto mantém o CharacterController colado ao chão (senão isGrounded oscila).
            if (controlador.isGrounded && velocidadeQueda < 0f) velocidadeQueda = -2f;

            // Aceleração: velocidade += aceleração * tempo. Time.deltaTime é a duração do último frame (em segundos),
            // por isso o resultado é igual em computadores rápidos e lentos.
            velocidadeQueda += gravidade * Time.deltaTime;

            // Junta a velocidade horizontal (andar) com a vertical (gravidade): metros por segundo.
            Vector3 movimento = direcao * velocidade + Vector3.up * velocidadeQueda;

            // Move() desloca o jogador, mas PARA nas paredes e sobe/desce pequenos degraus (stepOffset).
            // Multiplicar por Time.deltaTime converte "metros por segundo" em "metros neste frame".
            controlador.Move(movimento * Time.deltaTime);
        }

        // Método auxiliar: prende ou liberta o cursor.
        // "static" porque não usa nenhum campo deste objeto, só funções globais do Unity.
        static void PrenderCursor(bool prender)
        {
            // Locked = cursor fixo ao centro e invisível para movimento; None = cursor livre.
            Cursor.lockState = prender ? CursorLockMode.Locked : CursorLockMode.None;
            // Esconde o cursor quando está preso, mostra-o quando está livre.
            Cursor.visible = !prender;
        }
    }
}
