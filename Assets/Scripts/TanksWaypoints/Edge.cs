using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Representa unha aresta/ligazón entre dous nodos no grafo.
// Contén referencia ao nodo de inicio e ao nodo de destino.
public class Edge {

    // Nodo de orixe
    public Node startNode;
    // Nodo de destino
    public Node endNode;

    // Constructor que crea a aresta entre dous nodos
    public Edge(Node from, Node to) {
        startNode = from;
        endNode = to;
    }
}