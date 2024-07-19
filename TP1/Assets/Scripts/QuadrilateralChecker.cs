using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Vector
{
    public Vector2 Start;
    public Vector2 End;
}

public class QuadrilateralChecker : MonoBehaviour
{
    private Vector[][] vectorsExamples;

    public Vector[] customExample;

    public bool shouldDrawCustom = false;
    public bool shouldDrawExamples = false;


    private void Start()
    {
        // Definir vectores de ejemplo para cuadriláteros correctos e incorrectos
        Vector[] correctExample1 = new Vector[4]
        {
            new Vector { Start = new Vector2(0, 0), End = new Vector2(1, 0) },
            new Vector { Start = new Vector2(1, 0), End = new Vector2(1, 1) },
            new Vector { Start = new Vector2(1, 1), End = new Vector2(0, 1) },
            new Vector { Start = new Vector2(0, 1), End = new Vector2(0, 0) }
        };

        Vector[] correctExample2 = new Vector[4]
        {
            new Vector { Start = new Vector2(2, -1), End = new Vector2(3, 0) },
            new Vector { Start = new Vector2(3, 0), End = new Vector2(3, 1) },
            new Vector { Start = new Vector2(3, 1), End = new Vector2(2, 1) },
            new Vector { Start = new Vector2(2, -1), End = new Vector2(2, 1) }
        }; 
        
        Vector[] correctExample3 = new Vector[4]
        {
            new Vector { Start = new Vector2(5, -1), End = new Vector2(6, 0) },
            new Vector { Start = new Vector2(6, 0), End = new Vector2(8, 1) },
            new Vector { Start = new Vector2(8, 1), End = new Vector2(5, 1) },
            new Vector { Start = new Vector2(5, -1), End = new Vector2(5, 1) }
        };

        Vector[] correctExample4 = new Vector[4]
        {
            new Vector { Start = new Vector2(20, 0), End = new Vector2(22, 0) },
            new Vector { Start = new Vector2(22, 0), End = new Vector2(21, 2) },
            new Vector { Start = new Vector2(21, 2), End = new Vector2(20, 1) },
            new Vector { Start = new Vector2(20, 1), End = new Vector2(20, 0) }
        };
        
        Vector[] correctExample5 = new Vector[4]
        {
            new Vector { Start = new Vector2(25, 0), End = new Vector2(27, 2) },
            new Vector { Start = new Vector2(27, 2), End = new Vector2(29, 0) },
            new Vector { Start = new Vector2(29, 0), End = new Vector2(27, -2) },
            new Vector { Start = new Vector2(27, -2), End = new Vector2(25, 0) }
        };

        Vector[] incorrectExample1 = new Vector[4]
        {
            new Vector { Start = new Vector2(10, 0), End = new Vector2(11, 0) },
            new Vector { Start = new Vector2(11, 0), End = new Vector2(10, 2) },
            new Vector { Start = new Vector2(10, 2), End = new Vector2(11, 2) },
            new Vector { Start = new Vector2(11, 2), End = new Vector2(10, 0) }
        };

        Vector[] incorrectExample2 = new Vector[4]
        {
            new Vector { Start = new Vector2(15, 0), End = new Vector2(16, 2) },
            new Vector { Start = new Vector2(16, 2), End = new Vector2(15, 2) },
            new Vector { Start = new Vector2(15, 2), End = new Vector2(15, 0) },
            new Vector { Start = new Vector2(16, 2), End = new Vector2(16, 0) }
        };

        // Inicializar el arreglo y agregar los vectores
        vectorsExamples = new Vector[][]
        {
            correctExample1,
            correctExample2,
            correctExample3,
            correctExample4,
            correctExample5,
            incorrectExample1,
            incorrectExample2
        };
    }

    bool AreVectorsFormingQuadrilateral(Vector[] vectors)
    {
        // Utilizar un diccionario para contar las ocurrencias de cada punto final
        Dictionary<Vector2, int> pointCount = new Dictionary<Vector2, int>();

        foreach (var vector in vectors)
        {
            if (pointCount.ContainsKey(vector.Start))
                pointCount[vector.Start]++;
            else
                pointCount[vector.Start] = 1;

            if (pointCount.ContainsKey(vector.End))
                pointCount[vector.End]++;
            else
                pointCount[vector.End] = 1;
        }

        // Un cuadrilátero válido debe tener exactamente 4 puntos, cada uno apareciendo dos veces
        if (pointCount.Count != 4)
            return false;

        foreach (var count in pointCount.Values)
        {
            if (count != 2)
                return false;
        }

        // Verificar si hay líneas que se cruzan, ignorando segmentos que comparten puntos finales
        for (int i = 0; i < vectors.Length; i++)
        {
            for (int j = i + 1; j < vectors.Length; j++)
            {
                // Asegurarse de que las líneas no compartan puntos finales
                if (vectors[i].Start != vectors[j].Start && vectors[i].Start != vectors[j].End &&
                    vectors[i].End != vectors[j].Start && vectors[i].End != vectors[j].End)
                {
                    if (DoLinesIntersect(vectors[i], vectors[j]))
                    {
                        return false; // Se encontró una intersección inválida
                    }
                }
            }
        }

        return true; // Cuadrilátero válido si todas las verificaciones pasan
    }

    bool DoLinesIntersect(Vector line1, Vector line2)
    {
        Vector2 p1 = line1.Start;
        Vector2 p2 = line1.End;
        Vector2 p3 = line2.Start;
        Vector2 p4 = line2.End;

        Vector2 d1 = p2 - p1;
        Vector2 d2 = p4 - p3;

        float determinant = d1.x * d2.y - d1.y * d2.x;

        if (determinant == 0)
            return false; // Las líneas son paralelas o colineales

        float t = ((p3.x - p1.x) * d2.y - (p3.y - p1.y) * d2.x) / determinant;
        float u = ((p3.x - p1.x) * d1.y - (p3.y - p1.y) * d1.x) / determinant;

        // Verificar si la intersección está dentro de los segmentos de línea
        return (t >= 0 && t <= 1 && u >= 0 && u <= 1);
    }

    void OnDrawGizmos()
    {
        if (shouldDrawCustom)
        {
            if (customExample != null && customExample.Length > 0)
            {
                // Verificar si los vectores forman un cuadrilátero válido
                Gizmos.color = AreVectorsFormingQuadrilateral(customExample) ? Color.green : Color.red;

                foreach (var vector in customExample)
                {
                    Gizmos.DrawLine(vector.Start, vector.End);
                }
            }
        }

        if (shouldDrawExamples)
        {
            if (vectorsExamples != null)
            {
                foreach (Vector[] vectorArray in vectorsExamples)
                {
                    if (vectorArray != null && vectorArray.Length > 0)
                    {
                        // Verificar si los vectores forman un cuadrilátero válido
                        Gizmos.color = AreVectorsFormingQuadrilateral(vectorArray) ? Color.green : Color.red;

                        foreach (var vector in vectorArray)
                        {
                            Gizmos.DrawLine(vector.Start, vector.End);
                        }
                    }
                }
            }
        }

    }
}
