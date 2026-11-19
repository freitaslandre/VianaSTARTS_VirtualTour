// ============================================================================
// ViannaModelSetup.cs
// Ferramenta de EDITOR (só existe dentro do Unity, não vai para a build).
// Acrescenta o menu "Viana > Preparar modelo selecionado".
//
// Porquê precisamos disto:
//   O modelo do edifício tem 59 malhas (paredes, pavimentos, vidros, portas...).
//   Para o jogador não atravessar as paredes, cada malha sólida precisa de um "Mesh Collider".
//   E para o Unity poder cozinhar a luz (lightmaps) e otimizar o desenho, as malhas que nunca
//   se mexem têm de estar marcadas como "Static".
//   Fazer isto à mão em 59 objetos seria lento; este script faz tudo com um clique.
// ============================================================================

// O Unity só compila isto no Editor. Sem esta diretiva, a build falharia porque a build não tem UnityEditor.
#if UNITY_EDITOR
using UnityEditor;   // classes do Editor: MenuItem, Selection, Undo, EditorUtility
using UnityEngine;   // classes base: GameObject, MeshFilter, MeshCollider, Debug

namespace Viana.EditorTools
{
    /// <summary>
    /// Menu Viana > Preparar modelo selecionado.
    /// Seleciona na Hierarchy a instância do FBX do edifício e corre o menu: marca tudo como Static e
    /// adiciona Mesh Collider só às peças onde o jogador pode bater (paredes, pavimentos, lajes, escadas, pilares, terreno...).
    /// Pode ser desfeito com Ctrl+Z.
    /// </summary>
    // "static class": não se criam objetos desta classe; só serve para agrupar funções do menu.
    public static class ViannaModelSetup
    {
        // Lista dos prefixos dos nomes das malhas que devem ter colisão.
        // Se o nome de uma malha começar por um destes textos, ganha um Mesh Collider.
        // Ex.: "Paredes_Interiores" começa por "Paredes_" -> tem colisão.
        // Repara no que NÃO está aqui: Vidros, Portas, Caixilharia, Coberturas, Tetos, Guarda_*.
        // Por isso o jogador atravessa esses objetos.
        static readonly string[] PrefixosComColisao =
        {
            "Paredes_", "Pavimento_", "Lajes_", "Laje_", "Escada_", "Terraco_", "Pilares", "Terreno",
            "Colunas_Terraco", "Casa_Giratoria_Paredes", "Casa_Giratoria_Plataforma", "Elevador_Estrutura",
            "Vigas_Salao", "Soco_"
        };

        // [MenuItem] cria a entrada de menu "Viana > Preparar modelo selecionado" no topo do Unity.
        // O método tem de ser static para o menu o poder chamar.
        [MenuItem("Viana/Preparar modelo selecionado")]
        static void Preparar()
        {
            // O objeto que está selecionado na Hierarchy: deve ser a instância do FBX (a raiz do edifício).
            GameObject raiz = Selection.activeGameObject;
            if (raiz == null)
            {
                // Nada selecionado: mostra uma janela a explicar o que fazer e termina.
                EditorUtility.DisplayDialog("Viana", "Seleciona na Hierarchy o modelo do edifício (a instância do FBX) e volta a correr o menu.", "OK");
                return;
            }

            // Agrupa tudo o que se vai fazer numa só ação de "Desfazer" (Ctrl+Z desfaz tudo de uma vez).
            Undo.SetCurrentGroupName("Preparar modelo Viana");
            int grupo = Undo.GetCurrentGroup();

            // Contadores só para escrever o resultado na Consola no fim.
            int colisores = 0, estaticos = 0, total = 0;

            // Percorre todas as malhas do edifício. GetComponentsInChildren procura em todos os filhos;
            // o "true" inclui também os objetos desativados.
            foreach (MeshFilter mf in raiz.GetComponentsInChildren<MeshFilter>(true))
            {
                total++;
                GameObject go = mf.gameObject;  // o objeto que tem esta malha

                // 1) Marcar como Static (se ainda não estiver).
                // Undo.RecordObject guarda o estado anterior para o Ctrl+Z funcionar.
                if (!go.isStatic)
                {
                    Undo.RecordObject(go, "Static");
                    go.isStatic = true;
                    estaticos++;
                }

                // 2) Adicionar colisão, mas só se:
                //    - a malha existe,
                //    - o nome está na lista dos que têm colisão,
                //    - e ainda não tem um MeshCollider (assim correr o menu duas vezes não duplica).
                if (mf.sharedMesh != null && TemColisao(go.name) && go.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mc = Undo.AddComponent<MeshCollider>(go);
                    mc.sharedMesh = mf.sharedMesh;  // a colisão usa a mesma forma da malha visível
                    colisores++;
                }
            }

            // Junta todas as alterações feitas acima numa só entrada de "Desfazer".
            Undo.CollapseUndoOperations(grupo);

            // Escreve o resumo na Consola para podermos confirmar o resultado.
            Debug.Log("[Viana] Modelo preparado: " + total + " malhas, " + estaticos + " marcadas como Static, " + colisores + " Mesh Colliders adicionados.");
        }

        // Devolve verdadeiro se o nome da malha começa por algum dos prefixos da lista acima.
        static bool TemColisao(string nome)
        {
            foreach (string p in PrefixosComColisao)
                if (nome.StartsWith(p)) return true;  // StartsWith: "começa por"
            return false;
        }
    }
}
#endif
