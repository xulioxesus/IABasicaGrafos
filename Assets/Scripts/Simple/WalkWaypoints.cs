using UnityEngine;

// Sistema de navegación secuencial que move un obxecto seguindo waypoints de forma cíclica
// O obxecto move-se automaticamente entre os puntos definidos no array, voltando ao primeiro ao rematar
public class WalkWaypoints : MonoBehaviour
{
    // =============================================================================
    // CONFIGURACIÓN DE RUTA E NAVEGACIÓN
    // =============================================================================
    
    // Array de GameObjects que definen o camiño a seguir
    // A orde no array determina a secuencia de movemento
    public GameObject[] path;
    
    // Posición obxectivo actual (mantén Y do obxecto, toma X e Z do waypoint)
    private Vector3 goal;
    
    // =============================================================================
    // PARÁMETROS DE MOVEMENTO
    // =============================================================================
    
    // Velocidade de movemento en unidades por segundo
    public float speed = 4.0f;
    
    // Distancia mínima para considerar alcanzado un waypoint
    public float accuracy = 0.5f;
    
    // Velocidade de rotación cara á nova dirección
    public float rotationSpeed = 4f;
    
    // Índice do waypoint actual no array (0 = primeiro waypoint)
    public int currentNode = 0;

    // =============================================================================
    // LÓXICA PRINCIPAL DE NAVEGACIÓN
    // =============================================================================

    // Controla o movemento secuencial entre waypoints
    // Executa navegación cíclica automática
    void Update()
    {
        // Establecer obxectivo mantendo altura Y actual
        goal = new Vector3( path[currentNode].transform.position.x,
                            this.transform.position.y,
                            path[currentNode].transform.position.z);

        // Calcular dirección cara ao obxectivo
        Vector3 direction = goal - this.transform.position;

        // Se aínda non chegou ao waypoint
        if (direction.magnitude > accuracy)
        {
            // Rotar suavemente cara á dirección obxectivo
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                        Quaternion.LookRotation(direction),
                                                        Time.deltaTime * rotationSpeed);
            // Mover cara adiante
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
        {
            // Avanzar ao seguinte waypoint (navegación cíclica)
            if (currentNode < path.Length - 1)
            {
                currentNode++; // Seguinte waypoint
            }
            else
            {
                currentNode = 0; // Volta ao primeiro (ciclo)
            }
        }
    }
}
