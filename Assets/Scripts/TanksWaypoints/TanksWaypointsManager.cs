using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Representa un enlace/aresta entre dous nodos (waypoints) no editor
// Este struct úsase no inspector para definir conexións entre GameObjects
public struct Link {

    // Tipo de dirección do enlace
    // UNI -> unidireccional (node1 -> node2)
    // BI  -> bidireccional (node1 <-> node2)
    public enum direction {

        UNI,
        BI
    }

    // Nodo de orixe e nodo de destino (GameObjects con tag/representación de waypoint)
    public GameObject node1, node2;

    // Dirección do enlace
    public direction dir;

    // Exemplo de uso: en Start() percorremos os Links e engadimos as arestas ao grafo.
}

// Xestor de waypoints que construye un grafo simple a partir dos nodos e enlaces definidos
// Util para representar rutas non-lineais onde os nodos poden ter varias conexións
public class TanksWaypointsManager : MonoBehaviour {

    // Lista de GameObjects que representan os waypoints (nodos do grafo)
    public GameObject[] waypoints;

    // Enlaces/arestas entre nodos. Cada Link pode ser unidireccional ou bidireccional.
    public Link[] links;

    // Estrutura de grafo usada para xestionar nodos e arestas (implementación en outra parte do proxecto)
    public Graph graph = new Graph();

    // Ao comezo, enchemos a estrutura de grafo con nodos e arestas definidas no inspector
    void Start() {

        if (waypoints.Length > 0) {

            foreach (GameObject wp in waypoints) {

                // Engadimos o nodo ao grafo
                graph.AddNode(wp);

                // Percorremos os enlaces e engadimos as arestas correspondentes
                foreach (Link l in links) {

                    // Engadir aresta desde node1 a node2
                    graph.AddEdge(l.node1, l.node2);
                    // Se o enlace é bidireccional, engadimos a aresta inversa
                    if (l.dir == Link.direction.BI) {

                        graph.AddEdge(l.node2, l.node1);
                    }
                }
            }
        }
    }
}
