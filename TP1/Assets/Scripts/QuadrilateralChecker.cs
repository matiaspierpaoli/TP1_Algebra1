using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics.Tracing;

[System.Serializable]
public struct Vector
{
    public Vector2 Start;
    public Vector2 End;
    public int id;
}

[System.Serializable]
public class NamedQuadrilateral
{
    public Vector[] Vectors;
    public string Name { get; set; }
    public bool IsQuadrilateral { get; set; }
    public Color DrawColor { get; set; }
}

public class QuadrilateralChecker : MonoBehaviour
{
    [SerializeField] private NamedQuadrilateral customExample;
    private NamedQuadrilateral[] quadrilateralExamples;

    public bool shouldDrawCustom = false;
    public bool shouldDrawExamples = false;

    private void Start()
    {
        InitializeExampleQuads();
        QuadCheck();
    }

    private void InitializeExampleQuads()
    {
        // Definir vectores de ejemplo para cuadriláteros correctos e incorrectos
        NamedQuadrilateral correctExample1 = new NamedQuadrilateral
        {
            Name = "Correct Example 1",
            Vectors = new Vector[4]
            {
                new Vector { Start = new Vector2(0, 0), End = new Vector2(1, 0) },
                new Vector { Start = new Vector2(1, 0), End = new Vector2(1, 1) },
                new Vector { Start = new Vector2(1, 1), End = new Vector2(0, 1) },
                new Vector { Start = new Vector2(0, 1), End = new Vector2(0, 0) }
            }
        };

        NamedQuadrilateral correctExample2 = new NamedQuadrilateral
        {
            Name = "Correct Example 2",
            Vectors = new Vector[4]
            {
                new Vector { Start = new Vector2(2, -1), End = new Vector2(3, 0) },
                new Vector { Start = new Vector2(3, 0), End = new Vector2(3, 1) },
                new Vector { Start = new Vector2(3, 1), End = new Vector2(2, 1) },
                new Vector { Start = new Vector2(2, -1), End = new Vector2(2, 1) }
            }
        };

        NamedQuadrilateral correctExample3 = new NamedQuadrilateral
        {
            Name = "Correct Example 3",
            Vectors = new Vector[4]
            {
                new Vector { Start = new Vector2(5, -1), End = new Vector2(6, 0) },
                new Vector { Start = new Vector2(6, 0), End = new Vector2(8, 1) },
                new Vector { Start = new Vector2(8, 1), End = new Vector2(5, 1) },
                new Vector { Start = new Vector2(5, -1), End = new Vector2(5, 1) }
            }
        };

        NamedQuadrilateral correctExample4 = new NamedQuadrilateral
        {
            Name = "Correct Example 4",
            Vectors = new Vector[4]
            {
                new Vector { Start = new Vector2(20, 0), End = new Vector2(22, 0) },
                new Vector { Start = new Vector2(22, 0), End = new Vector2(21, 2) },
                new Vector { Start = new Vector2(21, 2), End = new Vector2(20, 1) },
                new Vector { Start = new Vector2(20, 1), End = new Vector2(20, 0) }
            }
        };

        NamedQuadrilateral correctExample5 = new NamedQuadrilateral
        {
            Name = "Correct Example 5",
            Vectors = new Vector[4]
            {
                new Vector { Start = new Vector2(25, 0), End = new Vector2(27, 2) },
                new Vector { Start = new Vector2(27, 2), End = new Vector2(29, 0) },
                new Vector { Start = new Vector2(29, 0), End = new Vector2(27, -2) },
                new Vector { Start = new Vector2(27, -2), End = new Vector2(25, 0) }
            }
        };

        NamedQuadrilateral incorrectExample1 = new NamedQuadrilateral
        {
            Name = "Incorrect Example 1",
            Vectors = new Vector[4]
            {
                new Vector { Start = new Vector2(10, 0), End = new Vector2(11, 0) },
                new Vector { Start = new Vector2(11, 0), End = new Vector2(10, 2) },
                new Vector { Start = new Vector2(10, 2), End = new Vector2(11, 2) },
                new Vector { Start = new Vector2(11, 2), End = new Vector2(10, 0) }
            }
        };

        NamedQuadrilateral incorrectExample2 = new NamedQuadrilateral
        {
            Name = "Incorrect Example 2",
            Vectors = new Vector[4]
            {
                new Vector { Start = new Vector2(15, 0), End = new Vector2(16, 2) },
                new Vector { Start = new Vector2(16, 2), End = new Vector2(15, 2) },
                new Vector { Start = new Vector2(15, 2), End = new Vector2(15, 0) },
                new Vector { Start = new Vector2(16, 2), End = new Vector2(16, 0) }
            }
        };

        // Inicializar el arreglo y agregar los vectores
        quadrilateralExamples = new NamedQuadrilateral[]
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

    private void QuadCheck()
    {
        // Verificar los ejemplos y guardar el resultado
        foreach (NamedQuadrilateral namedQuadrilateral in quadrilateralExamples)
        {
            namedQuadrilateral.IsQuadrilateral = AreVectorsFormingQuadrilateral(namedQuadrilateral);
            namedQuadrilateral.DrawColor = (namedQuadrilateral.IsQuadrilateral ? Color.green : Color.red);
            Debug.Log($"{namedQuadrilateral.Name}: {(namedQuadrilateral.IsQuadrilateral ? "Es un cuadrilátero válido" : "No es un cuadrilátero válido")}");
        }

        // Verificar el ejemplo personalizado
        if (customExample.Vectors != null && customExample.Vectors.Length > 0)
        {
            bool isCustomQuadrilateral = AreVectorsFormingQuadrilateral(new NamedQuadrilateral { Name = "Custom Example", Vectors = customExample.Vectors });
            customExample.DrawColor = (isCustomQuadrilateral ? Color.green : Color.red);
            Debug.Log($"Custom Example: {(isCustomQuadrilateral ? "Es un cuadrilátero válido" : "No es un cuadrilátero válido")}");
        }
    }

    private bool DoLinesIntersect(Vector line1, Vector line2)
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

    private float CalculateArea(Vector[] vectors)
    {
        // Utilizar la fórmula de la suma de áreas de los triángulos
        Vector2[] points = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            points[i] = vectors[i].Start;
        }

        float area = Mathf.Abs(
            (points[0].x * (points[1].y - points[3].y) +
             points[1].x * (points[2].y - points[0].y) +
             points[2].x * (points[3].y - points[1].y) +
             points[3].x * (points[0].y - points[2].y)) / 2f
        );

        return area;
    }

    private bool AreVectorsFormingQuadrilateral(NamedQuadrilateral namedQuadrilateral)
    {
        var vectors = namedQuadrilateral.Vectors;

        // Lista para almacenar intersecciones
        List<Vector2> intersections = new List<Vector2>();

        // Verificar si hay líneas que se cruzan
        for (int i = 0; i < vectors.Length; i++)
        {
            for (int j = i + 1; j < vectors.Length; j++)
            {
                {
                    if (DoLinesIntersect(vectors[i], vectors[j], out Vector2 intersection))
                    {
                        if (!intersections.Contains(intersection))
                            intersections.Add(intersection); 
                    }
                }
            }
        }

        if (intersections.Count < 4)
        {
            return false;
        }

        int counter = 0;
        int otherCounter = 0;

        for (int i = 0; i < vectors.Length; i++)
        {
            for (int j = i + 1; j < vectors.Length; j++)
            {
                if (otherCounter != 4)
                    otherCounter = 0;

                if (DoLinesIntersect(vectors[i], vectors[j]))
                {
                    for (int k = 0; k < vectors.Length; k++)
                    {
                        if (k != i)
                        {
                            if (DoLinesIntersect(vectors[i], vectors[k]))
                            {
                                counter++;
                                otherCounter++;
                            }
                        }
                    }

                    for (int k = 0; k < vectors.Length; k++)
                    {
                        if (k != j)
                        {
                            if (DoLinesIntersect(vectors[j], vectors[k]))
                            {
                                counter++;
                                otherCounter++;
                            }
                        }
                    }
                }                
            }
        }

        if (counter != 16)
        {
            return false;
        }

        if (otherCounter == 4)
        {
            return false;
        }


        float area = CalculateArea(vectors);
        Debug.Log($"Nombre del cuadrilátero: {namedQuadrilateral.Name}, Área del cuadrilátero: {area}");

        return true; // Cuadrilátero válido si todas las verificaciones pasan
    }

    private bool DoLinesIntersect(Vector line1, Vector line2, out Vector2 intersection)
    {
        Vector2 p1 = line1.Start;
        Vector2 p2 = line1.End;
        Vector2 p3 = line2.Start;
        Vector2 p4 = line2.End;

        Vector2 d1 = p2 - p1;
        Vector2 d2 = p4 - p3;

        float determinant = d1.x * d2.y - d1.y * d2.x;

        if (determinant == 0)
        {
            intersection = Vector2.zero;
            return false; // Las líneas son paralelas o colineales
        }

        float t = ((p3.x - p1.x) * d2.y - (p3.y - p1.y) * d2.x) / determinant;
        float u = ((p3.x - p1.x) * d1.y - (p3.y - p1.y) * d1.x) / determinant;

        // Verificar si la intersección está dentro de los segmentos de línea
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersection = p1 + t * d1;
            return true;
        }

        intersection = Vector2.zero;
        return false;
    }

    private void OnDrawGizmos()
    {
        if (shouldDrawCustom)
        {
            if (customExample.Vectors != null && customExample.Vectors.Length > 0)
            {
                Gizmos.color = customExample.DrawColor;

                foreach (var vector in customExample.Vectors)
                {
                    Gizmos.DrawLine(vector.Start, vector.End);
                }
            }
        }

        if (shouldDrawExamples)
        {
            if (quadrilateralExamples != null)
            {
                foreach (NamedQuadrilateral namedQuadrilateral in quadrilateralExamples)
                {
                    if (namedQuadrilateral.Vectors != null && namedQuadrilateral.Vectors.Length > 0)
                    {
                        Gizmos.color = namedQuadrilateral.DrawColor;

                        foreach (var vector in namedQuadrilateral.Vectors)
                        {
                            Gizmos.DrawLine(vector.Start, vector.End);
                        }
                    }
                }
            }
        }
    }
}
