using System;
using Npgsql; // Driver de PostgreSQL

namespace SimuladorTrayectoriaDron
{
    public class DatabaseRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ---------------------------------------------------------
        // PARTE D: Persistencia con ADO.NET y Transacciones
        // ---------------------------------------------------------
        public void GuardarRecorrido(int n, int startX, int startY, (int x, int y)[] secuencia)
        {
            Console.WriteLine("\n[INFO] Guardando recorrido en PostgreSQL...");

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar Cabecera
                        int masterId;
                        string sqlMaster = "INSERT INTO tb_master_control (tamanio_n, despegue_x, despegue_y) VALUES (@n, @x, @y) RETURNING id;";
                        using (var cmdMaster = new NpgsqlCommand(sqlMaster, connection, transaction))
                        {
                            cmdMaster.Parameters.AddWithValue("@n", n);
                            cmdMaster.Parameters.AddWithValue("@x", startX);
                            cmdMaster.Parameters.AddWithValue("@y", startY);
                            
                            masterId = Convert.ToInt32(cmdMaster.ExecuteScalar());
                        }
                        Console.WriteLine($"  -> Cabecera guardada exitosamente (ID: {masterId})");

                        // 2. Insertar Detalle usando bucle WHILE 
                        int i = 0; 
                        while (i < secuencia.Length)
                        {
                            int pasoReal = i;
                            int pasoOfuscado;

                            // Regla de ofuscación
                            if (pasoReal % 2 == 0) pasoOfuscado = pasoReal * 2;
                            else pasoOfuscado = -pasoReal;

                            string sqlDet = "INSERT INTO tb_det_log (id_master, etiqueta_paso, coordenada_x, coordenada_y) VALUES (@id_master, @etiqueta, @cx, @cy);";
                            using (var cmdDet = new NpgsqlCommand(sqlDet, connection, transaction))
                            {
                                cmdDet.Parameters.AddWithValue("@id_master", masterId);
                                cmdDet.Parameters.AddWithValue("@etiqueta", pasoOfuscado);
                                cmdDet.Parameters.AddWithValue("@cx", secuencia[i].x);
                                cmdDet.Parameters.AddWithValue("@cy", secuencia[i].y);
                                
                                cmdDet.ExecuteNonQuery();
                            }
                            i++; 
                        }

                        transaction.Commit();
                        Console.WriteLine($"  -> Movimientos guardados correctamente (Total: {i})");

                        // 3. Llamar al reporte inverso (Parte E)
                        ReporteInverso(connection, masterId);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"\n[ERROR BD] Se revirtieron los cambios por un error: {ex.Message}");
                    }
                }
            }
        }

        // ---------------------------------------------------------
        // PARTE E.5: Reporte inverso con lectura reconstruida
        // ---------------------------------------------------------
        private void ReporteInverso(NpgsqlConnection connection, int masterId)
        {
            Console.WriteLine("\n------------------------------------------------");
            Console.WriteLine(" REPORTE INVERSO (ÚLTIMOS 5 PASOS)");
            Console.WriteLine("------------------------------------------------");

            string sqlRead = "SELECT id, etiqueta_paso, coordenada_x, coordenada_y FROM tb_det_log WHERE id_master = @masterId ORDER BY id DESC LIMIT 5;";
            using (var cmdRead = new NpgsqlCommand(sqlRead, connection))
            {
                cmdRead.Parameters.AddWithValue("@masterId", masterId);

                using (var reader = cmdRead.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idRegistro = reader.GetInt32(0);
                        int pasoGuardado = reader.GetInt32(1);
                        int cx = reader.GetInt32(2);
                        int cy = reader.GetInt32(3);

                        int pasoReal;
                        if (pasoGuardado < 0) pasoReal = -pasoGuardado; 
                        else pasoReal = pasoGuardado / 2; 

                        Console.WriteLine($"ID Reg: {idRegistro} | Coord: ({cx}, {cy}) | BD: {pasoGuardado} -> Paso Real Recuperado: {pasoReal}");
                    }
                }
            }
        }
    }
}