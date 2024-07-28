using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

public static class Extensions
{
    /// <summary>
    /// returns a string from the posted form
    /// </summary>
    /// <param name="request"></param>
    /// <param name="formField"></param>
    /// <returns></returns>
    public static string GetString(this HttpRequest request, string formField)
    {
        if (request.Form.Keys.Contains<string>(formField))
            return request.Form[formField];

        return string.Empty;
    }

    /// <summary>
    /// returns a boolean from the posted form
    /// </summary>
    /// <param name="request"></param>
    /// <param name="formField"></param>
    /// <returns></returns>
    public static bool GetBoolean(this HttpRequest request, string formField)
    {
        if (request.Form.Keys.Contains<string>(formField))
        {
            return (request.Form[formField].ToString().StartsWith("true") || request.Form[formField].Equals("on") || request.Form[formField].Equals("1"));
        }
        return false;
    }

    /// <summary>
    /// retrieves an integer from the posted form with a min return value if 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="formField"></param>
    /// <param name="minVal"></param>
    /// <returns></returns>
    public static int GetInteger(this HttpRequest request, string formField, int minVal = 0)
    {
        if (request.Form.Keys.Contains<string>(formField))
        {
            int returnVal = -1;
            if (int.TryParse(request.Form[formField], out returnVal) && returnVal > minVal)
                return returnVal;
        }

        return 0;
    }

    /// <summary>
    /// returns a nullable int if < min vale
    /// </summary>
    /// <param name="request"></param>
    /// <param name="formField"></param>
    /// <param name="minVal"></param>
    /// <returns></returns>
    public static int? GetIntegerNull(this HttpRequest request, string formField, int minVal = 0)
    {
        if (request.Form.Keys.Contains<string>(formField))
        {
            int returnVal = -1;
            if (int.TryParse(request.Form[formField], out returnVal) && returnVal > minVal)
                return returnVal;
        }

        return null;
    }

    /// <summary>
    /// returns a nullable date
    /// </summary>
    /// <param name="request"></param>
    /// <param name="formField"></param>
    /// <returns></returns>
    public static DateTime? GetDate(this HttpRequest request, string formField)
    {
        if (request.Form.Keys.Contains<string>(formField))
        {
            DateTime oDate;
            if (DateTime.TryParse(request.Form[formField], out oDate))
            {
                return oDate;
            }
        }

        return null;
    }
}
