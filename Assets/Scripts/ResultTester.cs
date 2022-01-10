using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class ResultTester : MonoBehaviour
{
    [SerializeField]
    private InputField _inptSector;
    [SerializeField]
    private InputField _inptRounds;
    [SerializeField]
    private Toggle _tglFileOutput;
    [SerializeField]
    private InputField _inptFileOutput;
    [SerializeField]
    private WheelHandler _wheelHandler;
    private SO_WheelItemContainer so_wheelItemContainer;
    private List<int> _listTestResult = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void TestRounds()
    {
        if (_inptRounds.text == "") return;
        int testCount = int.Parse(_inptRounds.text);

        for (int i = 0; i < _listTestResult.Count; i++)
        {
            _listTestResult[i] = 0;
        }

        for (int i = 0; i < testCount; i++)
        {
            int nResultItem = _wheelHandler.GetTestResult();
            _listTestResult[nResultItem]++;
        }
        if (_tglFileOutput.isOn)
        {
            Write2File("================" + System.DateTime.UtcNow +"================");
        }

        for (int i = 0; i < _listTestResult.Count; i++)
        {
            string strLog = (i+1) + " | " + so_wheelItemContainer.ListWheelItems[i].ItemName +
                ", drop rate: " + so_wheelItemContainer.ListWheelItems[i].DropChance +
                " => " + _listTestResult[i] + "/" + testCount + "'\n";
            Debug.Log(strLog);
            if (_tglFileOutput.isOn)
            {        
                 Write2File(strLog);
            }
        }
    }

    public void TestSector()
    {
        if (_inptSector.text == "") return;
        int nSectorNum = int.Parse(_inptSector.text);
        _wheelHandler.TestSectorOccurance(nSectorNum - 1);
    }

    void Initialize()
    {
        so_wheelItemContainer = _wheelHandler.GetWheelItemContainer();
        for (int i = 0; i < so_wheelItemContainer.ListWheelItems.Count; i++)
        {
            _listTestResult.Add(0);
        }
    }

    void Write2File(string strResult)
    {
        string path = "output_sector" + ".txt";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(strResult);
        writer.Close();
    }
}
