using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using _Emulator;

public class PerfOverlay : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.F9;
    public bool visible = false;

    public float textUpdateInterval = 0.25f;   // UI text refresh rate
    public float memoryUpdateInterval = 0.5f;  // memory sampling rate
    public float bfUpdateInterval = 0.25f;     // BrickForce/server sampling rate

    public float fpsSmoothing = 0.1f;

    public int baseWidth = 360;
    public int margin = 10;
    public int padding = 8;

    private float dtSmoothed;
    private float fps;
    private float lastTextUpdate;
    private float lastMemUpdate;
    private float lastBfUpdate;

    private int fixedCount;
    private float fixedTimer;
    private float fixedFps;

    private long managedBytes;
    private long unityAllocBytes = -1;
    private long unityReservedBytes = -1;
    private long unityUnusedReservedBytes = -1;

    private bool gcCollectionCountAvailable;
    private int gc0, gc1, gc2;

    // Cached text lines (avoid string churn every OnGUI)
    private string line1, line2, line3, line4, line5, line6;

    // Frame time spike tracking
    private const int SpikeSamples = 120;
    private float[] frameMs = new float[SpikeSamples];
    private int frameMsIndex;
    private int frameMsCount;
    private float frameMaxMs;
    private float frameP99Ms;
    private float lastSpikeCalc;

    private GUIStyle textStyle;
    private GUIStyle bgStyle;
    private Texture2D bgTex;

    private MethodInfo miAlloc, miReserved, miUnused;

    private bool bfBound;
    private object serverInstance;
    private FieldInfo fiClientList, fiReadQueue, fiWriteQueue, fiRegMaps;

    private int bfClients = -1;
    private int bfReadQ = -1;
    private int bfWriteQ = -1;
    private int bfRegMaps = -1;

    public int graphHeight = 60;
    public int graphSamples = 120;
    public bool graphShowFps = false;    // false = ms graph, true = FPS graph

    private Texture2D whiteTex;          // 1x1 for drawing lines/bars

    private float fps1Low;


    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        BindProfilerAPI();
        BindGCApi();
        BindBrickForceServer();
        BuildBackground();
        CacheLines();
    }

    void Update()
    {
        if (!visible)
        {
            if (Input.GetKeyDown(toggleKey))
            {
                visible = true;
                lastTextUpdate = lastMemUpdate = lastBfUpdate = 0f;
            }
            return;
        }

        if (Input.GetKeyDown(toggleKey))
        {
            visible = false;
            return;
        }

        // FPS smoothing + frame ms tracking
        float dt = Time.deltaTime;
        dtSmoothed = Mathf.Lerp(dtSmoothed, dt, fpsSmoothing);
        fps = dtSmoothed > 0.00001f ? (1f / dtSmoothed) : 0f;

        float ms = dt * 1000f;
        TrackFrameTime(ms);

        // Timed updates
        float now = Time.realtimeSinceStartup;

        if (now - lastMemUpdate >= memoryUpdateInterval)
        {
            lastMemUpdate = now;
            SampleMemory();
        }

        if (now - lastBfUpdate >= bfUpdateInterval)
        {
            lastBfUpdate = now;
            SampleBrickForce();
        }

        if (now - lastTextUpdate >= textUpdateInterval)
        {
            lastTextUpdate = now;
            CacheLines();
        }
    }

    void FixedUpdate()
    {
        if (!visible) return;

        fixedCount++;
        fixedTimer += Time.fixedDeltaTime;

        if (fixedTimer >= 1f)
        {
            fixedFps = fixedCount / fixedTimer;
            fixedCount = 0;
            fixedTimer = 0f;
        }
    }

    void OnGUI()
    {
        if (!visible) return;

        InitStyles();

        float scale = Mathf.Clamp(Screen.height / 720f, 0.85f, 1.6f);

        int width = Mathf.RoundToInt(baseWidth * scale);
        float lineH = Mathf.Round(18f * scale);

        float gap = Mathf.Round(6f * scale);
        float graphPlotH = Mathf.Round(graphHeight * scale);
        float graphBlockH = lineH + graphPlotH;

        int lines = 0;
        if (!string.IsNullOrEmpty(line1)) lines++;
        if (!string.IsNullOrEmpty(line2)) lines++;
        if (!string.IsNullOrEmpty(line3)) lines++;
        if (!string.IsNullOrEmpty(line4)) lines++;
        if (!string.IsNullOrEmpty(line5)) lines++;
        if (!string.IsNullOrEmpty(line6)) lines++;

        float textH = lines * lineH;

        float panelH = padding * 2 + textH + graphBlockH;

        float x = Screen.width - width - margin;
        float y = margin;

        Rect panelRect = new Rect(x, y, width, panelH);

        GUI.Box(panelRect, GUIContent.none, bgStyle);

        float tx = x + padding;
        float ty = y + padding;

        DrawLine(tx, ty, line1, GetFpsColor(fps), width); ty += lineH;
        DrawLine(tx, ty, line2, Color.white, width); ty += lineH;
        DrawLine(tx, ty, line3, Color.white, width); ty += lineH;

        if (!string.IsNullOrEmpty(line4)) { DrawLine(tx, ty, line4, Color.white, width); ty += lineH; }
        if (!string.IsNullOrEmpty(line5)) { DrawLine(tx, ty, line5, Color.white, width); ty += lineH; }
        if (!string.IsNullOrEmpty(line6)) { DrawLine(tx, ty, line6, Color.white, width); ty += lineH; }

        float gX = x + padding;
        float gW = width - padding * 2;
        float gY = y + padding + textH;

        Rect graphBlock = new Rect(gX, gY, gW, graphBlockH);
        DrawGraph(graphBlock, graphShowFps, lineH);

        GUI.color = Color.white;
    }

    private void DrawGraph(Rect block, bool showFps, float labelH)
    {
        if (whiteTex == null || frameMsCount < 2) return;

        Rect labelRect = new Rect(block.x, block.y, block.width, 22);

        Rect plotRect = new Rect(block.x, block.y + labelH, block.width, block.height - labelH);
        if (plotRect.height < 2f) return;

        string label = showFps ? "FPS history" : "Frame time history (ms)";
        GUI.color = Color.white;
        GUI.Label(labelRect, label, textStyle);

        GUI.color = new Color(0f, 0f, 0f, 0.25f);
        GUI.DrawTexture(plotRect, whiteTex);
        GUI.color = Color.white;

        float maxValue;
        if (!showFps)
            maxValue = Mathf.Max(20f, Mathf.Min(80f, frameP99Ms * 1.2f));
        else
            maxValue = Mathf.Max(60f, Mathf.Min(240f, fps * 1.5f));

        int n = Mathf.Min(frameMsCount, (int)plotRect.width);
        if (n < 2) return;

        float step = (float)frameMsCount / n;
        float barW = plotRect.width / n;

        for (int i = 0; i < n; i++)
        {
            int idx = (frameMsIndex - 1 - Mathf.RoundToInt(i * step));
            while (idx < 0) idx += SpikeSamples;
            idx %= SpikeSamples;

            float ms = frameMs[idx];
            float v = showFps ? ((ms > 0.0001f) ? (1000f / ms) : 0f) : ms;

            float t = Mathf.Clamp01(v / maxValue);
            float h = t * plotRect.height;

            Rect bar = new Rect(
                plotRect.x + (n - 1 - i) * barW,
                plotRect.y + (plotRect.height - h),
                Mathf.Max(1f, barW),
                h
            );

            GUI.color = showFps ? GetFpsColor(v)
                                : GetFpsColor((ms > 0.0001f) ? (1000f / ms) : 0f);

            GUI.DrawTexture(bar, whiteTex);
        }

        GUI.color = Color.white;
    }


    private void DrawLine(float x, float y, string text, Color col, int width)
    {
        if (string.IsNullOrEmpty(text)) return;
        textStyle.normal.textColor = col;
        GUI.Label(new Rect(x, y, width, 22), text, textStyle);
    }

    private void TrackFrameTime(float ms)
    {
        frameMs[frameMsIndex] = ms;
        frameMsIndex = (frameMsIndex + 1) % SpikeSamples;
        frameMsCount = Mathf.Min(frameMsCount + 1, SpikeSamples);

        if (ms > frameMaxMs) frameMaxMs = ms;

        float now = Time.realtimeSinceStartup;
        if (now - lastSpikeCalc >= 1.0f && frameMsCount >= 30)
        {
            lastSpikeCalc = now;

            float[] tmp = new float[frameMsCount];
            for (int i = 0; i < frameMsCount; i++)
                tmp[i] = frameMs[i];

            Array.Sort(tmp);

            // p99 frame time (ms): 99th percentile of frame time
            int p99i = Mathf.Clamp(Mathf.CeilToInt(frameMsCount * 0.99f) - 1, 0, frameMsCount - 1);
            frameP99Ms = tmp[p99i];

            // 1% low FPS: 1st percentile of FPS samples (derived from ms)
            // Convert ms->fps then sort fps
            float[] fpsTmp = new float[frameMsCount];
            for (int i = 0; i < frameMsCount; i++)
            {
                float m = tmp[i];
                fpsTmp[i] = (m > 0.0001f) ? (1000f / m) : 0f;
            }
            Array.Sort(fpsTmp);

            int p1i = Mathf.Clamp(Mathf.CeilToInt(frameMsCount * 0.01f) - 1, 0, frameMsCount - 1);
            fps1Low = fpsTmp[p1i];

            frameMaxMs = 0f;
        }
    }

    private Color GetFpsColor(float f)
    {
        if (f >= 60f) return new Color(0.6f, 1f, 0.6f, 1f);
        if (f >= 30f) return new Color(1f, 0.9f, 0.4f, 1f);
        return new Color(1f, 0.5f, 0.5f, 1f);
    }

    private void SampleMemory()
    {
        managedBytes = GC.GetTotalMemory(false);

        // GC collection counts
        if (gcCollectionCountAvailable)
        {
            try
            {
                gc0 = GC.CollectionCount(0);
                gc1 = GC.CollectionCount(1);
                gc2 = GC.CollectionCount(2);
            }
            catch
            {
                gcCollectionCountAvailable = false;
            }
        }

        // Unity native memory counters
        try
        {
            if (miAlloc != null) unityAllocBytes = Convert.ToInt64(miAlloc.Invoke(null, null));
            if (miReserved != null) unityReservedBytes = Convert.ToInt64(miReserved.Invoke(null, null));
            if (miUnused != null) unityUnusedReservedBytes = Convert.ToInt64(miUnused.Invoke(null, null));
        }
        catch
        {
            unityAllocBytes = unityReservedBytes = unityUnusedReservedBytes = -1;
        }
    }
    private void BindBrickForceServer()
    {
        try
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            Type t = null;
            for (int i = 0; i < asms.Length && t == null; i++)
                t = asms[i].GetType("ServerEmulator", false);

            if (t == null)
                return;

            var fiInstance = t.GetField("instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (fiInstance != null)
                serverInstance = fiInstance.GetValue(null);

            if (serverInstance == null)
            {
                var piInstance = t.GetProperty("instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (piInstance != null)
                    serverInstance = piInstance.GetValue(null, null);
            }

            fiClientList = t.GetField("clientList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            fiReadQueue = t.GetField("readQueue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            fiWriteQueue = t.GetField("writeQueue", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            fiRegMaps = t.GetField("regMaps", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            bfBound = true;
        }
        catch
        {
            bfBound = false;
        }
    }

    private void SampleBrickForce()
    {
        if (!bfBound) return;

        if (serverInstance == null)
        {
            BindBrickForceServer();
            if (serverInstance == null) return;
        }

        try
        {
            bfClients = ReadCountFromCollection(fiClientList, serverInstance);
            bfReadQ = ReadCountFromCollection(fiReadQueue, serverInstance);
            bfWriteQ = ReadCountFromCollection(fiWriteQueue, serverInstance);
            bfRegMaps = ReadCountFromCollection(fiRegMaps, serverInstance);
        }
        catch
        {
            bfClients = bfReadQ = bfWriteQ = bfRegMaps = -1;
        }
    }

    private int ReadCountFromCollection(FieldInfo fi, object obj)
    {
        if (fi == null || obj == null) return -1;
        object col = fi.GetValue(obj);
        if (col == null) return -1;

        var pi = col.GetType().GetProperty("Count", BindingFlags.Public | BindingFlags.Instance);
        if (pi != null)
            return Convert.ToInt32(pi.GetValue(col, null));

        return -1;
    }

    private void CacheLines()
    {
        float ms = dtSmoothed * 1000f;

        line1 = string.Format("FPS: {0:0.}  ({1:0.0} ms)", fps, ms);
        line2 = string.Format("Sim FPS (FixedUpdate): {0:0.}", fixedFps);

        if (frameMsCount > 0)
            line3 = string.Format("1% low: {0:0.} FPS | p99 {1:0.0} ms", fps1Low, frameP99Ms);
        else
            line3 = "Frame spikes: n/a";

        string managed = "Managed: " + FormatBytes(managedBytes);

        if (gcCollectionCountAvailable)
            managed += string.Format("  GC({0}/{1}/{2})", gc0, gc1, gc2);

        line4 = managed;

        if (unityAllocBytes >= 0)
        {
            line5 = "Unity: alloc " + FormatBytes(unityAllocBytes) +
                    "  res " + FormatBytes(unityReservedBytes);

            if (unityUnusedReservedBytes >= 0)
                line6 = "Unity: unused res " + FormatBytes(unityUnusedReservedBytes);
            else
                line6 = "";
        }
        else
        {
            line5 = "Unity: memory counters n/a";
            line6 = "";
        }

        string bf = MakeBrickForceLine();
        if (!string.IsNullOrEmpty(bf))
        {
            if (string.IsNullOrEmpty(line6))
                line6 = bf;
            else
                line6 = line6 + " | " + bf;
        }
    }

    private string MakeBrickForceLine()
    {
        if (bfClients < 0 && bfReadQ < 0 && bfWriteQ < 0 && bfRegMaps < 0)
            return "";

        string s = "BF";
        if (bfClients >= 0) s += " clients " + bfClients;
        if (bfReadQ >= 0) s += " rq " + bfReadQ;
        if (bfWriteQ >= 0) s += " wq " + bfWriteQ;
        if (bfRegMaps >= 0) s += " maps " + bfRegMaps;
        return s;
    }

    private void InitStyles()
    {
        if (textStyle != null) return;

        textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = 13;
        textStyle.normal.textColor = Color.white;

        bgStyle = new GUIStyle(GUI.skin.box);
        bgStyle.border = new RectOffset(0, 0, 0, 0);
        bgStyle.normal.background = bgTex;
    }

    private void BuildBackground()
    {
        bgTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        bgTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
        bgTex.Apply();
        whiteTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        whiteTex.SetPixel(0, 0, Color.white);
        whiteTex.Apply();

    }

    private void BindProfilerAPI()
    {
        try
        {
            Type pt = typeof(Profiler);
            miAlloc = pt.GetMethod("GetTotalAllocatedMemory", BindingFlags.Public | BindingFlags.Static);
            miReserved = pt.GetMethod("GetTotalReservedMemory", BindingFlags.Public | BindingFlags.Static);
            miUnused = pt.GetMethod("GetTotalUnusedReservedMemory", BindingFlags.Public | BindingFlags.Static);
        }
        catch
        {
            miAlloc = miReserved = miUnused = null;
        }
    }

    private void BindGCApi()
    {
        try
        {
            GC.CollectionCount(0);
            gcCollectionCountAvailable = true;
        }
        catch
        {
            gcCollectionCountAvailable = false;
        }
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 0) return "n/a";
        double b = bytes;
        string[] suf = { "B", "KB", "MB", "GB" };
        int i = 0;
        while (b >= 1024 && i < suf.Length - 1) { b /= 1024; i++; }
        return string.Format("{0:0.##} {1}", b, suf[i]);
    }
}
