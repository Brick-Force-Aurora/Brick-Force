using System;

public class DateTimeLocal
{
	public static string ToString(DateTime dateTime)
	{
		string empty = string.Empty;
		empty += dateTime.Year.ToString();
		empty += StringMgr.Instance.Get("YEAR");
		empty += dateTime.Month.ToString();
		empty += StringMgr.Instance.Get("MONTH");
		empty += dateTime.Day.ToString();
		empty += StringMgr.Instance.Get("DAY");
		empty += dateTime.Hour.ToString();
		empty += StringMgr.Instance.Get("HOUR");
		empty += dateTime.Minute.ToString();
		empty += StringMgr.Instance.Get("MIN");
		empty += dateTime.Second.ToString();
		return empty + StringMgr.Instance.Get("SEC");
	}
}
