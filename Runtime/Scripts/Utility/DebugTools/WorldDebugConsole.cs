using Chroma.Utility.Attributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chroma.Utility.DebugTools
{
    public class WorldDebugConsole : MonoBehaviour
    {
        [SerializeField, Min(1)] 
        int _MaxMessagesCount = 15;

        [SerializeField, OnValueChanged("AdjustMessageHeight")]
        float _MessageSize = 0.3f;

        [SerializeField, Tooltip("Log types to show.")]
        EnumNamedArray<bool> _LogTypes = new EnumNamedArray<bool>(typeof(LogType));

        [SerializeField, Tooltip("Colors of log types to show.")]
        EnumNamedArray<Color> _LogTypesColors = new EnumNamedArray<Color>(typeof(LogType));

        [SerializeField]
        WorldDebugLog _LogPrefab;

        [SerializeField, Tooltip("Transform that the logs should be childed to.")]
        Transform _LogsParent;

        [SerializeField]
        ScrollRect _LogsScrollView;

        [SerializeField]
        GameObject _StackTracePanel;

        [SerializeField]
        TextMeshProUGUI _StackTraceText;


        Queue<string> _messages = new Queue<string>();
        Queue<string> _stackTraces = new Queue<string>();
        Queue<WorldDebugLog> logObjects = new Queue<WorldDebugLog>();


        private void OnEnable()
        {
            Application.logMessageReceived += OnLogMessageReceived;
            _StackTracePanel?.SetActive(false);
            Debug.Log("[WorldConsole] Active");
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        private void OnLogMessageReceived(string log, string stackTrace, LogType type)
        {
            if (_LogTypes[type])
            {
                WorldDebugLog currentLog;

                while (logObjects.Count > _MaxMessagesCount)
                {
                    WorldDebugLog deleteLog = logObjects.Dequeue();
                    Destroy(deleteLog.gameObject);
                    _messages.Dequeue();
                    _stackTraces.Dequeue();
                }

                if (logObjects.Count < _MaxMessagesCount)
                {
                    currentLog = Instantiate(_LogPrefab, _LogsParent);
                    if (currentLog.TryGetComponent(out LayoutElement layoutElement))
                        layoutElement.preferredHeight = _MessageSize;
                }
                else
                    currentLog = logObjects.Dequeue();

                if (currentLog)
                {
                    currentLog.ShowStackTraceButton.onClick.RemoveAllListeners();
                    currentLog.SetupMessage(log, type, _LogTypesColors[type]);
                    currentLog.ShowStackTraceButton.onClick.AddListener(() => ShowStackTrace(stackTrace));
                    currentLog.transform.SetAsLastSibling();
                    logObjects.Enqueue(currentLog);
                    _messages.Enqueue(log);
                    _stackTraces.Enqueue(stackTrace);
                    _LogsScrollView.verticalNormalizedPosition = 0;
                }
            }
        }

        private void ShowStackTrace(string stackTrace)
        {
            _StackTracePanel?.SetActive(true);
            _StackTraceText?.SetText(stackTrace);
            _LogsScrollView.verticalNormalizedPosition = 0;
        }

        private void AdjustMessageHeight()
        {
            var logs = logObjects.ToArray();
            foreach (var log in logs)
            {
                if (log.TryGetComponent(out LayoutElement layoutElement))
                    layoutElement.preferredHeight = _MessageSize;
            }
        }
    }
}
