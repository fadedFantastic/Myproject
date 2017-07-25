using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AreaBlock : MonoBehaviour
{
    private static int m_InstanceID = 0;
    public int InstanceID { get{ return m_InstanceID; } }

    public List<GameObject> blocks = new List<GameObject>();
    public List<Vector3> handlesPosition = new List<Vector3>();


    public void AddBlockObj(Vector3 pos)
    {
        int count = blocks.Count;
        if(count > 0)
        {
            if(Vector3.Distance(blocks[count - 1].transform.position, pos) > float.MinValue)
            {
                return;
            }
        }

        GameObject go = Instantiate(Resources.Load("Cube")) as GameObject;

    }



    private void OnDrawGizmos()
    {

    }
}