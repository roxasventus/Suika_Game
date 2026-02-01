using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    public GameObject[] spawners;

    public GameObject[] prefabs;

    List<GameObject>[] pools;

    public GameObject spawnPool;

    public float spawnTime;

    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        pools = new List<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }

    }

    public GameObject Get(int index) {

        GameObject select = null;

        GameObject selectedSpawner = null;

        selectedSpawner = spawners[Random.Range(0, spawners.Length)];

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.transform.position =  selectedSpawner.transform.position;
                select.SetActive(true);

                break;
            }
        }

        if (select == null) {
            select = Instantiate(prefabs[index]);
            select.transform.position = selectedSpawner.transform.position;
            select.transform.SetParent(spawnPool.transform);
            pools[index].Add(select);
        }

        Object selectObject = select.GetComponent<Object>();
        if ( selectObject != null && selectObject.getLock == true)
            selectObject.setLock(false);

        return select;

    }


    public GameObject Get(int index, Transform transForm)
    {

        GameObject select = null;

        GameObject selectedSpawner = null;

        selectedSpawner = spawners[Random.Range(0, spawners.Length)];

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.transform.position = transForm.position;
                select.SetActive(true);

                break;
            }
        }

        if (select == null)
        {
            select = Instantiate(prefabs[index]);
            select.transform.position = transForm.position;
            select.transform.SetParent(spawnPool.transform);
            pools[index].Add(select);
        }

        Object selectObject = select.GetComponent<Object>();
        if (selectObject != null && selectObject.getLock == true)
            selectObject.setLock(false);

        return select;

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        if (GameManager.instance.isPlay == true)
        {
            StartCoroutine(spawnCoroutine());
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.instance.isPlay == false && Input.GetButtonDown("Jump"))
        {
            Get(0);
        }
        
    }

    IEnumerator spawnCoroutine()
    {
        while (GameManager.instance.isPlay) {
            yield return new WaitForSeconds(1.0f);
            Get(GetRandomOreIndex());
        }

    }

    int GetRandomOreIndex()
    {
        float typeR = Random.value * 100f;   // 광물이냐 장애물이냐?
        float r = Random.value * 100f;   // 어떤 종류의 광물?


        if (typeR < 90f) 
        {
            if (r < 45f) return 0;
            if (r < 75f) return 1;
            if (r < 90f) return 2;
            if (r < 97f) return 3;
            if (r < 99.5f) return 4;
            return 5;
        }
        else
        {
            return 6;
        }

    }
}
