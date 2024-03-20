#if IOS || MACCATALYST
using System.Globalization;
using Foundation;

namespace Raygun4Maui.MauiRUM.EventTrackers.Apple;

public static class NSObjectHelper
{
    public static Object ToObject(NSObject nsO, Type targetType)
    {
        if (nsO is NSString)
        {
            return nsO.ToString();
        }

        if (nsO is NSDate)
        {
            var nsDate = (NSDate)nsO;
            return DateTime.SpecifyKind((DateTime)nsDate, DateTimeKind.Unspecified);
        }

        if (nsO is NSDecimalNumber)
        {
            return decimal.Parse(nsO.ToString(), CultureInfo.InvariantCulture);
        }

        if (nsO is NSNumber)
        {
            var x = (NSNumber)nsO;

            switch (Type.GetTypeCode(targetType))
            {
                case TypeCode.Boolean:
                    return x.BoolValue;
                case TypeCode.Char:
                    return Convert.ToChar(x.ByteValue);
                case TypeCode.SByte:
                    return x.SByteValue;
                case TypeCode.Byte:
                    return x.ByteValue;
                case TypeCode.Int16:
                    return x.Int16Value;
                case TypeCode.UInt16:
                    return x.UInt16Value;
                case TypeCode.Int32:
                    return x.Int32Value;
                case TypeCode.UInt32:
                    return x.UInt32Value;
                case TypeCode.Int64:
                    return x.Int64Value;
                case TypeCode.UInt64:
                    return x.UInt64Value;
                case TypeCode.Single:
                    return x.FloatValue;
                case TypeCode.Double:
                    return x.DoubleValue;
            }
        }

        return nsO;
    }

    public static string ToString(NSObject nsO)
    {
        return (string)ToObject(nsO, typeof(string));
    }

    public static DateTime ToDateTime(NSObject nsO)
    {
        return (DateTime)ToObject(nsO, typeof(DateTime));
    }

    public static decimal ToDecimal(NSObject nsO)
    {
        return (decimal)ToObject(nsO, typeof(decimal));
    }

    public static bool ToBool(NSObject nsO)
    {
        return (bool)ToObject(nsO, typeof(bool));
    }

    public static char ToChar(NSObject nsO)
    {
        return (char)ToObject(nsO, typeof(char));
    }

    public static int ToInt(NSObject nsO)
    {
        return (int)ToObject(nsO, typeof(int));
    }

    public static uint ToUInt(NSObject nsO)
    {
        return (uint)ToObject(nsO, typeof(uint));
    }

    public static float ToFloat(NSObject nsO)
    {
        return (float)ToObject(nsO, typeof(float));
    }

    public static double ToDouble(NSObject nsO)
    {
        return (double)ToObject(nsO, typeof(double));
    }
}

#endif