using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricArc : MonoBehaviour
{
    public float range = 10;
    public Vector3 spawnPos = new Vector3(1, 0, 0);

    public LayerMask enemyMask;
    public KeyCode input;
    public GameObject electricArc;

    List<GameObject> instances = new List<GameObject>();
    List<GameObject> hittedEnemies = new List<GameObject>();

    struct enemy
    {
        public bool exist;
        public GameObject enemyObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(input))
        {
            Fire(transform, range);
        }
        
        if (Input.GetKeyUp(input))
        {
                StopFire();
        }
    }

    private void StopFire()
    {
        foreach (GameObject instance in instances)
        {
            Destroy(instance);
            hittedEnemies.Clear();
        }
    }

    private void Fire(Transform fireTransform, float range_)
    {
        if(!AnyEnemyInRange(range_).exist)
            return;


        GameObject electricArc_ = GameObject.Instantiate(electricArc);
        instances.Add(electricArc_);

        electricArc_.transform.rotation = fireTransform.rotation;
        electricArc_.transform.position = fireTransform.position + spawnPos;

        electricArc_.transform.Find("Pos4").transform.position = GetNearestEnemyInRange(range_).position;
        Fire(GetNearestEnemyInRange(range_), range_);
    }

    private Transform GetNearestEnemyInRange(float range)
    {
        GameObject nearestEnemy = new GameObject();
        Vector3 nearestEnemyPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        Collider[] enemies = Physics.OverlapSphere(transform.position, range, enemyMask);

        foreach (Collider enemy in enemies)
        {
            foreach (GameObject hittedEnemy in hittedEnemies)
            {
                if (hittedEnemy != nearestEnemy)
                {
                    hittedEnemies.Add(enemy.gameObject);

                    Vector3 enemyPosition = enemy.transform.position;

                    if (Vector3.Distance(enemyPosition, transform.position) < Vector3.Distance(transform.position, nearestEnemyPosition))
                        nearestEnemy = enemy.gameObject;
                }
            }

        }

        return nearestEnemy.transform;
    }
    private enemy AnyEnemyInRange(float range)
    {
        enemy enemy_ = new enemy();
        enemy_.exist = false;
        enemy_.enemyObject = null;

        Collider[] enemies = Physics.OverlapSphere(transform.position, range, enemyMask);

        if (enemies.Length == 0)
        {
            enemy_.exist = false;
            return enemy_;
        }

        foreach (Collider enemy in enemies)
        {
            foreach(GameObject hittedEnemy in hittedEnemies)
            {
                if (enemy.gameObject != hittedEnemy)
                {
                    enemy_.exist = true;
                    enemy_.enemyObject = enemy.gameObject;
                    return enemy_;
                }
            }
        }

        print(enemy_);
        return enemy_;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
