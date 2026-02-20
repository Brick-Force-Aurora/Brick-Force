using _Emulator;
using _Emulator.Network.Gamemodes;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LineTool;

public class BrickEditTool : EditorTool
{
    public static BrickEditTool Tool { get; private set; }

    private byte x1, y1, z1;
    private byte x2, y2, z2;
    private bool hasPos1 = false, hasPos2 = false;
    private Vector3 rotationNormal;
    private byte cameraRotation;

    public bool HasPos1 { get { return hasPos1; } }
    public bool HasPos2 { get { return hasPos2; } }

    private Queue<GameObject> wire;
    private Queue<GameObject> invisible;
    private GameObject dummyPrefab;

    private HashSet<int> usedPoints;

    public BrickEditTool(EditorToolScript ets, GameObject dummy, BattleChat _battleChat)
        : base(ets, null, _battleChat)
    {
        this.dummyPrefab = dummy;
        wire = new Queue<GameObject>();
        invisible = new Queue<GameObject>();
        usedPoints = new HashSet<int>();
        Tool = this;
    }

    public override bool Update()
    {
        if (!battleChat.IsChatting && custom_inputs.Instance.GetButtonDown(editorToolScript.inputKey))
        {
            active = true;
            if (hasPos1 && hasPos2)
            {
                UpdateWireframePreview();
            }
            return true;
        }
        return false;
    }
    public void SetRotation(Vector3 normal, byte rotation)
    {
        this.rotationNormal = normal;
        this.cameraRotation = rotation;
    }

    public void SetPos1(Vector3 pos)
    {
        hasPos1 = ToCoords(pos, out x1, out y1, out z1);
        SendPos1();
        UpdateWireframePreview();
    }

    public void SetPos1(byte x, byte y, byte z)
    {
        hasPos1 = true;
        x1 = x;
        y1 = y;
        z1 = z;
        SendPos1();
        UpdateWireframePreview();
    }

    private void SendPos1()
    {
        if (!hasPos1)
        {
            Actor.Instance.SendChat($"[BrickEdit] Position 1 cleared ({GetBlockCount()} brick(s))");
            return;
        }
        Actor.Instance.SendChat($"[BrickEdit] Position 1 set to {x1} {y1} {z1} ({GetBlockCount()} brick(s))");
    }

    public void SetPos2(Vector3 pos)
    {
        hasPos2 = ToCoords(pos, out x2, out y2, out z2);
        SendPos2();
        UpdateWireframePreview();
    }

    public void SetPos2(byte x, byte y, byte z)
    {
        hasPos2 = true;
        x2 = x;
        y2 = y;
        z2 = z;
        SendPos2();
        UpdateWireframePreview();
    }

    private void SendPos2()
    {
        if (!hasPos1)
        {
            Actor.Instance.SendChat($"[BrickEdit] Position 2 cleared ({GetBlockCount()} brick(s))");
            return;
        }
        Actor.Instance.SendChat($"[BrickEdit] Position 2 set to {x2} {y2} {z2} ({GetBlockCount()} brick(s))");
    }

    private bool ToCoords(Vector3 pos, out byte x, out byte y, out byte z)
    {
        if (pos.x < 0 || pos.y < 0 || pos.z < 0 || pos.x > 255 || pos.y > 255 || pos.z > 255)
        {
            x = 0; y = 0; z = 0;
            return false;
        }
        x = (byte)Mathf.FloorToInt(pos.x);
        y = (byte)Mathf.FloorToInt(pos.y);
        z = (byte)Mathf.FloorToInt(pos.z);
        return true;
    }

    public bool GetRotation(out byte rotation, byte brickIndex)
    {
        if (rotationNormal == null)
        {
            rotation = 0;
            return false;
        }
        Brick brick = BrickManager.Instance.GetBrick(brickIndex);
        if (brick == null)
        {
            rotation = 0;
            return false;
        }
        rotation = 0;
        if (brick.directionable)
        {
            rotation = (byte)((!(rotationNormal == Vector3.forward)) ? ((rotationNormal == Vector3.right) ? 1 : ((rotationNormal == Vector3.back) ? 2 : ((!(rotationNormal == Vector3.left)) ? cameraRotation : 3))) : 0);
        }
        return true;
    }
    public void GetPos1(out byte x, out byte y, out byte z)
    {
        x = x1;
        y = y1;
        z = z1;
    }
    public void GetPos2(out byte x, out byte y, out byte z)
    {
        x = x2;
        y = y2;
        z = z2;
    }
    public int GetBlockCount()
    {
        if (!hasPos1 || !hasPos2)
        {
            return 0;
        }
        int width = Math.Abs(x2 - x1) + 1;
        int height = Math.Abs(y2 - y1) + 1;
        int depth = Math.Abs(z2 - z1) + 1;
        return width * height * depth;
    }

    public void ClearSelection()
    {
        Actor.Instance.SendChat($"[BrickEdit] Selection cleared");
        hasPos1 = false; 
        hasPos2 = false;
        ClearWireframePreview();
    }

    private GameObject PopDummy(Vector3 pos)
    {
        GameObject go = null;
        if (invisible.Count > 0)
        {
            go = invisible.Peek();
            invisible.Dequeue();
        }
        if (go == null)
        {
            if (dummyPrefab == null)
                return null;

            go = (UnityEngine.Object.Instantiate((UnityEngine.Object)dummyPrefab) as GameObject);
        }

        go.transform.position = pos;
        var mr = go.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = true;
        return go;
    }

    private void PushDummy(GameObject dummy)
    {
        if (dummy == null) return;
        var mr = dummy.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
        invisible.Enqueue(dummy);
    }

    public void ClearWireframePreview()
    {
        if (wire == null) return;
        while (wire.Count > 0)
            PushDummy(wire.Dequeue());

        if (usedPoints != null)
            usedPoints.Clear();
    }

    private void PushPoint(int x, int y, int z)
    {
        // pack coords into key to avoid duplicates (x/y/z are 0..255)
        int key = (x & 0x3FF) | ((y & 0x3FF) << 10) | ((z & 0x3FF) << 20);
        if (!usedPoints.Add(key))
            return;

        GameObject go = PopDummy(new Vector3((float)x, (float)y, (float)z));
        if (go != null)
            wire.Enqueue(go);
    }

    public static void Swap<T>(ref T x, ref T y)
    {
        T val = y;
        y = x;
        x = val;
    }

    private void Draw3DLine(int x0, int y0, int z0, int x1, int y1, int z1)
    {
        bool flag = Mathf.Abs(y1 - y0) > Mathf.Abs(x1 - x0);
        if (flag)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }

        bool flag2 = Mathf.Abs(z1 - z0) > Mathf.Abs(x1 - x0);
        if (flag2)
        {
            Swap(ref x0, ref z0);
            Swap(ref x1, ref z1);
        }

        int num = Mathf.Abs(x1 - x0);
        int num2 = Mathf.Abs(y1 - y0);
        int num3 = Mathf.Abs(z1 - z0);
        int num4 = num / 2;
        int num5 = num / 2;

        int num6 = (x0 <= x1) ? 1 : (-1);
        int num7 = (y0 <= y1) ? 1 : (-1);
        int num8 = (z0 <= z1) ? 1 : (-1);

        int num9 = y0;
        int num10 = z0;

        // IMPORTANT: we call Draw3DLine with min->max endpoints for the major axis
        for (int i = x0; i <= x1; i += num6)
        {
            int x2 = i;
            int y2 = num9;
            int z2 = num10;

            if (flag2) Swap(ref x2, ref z2);
            if (flag) Swap(ref x2, ref y2);

            PushPoint(x2, y2, z2);

            num4 -= num2;
            num5 -= num3;

            if (num4 < 0)
            {
                num9 += num7;
                num4 += num;
            }
            if (num5 < 0)
            {
                num10 += num8;
                num5 += num;
            }
        }
    }

    private void UpdateWireframePreview()
    {
        ClearWireframePreview();

        if (!hasPos1 || !hasPos2)
            return;

        int minX = Math.Min(x1, x2);
        int maxX = Math.Max(x1, x2);
        int minY = Math.Min(y1, y2);
        int maxY = Math.Max(y1, y2);
        int minZ = Math.Min(z1, z2);
        int maxZ = Math.Max(z1, z2);

        // Bottom rectangle (z = minZ)
        Draw3DLine(minX, minY, minZ, maxX, minY, minZ);
        Draw3DLine(minX, maxY, minZ, maxX, maxY, minZ);
        Draw3DLine(minX, minY, minZ, minX, maxY, minZ);
        Draw3DLine(maxX, minY, minZ, maxX, maxY, minZ);

        // Top rectangle (z = maxZ)
        Draw3DLine(minX, minY, maxZ, maxX, minY, maxZ);
        Draw3DLine(minX, maxY, maxZ, maxX, maxY, maxZ);
        Draw3DLine(minX, minY, maxZ, minX, maxY, maxZ);
        Draw3DLine(maxX, minY, maxZ, maxX, maxY, maxZ);

        // Vertical edges
        Draw3DLine(minX, minY, minZ, minX, minY, maxZ);
        Draw3DLine(minX, maxY, minZ, minX, maxY, maxZ);
        Draw3DLine(maxX, minY, minZ, maxX, minY, maxZ);
        Draw3DLine(maxX, maxY, minZ, maxX, maxY, maxZ);
    }

    public override void OnClose()
    {
        ClearWireframePreview();
    }
}