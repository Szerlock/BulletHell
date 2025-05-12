using System.Collections.Generic;
using UnityEngine;

public class BombsPool : MonoBehaviour
{
    public static BombsPool Instance;

    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private int poolSize = 100;

    private Queue<GameObject> bombPool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;


        for (int i = 0; i < poolSize; i++)
        {
            GameObject bomb = Instantiate(bombPrefab);
            bomb.SetActive(false);
            bombPool.Enqueue(bomb);
        }
    }

    public GameObject GetBomb(Vector3 position, Quaternion rotation)
    {
        if (bombPool.Count > 0)
        {
            GameObject bomb = bombPool.Dequeue();
            bomb.transform.position = position;
            bomb.transform.rotation = rotation;
            bomb.SetActive(true);
            return bomb;
        }
        else
        {
            Debug.LogWarning("Bomb pool exhausted!");
            return null;
        }
    }

    public void ReturnBomb(GameObject bomb)
    {
        bomb.SetActive(false);
        bombPool.Enqueue(bomb);
    }
}
