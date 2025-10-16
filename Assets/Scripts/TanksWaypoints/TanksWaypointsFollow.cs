using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Seguidor de rutas baseado en waypoints usando A* para calcular camiños
// Os métodos GotoX chamarrán o AStar no grafo para poboar `g.pathList` e logo o obxecto
// moverase seguindo eses puntos en LateUpdate.
public class TanksWaypointsFollow : MonoBehaviour {

    // =============================================================================
    // REFERENCIAS E ESTADO
    // =============================================================================
    // Obxectivo actual ao que se dirixe (transform do waypoint corrente)
    Transform goal;

    // Array de waypoints (obtido desde o manager)
    GameObject[] wps;

    // Nodo/waypoint actual no que se atopa (GameObject)
    GameObject currentNode;

    // Referencia ao grafo que contén rutas e a lista de puntos calculados
    Graph g;

    // Manager que contén os waypoints (debe ter TanksWaypointsManager)
    public GameObject wpManager;

    // =============================================================================
    // PARÁMETROS DE MOVEMENTO
    // =============================================================================
    // Velocidade de movemento en unidades por segundo
    float speed = 5.0f;
    // Distancia mínima para considerar que chegou ao waypoint
    float accuracy = 5.0f;
    // Velocidade de rotación (suavizado)
    float rotSpeed = 2.0f;

    // Índice do waypoint actual na ruta (posición dentro de g.pathList)
    int currentWP = 0;

    // =============================================================================
    // INICIALIZACIÓN
    // =============================================================================
    // Configuración inicial: obtemos referencia aos waypoints e ao grafo desde o manager
    void Start() {
        // Nota: neste exemplo aceleramos o tempo para que a simulación vaia máis rápido
        Time.timeScale = 5.0f;

        // Obter array de waypoints do manager
        wps = wpManager.GetComponent<TanksWaypointsManager>().waypoints;

        // Obter referencia ao grafo definido no manager
        g = wpManager.GetComponent<TanksWaypointsManager>().graph;

        // Establecer nodo/waypoint inicial (o primeiro do array)
        currentNode = wps[0];

        // Exemplo de invocación retardada a un destino
        // Invoke("GotoRuin", 2.0f);
    }

    // =============================================================================
    // MÉTODOS PÚBLICOS PARA CALCULAR RUTAS
    // =============================================================================
    // Cada un destes métodos chama a función AStar do grafo para calcular o camiño
    // desde o nodo actual ata o destino desexado; logo reinicia o índice para comezar
    // a seguir a ruta dende o principio.

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

    // =============================================================================
    // BUCLE DE MOVEMENTO (LateUpdate)
    // =============================================================================
    // LateUpdate move o obxecto seguindo os puntos xerados por A* (g.pathList).
    // Compróbase a distancia ao punto actual; se se alcanza, avanzamos ao seguinte.
    void LateUpdate() {

        // Se non hai ruta calculada ou xerstamos o final da ruta, saímos
        if (g.pathList.Count == 0 || currentWP == g.pathList.Count) return;

        // Actualizar nodo actual segundo o índice da ruta
        currentNode = g.getPathPoint(currentWP);

        // Se estamos preto do waypoint actual, avanzar ao seguinte
        if (Vector3.Distance(g.pathList[currentWP].getID().transform.position, transform.position) < accuracy) {
            currentWP++;
        }

        // Se aínda hai puntos por visitar na ruta
        if (currentWP < g.pathList.Count) {

            // Establecer obxectivo actual como o transform do waypoint
            goal = g.pathList[currentWP].getID().transform;

            // Calcular posición á que mirar mantendo a altura (Y) actual
            Vector3 lookAtGoal = new Vector3(
                goal.position.x,
                transform.position.y,
                goal.position.z);

            // Dirección cara ao obxectivo
            Vector3 direction = lookAtGoal - this.transform.position;

            // Rotación suave cara á dirección obxectivo
            transform.rotation = Quaternion.Slerp(
                this.transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * rotSpeed);

            // Movemento cara adiante segundo a velocidade definida
            transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
        }
    }
}
