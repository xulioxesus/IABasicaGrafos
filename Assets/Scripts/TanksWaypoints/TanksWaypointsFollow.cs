using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que permite a un obxecto seguir unha ruta de waypoints usando A*
public class TanksWaypointsFollow : MonoBehaviour {

    // Obxectivo actual ao que se dirixe
    Transform goal;
    // Velocidade de movemento
    float speed = 5.0f;
    // Distancia mínima para considerar que chegou ao waypoint
    float accuracy = 5.0f;
    // Velocidade de rotación
    float rotSpeed = 2.0f;
    // Array de waypoints
    GameObject[] wps;
    // Nodo actual no que se atopa
    GameObject currentNode;
    // Índice do waypoint actual na ruta
    int currentWP = 0;
    // Referencia ao grafo para calcular rutas
    Graph g;

    // Manager que contén os waypoints
    public GameObject wpManager;

    // Inicialización do obxecto
    void Start() {
        // Acelerar o tempo de simulación
        Time.timeScale = 5.0f;
        // Obter waypoints do manager
        wps = wpManager.GetComponent<TanksWaypointsManager>().waypoints;
        // Obter referencia ao grafo
        g = wpManager.GetComponent<TanksWaypointsManager>().graph;
        // Establecer nodo inicial
        currentNode = wps[0];

        // Invoke("GotoRuin", 2.0f);
    }

    // Ir ao helicóptero (waypoint 0)
    public void GotoHeli() {

        g.AStar(currentNode, wps[0]);
        currentWP = 0;
    }

    // Ir ás ruínas (waypoint 7)
    public void GotoRuin() {

        g.AStar(currentNode, wps[7]);
        currentWP = 0;
    }

    // Ir á rocha (waypoint 1)
    public void GotoRock() {

        g.AStar(currentNode, wps[1]);
        currentWP = 0;
    }

    // Ir á fábrica (waypoint 4)
    public void GotoFactory() {

        g.AStar(currentNode, wps[4]);
        currentWP = 0;
    }

    // Actualización que manexa o movemento do obxecto seguindo a ruta
    void LateUpdate() {

        // Se non hai ruta ou xa chegamos ao final, non facer nada
        if (g.pathList.Count == 0 || currentWP == g.pathList.Count) return;

        // Actualizar nodo actual
        currentNode = g.getPathPoint(currentWP);

        // Se estamos preto do waypoint actual, avanzar ao seguinte
        if (Vector3.Distance(g.pathList[currentWP].getID().transform.position, transform.position) < accuracy) {

            currentWP++;
        }

        // Se aínda hai waypoints por visitar
        if (currentWP < g.pathList.Count) {

            // Establecer obxectivo actual
            goal = g.pathList[currentWP].getID().transform;
            // Calcular posición á que mirar (manter Y actual)
            Vector3 lookAtGoal = new Vector3(
                goal.position.x,
                transform.position.y,
                goal.position.z);

            // Calcular dirección cara ao obxectivo
            Vector3 direction = lookAtGoal - this.transform.position;

            // Rotar suavemente cara ao obxectivo
            transform.rotation = Quaternion.Slerp(
                this.transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * rotSpeed);

            // Mover cara adiante
            transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
        }
    }
}
