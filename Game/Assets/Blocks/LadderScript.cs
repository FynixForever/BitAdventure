using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour {

    private void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag != "Player" && coll.gameObject.tag != "StairEnemy")
            return;

        coll.gameObject.GetComponent<PlayerScript>().OnStairs = true;
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag != "Player" && coll.gameObject.tag != "StairEnemy")
            return;

        coll.gameObject.GetComponent<PlayerScript>().OnStairs = false;
    }
}
