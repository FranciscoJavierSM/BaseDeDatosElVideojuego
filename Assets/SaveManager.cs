using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	[Serializable]
	struct PlayerJson
	{
		public int municion;
		public Vector3 posicion;
	}
	[Serializable]
	struct EnemyJson
	{
		public int vida;
		public Vector3 posicion;
	}
	[Serializable]
	struct JsonObjToSave
	{
		public PlayerJson jugador;
		public EnemyJson[] enemigos;
	}

	string filePath;

	public GameObject enemyPrefab;

	// Start is called before the first frame update
	void Start()
    {
		filePath = Application.dataPath + "/Savegames/save1.sav";
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void guardarPartida()
	{
		JsonObjToSave jsonObj = new JsonObjToSave();

		jsonObj.jugador = new PlayerJson();
		jsonObj.jugador.municion = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().municion;
		jsonObj.jugador.posicion = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().transform.position;

		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		jsonObj.enemigos = new EnemyJson[enemies.Length];
		int i = 0;
		foreach(GameObject enemy in enemies)
		{
			jsonObj.enemigos[i].vida = enemy.GetComponent<EnemyController>().vida;
			jsonObj.enemigos[i].posicion = enemy.GetComponent<EnemyController>().transform.position;
			i++;
		}
		
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
		FileStream fileStream = new FileStream(filePath, FileMode.CreateNew);
		Aes aes = Aes.Create();
		byte[] key =
		{
			0xDE, 0xAD, 0xC0, 0xDE, 0xDE, 0xAD, 0xBE, 0xEF,
			0xDA, 0xC1, 0x05, 0x56, 0x45, 0x00, 0xEF, 0x1F
		};
		aes.Key = key;

		byte[] iv = aes.IV;
		fileStream.Write(iv, 0, iv.Length);

		CryptoStream cryptoStream = new CryptoStream(
			fileStream,
			aes.CreateEncryptor(),
			CryptoStreamMode.Write);
		StreamWriter encryptWriter = new StreamWriter(cryptoStream);
		encryptWriter.WriteLine(JsonUtility.ToJson(jsonObj));
		encryptWriter.Close();
	}

	public void cargarPartida()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in enemies)
		{
			Destroy(enemy);
		}


		FileStream fileStream = new FileStream(filePath, FileMode.Open);
		Aes aes = Aes.Create();
		byte[] iv = new byte[aes.IV.Length];
		int numBytesToRead = aes.IV.Length;
		int numBytesRead = 0;
		while (numBytesToRead > 0)
		{
			int n = fileStream.Read(iv, numBytesRead, numBytesToRead);
			if (n == 0) break;

			numBytesRead += n;
			numBytesToRead -= n;
		}

		byte[] key =
		{
			0xDE, 0xAD, 0xC0, 0xDE, 0xDE, 0xAD, 0xBE, 0xEF,
			0xDA, 0xC1, 0x05, 0x56, 0x45, 0x00, 0xEF, 0x1F
		};

		CryptoStream cryptoStream = new CryptoStream(
			fileStream,
			aes.CreateDecryptor(key, iv),
			CryptoStreamMode.Read);
		StreamReader decryptReader = new StreamReader(cryptoStream);
		string jsonString = decryptReader.ReadToEnd();
		decryptReader.Close();
		JsonObjToSave jsonObj = JsonUtility.FromJson<JsonObjToSave>(jsonString);
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().municion = jsonObj.jugador.municion;
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().transform.position = jsonObj.jugador.posicion;

		foreach(EnemyJson enemyJson in jsonObj.enemigos)
		{
			GameObject enemigoInstanciado = Instantiate(enemyPrefab, enemyJson.posicion, Quaternion.identity);
			enemigoInstanciado.GetComponent<EnemyController>().vida = enemyJson.vida;
		}
	}
}
