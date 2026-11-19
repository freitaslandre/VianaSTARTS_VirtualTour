# Diário de Desenvolvimento — Visita Virtual 3D Viana S+T+ARTS Center

> RASCUNHO para revisão da autora. Regista, por etapa, o que foi feito, quem fez
> (autora ou IA) e o que foi verificado. Só contém o que aconteceu; o que não foi
> confirmado está marcado como tal.

## Ferramentas de IA usadas
| Ferramenta | Para quê |
|---|---|
| Claude (sessão na nuvem) | Modelo 3D no Blender, relatórios e primeiros scripts C# (segundo o `CLAUDE.md` do pacote). |
| Claude Code (modelo Claude Sonnet 5.5) com Unity MCP | Ler a cena, importar o modelo, configurar materiais, colocar objetos na cena, escrever e comentar scripts C#, configurar o Git e o Visual Studio. |
| Blender 5.2 (linha de comandos) | Exportar o FBX a partir do `projeto_final.blend`, por instruções dadas pela IA. |

## Legenda
- **Autora**: decisão ou ação da Keila.
- **IA**: trabalho feito pela IA a pedido da autora.
- **Verificado**: visto na Consola do Unity ou em Play mode. **Por verificar**: não testado.

---

## Entrada 1 — 14 de novembro de 2026 — Modelo 3D no Unity

**Objetivo:** pôr o modelo do edifício na cena do Unity e prepará-lo para o jogador andar.

| Passo | Quem | Resultado |
|---|---|---|
| Ler a cena `SampleScene` | IA | Cena vazia do template URP: Main Camera, Directional Light e Global Volume. **Verificado.** |
| Confirmar a Consola | IA | Sem erros de compilação. Mensagens antigas do Unity AI (`NoSubscription`) e do MCP. **Verificado.** |
| Escolher o modelo `projeto_final.blend` | Autora | O FBX antigo da pasta do pacote era mais antigo que o `.blend`. |
| Cópia de segurança do projeto antes de importar | Autora | Feita antes de a IA importar. |
| Inspecionar o `.blend` sem o alterar | IA | 59 malhas, ~24 538 triângulos, 33 materiais, coleções EXT_* e INT_*. **Verificado.** |
| Exportar para FBX (escala 1, sem luz) | IA | `Assets/Models/Viana/Viana_STARTS_Modelo.fbx`. **Verificado.** |
| Importar com *Generate Lightmap UVs* e extrair materiais | IA | 33 materiais em `Assets/Models/Viana/Materials/`; já eram URP/Lit. **Verificado.** |
| `Vidro` em Transparent | IA | Surface = Transparent, alpha 0,35. **Verificado** nas propriedades; **por verificar** visualmente. |
| Corrigir os eixos | IA | O Unity trocou X e Z de sinal (rotação de 180° em Y, sem espelhar). Rodei a instância 180° em Y. Limites finais X −36..49, Z −40..35. **Verificado.** |
| Copiar `ViannaModelSetup.cs` (do pacote) e correr o menu | IA | 59 malhas Static, 27 Mesh Colliders. **Verificado na Consola.** |

**Limitações que ficaram:** vidros, portas, caixilharia, coberturas, tetos, elevador (cabine) e guarda-corpos não têm colisão.

---

## Entrada 2 — 15 de novembro de 2026 — Jogador, Visual Studio e Git

**Objetivo:** pôr um jogador a andar no edifício, usar o Visual Studio e registar no Git.

| Passo | Quem | Resultado |
|---|---|---|
| Starter Assets – First Person não está disponível no PC | IA / Autora | A autora escolheu um controlador simples. |
| Escrever `JogadorSimples.cs` | IA | CharacterController, W A S D, rato, olhos a 1,6 m, 1,5 m/s. Compila sem erros. **Verificado.** |
| Montar o objeto `Jogador` à entrada nascente | IA | Posição (24, 0, 0,3), virado para o edifício. **Verificado.** |
| Testar colisões em Play mode, por código | IA | Chão detetado; uma parede travou o movimento a norte. **Verificado por código.** |
| Andar com teclado e rato e ver a vista da câmara | — | **Por verificar** pela autora. |
| Configurar o Unity para usar o Visual Studio Community 2026 | IA | O script abriu no Visual Studio com a solução do projeto. **Verificado** (captura da autora). |
| Repositório Git local ligado ao GitHub; 3 commits no ramo `feat/modelo-e-jogador` | IA (por interpretação errada do pedido) | A autora esclareceu que faz ela os commits e os pull requests. |
| Comentar `JogadorSimples.cs` e `ViannaModelSetup.cs` linha a linha | IA | Sem mudar o comportamento. Compila. **Verificado.** |

---

## O que a autora deve conseguir explicar (defesa)
1. **Pipeline do modelo:** Blender → FBX → Unity. Porque escala 1, porque os eixos trocam e porque se roda a instância.
2. **Materiais:** o que é URP/Lit e porque o vidro é Transparent.
3. **Colisões:** o que faz um Mesh Collider, porque só algumas malhas o têm e o que fica de fora.
4. **`ViannaModelSetup.cs`:** como a lista de prefixos decide quem tem colisão e como funciona o Undo.
5. **`JogadorSimples.cs`:** rato → rotação do corpo e da câmara; teclado → direção; gravidade; `CharacterController.Move`.

## Pontos em aberto
- Testar a movimentação com teclado e rato e ver o aspeto do modelo.
- Decidir colisão para vidros, portas e guarda-corpos.
- Joystick virtual para Android (ainda não existe).
- Scripts de POI (`POIData`, `POITrigger`, `POIManager`) por copiar e compilar.

## Como acrescentar entradas
Depois de cada etapa: data, objetivo, passos, quem fez cada um, o que foi verificado e o que não foi.
