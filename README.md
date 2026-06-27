# 🚁 Simulador de Trayectoria de Dron Automatizado

Un sistema de simulación de vuelo y persistencia de datos desarrollado en **.NET 8** y **PostgreSQL**.

Este proyecto resuelve el recorrido de un dron sobre una cuadrícula dinámica utilizando algoritmos recursivos avanzados (**Backtracking**) y persiste los datos mediante **ADO.NET puro** con técnicas de ofuscación matemática.

---

# 🚀 Características Principales

## ✅ Algoritmia Avanzada (Backtracking)

Implementación de una heurística de **menor grado** para explorar caminos y optimizar los tiempos de respuesta evitando callejones sin salida.

## ✅ Movimiento Patrón 2x1

El dron se desplaza aplicando saltos en forma de **"L"**, similar al movimiento del caballo en ajedrez.

## ✅ Persistencia Segura (ADO.NET Síncrono)

Uso exclusivo del driver **Npgsql** administrando transacciones (`NpgsqlTransaction`) manualmente.

- Integridad referencial garantizada
- Sin uso de ORMs
- Persistencia directa mediante SQL

## ✅ Ofuscación y De-ofuscación de Datos

Los pasos del recorrido se transforman antes de almacenarse:

- Números pares → multiplicados ×2
- Números impares → almacenados como negativos

El sistema incorpora ingeniería inversa para reconstruir y visualizar los últimos **5 movimientos almacenados**.

## ✅ Validación Anti-Crash

Control dinámico de entradas del usuario para:

- Validar dimensiones del terreno
- Validar coordenadas iniciales
- Evitar errores por entradas inválidas

---

# 🛠️ Tecnologías Utilizadas

- **C#**
- **.NET 8**
- **PostgreSQL**
- **ADO.NET**
- **Npgsql**
- **Microsoft.Extensions.Configuration**
- **JSON (appsettings.json)**

---

# ⚙️ Requisitos Previos

Antes de ejecutar el proyecto instalar:

1. **SDK .NET 8.0** o superior.
2. **PostgreSQL** instalado o mediante Docker.
3. Crear una base de datos llamada:

```sql
parcial_dron
```

---

# 📦 Instalación y Configuración

## 1. Clonar el repositorio

```bash
git clone https://github.com/nicopazz/Parcial-Prog3-SimuladorDron.git

cd Parcial-Prog3-SimuladorDron
```

---

## 2. Crear la Base de Datos

Ejecutar el script:

```text
Scripts_DDL.sql
```

Se crearán las tablas:

- `tb_master_control`
- `tb_det_log`

Herramientas sugeridas:

- DBeaver
- pgAdmin
- psql

---

## 3. Configurar credenciales

Editar el archivo:

```text
appsettings.json
```

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=parcial_dron;Username=tu_usuario;Password=tu_password"
  }
}
```

---

## 4. Ejecutar el proyecto

```bash
dotnet run
```

---

# 🧠 Arquitectura y Lógica del Sistema

El proyecto aplica el principio **SRP (Single Responsibility Principle)**.

## Program.cs

Responsabilidades:

- Configuración del entorno
- Lectura segura de entradas
- Control del flujo principal
- Visualización de resultados

---

## DronNavigator.cs

Responsabilidades:

- Cálculo de parcelas alcanzables
- Implementación BFS
- Backtracking
- Construcción de trayectoria

---

## DatabaseRepository.cs

Responsabilidades:

- Gestión de conexión
- Persistencia del recorrido
- Manejo de transacciones
- Ofuscación de datos
- Recuperación mediante ingeniería inversa

---

# 📂 Estructura del Proyecto

```plaintext
SimuladorTrayectoriaDron/
│
├── Program.cs
├── DronNavigator.cs
├── DatabaseRepository.cs
├── appsettings.json
├── Scripts_DDL.sql
├── README.md
│
└── bin/
```

---

# ▶️ Ejemplo de Ejecución

```plaintext
================================================
 SIMULADOR DE TRAYECTORIA DE DRON AUTOMATIZADO
================================================

Ingrese la dimensión del espacio N: 5
Ingrese la fila inicial X: 0
Ingrese la columna inicial Y: 0

[INFO] Iniciando simulación...

MATRIZ DE RECORRIDO DEL DRON

0   11  4   17  2
5   16  1   12  7
10  21  6   3   18

[ÉXITO] Simulación terminada.
```

---

# 🔒 Consideraciones Técnicas

- Entrada validada contra errores
- Persistencia transaccional
- Separación por capas
- Diseño extensible
- Compatible con ejecución local y Docker

---

# 👨‍💻 Autor

**Christian Nicolás Paz Malizia**

Desarrollado como proyecto académico utilizando **.NET + PostgreSQL + algoritmos recursivos avanzados**.
