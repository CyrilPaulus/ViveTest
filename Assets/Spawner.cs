using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject ToSpawn;
    public Vector3 SpawnPosition;
    public float SpawnTime;
    private float _lastSpawnTime;

    private bool _active = false;

    public Color BaseColor;
    public Color ActiveColor;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if(_active)
            _lastSpawnTime += Time.deltaTime;

        if(_lastSpawnTime > SpawnTime)
        {        
            _lastSpawnTime = 0;
            _active = false;
        }

        GetComponent<Renderer>().material.color = GetCurrentColor();
    }

    private void OnTriggerEnter(Collider collider)
    {        
        if (_active)
            return;

        Object.Instantiate(ToSpawn, SpawnPosition, Quaternion.identity);
        _active = true;
    }

    private Color GetCurrentColor()
    {
        if (_active)
            return ActiveColor;
        else
            return BaseColor;
    }
}
