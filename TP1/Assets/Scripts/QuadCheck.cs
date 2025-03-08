using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class QuadCheck : MonoBehaviour
{
    [System.Serializable]
    public struct Segment
    {
        public Vector2 start;
        public Vector2 end;
    }

    public Segment[] segments = new Segment[4];
    public string figureName; // Nombre del objeto para la depuración
    private List<Vector2> intersections = new List<Vector2>(); // Lista de puntos de intersección
    private bool IsQuadFound = false; // Indica si se encontró un cuadrilátero válido

    void Start()
    {
        FindIntersections();
        CheckQuadrilateral();
    }

    // Encuentra todas las intersecciones entre los segmentos
    void FindIntersections()
    {
        intersections.Clear(); // Limpia la lista de intersecciones anteriores

        for (int i = 0; i < segments.Length; i++)
        {
            for (int j = i + 1; j < segments.Length; j++)
            {
                // Intenta encontrar la intersección entre dos segmentos
                if (TryGetIntersection(segments[i], segments[j], out Vector2 intersection))
                {
                    // Agrega la intersección si no está duplicada
                    if (!intersections.Contains(intersection))
                        intersections.Add(intersection);
                }
            }
        }
    }

    // Método para calcular la intersección entre dos segmentos
    bool TryGetIntersection(Segment seg1, Segment seg2, out Vector2 intersection)
    {
        Vector2 a = seg1.start, b = seg1.end;
        Vector2 c = seg2.start, d = seg2.end;

        // Se calcula la determinante
        // |bx - ax -(dx - cx)|
        // |by - ay -(dy - cy)|
        float denominator = (b.x - a.x) * (d.y - c.y) - (b.y - a.y) * (d.x - c.x);

        // Si es cero, los segmentos son paralelos o colineales
        if (Mathf.Approximately(denominator, 0))
        {
            intersection = Vector2.zero;
            return false;
        }

        // Calcula los parámetros t y u para determinar si hay intersección
        float t = ((c.x - a.x) * (d.y - c.y) - (c.y - a.y) * (d.x - c.x)) / denominator;
        float u = ((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x)) / denominator;

        // Si ambos parámetros están en el rango [0,1], hay intersección dentro de los segmentos
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersection = a + t * (b - a); // Calcula el punto de intersección
            return true;
        }

        intersection = Vector2.zero;
        return false;
    }

    // Verifica si se puede formar un cuadrilátero con las intersecciones encontradas
    void CheckQuadrilateral()
    {
        if (intersections.Count < 4)
        {
            Debug.Log("No hay suficientes intersecciones para formar un cuadrilátero en: " + figureName);
            return;
        }

        // Obtiene todas las combinaciones de 4 intersecciones posibles
        var validCombinations = GetValidCombinations(intersections, 4);

        foreach (var combination in validCombinations)
        {
            if (!AreSegmentsConnected(combination))
                continue;

            // Si los puntos forman un cuadrilátero válido, se guarda el resultado
            if (IsValidQuadrilateral(combination))
            {
                IsQuadFound = true;
                Debug.Log("Cuadrilátero válido llamado: " + figureName + " detectado");
                return;
            }
        }

        Debug.Log("No se encontró un cuadrilátero válido en: " + figureName);
    }

    // Metodo para obtener el resultado de todas las combinaciones posibles
    public List<List<T>> GetCombinations<T>(List<T> list, int length)
    {
        List<List<T>> result = new List<List<T>>();
        GenerateCombinations(list, length, new List<T>(), 0, result);
        return result;
    }

    // Genera todas las combinaciones posibles de n elementos de una lista
    private void GenerateCombinations<T>(List<T> list, int length, List<T> current, int index, List<List<T>> result)
    {
        if (current.Count == length)
        {
            result.Add(new List<T>(current)); // Se copia la lista actual a los resultados
            return;
        }

        for (int i = index; i < list.Count; i++)
        {
            current.Add(list[i]); // Agregar el elemento a la combinación actual
            GenerateCombinations(list, length, current, i + 1, result); // Llamada recursiva
            current.RemoveAt(current.Count - 1); // Retroceder en la combinación
        }
    }

    // Filtra combinaciones de 4 intersecciones que podrían formar un cuadrilátero válido
    List<List<Vector2>> GetValidCombinations(List<Vector2> intersections, int length)
    {
        List<List<Vector2>> validCombinations = new List<List<Vector2>>();
        List<string> validCombinationsHash = new List<string>();

        foreach (var combo in GetCombinations(intersections, length))
        {
            // Ordena las intersecciones en sentido horario o antihorario
            var orderedCombo = OrderIntersections(combo);

            //-> Hay una conversion a string para que pueda ser interpretada en .Contains()
            string comboString = string.Join(";", orderedCombo.Select(v => $"{v.x},{v.y}")); 

            if (!validCombinationsHash.Contains(comboString))
            {
                validCombinations.Add(orderedCombo);
                validCombinationsHash.Add(comboString);
            }
        }

        return validCombinations;
    }

    // Ordena los puntos en sentido horario para evitar duplicados
    List<Vector2> OrderIntersections(List<Vector2> combo)
    {
        //Suma todos los puntos en la lista (acc es el acumulador).
        // v representa cada punto en la lista.
        // Luego se divide por combo.Count, lo que da el punto medio del conjunto.
        Vector2 center = combo.Aggregate(Vector2.zero, (acc, v) => acc + v) / combo.Count;

        // Mathf.Atan2(v.y - center.y, v.x - center.x) calcula el ángulo del punto respecto al centroide.
        // OrderBy los ordena en sentido horario - OrderByDescending en antihorario
        return combo.OrderBy(v => Mathf.Atan2(v.y - center.y, v.x - center.x)).ToList();
    }

    // Verifica si los segmentos entre los puntos de una combinación están conectados
    bool AreSegmentsConnected(List<Vector2> combo)
    {
        int validConnections = 0;

        for (int i = 0; i < combo.Count; i++)
        {
            Vector2 p1 = combo[i];
            Vector2 p2 = combo[(i + 1) % combo.Count];

            // Comprueba si ambos puntos pertenecen al mismo segmento original
            bool validSegment = segments.Any(s => IsPointOnSegment(s, p1) && IsPointOnSegment(s, p2));

            if (validSegment)
            {
                validConnections++;
            }
        }

        // Un cuadrilátero válido debe tener exactamente 4 conexiones
        return validConnections == 4;
    }

    // Verifica si un punto pertenece a un segmento
    bool IsPointOnSegment(Segment s, Vector2 p)
    {
        float crossProduct = (p.y - s.start.y) * (s.end.x - s.start.x) - (p.x - s.start.x) * (s.end.y - s.start.y);

        if (Mathf.Abs(crossProduct) > 0.001f) // No está en la línea del segmento
            return false;

        float dotProduct = (p.x - s.start.x) * (s.end.x - s.start.x) + (p.y - s.start.y) * (s.end.y - s.start.y);

        if (dotProduct < 0 || dotProduct > (s.end - s.start).sqrMagnitude) // Está fuera de los límites del segmento
            return false;

        return true;
    }

    // Verifica si los puntos de la combinación forman un cuadrilátero válido
    bool IsValidQuadrilateral(List<Vector2> combo)
    {
        float totalAngle = 0f;

        for (int i = 0; i < combo.Count; i++)
        {
            Vector2 p1 = combo[i];
            Vector2 p2 = combo[(i + 1) % combo.Count];
            Vector2 p3 = combo[(i + 2) % combo.Count];

            Vector2 v1 = (p2 - p1).normalized;
            Vector2 v2 = (p3 - p2).normalized;

            float angle = Vector2.SignedAngle(v1, v2);

            if (Mathf.Abs(angle) == 180f || Mathf.Abs(angle) == 0f) // Las tres intersecciones recorren el mismo segmento
                return false;

            totalAngle += angle;
        }

        return Mathf.Abs(Mathf.Abs(totalAngle) - 360f) < 0.1f;
    }

    // Dibuja los segmentos en la escena para visualización
    void OnDrawGizmos()
    {
        Gizmos.color = IsQuadFound ? Color.green : Color.red;
        foreach (var segment in segments)
        {
            Gizmos.DrawLine(segment.start, segment.end);
        }
    }
}