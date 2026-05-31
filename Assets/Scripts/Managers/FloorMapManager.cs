using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MapNode
{
    public int floor;
    public int index;
    public FloorManager.FloorType type;
    public Vector2 position;
    
    public List<MapNode> nextNodes = new List<MapNode>(); 
    
    public GameObject gameObject;
    public Image backgroundImage;
    public Button button;
}

public class FloorMapManager : MonoBehaviour
{
    [Header("Map Ayarları")]
    public RectTransform mapContent;

    [Header("Düğüm Renkleri")]
    public Color savasColor = new Color(0.7f, 0.2f, 0.2f);
    public Color eliteColor = new Color(0.47f, 0.08f, 0.08f);
    public Color shopColor = new Color(0.2f, 0.59f, 0.2f);
    public Color hazineColor = new Color(0.7f, 0.59f, 0.08f);
    public Color dinlenmeColor = new Color(0.08f, 0.39f, 0.7f);
    public Color bossColor = new Color(0.59f, 0.08f, 0.59f);
    public Color finalBossColor = new Color(0.78f, 0f, 0f);
    public Color completedColor = new Color(0.24f, 0.24f, 0.24f);
    public Color currentColor = new Color(1f, 1f, 1f);

    [Header("Düğüm Boyutu")]
    public float nodeSize = 80f;
    public float floorHeight = 160f;
    public float horizontalSpacing = 250f;
    public float bottomPadding = 100f;

    private List<List<MapNode>> allNodes = new List<List<MapNode>>();
    private FloorManager floorManager;
    private ScrollRect scrollRect;

    void Start()
    {
        floorManager = FloorManager.Instance;
        
        if (floorManager == null)
        {
            GameObject fm = new GameObject("FloorManager");
            fm.AddComponent<FloorManager>();
            floorManager = FloorManager.Instance;
        }

        scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect == null)
            scrollRect = FindFirstObjectByType<ScrollRect>();

        GenerateGraph();
        DrawGraph();

        Canvas.ForceUpdateCanvases();
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }

    void GenerateGraph()
    {
        // --- HATA ÇÖZÜMÜ: SABİT TOHUM HAFIZASI ---
        // Savaş içindeki vuruş ihtimallerini bozmamak için eski rastgeleliği (state) kaydediyoruz.
        Random.State oldState = Random.state;
        
        // Zarları, FloorManager'daki kalıcı şifreye (mapSeed) göre atmaya zorluyoruz.
        // Bu sayede sahne 1000 kere de yüklense, oklar BİREBİR aynı çizilecek!
        Random.InitState(floorManager.mapSeed);

        allNodes.Clear();
        int totalFloors = floorManager.GetTotalFloors();

        for (int floor = 0; floor < totalFloors; floor++)
        {
            List<FloorManager.FloorType> options = floorManager.GetFloorOptions(floor);
            List<MapNode> floorNodes = new List<MapNode>();
            
            float yPos = bottomPadding + floor * floorHeight;

            for (int i = 0; i < options.Count; i++)
            {
                float xPos = GetXPosition(i, options.Count);
                MapNode newNode = new MapNode
                {
                    floor = floor, index = i, type = options[i], position = new Vector2(xPos, yPos)
                };
                floorNodes.Add(newNode);
            }
            allNodes.Add(floorNodes);
        }

        for (int floor = 0; floor < totalFloors - 1; floor++)
        {
            List<MapNode> currFloor = allNodes[floor];
            List<MapNode> nextFloor = allNodes[floor + 1];

            if (nextFloor.Count == 1)
            {
                foreach (var node in currFloor) 
                    if (!node.nextNodes.Contains(nextFloor[0])) 
                        node.nextNodes.Add(nextFloor[0]);
                continue;
            }

            foreach (var nextNode in nextFloor)
            {
                MapNode randomParent = currFloor[Random.Range(0, currFloor.Count)];
                if (!randomParent.nextNodes.Contains(nextNode))
                    randomParent.nextNodes.Add(nextNode);
            }

            foreach (var currNode in currFloor)
            {
                if (currNode.nextNodes.Count == 0)
                {
                    MapNode randomTarget = nextFloor[Random.Range(0, nextFloor.Count)];
                    currNode.nextNodes.Add(randomTarget);
                }
                
                if (Random.value > 0.6f)
                {
                    MapNode randomTarget = nextFloor[Random.Range(0, nextFloor.Count)];
                    if (!currNode.nextNodes.Contains(randomTarget))
                        currNode.nextNodes.Add(randomTarget);
                }
            }
        }
        
        // Okları çizme işlemi bitti, oyunun geri kalanı için rastgeleliği serbest bırakıyoruz.
        Random.state = oldState;
    }

    void DrawGraph()
    {
        foreach (Transform child in mapContent) Destroy(child.gameObject);

        int totalFloors = allNodes.Count;
        float totalHeight = totalFloors * floorHeight + bottomPadding * 2;
        mapContent.sizeDelta = new Vector2(mapContent.sizeDelta.x, totalHeight);

        foreach (var floorList in allNodes)
        {
            foreach (var node in floorList)
            {
                foreach (var targetNode in node.nextNodes)
                {
                    DrawLine(node.position, targetNode.position);
                }
            }
        }

        foreach (var floorList in allNodes)
        {
            foreach (var node in floorList)
            {
                CreateNodeUI(node);
            }
        }

        UpdateNodeStates();
    }

    float GetXPosition(int index, int total)
    {
        if (total == 1) return 0f;
        if (total == 2) return index == 0 ? -horizontalSpacing / 2 : horizontalSpacing / 2;
        return (index - 1) * horizontalSpacing; 
    }

    void CreateNodeUI(MapNode node)
    {
        GameObject obj = new GameObject($"Node_Floor{node.floor}_{node.type}");
        obj.transform.SetParent(mapContent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f); rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(nodeSize, nodeSize);
        rect.anchoredPosition = node.position;

        Image bg = obj.AddComponent<Image>();
        bg.color = GetNodeColor(node.type);

        Button btn = obj.AddComponent<Button>();
        btn.onClick.AddListener(() => OnNodeClicked(node));

        GameObject textObj = new GameObject("IconText");
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero; textRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = GetNodeIcon(node.type);
        text.fontSize = 16;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        GameObject floorTextObj = new GameObject("FloorText");
        floorTextObj.transform.SetParent(obj.transform, false);
        RectTransform floorTextRect = floorTextObj.AddComponent<RectTransform>();
        floorTextRect.anchorMin = new Vector2(0, 0); floorTextRect.anchorMax = new Vector2(1, 0);
        floorTextRect.pivot = new Vector2(0.5f, 1f); floorTextRect.sizeDelta = new Vector2(0, 25);
        floorTextRect.anchoredPosition = new Vector2(0, -5);

        TextMeshProUGUI floorText = floorTextObj.AddComponent<TextMeshProUGUI>();
        floorText.text = $"Kat {node.floor + 1}";
        floorText.fontSize = 12;
        floorText.alignment = TextAlignmentOptions.Center;
        floorText.color = Color.white;

        node.gameObject = obj;
        node.backgroundImage = bg;
        node.button = btn;
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject line = new GameObject("Line");
        line.transform.SetParent(mapContent, false);
        line.transform.SetAsFirstSibling();

        RectTransform rect = line.AddComponent<RectTransform>();
        Image img = line.AddComponent<Image>();
        img.color = new Color(0.4f, 0.4f, 0.6f, 0.8f);

        Vector2 dir = end - start;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        rect.anchorMin = new Vector2(0.5f, 0f); rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(distance, 4f);
        rect.anchoredPosition = start + dir / 2f;
        rect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void UpdateNodeStates()
    {
        int currentFloor = floorManager.currentFloor;
        int prevIndex = floorManager.currentNodeIndex;

        foreach (var floorList in allNodes)
        {
            foreach (var node in floorList)
            {
                if (node.floor < currentFloor)
                {
                    node.backgroundImage.color = completedColor;
                    node.button.interactable = false;
                }
                else if (node.floor == currentFloor)
                {
                    bool isInteractable = false;

                    if (currentFloor == 0 || prevIndex == -1)
                    {
                        isInteractable = true; 
                    }
                    else
                    {
                        MapNode prevNode = allNodes[currentFloor - 1][prevIndex];
                        if (prevNode.nextNodes.Contains(node))
                        {
                            isInteractable = true;
                        }
                    }

                    node.button.interactable = isInteractable;

                    if (isInteractable)
                    {
                        node.backgroundImage.color = GetNodeColor(node.type);
                        Outline outline = node.gameObject.GetComponent<Outline>();
                        if (outline == null) outline = node.gameObject.AddComponent<Outline>();
                        outline.effectColor = currentColor;
                        outline.effectDistance = new Vector2(4, 4);
                    }
                    else
                    {
                        Color c = GetNodeColor(node.type);
                        c.a = 0.3f;
                        node.backgroundImage.color = c;
                        
                        Outline outline = node.gameObject.GetComponent<Outline>();
                        if (outline != null) Destroy(outline);
                    }
                }
                else
                {
                    node.button.interactable = false;
                    Color c = GetNodeColor(node.type);
                    c.a = 0.3f;
                    node.backgroundImage.color = c;
                }
            }
        }
    }

    Color GetNodeColor(FloorManager.FloorType floorType)
    {
        switch (floorType)
        {
            case FloorManager.FloorType.Savas: return savasColor;
            case FloorManager.FloorType.Elite: return eliteColor;
            case FloorManager.FloorType.Shop: return shopColor;
            case FloorManager.FloorType.Hazine: return hazineColor;
            case FloorManager.FloorType.Dinlenme: return dinlenmeColor;
            case FloorManager.FloorType.Boss: return bossColor;
            case FloorManager.FloorType.FinalBoss: return finalBossColor;
            default: return Color.white;
        }
    }

    string GetNodeIcon(FloorManager.FloorType floorType)
    {
        switch (floorType)
        {
            case FloorManager.FloorType.Savas: return "SAVAS";
            case FloorManager.FloorType.Elite: return "ELITE";
            case FloorManager.FloorType.Shop: return "SHOP";
            case FloorManager.FloorType.Hazine: return "HAZINE";
            case FloorManager.FloorType.Dinlenme: return "DIN";
            case FloorManager.FloorType.Boss: return "BOSS";
            case FloorManager.FloorType.FinalBoss: return "FINAL";
            default: return "?";
        }
    }

    void OnNodeClicked(MapNode node)
    {
        if (node.floor != floorManager.currentFloor) return;
        floorManager.SelectFloor(node.type, node.index);
    }
}