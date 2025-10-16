using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Representa un nodo no grafo usado para o cálculo de rutas (A*).
// Cada Node referencia o GameObject correspondente (id) e mantén listas/valores
// usados polo algoritmo de busca (f, g, h, cameFrom) e as arestas saíntes.
public class Node {

    // Lista de arestas que saen deste nodo (veciños)
    public List<Edge> edgeList = new List<Edge>();

    // Campos auxiliares usados por algunhas implementacións/versións
    public Node path = null;

    // Valores para A*: f = g + h, g = custo acumulado, h = heurística
    public float f, g, h;

    // Nodo de procedencia (usado para reconstruír o camiño)
    public Node cameFrom;

    // Referencia ao GameObject que representa visualmente este nodo
    private GameObject id;

    // Constructor que asocia o Node a un GameObject
    public Node(GameObject i) {
        id = i;
        path = null;
    }

    // Devolve o GameObject asociado a este nodo
    public GameObject getID() {
        return id;
    }
}