using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class GameManager : MonoBehaviour
{
	//Variable para controlar la ruta de la base de datos, constructor de la ruta, y el nombre de la base de datos
	string rutaDB;
	string strConexion;
	string DBFileName = "gameDB.sqlite";

	public GameObject prefabEnemigo;

	//Variable para trabajar con las conexiones
	IDbConnection dbConnection;
	//Para poder ejecutar comandos
	IDbCommand dbCommand;
	//Variable para leer
	IDataReader reader;

	// Start is called before the first frame update
	void Start()
	{
		AbrirDB();
		//resetearDB(); //Descomentar para recrear la base de datos
		spawnearEnemigos();
		cerrarDB();
	}

	//Método para abrir la base de datos
	void AbrirDB()
	{
		// Crear y abrir la conexión
		// Comprobar en que plataforma estamos
		// Si es PC mantenemos la ruta
		if (Application.platform == RuntimePlatform.WindowsEditor)
		{
			rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
		}
		//Si es Android
		else if (Application.platform == RuntimePlatform.Android)
		{
			rutaDB = Application.persistentDataPath + "/" + DBFileName;
			// Comprobar si el archivo se encuentra almecenado en persistant data
			if (!File.Exists(rutaDB))
			{
				// Almaceno el archivo en load db
				WWW loadDB = new WWW("jar;file://" + Application.dataPath + "!/assets/" + DBFileName);
				while (!loadDB.isDone)
				{

				}
				// Copio el archivo a persistant data
				File.WriteAllBytes(rutaDB, loadDB.bytes);
			}
		}

		strConexion = "URI=file:" + rutaDB;
		dbConnection = new SqliteConnection(strConexion);
		dbConnection.Open();
	}

	void resetearDB()
	{
		// Borrar todo lo previo
		dbCommand = dbConnection.CreateCommand();
		string sqlQuery = "PRAGMA writable_schema = 1; DELETE FROM sqlite_master; PRAGMA writable_schema = 0; VACUUM; PRAGMA integrity_check; ";
		dbCommand.CommandText = sqlQuery;
		dbCommand.ExecuteReader();

		// Recrear tablas
		dbCommand = dbConnection.CreateCommand();
		sqlQuery = "CREATE TABLE IF NOT EXISTS Enemigo (id INTEGER PRIMARY KEY, vida INTEGER, posicion_x INTEGER, posicion_y INTEGER )";
		dbCommand.CommandText = sqlQuery;
		reader = dbCommand.ExecuteReader();

		dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = "INSERT INTO Enemigo (id, vida, posicion_x, posicion_y) VALUES (0, 1, 3, 3)";
		dbCommand.ExecuteNonQuery();
		dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = "INSERT INTO Enemigo (id, vida, posicion_x, posicion_y) VALUES (1, 2, -3, -3)";
		dbCommand.ExecuteNonQuery();
		dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = "INSERT INTO Enemigo (id, vida, posicion_x, posicion_y) VALUES (2, 4, -5, 3)";
		dbCommand.ExecuteNonQuery();
		dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = "INSERT INTO Enemigo (id, vida, posicion_x, posicion_y) VALUES (3, 5, 5, 3)";
		dbCommand.ExecuteNonQuery();
		dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = "INSERT INTO Enemigo (id, vida, posicion_x, posicion_y) VALUES (4, 2, 4, 0)";
		dbCommand.ExecuteNonQuery();
		dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = "INSERT INTO Enemigo (id, vida, posicion_x, posicion_y) VALUES (5, 3, 0, -3)";
		dbCommand.ExecuteNonQuery();
		dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = "INSERT INTO Enemigo (id, vida, posicion_x, posicion_y) VALUES (6, 1, 3, -2)";
		dbCommand.ExecuteNonQuery();
	}

	void spawnearEnemigos()
	{
		string query = "SELECT * FROM Enemigo";
		dbCommand = dbConnection.CreateCommand();
		dbCommand.CommandText = query;
		reader = dbCommand.ExecuteReader();
		while (reader.Read())
		{
			GameObject enemigo = Instantiate(prefabEnemigo, new Vector3(reader.GetFloat(2), reader.GetFloat(3), 0), Quaternion.identity);
			enemigo.GetComponent<EnemyController>().vida = reader.GetInt32(1);
		}
	}

	void cerrarDB()
	{
		// Cerrar las conexiones
		reader.Close();
		reader = null;
		dbCommand.Dispose();
		dbCommand = null;
		dbConnection.Close();
		dbConnection = null;
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
