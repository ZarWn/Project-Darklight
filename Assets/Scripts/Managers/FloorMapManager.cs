using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    private List<List<GameObject>> nodeObjects = new List<List<GameObject>>();
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

        GenerateMap();

        // En alta scroll et (başlangıç noktası altta)
        Canvas.ForceUpdateCanvases();
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }

    void GenerateMap()
    {
        foreach (Transform child in mapContent)
            Destroy(child.gameObject);
        nodeObjects.Clear();

        int totalFloors = floorManager.GetTotalFloors();

        // Content yüksekliğini ayarla
        float totalHeight = totalFloors * floorHeight + bottomPadding * 2;
        mapContent.sizeDelta = new Vector2(mapContent.sizeDelta.x, totalHeight);

        // Önce çizgileri oluştur (arkada kalsın)
        List<List<Vector2>> positions = new List<List<Vector2>>();

        for (int floor = 0; floor < totalFloors; floor++)
        {
            List<FloorManager.FloorType> options = floorManager.GetFloorOptions(floor);
            List<Vector2> floorPositions = new List<Vector2>();

            float yPos = bottomPadding + floor * floorHeight;

            for (int i = 0; i < options.Count; i++)
            {
                float xPos = GetXPosition(i, options.Count);
                floorPositions.Add(new Vector2(xPos, yPos));
            }

            positions.Add(floorPositions);
        }

        // Çizgileri çiz (önce, arkada kalsın)
        for (int floor = 1; floor < totalFloors; floor++)
        {
            foreach (Vector2 currentPos in positions[floor])
            {
                foreach (Vector2 prevPos in positions[floor - 1])
                {
                    DrawLine(prevPos, currentPos);
                }
            }
        }

        // Düğümleri oluştur (çizgilerin üstünde)
        for (int floor = 0; floor < totalFloors; floor++)
        {
            List<FloorManager.FloorType> options = floorManager.GetFloorOptions(floor);
            List<GameObject> floorNodes = new List<GameObject>();

            for (int i = 0; i < options.Count; i++)
            {
                GameObject node = CreateNode(
                    options[i],
                    positions[floor][i],
                    floor
                );
                floorNodes.Add(node);
            }

            nodeObjects.Add(floorNodes);
        }

        UpdateNodeStates();
    }

    float GetXPosition(int index, int total)
    {
        if (total == 1) return 0f;
        if (total == 2)
            return index == 0 ? -horizontalSpacing / 2 : horizontalSpacing / 2;
        return (index - 1) * horizontalSpacing;
    }

    GameObject CreateNode(FloorManager.FloorType floorType, Vector2 position, int floor)
    {
        GameObject node = new GameObject($"Node_Floor{floor}_{floorType}");
        node.transform.SetParent(mapContent, false);

        RectTransform rect = node.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(nodeSize, nodeSize);
        rect.anchoredPosition = position;

        Image bg = node.AddComponent<Image>();
        bg.color = GetNodeColor(floorType);

        Button button = node.AddComponent<Button>();
        int capturedFloor = floor;
        FloorManager.FloorType capturedType = floorType;
        button.onClick.AddListener(() => OnNodeClicked(capturedFloor, capturedType));

        // İkon text
        GameObject textObj = new GameObject("NodeText");
        textObj.transform.SetParent(node.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = GetNodeIcon(floorType);
        text.fontSize = 16;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        // Kat numarası text
        GameObject floorTextObj = new GameObject("FloorText");
        floorTextObj.transform.SetParent(node.transform, false);
        RectTransform floorTextRect = floorTextObj.AddComponent<RectTransform>();
        floorTextRect.anchorMin = new Vector2(0f, 0f);
        floorTextRect.anchorMax = new Vector2(1f, 0f);
        floorTextRect.pivot = new Vector2(0.5f, 1f);
        floorTextRect.sizeDelta = new Vector2(0, 25);
        floorTextRect.anchoredPosition = new Vector2(0, -5);

        TextMeshProUGUI floorText = floorTextObj.AddComponent<TextMeshProUGUI>();
        floorText.text = $"Kat {floor + 1}";
        floorText.fontSize = 12;
        floorText.alignment = TextAlignmentOptions.Center;
        floorText.color = Color.white;

        return node;
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

        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(distance, 4f);
        rect.anchoredPosition = start + dir / 2f;
        rect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void UpdateNodeStates()
    {
        int currentFloor = floorManager.currentFloor;

        for (int floor = 0; floor < nodeObjects.Count; floor++)
        {
            foreach (GameObject node in nodeObjects[floor])
            {
                Image bg = node.GetComponent<Image>();
                Button button = node.GetComponent<Button>();

                if (floor < currentFloor)
                {
                    bg.color = completedColor;
                    button.interactable = false;
                }
                else if (floor == currentFloor)
                {
                    button.interactable = true;

                    Outline outline = node.AddComponent<Outline>();
                    outline.effectColor = currentColor;
                    outline.effectDistance = new Vector2(4, 4);
                }
                else
                {
                    button.interactable = false;
                    Color c = bg.color;
                    c.a = 0.3f;
                    bg.color = c;
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

    void OnNodeClicked(int floor, FloorManager.FloorType floorType)
    {
        if (floor != floorManager.currentFloor) return;
        Debug.Log($"Kat {floor + 1} seçildi: {floorType}");
        floorManager.SelectFloor(floorType);
    }
}