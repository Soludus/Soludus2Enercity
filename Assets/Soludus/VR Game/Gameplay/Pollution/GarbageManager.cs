using Soludus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns garbages.
/// </summary>
public class GarbageManager : MonoBehaviour
{
    public int maxCount = 5;

    [MinMaxSlider(0, 100)]
    public Vector2 intervalRange = new Vector2(1, 4);

    public float spawnPointCooldown = 10;

    public List<Transform> spawnPositions = new List<Transform>();

    private class SpawnPoint
    {
        public Transform point;
        public GameObject spawned;

        public bool cooldown;
        public float lastActive;
    }

    private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    public GameObject spawnedPrefab = null;

    private List<SpawnPoint> spawned = new List<SpawnPoint>();
    private float lastSpawn = 0;
    private float interval = 0;

    private void OnEnable()
    {
        spawnPoints.Clear();
        for (int i = 0; i < spawnPositions.Count; ++i)
        {
            spawnPoints.Add(new SpawnPoint
            {
                point = spawnPositions[i],
                spawned = null,
                lastActive = 0
            });
        }
    }

    private void Update()
    {
        // add freed spawn points
        for (int i = spawned.Count - 1; i >= 0; --i)
        {
            if (spawned[i].spawned == null)
            {
                if (!spawned[i].cooldown)
                {
                    spawned[i].lastActive = Time.time;
                    spawned[i].cooldown = true;
                }
                if (Time.time >= spawned[i].lastActive + spawnPointCooldown)
                {
                    spawned[i].cooldown = false;
                    spawnPoints.Add(spawned[i]);
                    spawned.RemoveAt(i);
                }
            }
        }

        // spawn new
        if (spawned.Count < maxCount && spawnPoints.Count > 0 && Time.time >= lastSpawn + interval)
        {
            SpawnRandom();
            interval = Random.Range(intervalRange.x, intervalRange.y);
        }
    }

    private void SpawnRandom()
    {
        int i = Random.Range(0, spawnPoints.Count);

        var g = Instantiate(spawnedPrefab, spawnPoints[i].point.position, spawnPoints[i].point.rotation);

        var sp = spawnPoints[i];
        sp.spawned = g;
        spawned.Add(sp);
        spawnPoints.RemoveAt(i);

        lastSpawn = Time.time;

        //while(true)
        //{
        //    var point = new Vector3(Random.Range(-10, 10), 200, Random.Range(-10, 10));
        //    RaycastHit hit;
        //    if (Physics.Raycast(new Ray(point, Vector3.down), out hit))
        //    {

        //    }
        //}
    }

}
