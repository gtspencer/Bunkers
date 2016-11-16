using UnityEngine;
using System.Collections;
using System.IO;

public class ExceptionLogging : MonoBehaviour
{
    public string saveFile = @"Log.txt";
    private StringWriter logWriter;

    void OnEnable()
    {
        Application.RegisterLogCallback(ExceptionWriter);
    }

    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    void ExceptionWriter(string logString, string stackTrace, LogType type)
    {
        switch(type)
        {
            case LogType.Exception:
            case LogType.Error:
                /**
                using (SreamWriter writer = new StreamWriter(new FileStream(saveFile, FileMode.Append)))
                {
                    writer.WriteLine(type);
                    writer.WriteLine(logString);
                    writer.WriteLine(stackTrace);
                }
    */
                break;
            default:
                break;
        }
    }
}
