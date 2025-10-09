using UnityEngine;

// Clase que permite a un obxecto moverse seguindo unha serie de puntos de paso (waypoints)
// O obxecto moverase de forma cíclica entre todos os puntos definidos no array
public class WalkWaypoints : MonoBehaviour
{
    // Array de GameObjects que definen o camiño que debe seguir o obxecto
    public GameObject[] path;
    
    // Posición obxectivo actual cara á que se está movendo o obxecto
    private Vector3 goal;
    
    // Velocidade de movemento do obxecto en unidades por segundo
    public float speed = 4.0f;
    
    // Distancia mínima para considerar que se alcanzou un punto de paso
    public float accuracy = 0.5f;
    
    // Velocidade de rotación do obxecto ao xirar cara ao seguinte punto
    public float rotationSpeed = 4f;
    
    // Índice do punto de paso actual no array 'path'
    public int currentNode = 0;


    // Método que se executa en cada frame do xogo
    // Controla o movemento e rotación do obxecto cara aos puntos de paso
    void Update()
    {
        // Establece o obxectivo actual mantendo a altura Y do obxecto
        // e tomando as coordenadas X e Z do punto de paso actual
        goal = new Vector3( path[currentNode].transform.position.x,
                            this.transform.position.y,
                            path[currentNode].transform.position.z);

        // Calcula a dirección desde a posición actual cara ao obxectivo
        Vector3 direction = goal - this.transform.position;

        // Se a distancia ao obxectivo é maior que a precisión establecida
        if (direction.magnitude > accuracy)
        {
            // Rota gradualmente o obxecto para mirar cara á dirección do obxectivo
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                        Quaternion.LookRotation(direction),
                                                        Time.deltaTime * rotationSpeed);
            // Move o obxecto cara adiante na súa dirección actual
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
        {
            // Se chegou ao punto actual, pasa ao seguinte
            if (currentNode < path.Length - 1)
            {
                currentNode++; // Avanza ao seguinte punto
            }
            else
            {
                currentNode = 0; // Volta ao primeiro punto (movemento cíclico)
            }
        }
    }
}
