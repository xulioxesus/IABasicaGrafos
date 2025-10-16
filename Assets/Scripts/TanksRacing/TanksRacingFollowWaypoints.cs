using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sistema de seguimento de waypoints para tanques.
// O obxectivo é mover o GameObject ao longo dunha lista de puntos (waypoints) en loop.
public class TanksRacingFollowWaypoints : MonoBehaviour {

    // =============================================================================
    // CONFIGURACIÓN DE RUTA
    // =============================================================================
    // Array de puntos (GameObjects) que definen o camiño a seguir.
    // A orde no array determina a secuencia de movemento.
    public GameObject[] waypoints;

    // Índice do waypoint actual no array (0 = primeiro waypoint)
    int currentWP = 0;

    // =============================================================================
    // PARÁMETROS DE MOVEMENTO
    // =============================================================================
    // Velocidade de movemento en unidades por segundo
    public float speed = 10.0f;

    // Velocidade de rotación cara ao waypoint (suavizado)
    public float rotSpeed = 10.0f;

    // Distancia mínima para considerar alcanzado un waypoint
    public float detectionDistance = 3.0f;

    // =============================================================================
    // MÉTODOS DO CICLO DE VIDA
    // =============================================================================

    // Update é chamado unha vez por frame e controla a navegación cara aos waypoints.
    void Update() {

        // Se estamos preto do waypoint actual, avanzamos ao seguinte
        if (Vector3.Distance(this.transform.position, waypoints[currentWP].transform.position) < detectionDistance) {

            currentWP++; // pasar ao seguinte waypoint
        }

        // Se chegamos ao final da lista, facemos loop e volvemos ao inicio
        if (currentWP >= waypoints.Length) {

            currentWP = 0; // ciclo
        }

        // Rotación suave cara ao seguinte waypoint (mantemos movemento natural do tanque)
        Quaternion lookAtWP = Quaternion.LookRotation(waypoints[currentWP].transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookAtWP, Time.deltaTime * rotSpeed);

        // Movemento hacia adiante na dirección que mira o tanque
        this.transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
    }
}
