using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eTest : MonoBehaviour
{
    private cmGrid grid;

    public int x = 10;
    public int y = 10;
    public float z = 1f;
    public int a = -5;
    public int b = -5;

    // Start is called before the first frame update
    void Start()
    {
        grid = new cmGrid(x, y, z, new Vector3(a, b));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
