using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskFactory : MonoBehaviour {

    private GameObject diskPrefab;

    private Queue<GameObject> free = new Queue<GameObject>();
    private List<GameObject> used = new List<GameObject>();

    protected DiskFactory() { }

    private void Awake()
    {
        diskPrefab = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/disk"), Vector3.zero, Quaternion.identity);
        diskPrefab.SetActive(false);
    }

    public GameObject GetDisk(int round)
    {
        GameObject disk = null;
        int type = 0;
        if (free.Count != 0)
        {
            disk = free.Dequeue();
        } else
        {
            disk = GameObject.Instantiate<GameObject>(diskPrefab, Vector3.zero, Quaternion.identity);
            disk.AddComponent<DiskData>();
        }
        switch (round)
        {
            case 1:
                type = 1;
                break;
            case 2:
                type = Random.Range(0, 200);
                break;
            case 3:
                type = Random.Range(0, 1000);
                break;
        }
        float y = UnityEngine.Random.Range(0.0f, 4.0f);
        if (type <= 100)
        {
            setType(disk, Color.red, 8.0f, y);
        } else if(type <= 400) {
            setType(disk, Color.yellow, 10.0f, y);
        }else {
            setType(disk, Color.blue, 15.0f, y);
        }
        disk.GetComponent<DiskData>().name = disk.GetInstanceID().ToString();
        disk.SetActive(false);
        used.Add(disk);
        return disk;
    }

    private void setType(GameObject disk, Color color, float hspeed, float vspeed)
    {
        disk.GetComponent<DiskData>().color = color;
        disk.GetComponent<Renderer>().material.color = color;
        disk.GetComponent<DiskData>().horizontalSpeed = hspeed;
        disk.GetComponent<DiskData>().verticalSpeed = vspeed;
        int x = (UnityEngine.Random.Range(-1.0f, 1.0f) < 0 ? -1 : 1);
        float y = UnityEngine.Random.Range(0.0f, 4.0f);
        disk.GetComponent<DiskData>().direction = x;
        disk.transform.position = new Vector3(x * 7, y, 0);
    }

    public void FreeDisk(GameObject disk)
    {
        foreach(GameObject i in used)
        {
            if(i.GetInstanceID() == disk.GetInstanceID())
            {
                i.SetActive(false);
                free.Enqueue(i);
                used.Remove(i);
                break;
            }
        }
    }

    public void Reset()
    {
        used.Clear();
    }
}
