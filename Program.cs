using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SimuladorTrayectoriaDron
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configuración inicial de la aplicación
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
            Console.WriteLine("\n[OK] Configuración cargada correctamente.\n");

            // Ingreso y validación de la dimensión del terreno
            int N = -1;
            while (N < 1)
            {
                N = LeerEnteroSeguro("Ingrese la dimensión del espacio N (entero >= 1): ");
                if (N < 1) Console.WriteLine("  -> Error: El tamaño del terreno debe ser al menos 1.\n");
            }

            // Ingreso y validación de la coordenada inicial X
            int startX = -1;
            while (startX < 0 || startX >= N)
            {
                startX = LeerEnteroSeguro($"Ingrese la fila inicial X [0 a {N - 1}]: ");
                if (startX < 0 || startX >= N) Console.WriteLine($"  -> Error: La coordenada X debe estar entre 0 y {N - 1}.\n");
            }

            // Ingreso y validación de la coordenada inicial Y
            int startY = -1;
            while (startY < 0 || startY >= N)
            {
                startY = LeerEnteroSeguro($"Ingrese la columna inicial Y [0 a {N - 1}]: ");
                if (startY < 0 || startY >= N) Console.WriteLine($"  -> Error: La coordenada Y debe estar entre 0 y {N - 1}.\n");
            }

            Console.WriteLine($"\n[INFO] Iniciando simulación en terreno de {N}x{N} desde la posición ({startX}, {startY})...");

            // Ejecución del algoritmo de simulación
            DronNavigator dron = new DronNavigator(N);
            bool exito = dron.IniciarSimulacion(startX, startY);

            Console.WriteLine("\n------------------------------------------------");
            Console.WriteLine(" MATRIZ DE RECORRIDO DEL DRON");
            Console.WriteLine("------------------------------------------------\n");

            // Impresión de la matriz resultante en consola
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
                
                // Extracción de la secuencia de pasos en orden para su guardado
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

                // Persistencia de los datos en la base de datos
                DatabaseRepository repo = new DatabaseRepository(connectionString);
                repo.GuardarRecorrido(N, startX, startY, secuencia);
            }
            else
            {
                Console.WriteLine("[SIN SOLUCIÓN] El dron no encontró ruta para cubrir las parcelas sin repetir.");
            }
        }

        /// <summary>
        /// Solicita un número entero al usuario y maneja posibles errores de formato,
        /// evitando el cierre inesperado de la aplicación.
        /// </summary>
        /// <param name="mensaje">El mensaje a mostrar en la consola.</param>
        /// <returns>El número entero ingresado validado.</returns>
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