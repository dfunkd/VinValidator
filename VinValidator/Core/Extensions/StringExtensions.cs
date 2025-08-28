using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static bool IsValidVin(this string vin)
    {
        if (vin.Length != 17)
            return false;

        //Validate characters (exclude I, O, Q)
        //Regex allows alphanumeric characters except 'I', 'O', 'Q' (case-insensitive)
        Regex regex = new("^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.IgnoreCase);
        if (!regex.IsMatch(vin))
            return false;
        return true;
    }
}
