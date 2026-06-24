using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SimuladorTrayectoriaDron
{
    class Program
    {
        static void Main(string[] args)
        {
            // ---------------------------------------------------------
            // PARTE C: Cargar la configuración
            // ---------------------------------------------------------
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            string connectionString = config.GetConnectionString("DefaultConnection") ?? "";

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("ERROR: No se pudo cargar la cadena de conexión.");
                return;
            }

            Console.WriteLine("================================================");
            Console.WriteLine(" SIMULADOR DE TRAYECTORIA DE DRON AUTOMATIZADO ");
            Console.WriteLine("================================================");
            Console.WriteLine("\n[OK] Configuración cargada.\n");

            // ---------------------------------------------------------
            // PARTE E.2: Solicitar y validar datos
            // ---------------------------------------------------------
            int N = -1;
            while (N < 1)
            {
                N = LeerEnteroSeguro("Ingrese la dimensión del espacio N (entero >= 1): ");
                if (N < 1) Console.WriteLine("  -> Error: El tamaño del terreno debe ser al menos 1.\n");
            }

            int startX = -1;
            while (startX < 0 || startX >= N)
            {
                startX = LeerEnteroSeguro($"Ingrese la fila inicial X [0 a {N - 1}]: ");
                if (startX < 0 || startX >= N) Console.WriteLine($"  -> Error: La coordenada X debe estar entre 0 y {N - 1}.\n");
            }

            int startY = -1;
            while (startY < 0 || startY >= N)
            {
                startY = LeerEnteroSeguro($"Ingrese la columna inicial Y [0 a {N - 1}]: ");
                if (startY < 0 || startY >= N) Console.WriteLine($"  -> Error: La coordenada Y debe estar entre 0 y {N - 1}.\n");
            }

            Console.WriteLine($"\n[INFO] Iniciando simulación en terreno de {N}x{N} desde la posición ({startX}, {startY})...");

            // ---------------------------------------------------------
            // EJECUCIÓN DE LA SIMULACIÓN
            // ---------------------------------------------------------
            DronNavigator dron = new DronNavigator(N);
            bool exito = dron.IniciarSimulacion(startX, startY);

            Console.WriteLine("\n------------------------------------------------");
            Console.WriteLine(" MATRIZ DE RECORRIDO DEL DRON");
            Console.WriteLine("------------------------------------------------\n");

            // Dibujar matriz usando while
            int fila = 0;
            while (fila < N)
            {
                int col = 0;
                while (col < N)
                {
                    if (dron.Tablero[fila, col] == -1) Console.Write(". \t");
                    else Console.Write($"{dron.Tablero[fila, col]} \t");
                    col++;
                }
                Console.WriteLine("\n");
                fila++;
            }

            if (exito)
            {
                Console.WriteLine($"[ÉXITO] Simulación terminada. Se cubrieron {dron.TotalAlcanzables} parcelas.");
                
                // Extraer la secuencia en orden usando while
                var secuencia = new (int x, int y)[dron.TotalAlcanzables];
                int f = 0;
                while (f < N)
                {
                    int c = 0;
                    while (c < N)
                    {
                        int paso = dron.Tablero[f, c];
                        if (paso != -1)
                        {
                            secuencia[paso] = (f, c);
                        }
                        c++;
                    }
                    f++;
                }

                // ---------------------------------------------------------
                // LLAMADA A LA CAPA DE DATOS
                // ---------------------------------------------------------
                DatabaseRepository repo = new DatabaseRepository(connectionString);
                repo.GuardarRecorrido(N, startX, startY, secuencia);
            }
            else
            {
                Console.WriteLine("[SIN SOLUCIÓN] El dron no encontró ruta para cubrir las parcelas sin repetir.");
            }
        }

        // ---------------------------------------------------------
        // MÉTODO AUXILIAR (Anti-Crasheo)
        // ---------------------------------------------------------
        static int LeerEnteroSeguro(string mensaje)
        {
            int resultado;
            while (true)
            {
                Console.Write(mensaje);
                string input = Console.ReadLine() ?? "";
                
                if (int.TryParse(input, out resultado)) return resultado;
                
                Console.WriteLine("  -> Error: Ingreso no válido. Por favor ingrese un número entero.");
            }
        }
    }
}