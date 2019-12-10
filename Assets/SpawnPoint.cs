using System;
using System.Collections;
using System.Collections.Generic;
using Project.Networking;
using Project.Utilities;
using UnityEngine;

[Serializable]
public class SpawnPoint : MonoBehaviour
{
    public Position position { get {
        Position pos = new Position();
        pos.x = transform.position.x.TwoDecimals();
        pos.y = transform.position.y.TwoDecimals();
        pos.z = transform.position.z.TwoDecimals();
        return pos;
        }
    }
    public Rotation rotation { get {
        Rotation rot = new Rotation();
        rot.rotation = transform.rotation;
        return rot;
        }
    }
}
