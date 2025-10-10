using System.Collections.Generic;
using UnityEngine;

// Clase que representa un nodo individual nunha grella de navegación para algoritmos de pathfinding
// Cada nodo coñece a súa posición na grella, se é transitable, e mantén referencias aos seus veciños
// Úsase como unidade básica en algoritmos de busca como BFS, DFS, A*, etc.
public class LabNode
{
    // Profundidade do nodo desde o punto de inicio (usado principalmente en BFS)
    // Valor -1 indica que aínda non foi visitado polo algoritmo
    private int depth;
    
    // Determina se este nodo é transitable (true) ou é un obstáculo (false)
    // Os algoritmos só poden mover a través de nodos camiñables
    private bool walkable;
    
    // Coordenadas do nodo na grella lóxica
    // gridRow: fila na grella (eixe Y lóxico)
    // gridCol: columna na grella (eixe X lóxico)
    private int gridRow;
    private int gridCol;
    
    // Representación visual do nodo na escena de Unity
    // Permite ver o nodo no mundo 3D e aplicar materiais diferentes
    private GameObject waypoint = new GameObject();
    
    // Lista de nodos adxacentes aos que se pode acceder directamente desde este nodo
    // Só inclúe nodos camiñables nas 4 direccións cardinais (non diagonais)
    private List<LabNode> neighbors = new List<LabNode>();

    // Propiedades públicas para acceso controlado aos campos privados
    public int Depth { get => depth; set => depth = value; }
    public bool Walkable { get => walkable; set => walkable = value; }
    public int GridRow { get => gridRow; set => gridRow = value; }
    public int GridCol { get => gridCol; set => gridCol = value; }
    public GameObject Waypoint { get => waypoint; set => waypoint = value; }
    public List<LabNode> Neighbors { get => neighbors; set => neighbors = value; }

    // Constructor por defecto - crea un nodo camiñable sen posición na grella definida
    // Útil cando a posición se establecerá posteriormente
    public LabNode()
    {
        this.depth = -1;        // Marca como non visitado
        this.walkable = true;   // Por defecto é camiñable
        this.gridRow = -1;      // Posición indefinida
        this.gridCol = -1;      // Posición indefinida
    }

    // Constructor que permite especificar só a camiñabilidade do nodo
    // A posición na grella queda sen definir
    public LabNode(bool walkable)
    {
        this.depth = -1;
        this.walkable = walkable;
        this.gridRow = -1;
        this.gridCol = -1;
    }

    // Constructor completo que establece posición na grella e camiñabilidade
    // Recomendado para a maioría de casos de uso
    public LabNode(int row, int col, bool walkable = true)
    {
        this.depth = -1;
        this.walkable = walkable;
        this.gridRow = row;
        this.gridCol = col;
    }

    // Sobrescribe o método Equals para comparar nodos baseándose na súa posición
    // Permite usar nodos en coleccións como HashSet e Dictionary de forma eficiente
    public override bool Equals(System.Object obj)
    {
        if (obj == null) return false;
        LabNode n = obj as LabNode;
        if ((System.Object)n == null)
        {
            return false;
        }
        
        // Primeira opción: compara posicións na grella (máis rápido e fiable)
        if (this.gridRow != -1 && this.gridCol != -1 && 
            n.gridRow != -1 && n.gridCol != -1)
        {
            return this.gridRow == n.gridRow && this.gridCol == n.gridCol;
        }
        
        // Opción de respaldo: compara posicións físicas dos waypoints
        // Úsase só se as posicións na grella non están definidas
        if (this.waypoint != null && n.waypoint != null)
        {
            return this.waypoint.transform.position.x == n.Waypoint.transform.position.x &&
                   this.waypoint.transform.position.z == n.Waypoint.transform.position.z;
        }
        
        return false;
    }

    // Xera un código hash único para este nodo baseado na súa posición
    // Necesario para o correcto funcionamento de Equals() en coleccións
    public override int GetHashCode()
    {
        // Usa posición na grella se está dispoñible (máis eficiente)
        if (gridRow != -1 && gridCol != -1)
        {
            // Combina row e col nun único valor hash
            return (gridRow * 1000 + gridCol).GetHashCode();
        }
        // Respaldo: usa posición física do waypoint
        return this.waypoint.transform.position.GetHashCode();
    }

    // Proporciona unha representación en texto do nodo para depuración
    // Mostra información clave de forma concisa
    public override string ToString()
    {
        return $"LabNode(row:{gridRow}, col:{gridCol}, walkable:{walkable})";
    }

    // Método de conveniencia que retorna a posición na grella como Vector2Int
    // Útil para cálculos de distancia, direccións e operacións vectoriais
    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(gridRow, gridCol);
    }

    // Determina se este nodo é veciño directo (adxacente) de outro nodo
    // Considera só conexións ortogonais (arriba, abaixo, esquerda, dereita)
    // Non inclúe conexións diagonais
    public bool IsAdjacentTo(LabNode other)
    {
        if (other == null) return false;
        
        int rowDiff = Mathf.Abs(this.gridRow - other.gridRow);
        int colDiff = Mathf.Abs(this.gridCol - other.gridCol);
        
        // Son veciños se a distancia Manhattan é exactamente 1
        // (1,0) ou (0,1) pero non (1,1) nin (0,0)
        return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
    }

    // Calcula a distancia Manhattan (taxicab) ata outro nodo
    // Útil para heurísticas en algoritmos como A*
    public int ManhattanDistanceTo(LabNode other)
    {
        if (other == null) return int.MaxValue;
        return Mathf.Abs(this.gridRow - other.gridRow) + Mathf.Abs(this.gridCol - other.gridCol);
    }

    // Verifica se este nodo ten unha posición válida na grella
    // Útil para validar nodos antes de usar en algoritmos
    public bool HasValidGridPosition()
    {
        return gridRow >= 0 && gridCol >= 0;
    }
}