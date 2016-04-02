namespace Tortuga.Chain.Metadata
{
    internal static class Utilities
    {
        static readonly char[] InvalidCharacters = new char[] { ' ', '.', ',', '$', '@' };


        public static string ToClrName(string name)
        {
            string result = name;
            foreach (char c in InvalidCharacters)
            {
                result = result.Replace(c.ToString(), "");
            }

            if (!char.IsLetter(result[0]) && result[0] != '_')
                result = "_" + result;

            return result;
        }


    }
}
