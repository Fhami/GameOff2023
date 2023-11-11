using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Radishmouse;

public class MapUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MapNodeUI _node_ui_prefab;
    [SerializeField] MapRowUI _row_ui_prefab;
    [SerializeField] MapLineUI _map_line_ui;
    [SerializeField] RectTransform _content_parent;
    [SerializeField] MapInfo _map_info;
    [SerializeField] RectTransform _player_icon;

    [SerializeField] List<MapRowUI> _mapRows = new List<MapRowUI>();

    public List<MapRowUI> MapRows { get => _mapRows; set => _mapRows = value; }

    //List<List<MapNodeUI>> _mapNodes;

    //IEnumerator Start()
    //{
    //    yield return ieGenerateMap(_map_info);
    //}

    IEnumerator ieGenerateMap(MapInfo mapInfo)
    {

        //Clear Data
        foreach (var row in _mapRows)
        {
            Destroy(row.gameObject);
        }
        _mapRows = new List<MapRowUI>();
        ///////////////////////////////

        for(int i =0;i< mapInfo.rows.Count; i++)
        {
            var r = GenerateRow(mapInfo.rows[i], i);
            _mapRows.Add(r);
            if(i!=0) r.offsetChilds();
        }

        yield return new WaitForEndOfFrame();
        GenerateLine();
        _player_icon.SetAsLastSibling();
        _player_icon.position = _mapRows[0]._nodes[0].transform.position;
        //_player_icon.anchoredPosition = new Vector2(_player_icon.anchoredPosition.x, _player_icon.anchoredPosition.y + 56);
        //_player_icon.anchoredPosition = new Vector2(_mapRows[0]._nodes[0].Rect.anchoredPosition.x,
        //    _mapRows[0]._nodes[0].Rect.anchoredPosition.y + 56);

    }

    MapRowUI GenerateRow(RowSetting rowSetting, int rowIndex)
    {
       
        var rowUI = Instantiate(_row_ui_prefab);
        rowUI.transform.SetParent(_content_parent.transform,false);
        rowUI.transform.localScale = new(1, 1, 1);

        int rand = Random.Range(rowSetting.minNode, rowSetting.maxNode);
        //Debug.Log(rowSetting.minNode + " " + rowSetting.maxNode + " " + rand);
   
        for(int i = 0; i < rand; i++)
        {
            MapNodeUI nodeUI = GenerateNode(null);
            nodeUI.row = rowIndex;
            nodeUI.NodeInfo = rowSetting.possibleNode[Random.Range(0, rowSetting.possibleNode.Count - 1)];
            nodeUI.transform.SetParent(rowUI.transform,false);
            nodeUI.transform.localScale = new Vector3(1, 1, 1);
            nodeUI.onClick = OnClickMapNode;
            if(i!=0)nodeUI.SetLock(true);
            rowUI._nodes.Add(nodeUI);
        }
        return rowUI;
    }

    MapNodeUI GenerateNode(NodeInfo nodeInfo)
    {
        return Instantiate(_node_ui_prefab);
    }

    void GenerateLine()
    {
        //Connect line
        for(int row = _mapRows.Count-1; row>0; row--)//start from the last row
        {
            for (int j = 0; j < _mapRows[row]._nodes.Count; j++) {
                int n = Random.Range(-1, 1);
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
                    int n = Random.Range(-1, 1);
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

    void OnClickMapNode(MapNodeUI mapNode)
    {
        if (mapNode.isLock) return;

        Debug.Log("Click " + mapNode.NodeInfo.nodeType);

        //Lock same node row
        foreach(var node in _mapRows[mapNode.row]._nodes)
        {
            node.SetLock(true);
        }

        //Unlock next batch of nodes
        foreach (var node in mapNode.NextNodes)
        {
            node.SetLock(false);
        }

    }

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
        var lineUI = Instantiate(_map_line_ui);
        lineUI.transform.SetParent(_content_parent.transform, false);
        lineUI.transform.localScale = new(1, 1, 1);
        lineUI._rect.anchoredPosition = new Vector2(0, 0);
        lineUI.transform.SetAsFirstSibling();
        return lineUI;
    }

    MapLineUI CreateLine(Vector2 start, Vector2 end)
    {
        var lineUI = CreateLine();
        lineUI.SetLine(start, end);
        return lineUI;
    }


    public void Btn_GenerateMap()
    {
       StartCoroutine(ieGenerateMap(_map_info));
    }

    public void Btn_GenerateLine()
    {
        // CreateLine();
        //GenerateLine();
    }
}
