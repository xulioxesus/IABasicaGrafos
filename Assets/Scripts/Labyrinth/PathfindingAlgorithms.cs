using System.Collections.Generic;
using UnityEngine;

// Clase estática que contén algoritmos de busca de camiños (pathfinding)
// Permite reutilizar os algoritmos en diferentes partes do proxecto
public static class PathfindingAlgorithms
{
    // Algoritmo BFS (Breadth-First Search) - Busca en anchura
    // Explora todos os nodos á mesma distancia antes de avanzar
    // Garantiza o camiño máis curto en grafos non ponderados
    public static List<LabNode> BFS(LabNode start, LabNode end)
    {
        Queue<LabNode> toVisit = new Queue<LabNode>();
        List<LabNode> visited = new List<LabNode>();

        LabNode currentNode = start;
        currentNode.Depth = 0;
        toVisit.Enqueue(currentNode);

        List<LabNode> finalPath = new List<LabNode>();

        while (toVisit.Count > 0)
        {
            currentNode = toVisit.Dequeue();

            if (visited.Contains(currentNode))
                continue;

            visited.Add(currentNode);

            if (currentNode.Equals(end))
            {
                // Reconstrúe o camiño usando as profundidades
                while (currentNode.Depth != 0)
                {
                    foreach (LabNode n in currentNode.Neighbors)
                    {
                        if (n.Depth == currentNode.Depth - 1)
                        {
                            finalPath.Add(currentNode);
                            currentNode = n;
                            break;
                        }
                    }
                }
                finalPath.Reverse();
                break;
            }
            else
            {
                // Explora os veciños
                foreach (LabNode n in currentNode.Neighbors)
                {
                    if (!visited.Contains(n) && n.Walkable)
                    {
                        n.Depth = currentNode.Depth + 1;
                        toVisit.Enqueue(n);
                    }
                }
            }
        }

        return finalPath;
    }

    // Algoritmo DFS (Depth-First Search) - Busca en profundidade
    // Explora o máis fondo posible antes de retroceder
    // Non garantiza o camiño máis curto, pero atopa un camiño se existe
    public static List<LabNode> DFS(LabNode start, LabNode end)
    {
        Stack<LabNode> toVisit = new Stack<LabNode>();
        List<LabNode> visited = new List<LabNode>();
        Dictionary<LabNode, LabNode> parent = new Dictionary<LabNode, LabNode>();

        toVisit.Push(start);
        parent[start] = null;

        while (toVisit.Count > 0)
        {
            LabNode currentNode = toVisit.Pop();

            // Se xa foi visitado, continúa
            if (visited.Contains(currentNode))
                continue;

            visited.Add(currentNode);

            // Se chegamos ao destino, reconstrúe o camiño
            if (currentNode.Equals(end))
            {
                List<LabNode> finalPath = new List<LabNode>();
                LabNode pathNode = currentNode;

                // Reconstrúe o camiño desde o final ata o inicio
                while (pathNode != null)
                {
                    finalPath.Add(pathNode);
                    parent.TryGetValue(pathNode, out pathNode);
                }

                finalPath.Reverse(); // Inverte para ter o camiño desde inicio a fin
                return finalPath;
            }

            // Explora os veciños
            foreach (LabNode neighbor in currentNode.Neighbors)
            {
                if (!visited.Contains(neighbor) && neighbor.Walkable)
                {
                    toVisit.Push(neighbor);
                    if (!parent.ContainsKey(neighbor))
                    {
                        parent[neighbor] = currentNode;
                    }
                }
            }
        }

        // Se non se atopa camiño, retorna lista baleira
        return new List<LabNode>();
    }

    // Algoritmo DFS recursivo - Versión recursiva da busca en profundidade
    // Máis elegante pero pode ter problemas con pilas moi profundas
    public static bool DFSRecursive(LabNode start, LabNode end, 
                                   List<LabNode> visited, List<LabNode> path)
    {
        visited.Add(start);
        path.Add(start);

        // Se chegamos ao destino
        if (start.Equals(end))
        {
            return true;
        }

        // Explora os veciños
        foreach (LabNode neighbor in start.Neighbors)
        {
            if (!visited.Contains(neighbor) && neighbor.Walkable)
            {
                if (DFSRecursive(neighbor, end, visited, path))
                {
                    return true;
                }
            }
        }

        // Se non se atopa camiño por este nodo, retrocede
        path.RemoveAt(path.Count - 1);
        return false;
    }

    // Algoritmo de test - Camiño predefinido para probar o movemento
    // Útil para verificar que o sistema de movemento funciona correctamente

    public static List<LabNode> TestPath(LabNode start, LabNode end)
    {
        List<LabNode> finalPath = new List<LabNode>();

        if (start.Equals(end))
        {
            finalPath.Add(start);
        }
        else
        {
            finalPath.Add(start);
            finalPath.Add(end);
        }

        return finalPath;
    }
}