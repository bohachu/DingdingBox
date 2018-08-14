using UnityEngine;
using System;
using System.Collections.Generic;

namespace Cameo
{
	public enum LogType
	{
		Log,
		Warnning,
		Error,
	}

	public class LogObject
	{
		public LogType Type;
		public string Text;
		public DateTime LogTime;

		private bool _isDebugLogShowed = false;

		private Color GetColor()
		{
			switch(this.Type)
			{
			case LogType.Error:
				return Color.red;
			case LogType.Log:
				return Color.blue;
			case LogType.Warnning:
				return Color.yellow;
			}
			return GUI.color;
		}

		private void CreateDebugLog()
		{
			switch(this.Type)
			{
			case LogType.Error:
				Debug.LogError(Text);
				break;
			case LogType.Log:
				Debug.Log(Text);
				break;
			case LogType.Warnning:
				Debug.LogWarning(Text);
				break;
			}
		}

		public LogObject(LogType type, string text)
		{
			Type = type;
			Text = text;
			LogTime = DateTime.Now;
		}

		public void Display(Rect uiRect, GUIStyle style)
		{
			Color oriColor = GUI.color;
			GUI.color = GetColor();
			style.normal.textColor = GetColor ();
			GUI.Label(uiRect, "[" + Type.ToString() + "] " + Text, style);
			GUI.color = oriColor;

			if(!_isDebugLogShowed)
			{
				CreateDebugLog();
				_isDebugLogShowed = true;
			}
		}
	}

	public class DebugLogger : Singleton<DebugLogger> 
	{
		private List<LogObject> _logObjs = new List<LogObject>();

		public bool Show = false;
		public int MaxCount = 30;
		public int LineCount = 5;
		public int LabelHeight = 30;
		public int FontSize = 30;

		void OnGUI()
		{
			if(!Show)
			{
				return;
			}

			int scnWidth = Screen.width;
			int scnHeight = Screen.height;

			GUIStyle style = new GUIStyle ();
			style.fontSize = FontSize;

			for(int i = 1; i <= LineCount; ++i)
			{
				int index = _logObjs.Count - i;
				if(index < 0)
				{
					break;
				}
				else
				{
					_logObjs[index].Display(new Rect(0, scnHeight - i * LabelHeight, scnWidth, LabelHeight), style);
				}
			}
		}

		public void Log(string text)
		{
			AddLog(LogType.Log, text);
		}

		public void LogWarnning(string text)
		{
			AddLog(LogType.Warnning, text);
		}

		public void LogError(string text)
		{
			AddLog(LogType.Error, text);
		}

		public void Clear()
		{
			_logObjs.Clear();
		}

		private void AddLog(LogType type, string text)
		{
			_logObjs.Add(new LogObject(type, text));

			if(_logObjs.Count > MaxCount)
			{
				_logObjs.RemoveAt(0);
			}

			switch(type)
			{
			case LogType.Log:
				Debug.Log(text);
				break;
			case LogType.Warnning:
				Debug.LogWarning(text);
				break;
			case LogType.Error:
				Debug.LogError(text);
				break;
			}
		}
	}
}

