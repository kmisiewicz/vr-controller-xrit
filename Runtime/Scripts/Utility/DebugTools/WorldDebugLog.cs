using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chroma.Utility.DebugTools
{
    public class WorldDebugLog : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _Message;
        [SerializeField] Image _ColorRibbon;
        [SerializeField] Button _ShowStackTraceButon;

        public Button ShowStackTraceButton => _ShowStackTraceButon;

        public void SetupMessage(string logText, LogType logType, Color logTypeColor)
        {
            _Message.text = logText;
            _ColorRibbon.color = logTypeColor;
        }
    }
}
