using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridWaypoints : MonoBehaviour
{
    public LabNode[,] grid;
    List<LabNode> path = new List<LabNode>();
    int curNode = 0;

    public GameObject prefabWaypoint;
    public Material goalMat;
    public Material wallMat;
    Vector3 goal;
    float speed = 4.0f;
    float accuracy = 0.5f;
    float rotSpeed = 4f;
    int spacing = 5;

    LabNode startNode;
    LabNode endNode;

    // Obtén os nodos adxacentes (veciños) dun nodo na grella
    // Comproba as 4 direccións: arriba, abaixo, esquerda, dereita
    List<LabNode> GetAdjacentNodes(LabNode[,] grid, int row, int col)
    {
        List<LabNode> adjacentNodes = new List<LabNode>();
        
        // Definir as 4 direccións posibles: arriba, abaixo, esquerda, dereita
        int[,] directions = {
            { -1, 0 },  // Arriba
            { 1, 0 },   // Abaixo
            { 0, -1 },  // Esquerda
            { 0, 1 }    // Dereita
        };

        // Comprobar cada dirección
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newRow = row + directions[i, 0];
            int newCol = col + directions[i, 1];

            // Verificar se as coordenadas están dentro dos límites da grella
            if (IsValidPosition(grid, newRow, newCol))
            {
                LabNode adjacentNode = grid[newRow, newCol];
                
                // Só engadir se o nodo é camiñable
                if (adjacentNode.Walkable)
                {
                    adjacentNodes.Add(adjacentNode);
                }
            }
        }

        return adjacentNodes;
    }

    // Versión alternativa que usa a posición gardada no nodo
    List<LabNode> GetAdjacentNodes(LabNode[,] grid, LabNode node)
    {
        return GetAdjacentNodes(grid, node.GridRow, node.GridCol);
    }

    // Verifica se unha posición está dentro dos límites da grella
    bool IsValidPosition(LabNode[,] grid, int row, int col)
    {
        return row >= 0 && row < grid.GetLength(0) && 
               col >= 0 && col < grid.GetLength(1);
    }

    void Start()
    {
        // Crear e inicializar a grella
        InitializeGrid();
        
        // Configurar nodos de inicio e destino
        SetupStartAndEndNodes();
        
        // Posicionar o obxecto na posición de inicio
        MoveToStartPosition();
    }

    // Crea a grella con obstáculos predefinidos
    void InitializeGrid()
    {
        // Definir o patrón de obstáculos (true = camiñable, false = obstáculo)
        bool[,] walkablePattern = {
            { true,  true,  false, true,  true,  true  },
            { true,  false, true,  true,  true,  true  },
            { true,  false, true,  true,  true,  true  },
            { true,  true,  true,  false, true,  true  },
            { true,  true,  true,  true,  false, true  },
            { true,  true,  false, true,  false, true  },
            { true,  false, false, true,  true,  true  }
        };

        // Crear a grella baseada no patrón
        CreateGridFromPattern(walkablePattern);
        
        // Inicializar waypoints visuais e conexións
        InitializeWaypointsAndNeighbors();
    }

    // Crea a grella de nodos baseándose nun patrón de camiñabilidade
    void CreateGridFromPattern(bool[,] walkablePattern)
    {
        int rows = walkablePattern.GetLength(0);
        int cols = walkablePattern.GetLength(1);
        
        grid = new LabNode[rows, cols];
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Crear nodo coa súa posición na grella
                grid[i, j] = new LabNode(i, j, walkablePattern[i, j]);
            }
        }
    }

    // Inicializa os waypoints visuais e as conexións entre nodos
    void InitializeWaypointsAndNeighbors()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // Crear waypoint visual
                CreateWaypoint(i, j);
                
                // Configurar material do waypoint
                SetWaypointMaterial(i, j);
                
                // Establecer conexións cos veciños (só para nodos camiñables)
                if (grid[i, j].Walkable)
                {
                    grid[i, j].Neighbors = GetAdjacentNodes(grid, i, j);
                }
            }
        }
    }

    // Crea o waypoint visual para unha posición específica
    void CreateWaypoint(int row, int col)
    {
        Vector3 position = new Vector3(row * spacing, this.transform.position.y, col * spacing);
        grid[row, col].Waypoint = Instantiate(prefabWaypoint, position, Quaternion.identity);
    }

    // Establece o material do waypoint segundo o seu tipo
    void SetWaypointMaterial(int row, int col)
    {
        if (!grid[row, col].Walkable)
        {
            grid[row, col].Waypoint.GetComponent<Renderer>().material = wallMat;
        }
    }

    // Configura os nodos de inicio e destino
    void SetupStartAndEndNodes()
    {
        startNode = grid[0, 0];
        endNode = grid[6, 5];
        
        // Asegurar que inicio e destino son sempre camiñables
        startNode.Walkable = true;
        endNode.Walkable = true;
        
        // Marcar o destino visualmente
        endNode.Waypoint.GetComponent<Renderer>().material = goalMat;
    }

    // Move o obxecto á posición de inicio
    void MoveToStartPosition()
    {
        this.transform.position = new Vector3(startNode.Waypoint.transform.position.x,
                                                this.transform.position.y,
                                                startNode.Waypoint.transform.position.z);
    }

    void LateUpdate()
    {
        // Cambio de algoritmo con teclas
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Algoritmo cambiado a BFS (Busca en anchura)");
            ResetAndFindPath(PathfindingAlgorithms.BFS);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Algoritmo cambiado a DFS (Busca en profundidade)");
            ResetAndFindPath(PathfindingAlgorithms.DFS);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Algoritmo cambiado a DFS Recursivo (Busca en profundidade recursiva)");
            ResetAndFindPathRecursive();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Algoritmo cambiado a TestPath (Camiño de proba)");
            ResetAndFindPath(PathfindingAlgorithms.TestPath);
        }
        
        // calculate the shortest path when the return key is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Executando DFS por defecto (preme 1:BFS, 2:DFS, 3:DFS-Rec, 4:Test)");
            ResetAndFindPath(PathfindingAlgorithms.DFS);
        }

        // if there's no path, do nothing
        if (path.Count == 0) return;

        // set the goal position
        goal = new Vector3(path[curNode].Waypoint.transform.position.x,
                            this.transform.position.y, path[curNode].Waypoint.transform.position.z);

        // set the direction
        Vector3 direction = goal - this.transform.position;
        // move toward the goal or increase the counter to set another goal in the next iteration

        if (direction.magnitude > accuracy)
        {
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                        Quaternion.LookRotation(direction),
                                                        Time.deltaTime * rotSpeed);
            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
        else
        {
            if (curNode < path.Count - 1)
            {
                curNode++;
            }
        }
    }

    // Método auxiliar para resetear a posición e executar un algoritmo
    void ResetAndFindPath(System.Func<LabNode, LabNode, List<LabNode>> algorithm)
    {
        MoveToStartPosition();
        curNode = 0;
        
        path = algorithm(startNode, endNode);
        
        if (path.Count > 0)
            Debug.Log("Camiño atopado con " + path.Count + " nodos");
        else
            Debug.Log("Non se atopou camiño"); 
    }

    // Método auxiliar para resetear a posición e executar o DFS recursivo
    void ResetAndFindPathRecursive()
    {
        MoveToStartPosition();
        curNode = 0;
        
        // Preparar listas para o DFS recursivo
        List<LabNode> visited = new List<LabNode>();
        List<LabNode> currentPath = new List<LabNode>();
        
        // Executar o DFS recursivo
        bool pathFound = PathfindingAlgorithms.DFSRecursive(startNode, endNode, visited, currentPath);
        
        if (pathFound)
        {
            path = new List<LabNode>(currentPath); // Copiar o camiño atopado
            Debug.Log("Camiño atopado con DFS recursivo: " + path.Count + " nodos");
        }
        else
        {
            path = new List<LabNode>(); // Lista baleira se non se atopa camiño
            Debug.Log("Non se atopou camiño con DFS recursivo");
        }
    }
}
