using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHolderScript : MonoBehaviour {

    public enum Enemies { Ratbird, Bat, Roach, VenusFlyTrap, Shadowfox }
    public Enemies spawnEnemy;

    //Prefabs
    public GameObject ratbird;
    public GameObject bat;
    public GameObject roach;
    public GameObject venusFlyTrap;
    public GameObject shadowfox;

    //For pathfinding
    Transform[] waypoints;

    // Use this for initialization
    void Start () {
        List<Transform> wp = new List<Transform>(GetComponentsInChildren<Transform>(false));
        wp.RemoveAt(0);
        waypoints = wp.ToArray();

        GameObject o = null;
		switch (spawnEnemy)
        {
            case (Enemies.Ratbird):
                o = Instantiate(ratbird, this.transform.position, this.transform.rotation);
                break;
            case (Enemies.Bat):
                o = Instantiate(bat, this.transform.position, this.transform.rotation);
                break;
            case (Enemies.Roach):
                o = Instantiate(roach, this.transform.position, this.transform.rotation);
                break;
            case (Enemies.VenusFlyTrap):
                o = Instantiate(venusFlyTrap, this.transform.position, this.transform.rotation);
                break;
            case (Enemies.Shadowfox):
                o = Instantiate(shadowfox, this.transform.position, this.transform.rotation);
                break;
        }

        if (o != null)
            o.GetComponent<EnemyScript>().Waypoints = waypoints;

        Destroy(this.gameObject);
	}
}
