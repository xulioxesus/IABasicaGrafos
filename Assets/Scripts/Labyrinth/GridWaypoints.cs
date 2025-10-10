using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * CLASE GRIDWAYPOINTS
 * ===================
 * 
 * Sistema completo de navegación e pathfinding nun labirinto baseado en grella.
 * Esta clase integra visualización 3D, algoritmos de busca de camiños e movemento
 * autónomo nun entorno Unity.
 * 
 * FUNCIONALIDADES PRINCIPAIS:
 * 
 * 1. GRELLA DE NAVEGACIÓN:
 *    - Crea unha grella bidimensional de nodos
 *    - Cada nodo pode ser camiñable ou obstáculo
 *    - Conexións automáticas entre nodos adxacentes
 * 
 * 2. VISUALIZACIÓN 3D:
 *    - Waypoints visuais para cada nodo
 *    - Materiais diferentes para obstáculos e destinos
 *    - Representación visual da grella no mundo 3D
 * 
 * 3. ALGORITMOS DE PATHFINDING:
 *    - Integración con PathfindingAlgorithms
 *    - Soporte para BFS, DFS, DFS Recursivo
 *    - Cambio dinámico de algoritmos en tempo de execución
 * 
 * 4. MOVEMENTO AUTÓNOMO:
 *    - Movemento suave entre waypoints
 *    - Rotación cara á dirección de movemento
 *    - Precisión configurable para alcanzar obxectivos
 * 
 * 5. CONTROL INTERACTIVO:
 *    - Controis por teclado para probar algoritmos
 *    - Feedback visual e logs informativos
 *    - Sistema de comparación entre algoritmos
 * 
 * USO:
 * - Anexar este script a un GameObject na escena
 * - Configurar os prefabs e materiais no inspector
 * - Executar e usar as teclas 1-4 para probar algoritmos
 */
public class GridWaypoints : MonoBehaviour
{
    // =============================================================================
    // CONFIGURACIÓN DA GRELLA E VISUALIZACIÓN
    // =============================================================================
    
    // Grella bidimensional que contén todos os nodos do labirinto
    // Cada posición [row, col] representa un punto navegable ou obstáculo
    public LabNode[,] grid;
    
    // Camiño actual calculado polo algoritmo de pathfinding
    // Contén a secuencia de nodos desde inicio ata destino
    List<LabNode> path = new List<LabNode>();
    
    // Índice do nodo actual no camiño durante o movemento
    // Incrementa-se á medida que se alcanzan waypoints
    int curNode = 0;

    // =============================================================================
    // PREFABS E MATERIAIS PARA VISUALIZACIÓN
    // =============================================================================
    
    // Prefab que se instantia para crear waypoints visuais
    // Debe ter un Renderer para aplicar materiais
    public GameObject prefabWaypoint;
    
    // Material que se aplica ao nodo de destino
    // Permite identificar visualmente o obxectivo
    public Material goalMat;
    
    // Material que se aplica aos nodos obstáculo
    // Identifica visualmente as paredes ou zonas non transitables
    public Material wallMat;
    
    // =============================================================================
    // PARÁMETROS DE MOVEMENTO E NAVEGACIÓN
    // =============================================================================
    
    // Posición obxectivo actual cara á que se move o obxecto
    Vector3 goal;
    
    // Velocidade de movemento en unidades por segundo
    // Controla a rapidez do desprazamento entre waypoints
    float speed = 4.0f;
    
    // Distancia mínima para considerar alcanzado un waypoint
    // Valores menores = maior precisión, valores maiores = movemento máis fluido
    float accuracy = 0.5f;
    
    // Velocidade de rotación cara ao obxectivo
    // Controla a rapidez de xirado cara á nova dirección
    float rotSpeed = 4f;
    
    // Distancia entre nodos na grella (escala visual)
    // Determina o tamaño físico da grella no mundo 3D
    int spacing = 5;

    // =============================================================================
    // NODOS ESPECIAIS DE NAVEGAÇÃO
    // =============================================================================
    
    // Nodo de inicio da navegación (orixe dos camiños)
    LabNode startNode;
    
    // Nodo de destino da navegação (obxectivo dos algoritmos)
    LabNode endNode;

    // =============================================================================
    // MÉTODOS DE CÁLCULO DE VECIÑOS E NAVEGACIÓN
    // =============================================================================

    /*
     * MÉTODO GETADJACENTNODES - CÁLCULO DE NODOS ADXACENTES
     * ======================================================
     * 
     * Calcula os nodos veciños dun nodo específico na grella que son accesibles
     * para a navegación. Só considera conexións ortogonais (non diagonais).
     * 
     * FUNCIONAMENTO:
     * - Define as 4 direccións cardinais (arriba, abaixo, esquerda, dereita)
     * - Para cada dirección, calcula a nova posición
     * - Verifica que a posición estea dentro dos límites da grella
     * - Comproba que o nodo destino sexa camiñable
     * - Só inclúe nodos válidos na lista de veciños
     * 
     * PARÁMETROS:
     * - grid: Grella bidimensional de nodos
     * - row, col: Coordenadas do nodo central
     * 
     * RETORNA: Lista de nodos adxacentes accesibles
     */
    List<LabNode> GetAdjacentNodes(LabNode[,] grid, int row, int col)
    {
        List<LabNode> adjacentNodes = new List<LabNode>();
        
        // Definir as 4 direccións cardinais como desplazamentos (row, col)
        // Non inclúe direccións diagonais para simplificar a navegación
        int[,] directions = {
            { -1, 0 },  // Arriba (diminúe row)
            { 1, 0 },   // Abaixo (aumenta row)
            { 0, -1 },  // Esquerda (diminúe col)
            { 0, 1 }    // Dereita (aumenta col)
        };

        // Explorar cada unha das 4 direccións posibles
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newRow = row + directions[i, 0];
            int newCol = col + directions[i, 1];

            // Verificar que as novas coordenadas estean dentro dos límites
            if (IsValidPosition(grid, newRow, newCol))
            {
                LabNode adjacentNode = grid[newRow, newCol];
                
                // Só engadir nodos que sexan transitables
                if (adjacentNode.Walkable)
                {
                    adjacentNodes.Add(adjacentNode);
                }
            }
        }

        return adjacentNodes;
    }

    /*
     * SOBRECARGA DE GETADJACENTNODES
     * Versión de conveniencia que usa a posición gardada no propio nodo
     */
    List<LabNode> GetAdjacentNodes(LabNode[,] grid, LabNode node)
    {
        return GetAdjacentNodes(grid, node.GridRow, node.GridCol);
    }

    /*
     * MÉTODO ISVALIDPOSITION - VALIDACIÓN DE COORDENADAS
     * ==================================================
     * 
     * Verifica se unhas coordenadas específicas están dentro dos límites
     * válidos da grella. Prevén erros de índice fóra de rango.
     * 
     * FUNCIONAMENTO:
     * - Comproba que row sexa >= 0 e < número total de filas
     * - Comproba que col sexa >= 0 e < número total de columnas
     * - Retorna true só se ambas condicións se cumpren
     */
    bool IsValidPosition(LabNode[,] grid, int row, int col)
    {
        return row >= 0 && row < grid.GetLength(0) && 
               col >= 0 && col < grid.GetLength(1);
    }

    // =============================================================================
    // CICLO DE VIDA DE UNITY E INICIALIZACIÓN
    // =============================================================================

    /*
     * MÉTODO START - INICIALIZACIÓN PRINCIPAL DO SISTEMA
     * ==================================================
     * 
     * Configuración inicial completa do sistema de navegación.
     * Execútase unha vez ao inicio da escena.
     * 
     * SECUENCIA DE INICIALIZACIÓN:
     * 1. Crear a estrutura da grella con obstáculos predefinidos
     * 2. Establecer puntos especiais (inicio e destino)
     * 3. Posicionar o obxecto controlado no punto de inicio
     * 
     * Esta orde é importante: primeiro a grella, despois os puntos especiais,
     * finalmente o posicionamento do obxecto.
     */
    void Start()
    {
        // Fase 1: Crear toda a estrutura da grella
        InitializeGrid();
        
        // Fase 2: Configurar puntos de referencia especiais
        SetupStartAndEndNodes();
        
        // Fase 3: Posicionar o obxecto na posición inicial
        MoveToStartPosition();
    }

    /*
     * MÉTODO INITIALIZEGRID - CONFIGURACIÓN DA GRELLA DE NAVEGACIÓN
     * =============================================================
     * 
     * Configura completamente a grella que define o labirinto.
     * Este método coordina a creación da estrutura lóxica e visual.
     * 
     * FASES:
     * 1. Define o patrón de obstáculos do labirinto
     * 2. Crea a estrutura de datos (array de nodos)
     * 3. Inicializa waypoints visuais e conexións
     */
    void InitializeGrid()
    {
        // Patrón do labirinto: true = camiñable, false = obstáculo
        // Este patrón pode modificarse para crear diferentes labirintos
        bool[,] walkablePattern = {
            { true,  true,  false, true,  true,  true  },   // Fila 0
            { true,  false, true,  true,  true,  true  },   // Fila 1
            { true,  false, true,  true,  true,  true  },   // Fila 2
            { true,  true,  true,  false, true,  true  },   // Fila 3
            { true,  true,  true,  true,  false, true  },   // Fila 4
            { true,  true,  false, true,  false, true  },   // Fila 5
            { true,  false, false, true,  true,  true  }    // Fila 6
        };

        // Crear a estrutura de datos baseada no patrón
        CreateGridFromPattern(walkablePattern);
        
        // Configurar toda a parte visual e conexións
        InitializeWaypointsAndNeighbors();
    }

    /*
     * MÉTODO CREATEGRIDFROMPATTERN - CREACIÓN DA ESTRUTURA DE DATOS
     * =============================================================
     * 
     * Converte un patrón de booleanos nunha grella de obxectos LabNode.
     * Este método separa a definición do deseño da implementación.
     * 
     * FUNCIONAMENTO:
     * - Analiza as dimensións do patrón de entrada
     * - Crea un array bidimensional de LabNode coas mesmas dimensións
     * - Para cada posición, crea un nodo co estado de camiñabilidade correspondente
     * - Cada nodo coñece automaticamente a súa posición na grella
     * 
     * VANTAXE: Permite cambiar facilmente o deseño do labirinto
     * modificando só o patrón de booleanos.
     */
    void CreateGridFromPattern(bool[,] walkablePattern)
    {
        int rows = walkablePattern.GetLength(0);    // Número de filas
        int cols = walkablePattern.GetLength(1);    // Número de columnas
        
        // Inicializar o array principal que conterá todos os nodos
        grid = new LabNode[rows, cols];
        
        // Crear cada nodo individual coa súa configuración
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Crear nodo pasándolle a súa posición e estado de camiñabilidade
                // O construtor LabNode(row, col, walkable) establece todo automaticamente
                grid[i, j] = new LabNode(i, j, walkablePattern[i, j]);
            }
        }
    }

    /*
     * MÉTODO INITIALIZEWAYPOINTSANDNEIGHBORS - CONFIGURACIÓN VISUAL E CONEXIÓNS
     * ========================================================================
     * 
     * Configura a representación visual de cada nodo e estabelece as conexións
     * de navegación entre nodos adxacentes. Este é o paso que conecta a lóxica
     * coa visualización 3D.
     * 
     * FUNCIONAMENTO:
     * Para cada nodo na grella:
     * 1. Crea un waypoint visual (GameObject) na posición correspondente
     * 2. Aplica o material visual apropiado (normal, obstáculo)
     * 3. Calcula e asigna a lista de nodos veciños accesibles
     * 
     * NOTA: Só os nodos camiñables teñen veciños. Os obstáculos non precisan
     * conexións xa que nunca se visitarán durante a navegación.
     */
    void InitializeWaypointsAndNeighbors()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // Crear a representación visual do nodo
                CreateWaypoint(i, j);
                
                // Aplicar o material visual correspondente ao tipo de nodo
                SetWaypointMaterial(i, j);
                
                // Establecer conexións só para nodos transitables
                if (grid[i, j].Walkable)
                {
                    grid[i, j].Neighbors = GetAdjacentNodes(grid, i, j);
                }
            }
        }
    }

    /*
     * MÉTODO CREATEWAYPOINT - CREACIÓN DE WAYPOINTS VISUAIS
     * =====================================================
     * 
     * Crea a representación visual tridimensional dun nodo específico.
     * Instantia un prefab na posición correcta do mundo 3D.
     * 
     * CÁLCULO DE POSICIÓN:
     * - X = row * spacing (filas afectan ao eixe X)
     * - Y = altura do obxecto actual (mantén o nivel)
     * - Z = col * spacing (columnas afectan ao eixe Z)
     * 
     * Isto crea unha grella visual onde cada nodo está separado pola
     * distancia definida en 'spacing'.
     */
    void CreateWaypoint(int row, int col)
    {
        Vector3 position = new Vector3(row * spacing, this.transform.position.y, col * spacing);
        grid[row, col].Waypoint = Instantiate(prefabWaypoint, position, Quaternion.identity);
    }

    /*
     * MÉTODO SETWAYPOINTMATERIAL - CONFIGURACIÓN VISUAL DE NODOS
     * ==========================================================
     * 
     * Aplica o material visual correspondente a cada tipo de nodo.
     * Permite identificar rapidamente obstáculos e zonas transitables.
     * 
     * TIPOS DE MATERIAIS:
     * - wallMat: Para nodos obstáculo (non camiñables)
     * - Material por defecto: Para nodos normais (camiñables)
     * - goalMat: Aplicarase posteriormente ao nodo de destino
     */
    void SetWaypointMaterial(int row, int col)
    {
        if (!grid[row, col].Walkable)
        {
            grid[row, col].Waypoint.GetComponent<Renderer>().material = wallMat;
        }
    }

    /*
     * MÉTODO SETUPSTARTANDENDNODES - CONFIGURACIÓN DE PUNTOS ESPECIAIS
     * ================================================================
     * 
     * Define os nodos de inicio e destino da navegación e configura
     * as súas propiedades especiais.
     * 
     * CONFIGURACIÓN:
     * - Nodo de inicio: Esquina superior esquerda (0,0)
     * - Nodo de destino: Esquina inferior dereita (6,5)
     * - Ambos fórzanse a ser camiñables independentemente do patrón inicial
     * - O destino aplícase o material especial para identificación visual
     * 
     * NOTA: Estas posicións son fixas nesta implementación, pero
     * poderían facerse configurables no futuro.
     */
    void SetupStartAndEndNodes()
    {
        startNode = grid[0, 0];     // Esquina superior esquerda
        endNode = grid[6, 5];       // Esquina inferior dereita
        
        // Garantir que inicio e destino sexan sempre accesibles
        startNode.Walkable = true;
        endNode.Walkable = true;
        
        // Marcar visualmente o destino
        endNode.Waypoint.GetComponent<Renderer>().material = goalMat;
    }

    /*
     * MÉTODO MOVETOSTARTPOSITION - POSICIONAMENTO INICIAL
     * ==================================================
     * 
     * Coloca o obxecto controlado na posición inicial da navegación.
     * Usa as coordenadas do waypoint do nodo de inicio.
     * 
     * FUNCIONAMENTO:
     * - Toma a posición X e Z do waypoint de inicio
     * - Mantén a altura Y actual do obxecto
     * - Asigna directamente a nova posición (sen animación)
     */
    void MoveToStartPosition()
    {
        this.transform.position = new Vector3(startNode.Waypoint.transform.position.x,
                                                this.transform.position.y,
                                                startNode.Waypoint.transform.position.z);
    }

    // =============================================================================
    // BUCLE PRINCIPAL DE ACTUALIZACIÓN E CONTROL
    // =============================================================================

    /*
     * MÉTODO LATEUPDATE - BUCLE PRINCIPAL DO SISTEMA
     * ==============================================
     * 
     * Xestiona tanto o control de entrada do usuario como o movemento
     * autónomo do obxecto. Execútase cada frame despois de Update().
     * 
     * FUNCIONALIDADES:
     * 1. CONTROL POR TECLADO: Permite cambiar algoritmos en tempo real
     * 2. MOVEMENTO AUTÓNOMO: Move o obxecto seguindo o camiño calculado
     * 3. ROTACIÓN SUAVE: Orienta o obxecto cara á dirección de movemento
     * 4. TRANSICIÓN ENTRE WAYPOINTS: Detecta cando se alcanza un nodo
     * 
     * CONTROIS DISPOÑIBLES:
     * - Tecla 1: BFS (Breadth-First Search) - Camiño óptimo
     * - Tecla 2: DFS (Depth-First Search) - Exploración en profundidade
     * - Tecla 3: DFS Recursivo - Versión recursiva con backtracking
     * - Tecla 4: TestPath - Camiño predefinido para probas
     * - Enter: Executar DFS por defecto
     */
    void LateUpdate()
    {
        // =============================================================================
        // PROCESAMENTO DE ENTRADA DO USUARIO
        // =============================================================================
        
        // Tecla 1: Executar algoritmo BFS (garantiza camiño máis curto)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Algoritmo cambiado a BFS (Busca en anchura)");
            ResetAndFindPath(PathfindingAlgorithms.BFS);
        }
        // Tecla 2: Executar algoritmo DFS iterativo
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Algoritmo cambiado a DFS (Busca en profundidade)");
            ResetAndFindPath(PathfindingAlgorithms.DFS);
        }
        // Tecla 3: Executar algoritmo DFS recursivo
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Algoritmo cambiado a DFS Recursivo (Busca en profundidade recursiva)");
            ResetAndFindPathRecursive();
        }
        // Tecla 4: Executar algoritmo de proba
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Algoritmo cambiado a TestPath (Camiño de proba)");
            ResetAndFindPath(PathfindingAlgorithms.TestPath);
        }
        
        // Enter: Executar algoritmo por defecto
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Executando DFS por defecto (preme 1:BFS, 2:DFS, 3:DFS-Rec, 4:Test)");
            ResetAndFindPath(PathfindingAlgorithms.DFS);
        }

        // =============================================================================
        // SISTEMA DE MOVEMENTO AUTÓNOMO
        // =============================================================================
        
        // Se non hai camiño calculado, non facer nada
        if (path.Count == 0) return;

        // Establecer o obxectivo actual baseado no nodo do camiño
        goal = new Vector3(path[curNode].Waypoint.transform.position.x,
                            this.transform.position.y, 
                            path[curNode].Waypoint.transform.position.z);

        // Calcular dirección cara ao obxectivo
        Vector3 direction = goal - this.transform.position;

        // Comprobar se estamos preto dabondo do obxectivo actual
        if (direction.magnitude > accuracy)
        {
            // =============================================================================
            // MOVEMENTO CARA AO OBXECTIVO
            // =============================================================================
            
            // Rotación suave cara á dirección de movemento
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                        Quaternion.LookRotation(direction),
                                                        Time.deltaTime * rotSpeed);
            
            // Movemento cara adiante na dirección actual
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
        {
            // =============================================================================
            // TRANSICIÓN AO SEGUINTE WAYPOINT
            // =============================================================================
            
            // Se non é o último nodo do camiño, avanzar ao seguinte
            if (curNode < path.Count - 1)
            {
                curNode++;  // Pasar ao seguinte waypoint
            }
            // Se chegamos ao final do camiño, parar (non resetear)
        }
    }

    // =============================================================================
    // MÉTODOS AUXILIARES PARA EXECUCIÓN DE ALGORITMOS
    // =============================================================================

    /*
     * MÉTODO RESETANDFINDPATH - EXECUTAR ALGORITMOS ESTÁNDAR
     * ======================================================
     * 
     * Método de conveniencia que reinicia o sistema e executa un algoritmo
     * de pathfinding que segue a sinatura estándar (start, end) -> List<LabNode>.
     * 
     * FUNCIONALIDADE:
     * 1. Reposiciona o obxecto no punto de inicio
     * 2. Reinicia o contador de nodos do camiño
     * 3. Executa o algoritmo especificado
     * 4. Proporciona feedback visual do resultado
     * 
     * COMPATIBLE CON: BFS, DFS, TestPath
     */
    void ResetAndFindPath(System.Func<LabNode, LabNode, List<LabNode>> algorithm)
    {
        MoveToStartPosition();     // Volta ao inicio
        curNode = 0;               // Reinicia o progreso do camiño
        
        // Executar o algoritmo seleccionado
        path = algorithm(startNode, endNode);
        
        // Feedback ao usuario
        if (path.Count > 0)
            Debug.Log("Camiño atopado con " + path.Count + " nodos");
        else
            Debug.Log("Non se atopou camiño"); 
    }

    /*
     * MÉTODO RESETANDFINDPATHRECURSIVE - EXECUTAR DFS RECURSIVO
     * =========================================================
     * 
     * Método especializado para executar o algoritmo DFS recursivo,
     * que ten unha sinatura diferente aos outros algoritmos.
     * 
     * DIFERENZAS CON OUTROS ALGORITMOS:
     * - Require listas de visited e path como parámetros
     * - Retorna boolean en lugar de List<LabNode>
     * - O camiño constrúese durante a execución
     * 
     * FUNCIONAMENTO:
     * 1. Prepara as estruturas de datos requiridas
     * 2. Executa o algoritmo DFS recursivo
     * 3. Procesa o resultado (boolean) para obter o camiño
     * 4. Proporciona feedback específico para este algoritmo
     */
    void ResetAndFindPathRecursive()
    {
        MoveToStartPosition();     // Volta ao inicio
        curNode = 0;               // Reinicia o progreso do camiño
        
        // Preparar estruturas de datos específicas para DFS recursivo
        List<LabNode> visited = new List<LabNode>();        // Nodos xa explorados
        List<LabNode> currentPath = new List<LabNode>();    // Camiño en construcción
        
        // Executar o algoritmo DFS recursivo
        bool pathFound = PathfindingAlgorithms.DFSRecursive(startNode, endNode, visited, currentPath);
        
        // Procesar o resultado
        if (pathFound)
        {
            path = new List<LabNode>(currentPath); // Copiar o camiño válido
            Debug.Log("Camiño atopado con DFS recursivo: " + path.Count + " nodos");
        }
        else
        {
            path = new List<LabNode>(); // Lista baleira se falla
            Debug.Log("Non se atopou camiño con DFS recursivo");
        }
    }
}
