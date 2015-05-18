﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx;

public class DataDefinition {
	public string name;
	public string type;
}

public class API : MonoBehaviour {

	public string[] outputNames = {"position"};
	public string[] outputTypes = {"float"};
	public string[] inputNames;
	public string[] inputTypes;
	public bool blockOnResponse = false;
	public float updateRate = 30;  // hertz
	public float runSpeed = 1.0f;
	public string serverURL = "http://localhost:8080";

	private bool _isWaitingForResponse;
	private float _lastSyncTime;
	private Dictionary<string, object> _outputData;
	private Dictionary<string, object> _inputData;

	public void SetOutput(string name, object data) {
		_outputData [name] = data;
	}

	public object GetInput(string name) {
		if (_inputData.ContainsKey (name)) {
			return _inputData [name];
		}
		return null;
	}

	void Clear() {
		_outputData = new Dictionary<string, object>();
		_inputData = new Dictionary<string, object>();
	}

	/* Data transfer */

	IEnumerator SendInit() {
		WWWForm form = new WWWForm ();
		form.AddField ("outputNames", JsonWriter.Serialize(outputNames));
		form.AddField ("outputTypes", JsonWriter.Serialize(outputTypes));
		form.AddField ("inputNames", JsonWriter.Serialize(inputNames));
		form.AddField ("inputTypes", JsonWriter.Serialize(inputTypes));
		string url = "http://localhost:8080/init";
		WWW www = new WWW (url, form);
		yield return www;
	}

	IEnumerator Sync() {
		WWWForm form = new WWWForm ();
		form.AddField ("outputData", JsonWriter.Serialize(_outputData));
		string url = "http://localhost:8080/sync";
		WWW www = new WWW (url, form);
		yield return www;
		_inputData = JsonReader.Deserialize<Dictionary<string, object>>(www.text);

		_isWaitingForResponse = false;
		if (blockOnResponse) {
			Time.timeScale = 1.0f;
		}
	}

	/* Events */

	void Start() {
		Clear();
		StartCoroutine("SendInit");
	}

	void OnLevelWasLoaded(int level) {
		Clear();
		StartCoroutine("SendInit");
	}

	void Update() {
		Time.timeScale = runSpeed;
	}

	void LateUpdate () {
		if (Time.time - _lastSyncTime < 1 / updateRate) {
			return;
		}

		if (blockOnResponse && _isWaitingForResponse) {
			return;
		}

		_lastSyncTime = Time.time;
		StartCoroutine ("Sync");
		_isWaitingForResponse = true;

		if (blockOnResponse) {
			Time.timeScale = 0.0f;
		}
	}


	/* Persistent Singleton */

	private static API _instance;

	public static API instance
	{
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<API>();
				DontDestroyOnLoad(_instance.gameObject);
			}

			return _instance;
		}
	}

	void Awake()
	{
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else {
			if(this != _instance)
			Destroy(this.gameObject);
		}
	}

}
