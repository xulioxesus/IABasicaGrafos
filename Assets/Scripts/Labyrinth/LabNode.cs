using System.Collections.Generic;
using UnityEngine;

// Clase que representa un nodo na grella de navegación
// Cada nodo contén información sobre a súa posición, estado e conexións
public class LabNode
{
    // Profundidade do nodo no algoritmo de busca (usado en BFS)
    private int depth;
    
    // Indica se o nodo é transitable ou é un obstáculo
    private bool walkable;
    
    // Posición do nodo na grella (row, col)
    private int gridRow;
    private int gridCol;
    
    // GameObject visual que representa este nodo na escena
    private GameObject waypoint = new GameObject();
    
    // Lista de nodos veciños aos que se pode acceder desde este nodo
    private List<LabNode> neighbors = new List<LabNode>();

    // Propiedades públicas para acceder aos campos privados
    public int Depth { get => depth; set => depth = value; }
    public bool Walkable { get => walkable; set => walkable = value; }
    public int GridRow { get => gridRow; set => gridRow = value; }
    public int GridCol { get => gridCol; set => gridCol = value; }
    public GameObject Waypoint { get => waypoint; set => waypoint = value; }
    public List<LabNode> Neighbors { get => neighbors; set => neighbors = value; }

    // Constructor por defecto - crea un nodo camiñable sen posición definida
    public LabNode()
    {
        this.depth = -1;
        this.walkable = true;
        this.gridRow = -1;
        this.gridCol = -1;
    }

    // Constructor que permite especificar se o nodo é camiñable
    public LabNode(bool walkable)
    {
        this.depth = -1;
        this.walkable = walkable;
        this.gridRow = -1;
        this.gridCol = -1;
    }

    // Constructor completo que establece posición e camiñabilidade
    public LabNode(int row, int col, bool walkable = true)
    {
        this.depth = -1;
        this.walkable = walkable;
        this.gridRow = row;
        this.gridCol = col;
    }

    // Compara dous nodos baseándose na súa posición na grella
    public override bool Equals(System.Object obj)
    {
        if (obj == null) return false;
        LabNode n = obj as LabNode;
        if ((System.Object)n == null)
        {
            return false;
        }
        
        // Compara as posicións na grella primeiro (máis eficiente)
        if (this.gridRow != -1 && this.gridCol != -1 && 
            n.gridRow != -1 && n.gridCol != -1)
        {
            return this.gridRow == n.gridRow && this.gridCol == n.gridCol;
        }
        
        // Fallback: compara as posicións físicas se non hai posición na grella
        if (this.waypoint != null && n.waypoint != null)
        {
            return this.waypoint.transform.position.x == n.Waypoint.transform.position.x &&
                   this.waypoint.transform.position.z == n.Waypoint.transform.position.z;
        }
        
        return false;
    }

    // Xera un código hash baseado na posición na grella
    public override int GetHashCode()
    {
        if (gridRow != -1 && gridCol != -1)
        {
            return (gridRow * 1000 + gridCol).GetHashCode();
        }
        return this.waypoint.transform.position.GetHashCode();
    }

    // Representación en texto do nodo (útil para debug)
    public override string ToString()
    {
        return $"LabNode(row:{gridRow}, col:{gridCol}, walkable:{walkable})";
    }

    // Método de conveniencia para obter a posición como Vector2Int
    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(gridRow, gridCol);
    }

    // Verifica se este nodo é veciño directo de outro nodo
    public bool IsAdjacentTo(LabNode other)
    {
        if (other == null) return false;
        
        int rowDiff = Mathf.Abs(this.gridRow - other.gridRow);
        int colDiff = Mathf.Abs(this.gridCol - other.gridCol);
        
        // Son veciños se están a distancia 1 en manhatan e non en diagonal
        return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
    }
}