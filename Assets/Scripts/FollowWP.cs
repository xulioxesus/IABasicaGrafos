using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWP : MonoBehaviour {

    public GameObject[] waypoints;
    int currentWP = 0;

    public float speed = 10.0f;
    public float rotSpeed = 10.0f;

    void Start() {

    }

    void Update() {

        if (Vector3.Distance(this.transform.position, waypoints[currentWP].transform.position) < 3.0f) {

            currentWP++;
        }

        if (currentWP >= waypoints.Length) {

            currentWP = 0;
        }

        // this.transform.LookAt(waypoints[currentWP].transform);

        Quaternion lookAtWP = Quaternion.LookRotation(waypoints[currentWP].transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookAtWP, Time.deltaTime * rotSpeed);
        this.transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
    }
}
