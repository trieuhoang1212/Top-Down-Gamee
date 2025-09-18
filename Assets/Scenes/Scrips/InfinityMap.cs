using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InfinityMap : MonoBehaviour
{
    [SerializeField]
    Transform player;

    [SerializeField]
    float checkInterval = 0.08f;

    bool midpointSet;
    Vector3 anchor;
    float timer;
    Transform activeGroup;

    Tilemap[] allTilemaps;
    readonly Dictionary<Transform, GroupInfo> groups = new();
    readonly Transform[,] grid = new Transform[3, 3];
    bool gridBuilt;
    float stepX,
        stepY,
        cellX = 1f,
        cellY = 1f;

    class GroupInfo
    {
        public Transform root;
        public List<Tilemap> tms = new();
        public Vector3 localCenter;
    }

    void Awake()
    {
        allTilemaps = GetComponentsInChildren<Tilemap>(true);
        foreach (var tm in allTilemaps)
            if (tm)
                tm.CompressBounds();
        foreach (var tm in allTilemaps)
        {
            var g = tm?.layoutGrid;
            if (g)
            {
                cellX = Mathf.Abs(g.cellSize.x);
                cellY = Mathf.Abs(g.cellSize.y);
                break;
            }
        }
        BuildGroups();
        BuildGrid();
    }

    void Start()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            player = p ? p.transform : null;
        }
        if (player && player.IsChildOf(transform))
            player.SetParent(null, true);
        foreach (var cam in GetComponentsInChildren<Camera>(true))
            if (cam.transform.IsChildOf(transform))
                cam.transform.SetParent(null, true);
    }

    void Update()
    {
        if (!player || !gridBuilt || allTilemaps.Length == 0)
            return;
        timer += Time.deltaTime;
        if (timer < checkInterval)
            return;
        timer = 0f;
        var g = FindGroupUnder(player.position) ?? FindNearestGroup(player.position);
        if (g && g != activeGroup)
            OnEnterGroup(g);
    }

    void BuildGroups()
    {
        groups.Clear();
        foreach (var tm in allTilemaps)
        {
            if (!tm)
                continue;
            var root = DirectChildOfThis(tm.transform);
            if (!groups.TryGetValue(root, out var gi))
            {
                gi = new GroupInfo { root = root };
                groups.Add(root, gi);
            }
            gi.tms.Add(tm);
        }
        foreach (var kv in groups)
        {
            var gi = kv.Value;
            bool ok = false;
            Bounds total = new Bounds(Vector3.zero, Vector3.zero);
            foreach (var tm in gi.tms)
            {
                var lb = tm.localBounds;
                if (lb.size.sqrMagnitude <= 1e-8f)
                    continue;
                var wb = TransformBounds(tm.transform.localToWorldMatrix, lb);
                if (!ok)
                {
                    total = wb;
                    ok = true;
                }
                else
                    total.Encapsulate(wb);
            }
            var centerW = ok ? total.center : gi.root.position;
            gi.localCenter = gi.root.InverseTransformPoint(centerW);
        }
    }

    void BuildGrid()
    {
        var roots = new List<Transform>(groups.Keys);
        if (roots.Count != 9)
        {
            gridBuilt = false;
            return;
        }
        var xs = new List<float>();
        var ys = new List<float>();
        var center = new Dictionary<Transform, Vector2>();
        foreach (var r in roots)
        {
            var c = (Vector2)GetCenter(r);
            center[r] = c;
            xs.Add(c.x);
            ys.Add(c.y);
        }
        var cx = Centroids3(xs);
        var cy = Centroids3(ys);
        cx.Sort();
        cy.Sort();
        Array.Clear(grid, 0, grid.Length);
        foreach (var r in roots)
        {
            var c = center[r];
            grid[IndexOfClosest(cy, c.y), IndexOfClosest(cx, c.x)] = r;
        }
        stepX = RoundToCell(((Mathf.Abs(cx[1] - cx[0]) + Mathf.Abs(cx[2] - cx[1])) * 0.5f), cellX);
        stepY = RoundToCell(((Mathf.Abs(cy[1] - cy[0]) + Mathf.Abs(cy[2] - cy[1])) * 0.5f), cellY);
        anchor = SnapPoint(GetCenter(grid[1, 1]));
        midpointSet = true;
        gridBuilt = true;
        SnapAllToAnchor();
    }

    Transform FindGroupUnder(Vector3 world)
    {
        foreach (var kv in groups)
        foreach (var tm in kv.Value.tms)
            if (tm && tm.HasTile(tm.WorldToCell(world)))
                return kv.Key;
        return null;
    }

    Transform FindNearestGroup(Vector3 world)
    {
        Transform best = null;
        float d = float.PositiveInfinity;
        foreach (var kv in groups)
        {
            float dd = (GetCenter(kv.Key) - world).sqrMagnitude;
            if (dd < d)
            {
                d = dd;
                best = kv.Key;
            }
        }
        return best;
    }

    void OnEnterGroup(Transform root)
    {
        activeGroup = root;
        if (!TryIndex(root, out int x, out int y))
            return;
        int dx = x - 1,
            dy = y - 1;
        if (!midpointSet)
        {
            anchor = SnapPoint(GetCenter(root));
            midpointSet = true;
        }
        else
            anchor = SnapPoint(anchor + new Vector3(dx * stepX, dy * stepY, 0));
        var curCenter = GetCenter(root);
        var deltaMap = SnapDelta(anchor - curCenter);
        if (deltaMap.sqrMagnitude > 1e-8f)
            transform.position += deltaMap;
        if (dx > 0)
            ShiftColsLeft();
        else if (dx < 0)
            ShiftColsRight();
        if (dy > 0)
            ShiftRowsUp();
        else if (dy < 0)
            ShiftRowsDown();
        SnapAllToAnchor();
    }

    bool TryIndex(Transform r, out int ix, out int iy)
    {
        for (int yy = 0; yy < 3; yy++)
        for (int xx = 0; xx < 3; xx++)
            if (grid[yy, xx] == r)
            {
                ix = xx;
                iy = yy;
                return true;
            }
        ix = iy = -1;
        return false;
    }

    void ShiftColsLeft()
    {
        for (int y = 0; y < 3; y++)
            if (grid[y, 0])
                grid[y, 0].position += new Vector3(stepX * 3f, 0, 0);
        for (int y = 0; y < 3; y++)
        {
            var L = grid[y, 0];
            grid[y, 0] = grid[y, 1];
            grid[y, 1] = grid[y, 2];
            grid[y, 2] = L;
        }
    }

    void ShiftColsRight()
    {
        for (int y = 0; y < 3; y++)
            if (grid[y, 2])
                grid[y, 2].position += new Vector3(-stepX * 3f, 0, 0);
        for (int y = 0; y < 3; y++)
        {
            var R = grid[y, 2];
            grid[y, 2] = grid[y, 1];
            grid[y, 1] = grid[y, 0];
            grid[y, 0] = R;
        }
    }

    void ShiftRowsUp()
    {
        for (int x = 0; x < 3; x++)
            if (grid[0, x])
                grid[0, x].position += new Vector3(0, stepY * 3f, 0);
        for (int x = 0; x < 3; x++)
        {
            var B = grid[0, x];
            grid[0, x] = grid[1, x];
            grid[1, x] = grid[2, x];
            grid[2, x] = B;
        }
    }

    void ShiftRowsDown()
    {
        for (int x = 0; x < 3; x++)
            if (grid[2, x])
                grid[2, x].position += new Vector3(0, -stepY * 3f, 0);
        for (int x = 0; x < 3; x++)
        {
            var T = grid[2, x];
            grid[2, x] = grid[1, x];
            grid[1, x] = grid[0, x];
            grid[0, x] = T;
        }
    }

    Vector3 DesiredCenter(int ix, int iy) =>
        anchor + new Vector3((ix - 1) * stepX, (iy - 1) * stepY, 0);

    void SnapAllToAnchor()
    {
        for (int y = 0; y < 3; y++)
        for (int x = 0; x < 3; x++)
        {
            var r = grid[y, x];
            if (!r)
                continue;
            var want = DesiredCenter(x, y);
            var have = GetCenter(r);
            var d = want - have;
            if (d.sqrMagnitude > 1e-7f)
                r.position += SnapDelta(d);
        }
    }

    Transform DirectChildOfThis(Transform t)
    {
        Transform c = t,
            p = t;
        while (c && c != transform)
        {
            p = c;
            c = c.parent;
        }
        return p ? p : transform;
    }

    Vector3 GetCenter(Transform root) => groups[root].root.TransformPoint(groups[root].localCenter);

    static Bounds TransformBounds(Matrix4x4 m, Bounds b)
    {
        var c = m.MultiplyPoint3x4(b.center);
        var e = b.extents;
        var ax = m.MultiplyVector(new Vector3(e.x, 0, 0));
        var ay = m.MultiplyVector(new Vector3(0, e.y, 0));
        var az = m.MultiplyVector(new Vector3(0, 0, e.z));
        var we = new Vector3(
            Mathf.Abs(ax.x) + Mathf.Abs(ay.x) + Mathf.Abs(az.x),
            Mathf.Abs(ax.y) + Mathf.Abs(ay.y) + Mathf.Abs(az.y),
            Mathf.Abs(ax.z) + Mathf.Abs(ay.z) + Mathf.Abs(az.z)
        );
        return new Bounds(c, we * 2f);
    }

    static List<float> Centroids3(List<float> v)
    {
        var s = new List<float>(v);
        s.Sort();
        return new List<float> { s[0], s[s.Count / 2], s[^1] };
    }

    static int IndexOfClosest(List<float> s, float v)
    {
        int k = 0;
        float d = Mathf.Abs(v - s[0]);
        for (int i = 1; i < s.Count; i++)
        {
            float di = Mathf.Abs(v - s[i]);
            if (di < d)
            {
                d = di;
                k = i;
            }
        }
        return Mathf.Clamp(k, 0, 2);
    }

    static float RoundToCell(float v, float cell) =>
        cell > 1e-6f ? Mathf.Round(v / cell) * cell : v;

    Vector3 SnapPoint(Vector3 p) => new(RoundToCell(p.x, cellX), RoundToCell(p.y, cellY), 0);

    Vector3 SnapDelta(Vector3 d) => new(RoundToCell(d.x, cellX), RoundToCell(d.y, cellY), 0);
}
