using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Graph: estrutura simple para representar nodos e arestas e executar A*
//
// Contrato da clase (resumido):
// - Inputs: nodos engadidos mediante `AddNode(GameObject)` e arestas mediante `AddEdge(from, to)`.
// - Operación principal: `AStar(GameObject startID, GameObject endID)` que devolve true se existe camiño
//   e poboará `pathList` con a secuencia de `Node` desde inicio ata fin.
// - Erros: se os GameObjects de inicio/fin non teñen nodo asociado, AStar devolve false.
//
// Notas de uso:
// - Os nodos son `Node` (arquivo Node.cs) e as arestas son `Edge` (arquivo Edge.cs).
// - A heurística empregada aquí é a distancia euclidiana ao cadrado entre as posicións dos GameObjects
//   (Vector3.SqrMagnitude). Usar o cadrado evita a chamada a Mathf.Sqrt e mellora o rendemento.
//
// Limitacións e recomendacións:
// - Actualmente `open` e `closed` son `List<Node>`. Para grafos máis grandes recoméndase usar
//   unha `PriorityQueue` (heap) para `open` e `HashSet<Node>` para `closed` para reducir custos.
// - Non hai reinicialización explícita dos campos `g/h/f/cameFrom` antes de executar AStar: se se chama
//   repetidamente en tempo de execución, considera limpar ou reinicializar eses campos para todos os nodos.
// - A función auxiliar `lowestF` devolve o índice do nodo con menor `f` en `open`. Pódese simplificar
//   e optimizar substituíndoa por unha cola de prioridade.
//
// Complexidade (resumo):
// - Temporal: depende da implementación; con lista non ordenada para `open` pode chegar a O(V^2) no peor caso;
//   cun heap e HashSet, acada O(E log V) aproximadamente.
// - Espacial: O(V) para gardar `open`/`closed` e estruturas auxiliares.
public class Graph {

    // Lista global de arestas e nodos do grafo
    List<Edge> edges = new List<Edge>();
    List<Node> nodes = new List<Node>();

    // Resultado da última busca: lista de nodos que forman o camiño (de inicio a fin)
    public List<Node> pathList = new List<Node>();

    public Graph() { }

    // Engade un novo nodo ao grafo asociado a un GameObject.
    // Input: GameObject id - o GameObject que representa visualmente o nodo.
    // Efecto: crea un Node e engadeo á lista interna `nodes`.
    public void AddNode(GameObject id) {
        Node node = new Node(id);
        nodes.Add(node);
    }

    // Engade unha aresta entre dous nodos identificados polos seus GameObjects.
    // Se algún dos nodos non existe na lista interna non se fai nada.
    // Nota: non se comprueba duplicidade de arestas - pódense crear arestas paralelas se se chama repetidamente.
    public void AddEdge(GameObject fromNode, GameObject toNode) {

        Node from = FindNode(fromNode);
        Node to = FindNode(toNode);

        if (from != null && to != null) {

            Edge e = new Edge(from, to);
            edges.Add(e);
            from.edgeList.Add(e);
        }
    }

    // Devolve o GameObject correspondente ao nodo na posición `index` dentro de `pathList`.
    // Útil para iterar o camiño calculado dende un seguidor.
    public GameObject getPathPoint(int index) {
        return pathList[index].getID();
    }

    // Busca un Node dado o seu GameObject asociado. Devolve null se non existe.
    private Node FindNode(GameObject id) {
        foreach (Node n in nodes) {
            if (n.getID() == id) return n;
        }
        return null;
    }

    // Implementación do algoritmo A* sobre o grafo
    // Inputs: GameObject startID, GameObject endID
    // Output: bool - verdadeiro se se encontrou un camiño; en caso afirmativo `pathList` conterá os nodos
    // que forman o camiño dende `startID` ata `endID` (orde: inicio -> fin).
    // Erros/Comportamento: devolve false se os nodos de inicio ou fin non existen.
    public bool AStar(GameObject startID, GameObject endID) {

        // Obter referencias aos nodos de inicio e fin
        Node start = FindNode(startID);
        Node end = FindNode(endID);

        if (start == null || end == null) return false;

        // Listas abertas e pechadas clásicas de A*
        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        float tentative_g_score = 0.0f;
        bool tentative_is_better;

        // Inicialización do nodo de inicio: g=0, h = heurística ata fin, f = g + h
        start.g = 0.0f;
        start.h = distance(start, end);
        start.f = start.h;

        open.Add(start);

        // Bucle principal: mentres haxa nodos abertos por explorar
        while (open.Count > 0) {

            int i = lowestF(open);
            Node thisNode = open[i];
            // Se chegamos ao nodo destino, reconstruímos o camiño
            if (thisNode.getID() == endID) {
                ReconstructPath(start, end);
                return true;
            }

            // Mover nodo actual de abierto a pechado
            open.RemoveAt(i);
            closed.Add(thisNode);
            Node neighbour;
            // Para cada aresta saínte, avaliar veciños
            foreach (Edge e in thisNode.edgeList) {

                neighbour = e.endNode;

                // Se xa está en pechados, ignoralo
                if (closed.IndexOf(neighbour) > -1) continue;

                // Cálculo do custo tentativo g desde o inicio ata o veciño pasando por esteNode
                tentative_g_score = thisNode.g + distance(thisNode, neighbour);
                if (open.IndexOf(neighbour) == -1) {
                    // Primeiro encontro deste nodo: engadímolo a abertos
                    open.Add(neighbour);
                    tentative_is_better = true;
                } else if (tentative_g_score < neighbour.g) {
                    // Este camiño é mellor que o coñecido
                    tentative_is_better = true;
                } else
                    tentative_is_better = false;

                if (tentative_is_better) {
                    // Actualizar información do veciño
                    neighbour.cameFrom = thisNode;
                    neighbour.g = tentative_g_score;
                    neighbour.h = distance(thisNode, end);
                    neighbour.f = neighbour.g + neighbour.h;
                }
            }
        }
        return false;
    }

    // Reconstrúe o camiño desde o nodo de fin seguindo os punteiros cameFrom
    // Resultado: `pathList` conterá os nodos dende inicio ata fin en orde
    private void ReconstructPath(Node startId, Node endId) {

        pathList.Clear();
        pathList.Add(endId);

        var p = endId.cameFrom;

        while (p != startId && p != null) {
            pathList.Insert(0, p);
            p = p.cameFrom;
        }

        pathList.Insert(0, startId);
    }

    // Heurística (aquí usamos distancia euclidiana ao cadrado entre os GameObjects)
    // Devolve un valor non-negativo que estima o custo restante entre dous nodos.
    float distance(Node a, Node b) {
        return Vector3.SqrMagnitude(a.getID().transform.position - b.getID().transform.position);
    }

    // Selecciona o índice do nodo con menor valor f nunha lista
    // Nota: esta implementación itera a lista para atopar o mínimo. Para mellorar o rendemento,
    // substitúe `open` por unha estrutura do tipo PriorityQueue ordenada por f.
    int lowestF(List<Node> l) {

        float lowestf = 0.0f;
        int count = 0;
        int iteratorCount = 0;

        lowestf = l[0].f;

        for (int i = 1; i < l.Count; ++i) {

            if (l[i].f < lowestf) {

                lowestf = l[i].f;
                iteratorCount = count;
            }
            count++;
        }
        return iteratorCount;
    }
}