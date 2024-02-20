using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField]private List<Transform> spawnPoints;
    [SerializeField]private List<Transform> sortedSpawnPoints;
    private void Awake()
    {
        //Finding Transforms on scene with tag SpawnPoint and distincting
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint").Select(a => a.GetComponent<Transform>()));
        sortedSpawnPoints = spawnPoints.Distinct().ToList();
    }


    public Vector3 GetRandomPos()
    {
        int spawnPointsCount = sortedSpawnPoints.Count;
        int randSpawnPoint = Random.Range(0, spawnPointsCount);
        return sortedSpawnPoints[randSpawnPoint].transform.position;
    }
}
