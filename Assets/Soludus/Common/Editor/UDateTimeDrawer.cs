using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Soludus
{

    [CustomPropertyDrawer(typeof(UDateTime))]
    public class UDateTimeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty year = property.FindPropertyRelative("year");
            SerializedProperty month = property.FindPropertyRelative("month");
            SerializedProperty day = property.FindPropertyRelative("day");
            SerializedProperty hour = property.FindPropertyRelative("hour");
            SerializedProperty minute = property.FindPropertyRelative("minute");
            SerializedProperty seconds = property.FindPropertyRelative("seconds");

            float yearWidth = 1;
            float monthWidth = 0.77f;
            float dayWidth = 0.77f;
            float hourWidth = 0.77f;
            float minuteWidth = 0.77f;
            float secondsWidth = 1;

            GUIContent yearLabel = new GUIContent("Y");
            GUIContent monthLabel = new GUIContent("M");
            GUIContent dayLabel = new GUIContent("D");
            GUIContent hourLabel = new GUIContent("h");
            GUIContent minuteLabel = new GUIContent("m");
            GUIContent secondsLabel = new GUIContent("s");


            float totalWidth = yearWidth + monthWidth + dayWidth + hourWidth + minuteWidth + secondsWidth;

            position = EditorGUI.PrefixLabel(position, label);
            Rect pos = position;

            EditorGUIUtility.labelWidth = 16;

            // year
            pos.width = yearWidth / totalWidth * position.width;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(pos, year, yearLabel);
            if (EditorGUI.EndChangeCheck())
            {
                year.intValue = Mathf.Clamp(year.intValue, System.DateTime.MinValue.Year, System.DateTime.MaxValue.Year);
                day.intValue = Mathf.Clamp(day.intValue, 1, System.DateTime.DaysInMonth(year.intValue, month.intValue));
            }
            pos.x += pos.width;

            // month
            pos.width = monthWidth / totalWidth * position.width;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(pos, month, monthLabel);
            if (EditorGUI.EndChangeCheck())
            {
                month.intValue = Mathf.Clamp(month.intValue, 1, 12);
                day.intValue = Mathf.Clamp(day.intValue, 1, System.DateTime.DaysInMonth(year.intValue, month.intValue));
            }
            pos.x += pos.width;

            // day
            pos.width = dayWidth / totalWidth * position.width;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(pos, day, dayLabel);
            if (EditorGUI.EndChangeCheck())
            {
                day.intValue = Mathf.Clamp(day.intValue, 1, System.DateTime.DaysInMonth(year.intValue, month.intValue));
            }
            pos.x += pos.width;

            // hour
            pos.width = hourWidth / totalWidth * position.width;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(pos, hour, hourLabel);
            if (EditorGUI.EndChangeCheck())
            {
                hour.intValue = Mathf.Clamp(hour.intValue, 0, 23);
            }
            pos.x += pos.width;

            // minute
            pos.width = minuteWidth / totalWidth * position.width;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(pos, minute, minuteLabel);
            if (EditorGUI.EndChangeCheck())
            {
                minute.intValue = Mathf.Clamp(minute.intValue, 0, 59);
            }
            pos.x += pos.width;

            // seconds
            pos.width = secondsWidth / totalWidth * position.width;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(pos, seconds, secondsLabel);
            if (EditorGUI.EndChangeCheck())
            {
                seconds.floatValue = Mathf.Clamp(seconds.floatValue, 0, 60 - float.Epsilon);
            }
            pos.x += pos.width;


            //base.OnGUI(position, property, label);
        }
    }

}