using UnityEngine;
using UnityEngine.InputSystem;

namespace Viana
{
    /// <summary>
    /// Controlador de primeira pessoa PROVISORIO (ate se importar o Starter Assets - First Person).
    /// Desktop: W A S D para andar, rato para olhar, Esc liberta o cursor, clique volta a prende-lo.
    /// Nao tem joystick virtual para Android (fica para depois).
    /// Requer um CharacterController e uma camara filha (olhos a 1,6 m).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class JogadorSimples : MonoBehaviour
    {
        [Header("Movimento")]
        [Tooltip("Velocidade de caminhada em m/s (especificacao: 1,5)")]
        public float velocidade = 1.5f;
        public float gravidade = -9.81f;

        [Header("Olhar")]
        [Tooltip("Graus por pixel de movimento do rato")]
        public float sensibilidade = 0.1f;
        public float limiteVertical = 85f;

        [Tooltip("Camara do jogador (filha deste objeto)")]
        public Transform camara;

        CharacterController controlador;
        float inclinacao;      // rotacao vertical da camara
        float velocidadeQueda; // velocidade vertical acumulada

        void Awake()
        {
            controlador = GetComponent<CharacterController>();
            if (camara == null && Camera.main != null)
                camara = Camera.main.transform;
        }

        void OnEnable()
        {
            PrenderCursor(true);
        }

        void OnDisable()
        {
            PrenderCursor(false);
        }

        void Update()
        {
            Keyboard teclado = Keyboard.current;
            Mouse rato = Mouse.current;
            if (teclado == null) return;

            // Esc liberta o cursor; clique do rato volta a prende-lo
            if (teclado.escapeKey.wasPressedThisFrame) PrenderCursor(false);
            if (rato != null && rato.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
                PrenderCursor(true);

            bool cursorPreso = Cursor.lockState == CursorLockMode.Locked;

            // --- Olhar ---
            if (cursorPreso && rato != null)
            {
                Vector2 delta = rato.delta.ReadValue() * sensibilidade;
                transform.Rotate(0f, delta.x, 0f);
                inclinacao = Mathf.Clamp(inclinacao - delta.y, -limiteVertical, limiteVertical);
                if (camara != null)
                    camara.localRotation = Quaternion.Euler(inclinacao, 0f, 0f);
            }

            // --- Andar ---
            float frente = (teclado.wKey.isPressed ? 1f : 0f) - (teclado.sKey.isPressed ? 1f : 0f);
            float lado = (teclado.dKey.isPressed ? 1f : 0f) - (teclado.aKey.isPressed ? 1f : 0f);
            Vector3 direcao = transform.forward * frente + transform.right * lado;
            if (direcao.sqrMagnitude > 1f) direcao.Normalize();

            // Gravidade simples
            if (controlador.isGrounded && velocidadeQueda < 0f) velocidadeQueda = -2f;
            velocidadeQueda += gravidade * Time.deltaTime;

            Vector3 movimento = direcao * velocidade + Vector3.up * velocidadeQueda;
            controlador.Move(movimento * Time.deltaTime);
        }

        static void PrenderCursor(bool prender)
        {
            Cursor.lockState = prender ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !prender;
        }
    }
}
