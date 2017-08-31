using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {
    public Transform mainParent;
    public GameObject prefab, connectTo;
    public List<GameObject> rope = new List<GameObject>();
    public int count;

    public void Awake()
    {
        for(int i = 0; i < count; i++)
        {
            rope.Add(CreateJoint());
        }
    }
    public void Update()
    {
        rope[0].transform.position = connectTo.transform.position;
        rope[0].transform.rotation = connectTo.transform.rotation;
    }
    public GameObject CreateJoint()
    {
        GameObject go = Instantiate(prefab);
        go.name = rope.Count.ToString();
        go.transform.localPosition = new Vector3(0, 0, 0);
        go.transform.SetParent(null);
        if(rope.Count > 0)
        {
            go.transform.position = new Vector3(rope[rope.Count - 1].transform.position.x, rope[rope.Count - 1].transform.position.y - go.GetComponent<MeshFilter>().mesh.bounds.size.y, rope[rope.Count - 1].transform.position.z);
            CharacterJoint cj = go.AddComponent<CharacterJoint>();
            cj.connectedBody = rope[rope.Count - 1].transform.GetComponent<Rigidbody>();
            cj.connectedAnchor = new Vector3(0, 0f, 0);
        }
        else
        {

        }
        return go;
    }
}
