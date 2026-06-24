-- Script DDL para el Simulador de Dron
-- Creación de la tabla cabecera
CREATE TABLE tb_master_control (
    id SERIAL PRIMARY KEY, 
    fecha_ejecucion TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
    tamanio_n INTEGER NOT NULL, 
    despegue_x INTEGER NOT NULL, 
    despegue_y INTEGER NOT NULL 
);

-- Creación de la tabla detalle
CREATE TABLE tb_det_log (
    id SERIAL PRIMARY KEY, 
    id_master INTEGER NOT NULL, 
    etiqueta_paso INTEGER NOT NULL, 
    coordenada_x INTEGER NOT NULL, 
    coordenada_y INTEGER NOT NULL, 
    CONSTRAINT fk_master FOREIGN KEY (id_master) 
        REFERENCES tb_master_control (id) ON DELETE CASCADE
);