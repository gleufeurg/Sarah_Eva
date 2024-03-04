using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

public class ElectricArc : MonoBehaviour
{
    public float range = 10;
    public Vector3 spawnOffset = new Vector3(1, 0, 0);

    public LayerMask enemyMask;
    public KeyCode input;
    public GameObject electricArc;

    List<GameObject> electricArcs = new List<GameObject>();
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
            Fire();
        }
        
        if (Input.GetKeyUp(input))
        {
                //StopFire();
        }
    }

    private void StopFire()
    {
        foreach (GameObject electricArc in electricArcs)
        {
            Destroy(electricArc);
        }
            hittedEnemies.Clear();
            electricArcs.Clear();
    }

    private void Fire()
    {
        //GameObject arc = GameObject.Instantiate(electricArc, transform.position, Quaternion.identity);

        GameObject target = GetNearestEnemy(transform.position);

        if (target == null)
            return;

        GameObject arc = GameObject.Instantiate(electricArc, transform.position, Quaternion.identity);

        //arc.transform.position = arc.transform.position - arc.transform.position/2;
        arc.transform.Find("Pos4").transform.position = target.transform.position;

        hittedEnemies.Add(target);
        electricArcs.Add(arc);
        arc.transform.Find("VFXG_ElectricArc").GetComponent<VisualEffect>().Play();
    }

    private GameObject GetNearestEnemy(Vector3 position)
    {
        GameObject nearestEnemy = null;
        float distance = float.MaxValue;
        Collider[] enemies = Physics.OverlapSphere(position, range, enemyMask);

        if(enemies.Length == 0 )
        { return null; }

        foreach (Collider enemy in enemies)
        {
            if(hittedEnemies.Count > 0)
            {
               foreach(GameObject hittedEnemy in hittedEnemies)
               {
                   if (hittedEnemy == enemy)
                       continue;

                   float distFromEnemy = Vector3.Distance(position, enemy.transform.position);
                   if (distFromEnemy < distance)
                   {
                       distance = distFromEnemy;
                       nearestEnemy = enemy.gameObject;
                   }
               }
            }
            else
            {
                float distFromEnemy = Vector3.Distance(position, enemy.transform.position);
                if (distFromEnemy < distance)
                {
                    distance = distFromEnemy;
                    nearestEnemy = enemy.gameObject;
                }
            }

        }
        return nearestEnemy;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
