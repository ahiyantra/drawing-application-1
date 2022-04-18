using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum PenMode
{
    NIL,
    DRAW,
    REPLICATE
}

public class PenTool : MonoBehaviour
{
    [Header("Pen Canvas")]
    [SerializeField] private PenCanvas penCanvas;

    [Header("Dots")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotParent;

    [Header("Lines")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform lineParent;
    [SerializeField] private LineController currentLine;

    [Header("Colors")]
    [SerializeField] private Color activeColor;
    [SerializeField] private Color normalColor;

    [Header("Loop Toggle")]
    [SerializeField] Image loopToggle;
    [SerializeField] Sprite loopSprite;
    [SerializeField] Sprite unloopSprite;

    public List<Vector2> vertices2l;
    public List<Vector3> vertices3l;

    public GameObject shapePrefab;
    public Transform shapeParent;

    public PenMode activeMode = PenMode.NIL;
 
    private void Start() {
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && activeMode == PenMode.REPLICATE && activeMode != PenMode.DRAW)
        {
            //Instantiate(GameObject.FindGameObjectWithTag("Line Renderer"), GetMousePosition(), Quaternion.identity, lineParent);
            var go = GameObject.FindGameObjectWithTag("Line Renderer");

            var lr = go.GetComponent<LineRenderer>();
            var pc = lr.positionCount;

            //Debug.Log(pc);

            Vector3[] pos_0 = new Vector3[pc];
            lr.GetPositions(pos_0);

            //Debug.Log(pos_0[0]);

            GameObject kopi = GameObject.Instantiate(go, GetMousePosition(), Quaternion.identity, lineParent);

            kopi.GetComponent<LineRenderer>().positionCount = pc;

            kopi.GetComponent<LineRenderer>().SetPositions(pos_0);

            kopi.GetComponent<LineRenderer>().useWorldSpace = false;

            kopi.transform.position = GetMousePosition();

            //var lr2 = replica.GetComponent<LineRenderer>();
            //var pc2 = lr2.positionCount;

            //Debug.Log(pc2);

            //Vector3[] pos_1 = new Vector3[pc2];
            //lr2.GetPositions(pos_1);

            //Debug.Log(pos_1[0]);

            var gogo = GameObject.FindGameObjectWithTag("Shape Renderer");

            GameObject kopikopi = GameObject.Instantiate(gogo, GetMousePosition(), Quaternion.identity, shapeParent);

            kopikopi.transform.position = GetMousePosition();
        }   
        if (activeMode == PenMode.DRAW && activeMode != PenMode.REPLICATE)
        {
        penCanvas.OnPenCanvasLeftClickEvent += AddDot;
        penCanvas.OnPenCanvasRightClickEvent += EndCurrentLine;
        }

    }

    public void MakeShapes()
    {
        //var go = GameObject.FindGameObjectsWithTag("Dot");
        /*
        Vector2[] vertices2D = new Vector2[go.Length];
        for (int i = 0; i < go.Length; i++)
        {
            vertices2D[i] = new Vector2(go[i].transform.position.x, go[i].transform.position.y);
        }
        
        if (vertices2D.Length >=1)
        {
            Debug.Log(vertices2D[0]);
        }
        
        Vector3[] vertices3D = new Vector3[go.Length];
        for (int i = 0; i < go.Length; i++)
        {
            vertices3D[i] = go[i].transform.position;
        }
        
        if (vertices3D.Length >= 1)
        {
            Debug.Log(vertices3D[0]);
        }
        */

        var gogogo = shapePrefab;

        GameObject kopikopikopi = GameObject.Instantiate(gogogo, GetMousePosition(), Quaternion.identity, shapeParent);

        //kopikopikopi.transform.position = GetMousePosition();

        Vector2[] vertices2v = vertices2l.ToArray();
        Vector3[] vertices3v = vertices3l.ToArray();

        // Use the triangulator to get indices for creating triangles
        var triangulator = new Triangulator(vertices2v);
        var indices = triangulator.Triangulate();

        // Generate a color for each vertex
        /*
        var colors = Enumerable.Range(0, vertices3v.Length)
            .Select(i => UnityEngine.Random.ColorHSV())
            .ToArray();
        */
        ///*
        Color[] colors = new Color[vertices3v.Length];
        for (int i = 0; i < vertices3v.Length; i++)
            colors[i] = new Color(231f, 76f, 60f); // Color.Lerp(Color.red, Color.green, vertices3v[i].y);
        //*/
        // Create the mesh
        var mesh = new Mesh
        {
            vertices = vertices3v,
            triangles = indices,
            colors = colors
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Set up game object with mesh;
        //var meshRenderer = GameObject.Find("Shape Renderer").AddComponent<MeshRenderer>(); // gameObject
        //var meshRenderer = GameObject.Find("Shape Renderer").GetComponent<MeshRenderer>();
        kopikopikopi.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color")); // GameObject.Find("Shape Renderer") Sprites/Default

        //meshRenderer.material.color = new Color(231f, 76f, 60f);
        //GameObject.Find("Shape Renderer").GetComponent<Renderer>().material.SetColor("_Color", new Color(231f, 76f, 60f)); 

        //var filter = GameObject.Find("Shape Renderer").AddComponent<MeshFilter>(); // gameObject
        //var meshFilter = GameObject.Find("Shape Renderer").GetComponent<MeshFilter>();
        kopikopikopi.GetComponent<MeshFilter>().mesh = mesh; // GameObject.Find("Shape Renderer")

        kopikopikopi.transform.position = GameObject.FindGameObjectWithTag("Line Renderer").transform.position; // GameObject.Find("Shape Renderer")

    }

    public void Complete() {
        if (currentLine != null) {

            currentLine.ToggleLoop();
            //loopToggle.sprite = (currentLine.isLooped()) ? unloopSprite : loopSprite;
            MakeShapes();
        }
    }

    public void DrawLines()
    {
        activeMode = PenMode.DRAW;
    }

    public void ClearStuff()
    {
        var clones_0 = GameObject.FindGameObjectsWithTag("Line Renderer");
        foreach (var clone in clones_0)
        {
            Destroy(clone);
        }
        var clones_1 = GameObject.FindGameObjectsWithTag("Dot");
        foreach (var clone in clones_1)
        {
            Destroy(clone);
        }
        var clones_2 = GameObject.FindGameObjectsWithTag("Shape Renderer");
        foreach (var clone in clones_2)
        {
            Destroy(clone);
        }
        vertices2l.Clear();
        vertices3l.Clear();
    }

    public void Replicate()
    {
        activeMode = PenMode.REPLICATE;
        currentLine = null;
    }

    private void AddDot() {

        if (currentLine == null && activeMode == PenMode.DRAW && activeMode != PenMode.REPLICATE) {
            LineController lineController = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineParent).GetComponent<LineController>();
            SetCurrentLine(lineController);
        }
        
        if (activeMode == PenMode.DRAW && activeMode != PenMode.REPLICATE)
        {
            DotController dot = Instantiate(dotPrefab, GetMousePosition(), Quaternion.identity, dotParent).GetComponent<DotController>();
            dot.OnDragEvent += MoveDot;
            dot.OnRightClickEvent += RemoveDot;
            dot.OnLeftClickEvent += SetCurrentLine;

            currentLine.AddDot(dot);

            if (vertices2l.Contains(new Vector2(GetMousePosition().x, GetMousePosition().y)) == false)
            {
                vertices2l.Add(new Vector2(GetMousePosition().x, GetMousePosition().y));
            }
            if (vertices3l.Contains(GetMousePosition()) == false)
            {
                vertices3l.Add(GetMousePosition());
            }
        }
        
    }

    private void RemoveDot(DotController dot) {
        dot.line.SplitPointsAtIndex(dot.index, out List<DotController> before, out List<DotController> after);

        Destroy(dot.line.gameObject);
        Destroy(dot.gameObject);

        LineController beforeLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineParent).GetComponent<LineController>();
        for (int i = 0; i < before.Count; i++) {
            beforeLine.AddDot(before[i]);
        }

        LineController afterLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineParent).GetComponent<LineController>();
        for (int i = 0; i < after.Count; i++) {
            afterLine.AddDot(after[i]);
        }
    }

    private void EndCurrentLine() {
        if (currentLine != null) {
            currentLine.SetColor(normalColor);
            loopToggle.enabled = false;
            currentLine = null;
        }
    }

    private void SetCurrentLine(LineController newLine) {
        EndCurrentLine();

        currentLine = newLine;
        currentLine.SetColor(activeColor);

        loopToggle.enabled = true;
        loopToggle.sprite = (currentLine.isLooped()) ? unloopSprite : loopSprite;
    }

    private void MoveDot(DotController dot) {
        dot.transform.position = GetMousePosition();
    }

    private Vector3 GetMousePosition() {
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMousePosition.z = 0;

        return worldMousePosition;
    }
}
