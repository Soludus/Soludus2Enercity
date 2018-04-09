using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    public LocationHandle locationHandle = null;
    public List<EnergyValue> energyValues = new List<EnergyValue>();
    public List<EnergyDeviceType> deviceTypes = new List<EnergyDeviceType>();

    private StringBuilder debug = new StringBuilder();
    private Rect windowRect;

    private void OnEnable()
    {
        windowRect = new Rect(10, 120, 400, 100);
    }

    private void OnGUI()
    {
        windowRect = GUI.Window(1, windowRect, DrawDebugGUI, "Energy Debug");
    }

    private void DrawDebugGUI(int windowId)
    {
        debug.Length = 0;
        debug.Append("Location:\n");
        debug.AppendFormat("{0}: lat = {1}, lon = {2},\n", locationHandle.location.name, locationHandle.location.latitude, locationHandle.location.longitude);
        debug.AppendFormat("time = {0}, timescale = {1}\n", locationHandle.location.dateTimeOffset, locationHandle.timeScale);

        debug.AppendFormat("\nsolar elevation angle = {0:0.###}\n", SolarEnergy.SolarElevationAngle(locationHandle.location.dateTimeOffset.UtcDateTime, locationHandle.location.latitude, locationHandle.location.longitude));

        debug.Append("\nEnergy Values:\n");
        for (int i = 0; i < energyValues.Count; ++i)
        {
            debug.AppendFormat("{0}: value = {1}, rate = {2},\n", energyValues[i].name, energyValues[i].value, energyValues[i].rate);
            debug.AppendFormat("target value = {0}, target rate = {1},\n", energyValues[i].targetValue, energyValues[i].targetRate);
            debug.AppendFormat("elapsed days = {0:0.###}\n", energyValues[i].elapsedTime.TotalDays);
        }

        debug.Append("\nDevices:\n");
        for (int i = 0; i < deviceTypes.Count; ++i)
        {
            debug.AppendFormat("{0}: device count = {1}, slot count = {2},\n", deviceTypes[i].name, deviceTypes[i].allDevices.Count, deviceTypes[i].allSlots.Count);
            debug.AppendFormat("filled area = {0}, available area = {1}\n", deviceTypes[i].totalActiveGridArea, deviceTypes[i].totalSlotGridArea);
        }

        var content = new GUIContent(debug.ToString());
        windowRect.height = GUI.skin.label.CalcHeight(content, windowRect.width) + 16;
        GUILayout.Label(content);

        GUI.DragWindow();
    }

}
