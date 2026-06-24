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

        private readonly int[] dx = { -2, -2,  2, 2, -1, -1,  1, 1 };
        private readonly int[] dy = { -1,  1, -1, 1, -2,  2, -2, 2 };

        public DronNavigator(int tamanioN)
        {
            N = tamanioN;
            Tablero = new int[N, N];
            
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Tablero[i, j] = -1;
                }
            }
        }

        public bool IniciarSimulacion(int startX, int startY)
        {
            CalcularAlcanzables(startX, startY);
            Tablero[startX, startY] = 0;
            return ResolverRecursivo(startX, startY, 1);
        }

        private bool ResolverRecursivo(int x, int y, int pasoActual)
        {
            if (pasoActual == TotalAlcanzables)
                return true;

            var candidatos = new List<(int nx, int ny, int grado)>();

            for (int i = 0; i < 8; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (EsValido(nx, ny) && Tablero[nx, ny] == -1)
                {
                    candidatos.Add((nx, ny, CalcularGrado(nx, ny)));
                }
            }

            candidatos = candidatos.OrderBy(c => c.grado).ToList();

            foreach (var candidato in candidatos)
            {
                Tablero[candidato.nx, candidato.ny] = pasoActual;

                if (ResolverRecursivo(candidato.nx, candidato.ny, pasoActual + 1))
                    return true;

                Tablero[candidato.nx, candidato.ny] = -1;
            }

            return false;
        }

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

        private bool EsValido(int x, int y)
        {
            return x >= 0 && x < N && y >= 0 && y < N;
        }
    }
}