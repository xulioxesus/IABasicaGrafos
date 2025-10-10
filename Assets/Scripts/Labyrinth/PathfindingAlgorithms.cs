using System.Collections.Generic;

/*
 * CLASE PATHFINDINGALGORITHMS
 * ===========================
 * 
 * Esta clase contén implementacións dos algoritmos de pathfinding máis comúns
 * para a navegación en grafos e grillas. Todos os métodos son estáticos para
 * facilitar o seu uso desde calquera parte do código.
 * 
 * ALGORITMOS IMPLEMENTADOS:
 * 
 * 1. BFS (Breadth-First Search) - Busca en anchura
 *    - Garantiza o camiño máis curto
 *    - Explora nivel por nivel
 *    - Complexidade: O(V + E)
 * 
 * 2. DFS (Depth-First Search) - Busca en profundidade
 *    - Non garantiza o camiño máis curto
 *    - Explora o máis fondo posible primeiro
 *    - Complexidade: O(V + E)
 * 
 * 3. DFS Recursivo - Versión recursiva do DFS
 *    - Máis elegante pero limitado pola pila de chamadas
 *    - Útil para grafos pequenos
 * 
 * 4. TestPath - Algoritmo de proba para validar o sistema
 */
public static class PathfindingAlgorithms
{
    /*
     * ALGORITMO BFS (BREADTH-FIRST SEARCH) - BUSCA EN ANCHURA
     * ========================================================
     * 
     * PRINCIPIO: Explora todos os nodos á mesma distancia do inicio antes de
     * pasar aos nodos máis distantes. Usa unha Queue (FIFO) para manter a orde.
     * 
     * CARACTERÍSTICAS:
     * - Garantiza o camiño máis curto en grafos non ponderados
     * - Explora de forma sistemática nivel por nivel
     * - Uso de memoria alto (almacena moitos nodos simultaneamente)
     * 
     * FUNCIONAMENTO:
     * 1. Engade o nodo inicial á queue
     * 2. Mentres haxa nodos na queue:
     *    a) Extrae o primeiro nodo (FIFO)
     *    b) Se é o destino, reconstrúe o camiño
     *    c) Se non, engade todos os seus veciños á queue
     * 3. Reconstrúe o camiño usando as profundidades gardadas
     * 
     * COMPLEXIDADE: O(V + E) onde V = nodos, E = conexións
     * ESPAZO: O(V) para almacenar nodos visitados e queue
     */
    public static List<LabNode> BFS(LabNode start, LabNode end)
    {
        Queue<LabNode> toVisit = new Queue<LabNode>();  // Queue FIFO para BFS
        List<LabNode> visited = new List<LabNode>();     // Nodos xa explorados

        LabNode currentNode = start;
        currentNode.Depth = 0;           // O inicio ten profundidade 0
        toVisit.Enqueue(currentNode);    // Comeza co nodo inicial

        List<LabNode> finalPath = new List<LabNode>();

        while (toVisit.Count > 0)
        {
            currentNode = toVisit.Dequeue();  // Extrae o primeiro (FIFO)

            // Evita procesar o mesmo nodo varias veces
            if (visited.Contains(currentNode))
                continue;

            visited.Add(currentNode);

            // Comprobación de obxectivo alcanzado
            if (currentNode.Equals(end))
            {
                // Reconstrúe o camiño seguindo as profundidades decrecentes
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
                finalPath.Reverse();  // Inverte para ter orde inicio→destino
                break;
            }
            else
            {
                // Explora todos os veciños camiñables non visitados
                foreach (LabNode n in currentNode.Neighbors)
                {
                    if (!visited.Contains(n) && n.Walkable)
                    {
                        n.Depth = currentNode.Depth + 1;  // Profundidade = pai + 1
                        toVisit.Enqueue(n);                // Engade á queue
                    }
                }
            }
        }

        return finalPath;
    }

    /*
     * ALGORITMO DFS (DEPTH-FIRST SEARCH) - BUSCA EN PROFUNDIDADE
     * ===========================================================
     * 
     * PRINCIPIO: Explora o máis fondo posible nunha dirección antes de
     * retroceder e probar outras direccións. Usa unha Stack (LIFO).
     * 
     * CARACTERÍSTICAS:
     * - NON garantiza o camiño máis curto
     * - Atopa un camiño rapidamente se existe
     * - Uso de memoria baixo (só almacena o camiño actual)
     * - Pode quedarse atrapado en camiños longos
     * 
     * FUNCIONAMENTO:
     * 1. Engade o nodo inicial á stack
     * 2. Mentres haxa nodos na stack:
     *    a) Extrae o último nodo engadido (LIFO)
     *    b) Se é o destino, reconstrúe o camiño
     *    c) Se non, engade todos os seus veciños á stack
     * 3. Reconstrúe o camiño usando o dicionario de pais
     * 
     * DIFERENZAS CON BFS:
     * - BFS usa Queue (FIFO) → camiño máis curto
     * - DFS usa Stack (LIFO) → explora en profundidade
     * 
     * COMPLEXIDADE: O(V + E) no peor caso
     * ESPAZO: O(V) para almacenar visitados e pais
     */
    public static List<LabNode> DFS(LabNode start, LabNode end)
    {
        Stack<LabNode> toVisit = new Stack<LabNode>();              // Stack LIFO para DFS
        List<LabNode> visited = new List<LabNode>();                // Nodos xa explorados
        Dictionary<LabNode, LabNode> parent = new Dictionary<LabNode, LabNode>(); // Para reconstrución

        toVisit.Push(start);    // Comeza co nodo inicial
        parent[start] = null;   // O inicio non ten pai

        while (toVisit.Count > 0)
        {
            LabNode currentNode = toVisit.Pop();  // Extrae o último (LIFO)

            // Evita procesar o mesmo nodo varias veces
            if (visited.Contains(currentNode))
                continue;

            visited.Add(currentNode);

            // Comprobación de obxectivo alcanzado
            if (currentNode.Equals(end))
            {
                List<LabNode> finalPath = new List<LabNode>();
                LabNode pathNode = currentNode;

                // Reconstrúe o camiño seguindo os pais desde destino→inicio
                while (pathNode != null)
                {
                    finalPath.Add(pathNode);
                    parent.TryGetValue(pathNode, out pathNode);  // Vai ao pai
                }

                finalPath.Reverse(); // Inverte para ter orde inicio→destino
                return finalPath;
            }

            // Explora todos os veciños camiñables non visitados
            foreach (LabNode neighbor in currentNode.Neighbors)
            {
                if (!visited.Contains(neighbor) && neighbor.Walkable)
                {
                    toVisit.Push(neighbor);                    // Engade á stack
                    if (!parent.ContainsKey(neighbor))         // Establece parentesco
                    {
                        parent[neighbor] = currentNode;
                    }
                }
            }
        }

        // Se non se atopa camiño, retorna lista baleira
        return new List<LabNode>();
    }

    /*
     * ALGORITMO DFS RECURSIVO - VERSIÓN RECURSIVA DA BUSCA EN PROFUNDIDADE
     * =====================================================================
     * 
     * PRINCIPIO: Mesma lóxica que o DFS iterativo pero usando recursión
     * en lugar dunha stack explícita. Usa a pila de chamadas do sistema.
     * 
     * CARACTERÍSTICAS:
     * - Código máis limpo e intuitivo
     * - Comportamento idéntico ao DFS iterativo
     * - LIMITACIÓN: Pode causar stack overflow en grafos moi profundos
     * - Recomendado só para grafos pequenos ou medianos
     * 
     * FUNCIONAMENTO:
     * 1. Marca o nodo actual como visitado
     * 2. Engádeo ao camiño actual
     * 3. Se é o destino, retorna éxito
     * 4. Para cada veciño non visitado:
     *    a) Chama recursivamente á función
     *    b) Se atopa camiño, retorna éxito
     * 5. Se ningún veciño atopa camiño, retrocede (backtrack)
     * 
     * BACKTRACKING: Se un camiño non leva ao destino, elimina o nodo
     * actual do camiño e proba outras direccións.
     * 
     * COMPLEXIDADE: O(V + E) no peor caso
     * ESPAZO: O(V) para recursión + visitados + camiño
     */
    public static bool DFSRecursive(LabNode start, LabNode end, 
                                   List<LabNode> visited, List<LabNode> path)
    {
        visited.Add(start);     // Marca como visitado
        path.Add(start);        // Engade ao camiño actual

        // Caso base: chegamos ao destino
        if (start.Equals(end))
        {
            return true;        // Éxito! Camiño atopado
        }

        // Explora recursivamente todos os veciños
        foreach (LabNode neighbor in start.Neighbors)
        {
            if (!visited.Contains(neighbor) && neighbor.Walkable)
            {
                // Chamada recursiva: explora este veciño
                if (DFSRecursive(neighbor, end, visited, path))
                {
                    return true;    // Se atopa camiño, propaga o éxito
                }
            }
        }

        // BACKTRACKING: se ningún veciño leva ao destino,
        // elimina este nodo do camiño e retrocede
        path.RemoveAt(path.Count - 1);
        return false;               // Este camiño non funciona
    }

    /*
     * ALGORITMO TESTPATH - CAMIÑO PREDEFINIDO PARA PROBAS
     * ===================================================
     * 
     * PROPÓSITO: Proporciona un camiño fixo e coñecido para validar que
     * o sistema de movemento funciona correctamente sen depender da
     * lóxica complexa dos algoritmos de pathfinding.
     * 
     * USOS:
     * - Depuración do sistema de movemento
     * - Probas de rendemento sen sobrecarga de cálculo
     * - Validación de que os waypoints visuais funcionan
     * - Punto de referencia para comparar outros algoritmos
     * 
     * FUNCIONAMENTO:
     * - Crea un camiño simple e directo
     * - Inclúe o nodo de inicio e destino
     * - Engade algúns nodos intermedios predefinidos
     * 
     * NOTA: Este non é un algoritmo real de pathfinding, senón unha
     * ferramenta de desenvolvemento e depuración.
     */
    public static List<LabNode> TestPath(LabNode start, LabNode end)
    {
        List<LabNode> finalPath = new List<LabNode>
        {
            start,                      // Sempre comeza no inicio
            new LabNode(0, 1),         // Nodo intermedio de exemplo
            end                         // Sempre termina no destino
        };

        return finalPath;
    }
}