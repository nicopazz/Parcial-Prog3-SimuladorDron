using System;
using System.Collections.Generic;
using System.Linq;

namespace SimuladorTrayectoriaDron
{
    public class DronNavigator
    {
        private int N;
        public int[,] Tablero { get; private set; }
        public int TotalAlcanzables { get; private set; }

        // Definimos el patrón de movimiento 2x1 (en L)
        // Cada par (dx[i], dy[i]) representa uno de los 8 saltos posibles
        private readonly int[] dx = { -2, -2,  2, 2, -1, -1,  1, 1 };
        private readonly int[] dy = { -1,  1, -1, 1, -2,  2, -2, 2 };

        // Constructor: inicializa el tamaño y la matriz vacía
        public DronNavigator(int tamanioN)
        {
            N = tamanioN;
            Tablero = new int[N, N];
            
            // Llenamos la matriz con -1 (indica que la parcela está libre/sin pisar)
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Tablero[i, j] = -1;
                }
            }
        }

        // Método principal para arrancar el vuelo
        public bool IniciarSimulacion(int startX, int startY)
        {
            CalcularAlcanzables(startX, startY);
            
            // Registramos el despegue como el paso 0
            Tablero[startX, startY] = 0;
            
            // Arrancamos la recursividad buscando el paso 1
            return ResolverRecursivo(startX, startY, 1);
        }

        // Algoritmo recursivo con Backtracking
        private bool ResolverRecursivo(int x, int y, int pasoActual)
        {
            // Condición de éxito: Si ya dimos tantos pasos como parcelas alcanzables hay
            if (pasoActual == TotalAlcanzables)
                return true;

            var candidatos = new List<(int nx, int ny, int grado)>();

            // Evaluamos los 8 destinos posibles desde la posición actual
            for (int i = 0; i < 8; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (EsValido(nx, ny) && Tablero[nx, ny] == -1)
                {
                    candidatos.Add((nx, ny, CalcularGrado(nx, ny)));
                }
            }

            // Heurística obligatoria: ordenamos para probar primero los destinos de menor grado
            candidatos = candidatos.OrderBy(c => c.grado).ToList();

            foreach (var candidato in candidatos)
            {
                // Avanzamos marcando la parcela
                Tablero[candidato.nx, candidato.ny] = pasoActual;

                // Llamada recursiva al siguiente paso
                if (ResolverRecursivo(candidato.nx, candidato.ny, pasoActual + 1))
                    return true;

                // Backtracking: si falló, deshacemos el paso marcando la parcela como libre (-1)
                Tablero[candidato.nx, candidato.ny] = -1;
            }

            // Si exploramos todas las rutas y ninguna sirvió, retornamos false
            return false;
        }

        // Calcula cuántas salidas libres tiene una parcela candidata
        private int CalcularGrado(int x, int y)
        {
            int grado = 0;
            for (int i = 0; i < 8; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (EsValido(nx, ny) && Tablero[nx, ny] == -1)
                {
                    grado++;
                }
            }
            return grado;
        }

        // Cuenta cuántas parcelas son conectables mediante el patrón de salto (Ignora la regla de no repetir)
        private void CalcularAlcanzables(int startX, int startY)
        {
            bool[,] visitadoBFS = new bool[N, N];
            Queue<(int, int)> cola = new Queue<(int, int)>();
            
            cola.Enqueue((startX, startY));
            visitadoBFS[startX, startY] = true;
            int count = 0;

            while (cola.Count > 0)
            {
                var actual = cola.Dequeue();
                count++;

                for (int i = 0; i < 8; i++)
                {
                    int nx = actual.Item1 + dx[i];
                    int ny = actual.Item2 + dy[i];

                    if (EsValido(nx, ny) && !visitadoBFS[nx, ny])
                    {
                        visitadoBFS[nx, ny] = true;
                        cola.Enqueue((nx, ny));
                    }
                }
            }
            
            TotalAlcanzables = count;
        }

        // Valida que las coordenadas no caigan fuera del mapa
        private bool EsValido(int x, int y)
        {
            return x >= 0 && x < N && y >= 0 && y < N;
        }
    }
}