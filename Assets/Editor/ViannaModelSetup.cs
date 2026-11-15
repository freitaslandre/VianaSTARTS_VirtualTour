#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Viana.EditorTools
{
    /// <summary>
    /// Menu Viana > Preparar modelo selecionado.
    /// Seleciona na Hierarchy a instancia do FBX do edificio e corre o menu: marca tudo como Static e
    /// adiciona Mesh Collider so as pecas onde o jogador pode bater (paredes, pavimentos, lajes, escadas, pilares, terreno...).
    /// Pode ser desfeito com Ctrl+Z.
    /// </summary>
    public static class ViannaModelSetup
    {
        static readonly string[] PrefixosComColisao =
        {
            "Paredes_", "Pavimento_", "Lajes_", "Laje_", "Escada_", "Terraco_", "Pilares", "Terreno",
            "Colunas_Terraco", "Casa_Giratoria_Paredes", "Casa_Giratoria_Plataforma", "Elevador_Estrutura",
            "Vigas_Salao", "Soco_"
        };

        [MenuItem("Viana/Preparar modelo selecionado")]
        static void Preparar()
        {
            GameObject raiz = Selection.activeGameObject;
            if (raiz == null)
            {
                EditorUtility.DisplayDialog("Viana", "Seleciona na Hierarchy o modelo do edificio (a instancia do FBX) e volta a correr o menu.", "OK");
                return;
            }

            Undo.SetCurrentGroupName("Preparar modelo Viana");
            int grupo = Undo.GetCurrentGroup();
            int colisores = 0, estaticos = 0, total = 0;

            foreach (MeshFilter mf in raiz.GetComponentsInChildren<MeshFilter>(true))
            {
                total++;
                GameObject go = mf.gameObject;

                if (!go.isStatic)
                {
                    Undo.RecordObject(go, "Static");
                    go.isStatic = true;
                    estaticos++;
                }

                if (mf.sharedMesh != null && TemColisao(go.name) && go.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mc = Undo.AddComponent<MeshCollider>(go);
                    mc.sharedMesh = mf.sharedMesh;
                    colisores++;
                }
            }

            Undo.CollapseUndoOperations(grupo);
            Debug.Log("[Viana] Modelo preparado: " + total + " malhas, " + estaticos + " marcadas como Static, " + colisores + " Mesh Colliders adicionados.");
        }

        static bool TemColisao(string nome)
        {
            foreach (string p in PrefixosComColisao)
                if (nome.StartsWith(p)) return true;
            return false;
        }
    }
}
#endif
