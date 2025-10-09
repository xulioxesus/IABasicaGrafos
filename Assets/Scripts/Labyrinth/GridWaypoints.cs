using UnityEngine;

public class GridWaypoints : MonoBehaviour
{
    public Node[,] grid;
    List<Node> path = new List<Node>();
    int curNode = 0;

    public GameObject prefabWaypoint;
    public Material goalMat;
    public Material wallMat;
    Vector3 goal;
    float speed = 4.0f;
    float accuracy = 0.5f;
    float rotSpeed = 4f;
    int spacing = 5;

    Node startNode;
    Node endNode;

    public class Node
    {
        private int depth;
        private bool walkable;

        private GameObject waypoint = new GameObject();
        private List<Node> neighbors = new List<Node>();
        public int Depth { get => depth; set => depth = value; }
        public bool Walkable { get => walkable; set => walkable = value; }
        public GameObject Waypoint { get => waypoint; set => waypoint = value; }
        public List<Node> Neighbors { get => neighbors; set => neighbors = value; }

        public Node()
        {
            this.depth = -1;
            this.walkable = true;
        }

        public Node(bool walkable)
        {
            this.depth = -1;
            this.walkable = walkable;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null) return false;
            Node n = obj as Node;
            if ((System.Object)n == null)
            {
                return false;
            }
            if (this.waypoint.transform.position.x == n.Waypoint.transform.position.x &&
                this.waypoint.transform.position.z == n.Waypoint.transform.position.z)
            {
                return true;
            }
            return false;
        }
    }

    List<Node> getAdjacentNodes(Node[,] grid, int i, int j)
    {
        List<Node> list = new List<Node>();

        // node up
        if (i - 1 >= 0)
            if (grid[i - 1, j].Walkable)
            {
                list.Add(grid[i - 1, j]);
            }
        // node down
        if (i + 1 < grid.GetLength(0))
            if (grid[i + 1, j].Walkable)
            {
                list.Add(grid[i + 1, j]);
            }

        // node left
        if (j - 1 >= 0)
            if (grid[i, j - 1].Walkable)
            {
                list.Add(grid[i, j - 1]);
            }

        // node right
        if (j + 1 < grid.GetLength(1))
            if (grid[i, j + 1].Walkable)
            {
                list.Add(grid[i, j + 1]);
            }

        return list;
    }

    void Start()
    {
        // create grid
        grid = new Node[,] {
            { new Node(), new Node(), new Node(false), new Node(), new Node(), new Node() },
            { new Node(), new Node(false), new Node(), new Node(), new Node(), new Node() },
            { new Node(), new Node(false), new Node(), new Node(), new Node(), new Node() },
            { new Node(), new Node(), new Node(), new Node(false), new Node(), new Node() },
            { new Node(), new Node(), new Node(), new Node(), new Node(false), new Node() },
            { new Node(), new Node(), new Node(false), new Node(), new Node(false), new Node() },
            { new Node(), new Node(false), new Node(false), new Node(), new Node(), new Node() },
            { new Node(), new Node(), new Node(), new Node(), new Node(), new Node() }
        };

        // initialize grid points
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j].Waypoint = Instantiate(prefabWaypoint,
                                                    new Vector3(i * spacing, this.transform.position.y, j * spacing),
                                                    Quaternion.identity);
            }

            if (!grid[i, j].Walkable)
            {
                grid[i, j].Waypoint.GetComponent<Renderer>()
                    .material = wallMat;
            }
            else
            {
                grid[i, j].Neighbors = getAdjacentNodes(grid, i, j);
            }
        }

        startNode = grid[0, 0];
        endNode = grid[6, 5];
        startNode.Walkable = true;
        endNode.Walkable = true;
        endNode.Waypoint.GetComponent<Renderer>().material = goalMat;

        this.transform.position = new Vector3(  startNode.Waypoint.transform.position.x,
                                                this.transform.position.y,
                                                startNode.Waypoint.transform.position.z);
    }   
}
