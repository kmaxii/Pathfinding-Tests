using MaxisGeneralPurpose.Event;
using MaxisGeneralPurpose.Scriptable_objects;
using Scriptable_objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/data/time")]
public class TimeVariable : DataCarrier
{
    [SerializeField] private Time24H time24H;

    [SerializeField] private GameEvent onResetHour;


    public Time24H Time24H
    {
        get => time24H;
        set
        {
            time24H = value;
            if (raiseOnValueChanged)
                raiseOnValueChanged.Raise();
        }
    }

    //Increase time by Time24H
    public void IncreaseTime(Time24H toIncrease)
    {
        bool resetHour = Time24H.WillHourResetToZero(time24H, toIncrease);
        time24H += toIncrease;
        if (raiseOnValueChanged)
            raiseOnValueChanged.Raise();
        if (resetHour && onResetHour)
        {
            onResetHour.Raise();
        }
    }

    public override string ToString()
    {
        return time24H.ToString();
    }
}