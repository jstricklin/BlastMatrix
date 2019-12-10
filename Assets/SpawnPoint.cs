using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Position position { get {
        Position pos = new Position();
        pos.x = transform.position.x;
        pos.y = transform.position.y;
        pos.z = transform.position.z;
        return pos;
        }
    }
}
