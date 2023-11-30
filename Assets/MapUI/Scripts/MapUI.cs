using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Radishmouse;
using System;
using DG.Tweening;
using AYellowpaper.SerializedCollections;

public class MapUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MapNodeUI _node_ui_prefab;
    [SerializeField] MapRowUI _row_ui_prefab;
    [SerializeField] MapLineUI _map_line_ui;
    [SerializeField] RectTransform _content_parent;
    [SerializeField] MapInfo _map_info;
    [SerializeField] RectTransform _player_icon;
    [SerializeField] ScrollRect _scroll_rect;
    [SerializeField] CanvasGroup _canvasGroup;
    bool isShow = true;
    Tween _mapTween;
    Tween _player_tween;

    [SerializeField] List<MapRowUI> _mapRows = new List<MapRowUI>();
    [SerializeField] List<MapLineUI> _mapLines = new List<MapLineUI>();

    public List<MapRowUI> MapRows { get => _mapRows; set => _mapRows = value; }

    void LoadNewMap(MapInfo mapInfo)
    {
        //Start player at the first node, lock the node 
        StartCoroutine(ieGenerateMap(mapInfo, () => {
            var _nodeToStart = _mapRows[0]._nodes[0];
            _player_icon.SetAsLastSibling();
            _player_icon.position = _nodeToStart.transform.position;//Move player to first node always
            UnlockNextNode(_nodeToStart);//WIP Need to do something
        }));
    }

    IEnumerator ieGenerateMap(MapInfo mapInfo,Action onComplete = null)
    {
        //Clear Data
        ClearMap();
        ///////////////////////////////
      
        for (int i =0;i< mapInfo.rows.Count; i++)
        {
            var r = GenerateRow(mapInfo.rows[i], i);
            _mapRows.Add(r);
            if(i!=0) r.offsetChilds();
        }

        yield return new WaitForEndOfFrame();
        GenerateLine();
        onComplete?.Invoke();

    }

    MapRowUI GenerateRow(RowSetting rowSetting, int rowIndex)
    {  
        var rowUI = Instantiate(_row_ui_prefab);
        rowUI.transform.SetParent(_content_parent.transform,false);
        rowUI.transform.localScale = new(1, 1, 1);

        int rand = UnityEngine.Random.Range(rowSetting.minMaxNode.x, rowSetting.minMaxNode.y + 1);//Random how many nodes
        //Debug.Log(rowSetting.minNode + " " + rowSetting.maxNode + " " + rand);
        List<NodeInfo> tempChoosingNode = new List<NodeInfo>(rowSetting.fixedNodes);
        
        if (tempChoosingNode.Count > rand) 
        {
            tempChoosingNode.Shuffle();
            tempChoosingNode.RemoveRange(0, tempChoosingNode.Count - rand); 
        }
        else if (tempChoosingNode.Count < rand)
        {
            var left = rand - tempChoosingNode.Count;
            for (int i = 0; i < left; i++)
            {
                tempChoosingNode.Add( RandomPossibleNode(rowSetting.possibleNodes));
            }
            tempChoosingNode.Shuffle();
        }

        for (int i = 0; i < rand; i++)//Generate node
        {      
            MapNodeUI nodeUI = Instantiate(_node_ui_prefab);
            nodeUI.row = rowIndex;
            //nodeUI.NodeInfo = RandomPossibleNode(rowSetting.possibleNodes);
            nodeUI.NodeInfo = tempChoosingNode[i];
            nodeUI.EncounterData =RandomPossibleEncounter(nodeUI.NodeInfo.nodeType , rowSetting);

            if (nodeUI.NodeInfo.iconGameObject != null)
            {
                nodeUI.SetIconPrefab(nodeUI.NodeInfo.iconGameObject);
            }
            else
            {
                nodeUI.SetImage(nodeUI.NodeInfo.icon);
            }

            nodeUI.transform.SetParent(rowUI.transform,false);
            nodeUI.transform.localScale = new Vector3(1, 1, 1);
            nodeUI.onClick = OnClickMapNode;
            nodeUI.SetLock(true);
            rowUI._nodes.Add(nodeUI);
        }
        return rowUI;
    }

    //Weight random node
    NodeInfo RandomPossibleNode(SerializedDictionary<NodeInfo, int> possibleNodes)
    {
        int amount = possibleNodes.Count;
        NodeInfo[] tempNodes = new NodeInfo[amount];
        int[] sum = new int[amount];

        int x = 0;
        int i = 0;
        foreach (var node in possibleNodes)
        {
            tempNodes[i] = node.Key;
            sum[i] = x += node.Value;
            Debug.Log(sum[i]);
            i++;
        }
        int choose = UnityEngine.Random.Range(0, x+1);

        for(int j = 0; j <sum.Length; j++)
        {

            Debug.Log(choose +": =>"+ sum[j]);
            if (choose <= sum[j]) return tempNodes[j];
        }

        return null;
    }

    EncounterData RandomPossibleEncounter(NodeType nodeType, RowSetting rowSetting)
    {
        List<EncounterData> encounterData;
        if (rowSetting.possibleEncounters.TryGetValue(nodeType, out encounterData))
        {
            int rand = UnityEngine.Random.Range(0,encounterData.Count);
            return encounterData[rand];
        }
        else return null;
    }

    void ClearMap()
    {
        //Clear Data
        foreach (var row in _mapRows)
        {
            Destroy(row.gameObject);
        }
        foreach (var line in _mapLines)
        {
            Destroy(line.gameObject);
        }
        _mapRows.Clear();
        _mapLines.Clear();
    }

    void GenerateLine()
    {
        //Connect line
        for(int row = _mapRows.Count-1; row>0; row--)//start from the last row
        {
            for (int j = 0; j < _mapRows[row]._nodes.Count; j++) {
                int n = UnityEngine.Random.Range(-1, 2);
                int pick = 0;
                int prevRowCount = _mapRows[row-1]._nodes.Count;

                if (j + n < 0) pick = 0;
                else if ((j + n) >= prevRowCount) pick = prevRowCount - 1;
                else pick = j + n;

                MapNodeUI endNode = _mapRows[row]._nodes[j];
                MapNodeUI startNode = _mapRows[row - 1]._nodes[pick];

                endNode.PrevNodes.Add(startNode);
                startNode.NextNodes.Add(endNode);

                //Debug.Log("On row " + (row+1) +" Node " + (j+1) + " pick " + (pick+1));
                //Debug.Log("1:" + RectTransformUtility.WorldToScreenPoint(Camera.main, _mapRows[row - 1]._nodes[pick].Parent_img.transform.position) +
                //    " 2:" + RectTransformUtility.WorldToScreenPoint(Camera.main, _mapRows[row]._nodes[j].Parent_img.transform.position));
                var line = DrawLine(startNode.Parent_img.transform.position, endNode.Parent_img.transform.position);
                startNode.Lines.Add(line);

            }
        }

        for (int row = 0; row < _mapRows.Count-1; row++)//start from the first row
        {
            for (int j = 0; j < _mapRows[row]._nodes.Count; j++)
            {
                if (_mapRows[row]._nodes[j].NextNodes.Count == 0)
                {
                    int n = UnityEngine.Random.Range(-1, 2);
                    int pick = 0;
                    int nextRowCount = _mapRows[row + 1]._nodes.Count;

                    if (j + n < 0) pick = 0;
                    else if ((j + n) >= nextRowCount) pick = nextRowCount - 1;
                    else pick = j + n;

                    MapNodeUI startNode = _mapRows[row]._nodes[j];
                    MapNodeUI endNode = _mapRows[row + 1]._nodes[pick];

                    endNode.PrevNodes.Add(startNode);
                    startNode.NextNodes.Add(endNode);

                    startNode.NextNodes.Add(endNode);
                    endNode.PrevNodes.Add(startNode);

                    var line = DrawLine(startNode.Parent_img.transform.position, endNode.Parent_img.transform.position);
                    startNode.Lines.Add(line);
                }
            }
        }

    }

    void OnClickMapNode(MapNodeUI selectedNode)
    {
        if (selectedNode.isLock) return;

        Debug.Log("Click " + selectedNode.NodeInfo.nodeType);

        //Lock same node row
        foreach(var node in _mapRows[selectedNode.row]._nodes)
        {
            if (node == selectedNode) node.SetLock(true);
            else node.Disable();
        }

        DisablePrevNode(selectedNode);

        _scroll_rect.enabled = false;
        _player_tween = _player_icon.DOMove(selectedNode.Parent_img.transform.position,0.4f).OnComplete(()=> {
            _scroll_rect.enabled = true;
            StartEncounter(selectedNode,()=> { UnlockNextNode(selectedNode); });
            
        });
    }

    /// <summary>
    /// Start event,shop,encouter scene here
    /// </summary>
    /// <param name="selectedNode"></param>
    /// <param name="onWin">what you want to do after win</param>
    void StartEncounter(MapNodeUI selectedNode, Action onWin = null)
    {
        if(selectedNode.NodeInfo.nodeType == NodeType.Start)
        {
            UnlockNextNode(_mapRows[0]._nodes[0]);
        }
        else if (selectedNode.NodeInfo.nodeType == NodeType.Minor)
        {
            //Start Minor Encounter
            // Do something with selectedNode.EncounterData
        }
        else if (selectedNode.NodeInfo.nodeType == NodeType.Elite)
        {
            //Start Elite Encounter
            // Do something with selectedNode.EncounterData
        }
        else if (selectedNode.NodeInfo.nodeType == NodeType.Boss)
        {
            //Start Boss Encounter
            // Do something with selectedNode.EncounterData
        }

        UnlockNextNode(selectedNode);
        Debug.Log("<b>Load Encounter</b> " + selectedNode.NodeInfo.nodeType);
    }

    #region helper
    void UnlockNextNode(MapNodeUI mapNode)
    {
        foreach (var node in mapNode.NextNodes)
        {
            node.Unlock();
        }
    }

    void DisablePrevNode(MapNodeUI mapNode)
    {
        foreach (var node in mapNode.PrevNodes)
        {
            node.Disable();
        }
    }
    #endregion

    public MapLineUI DrawLine(Vector3 start, Vector3 end)
    {
        var p1 = RectTransformUtility.WorldToScreenPoint(Camera.main, start);
        var p2 = RectTransformUtility.WorldToScreenPoint(Camera.main, end);


        RectTransformUtility.ScreenPointToLocalPointInRectangle(_content_parent, p1, Camera.main, out p1);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_content_parent, p2, Camera.main, out p2);
        //Debug.Log(p1 + " " + p2);

        return CreateLine(p1, p2);
    
    }

    MapLineUI CreateLine()
    {
        var line = Instantiate(_map_line_ui);
        line.transform.SetParent(_content_parent.transform, false);
        line.transform.localScale = new(1, 1, 1);
        line._rect.anchoredPosition = new Vector2(0, 0);
        line.transform.SetAsFirstSibling();
        _mapLines.Add(line);
        return line;
    }

    MapLineUI CreateLine(Vector2 start, Vector2 end)
    {
        var line = CreateLine();
        line.SetLine(start, end);
        return line;
    }

    void Show()
    {
        _mapTween = _canvasGroup.DOFade(1, 1).
            OnComplete(()=> { isShow = true; });
    }

    void Hide()
    {
        isShow = false;
        _canvasGroup.alpha = 0;
    }

    public void Toggle()
    {
        if (isShow) Hide();
        else Show();
    }

    public void Btn_GenerateMap()
    {
        //StartCoroutine(ieGenerateMap(_map_info));
        LoadNewMap(_map_info);
    }

    public void Btn_GenerateLine()
    {
        // CreateLine();
        //GenerateLine();
    }
}
