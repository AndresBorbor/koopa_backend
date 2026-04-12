import os
import time
import glob
from sqlalchemy import create_engine, text
import pandas as pd

# Variables de entorno
DB_HOST = os.environ.get("DB_HOST", "db2")
DB_PORT = os.environ.get("DB_PORT", "50000")
DB_NAME = os.environ.get("DB_NAME", "KOOPA")
DB_USER = os.environ.get("DB_USER", "db2inst1")
DB_PASS = os.environ.get("DB_PASS", "admin123")

# Dónde buscar los archivos
DATA_DIR = os.environ.get("DATA_DIR", "/app/data")

def get_engine():
    # ibm_db_sa://<username>:<password>@<hostname>:<port>/<database>
    connection_string = f"ibm_db_sa://{DB_USER}:{DB_PASS}@{DB_HOST}:{DB_PORT}/{DB_NAME}"
    return create_engine(connection_string)

def run_schema(engine):
    schema_path = "schema.sql"
    if not os.path.exists(schema_path):
        print(f"No se encontró el archivo {schema_path}.")
        return

    print("Leyendo schema.sql...")
    with open(schema_path, "r", encoding="utf-8") as f:
        sql_script = f.read()

    statements = sql_script.split(";")
    
    with engine.connect() as conn:
        for statement in statements:
            stmt = statement.strip()
            if stmt:
                print(f"Ejecutando: {stmt[:60]}...")
                try:
                    conn.execute(text(stmt))
                    conn.commit()
                except Exception as e:
                    print(f"Advertencia/Error ejecutando sentencia: {e}")

    print("Schema.sql procesado.")

def run_views(engine):
    schema_path = "views.sql"
    if not os.path.exists(schema_path):
        print(f"No se encontró el archivo {schema_path}.")
        return

    print("Leyendo views.sql...")
    with open(schema_path, "r", encoding="utf-8") as f:
        sql_script = f.read()

    statements = sql_script.split(";")
    
    with engine.connect() as conn:
        for statement in statements:
            stmt = statement.strip()
            if stmt:
                print(f"Ejecutando: {stmt[:60]}...")
                try:
                    conn.execute(text(stmt))
                    conn.commit()
                except Exception as e:
                    print(f"Advertencia/Error ejecutando sentencia: {e}")

    print("views.sql procesado.")

def load_data(engine):
    # El orden de carga es importante para mantener integridad referencial
    # si se agregaran foreign keys. Ordenaremos basado en dependencias lógicas:
    tables_order = [
         # 1. Catálogos base (Sin FKs)
        "facultad",
        "tipo_credito",
        "tipo_requisito",
        "materia",
        "periodo",
        "usuario",
         
         # 2. Catálogos dependientes
        "carrera",       # Depende de facultad
         
         # 3. Tablas puente o de relación media
        "estudiante",    # Depende de carrera
        "materia_carrera", # Depende de materia y carrera
        "requisitos",    # Depende de materia y tipo_requisito
        
         # 4. Transaccionales / Hechos
        "graduados",     # Depende de carrera
        "ingresos",      # Depende de carrera
        "planificacion", # Depende de materia
        "inscripciones"  # Depende de estudiante, materia
    ]

    print(f"Buscando archivos CSV en {DATA_DIR}...")
    
    # Obtener el listado de archivos en formato en minúsculas para coincidencia en Linux (Case-Sensitive)
    archivos_reales = {}
    if os.path.exists(DATA_DIR):
        for f in os.listdir(DATA_DIR):
            if f.endswith('.csv') or f.endswith('.CSV'):
                archivos_reales[f.lower()] = f
    
    for table_name in tables_order:
        nombre_esperado = f"{table_name}.csv".lower()
        
        if nombre_esperado in archivos_reales:
            file_real_name = archivos_reales[nombre_esperado]
            file_path = os.path.join(DATA_DIR, file_real_name)
            
            print(f"\n--- Cargando {file_path} en tabla '{table_name.upper()}' ---")
            try:
                # Leer el CSV
                df = pd.read_csv(file_path, sep=';', encoding='utf-8') # Ajustar a ',' si es el caso
                
                # Si pandas no encontró el separador correcto (p.ej era coma en vez de punto y coma) verificamos si detectó 1 sola columna agrupada.
                if len(df.columns) == 1:
                   df = pd.read_csv(file_path, sep=',', encoding='utf-8')

                # Opcional: convertir nombres de columnas a mayúsculas para DB2
                df.columns = [c.upper() for c in df.columns]

                # Eliminar filas que estén completamente en blanco (vacías)
                df.dropna(how='all', inplace=True)

                # Reemplazar NaN por None para evitar problemas en SQL o base de datos relacionales
                df = df.where(pd.notnull(df), None)

                print(f"Procesando {len(df)} filas...")
                if not df.empty:
                    try:
                        # Insertar en base de datos. Usamos chunksize para archivos grandes.
                        df.to_sql(table_name.upper(), con=engine, schema='DBO', if_exists='append', index=False, chunksize=10000)
                        print(f"  -> {len(df)} filas insertadas en 'DBO.{table_name.upper()}'.")
                    except Exception as e:
                        print(f"  -> Error insertando datos en tabla 'DBO.{table_name.upper()}': {e}")
                else:
                    print(f"  -> El archivo '{file_path}' está vacío, se omite.")
            except Exception as e:
                print(f"Error abriendo o procesando el archivo {file_path}: {e}")
        else:
             print(f"Advertencia: No se encontró el archivo para la tabla '{table_name}' (se buscó {table_name}.csv). Pasando al siguiente...")
             

if __name__ == "__main__":
    print("======================================================")
    print(f"Iniciando Data-Loader hacia {DB_HOST}:{DB_PORT} / {DB_NAME}")
    print("======================================================")
    
    time.sleep(5)  
    
    engine = get_engine()
    
    print("\n[PASO 1] Ejecutar modelo SQL")
    run_schema(engine)
    
    print("\n[PASO 2] Cargar datos desde archivos CSV")
    load_data(engine)
    
    print("\n[PASO 3] Ejecutar vistas SQL")
    run_views(engine)
    
    print("\n======================================================")
    print("Proceso del Data-Loader finalizado.")
    print("======================================================")
