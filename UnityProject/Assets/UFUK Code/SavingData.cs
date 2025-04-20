using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class SavingData : MonoBehaviour
{

    public GameObject cursor;
    public GameObject headset;
    public Transform obje1;
    public Transform obje2;

    private StreamWriter dosya;

    private string FilePath;
    // Start is called before the first frame update
    void Start()
    {
	string FileName = "test.txt";
	FilePath = Application.dataPath + "/OurResources/" +  FileName;
	Debug.Log(FilePath);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDir = obje1.position - headset.transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        Vector3 targetDir2 = obje2.position - headset.transform.position;
        float angle2 = Vector3.Angle(targetDir2, transform.forward);

        string textToWrite;
        textToWrite = cursor.transform.position.ToString("F4") + ";" + 
	    headset.transform.position.ToString("F4") + ";" +
        angle + ";" + angle2;

        //Debug.Log(textToWrite);


        using (StreamWriter sw = File.AppendText(FilePath))
        {
            sw.WriteLine(textToWrite);
        }
            
    }
}
