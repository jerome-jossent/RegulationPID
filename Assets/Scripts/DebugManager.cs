using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    TMPro.TMP_Text text;
    public static DebugManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    public void _PRINT(string text, bool append = false)
    {
        if (append)
            this.text.text += "\n" + text;
        else
            this.text.text = text;
    }
}
