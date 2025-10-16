using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {

    // Lista global de arestas e nodos do grafo
    List<Edge> edges = new List<Edge>();
    List<Node> nodes = new List<Node>();

    // Resultado da última busca: lista de nodos que forman o camiño (de inicio a fin)
    public List<Node> pathList = new List<Node>();

    public Graph() { }

    // Engade un novo nodo ao grafo asociado a un GameObject
    public void AddNode(GameObject id) {
        Node node = new Node(id);
        nodes.Add(node);
    }

    // Engade unha aresta entre dous nodos identificados polos seus GameObjects
    public void AddEdge(GameObject fromNode, GameObject toNode) {

        Node from = FindNode(fromNode);
        Node to = FindNode(toNode);

        if (from != null && to != null) {

            Edge e = new Edge(from, to);
            edges.Add(e);
            from.edgeList.Add(e);
        }
    }

    // Devolve o GameObject dun punto do camiño calculado (índice na pathList)
    public GameObject getPathPoint(int index) {
        return pathList[index].getID();
    }

    // Busca un Node dado o seu GameObject asociado
    private Node FindNode(GameObject id) {
        foreach (Node n in nodes) {
            if (n.getID() == id) return n;
        }
        return null;
    }

    // Implementación do algoritmo A* sobre o grafo
    // Parámetros: GameObject de inicio e fin (deben corresponder a nodos existentes)
    // Devolve true se atopa un camiño, e en tal caso poboará `pathList` coa secuencia de nodos.
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

        // Inicialización do nodo de inicio
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

                // Cálculo do custo tentativo g
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
    float distance(Node a, Node b) {
        return Vector3.SqrMagnitude(a.getID().transform.position - b.getID().transform.position);
    }

    // Selecciona o índice do nodo con menor valor f nunha lista
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