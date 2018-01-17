using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class Plant : MonoBehaviour
{
    public Transform[] nodes;
    public Transform head;
    public Transform target;
    public Transform root;

    private void Update()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            var dir = Engine.Vector.Direction(nodes[i].GetChild(0).position, nodes[i].parent.position);
            var dis = Vector3.Distance(nodes[i].GetChild(0).position, nodes[i].parent.position);
            nodes[i].position = nodes[i].GetChild(0).position + dir * dis / 2;
            nodes[i].rotation = Quaternion.Slerp(nodes[i].rotation, Quaternion.LookRotation(dir), Time.deltaTime);
        }
        head.position = target.position;
        var euler = Quaternion.LookRotation(Engine.Vector.Direction(root.position, target.position)).eulerAngles;
        euler.y += 90;
        head.rotation = Quaternion.Euler(euler);
        root.rotation = Quaternion.LookRotation(Engine.Vector.Direction(root.position, nodes[nodes.Length-1].position));
    }

}