using System.Data;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static SqlDbType? TypeNameToSqlDbType(string typeName)
        {
            switch (typeName)
            {
                case "bigint": return SqlDbType.BigInt;
                case "binary": return SqlDbType.Binary;
                case "bit": return SqlDbType.Bit;
                case "char": return SqlDbType.Char;
                case "date": return SqlDbType.Date;
                case "datetime": return SqlDbType.DateTime;
                case "datetime2": return SqlDbType.DateTime2;
                case "datetimeoffset": return SqlDbType.DateTimeOffset;
                case "decimal": return SqlDbType.Decimal;
                case "float": return SqlDbType.Float;
                //case "geography": m_SqlDbType = SqlDbType.; 
                //case "geometry": m_SqlDbType = SqlDbType; 
                //case "hierarchyid": m_SqlDbType = SqlDbType.; 
                case "image": return SqlDbType.Image;
                case "int": return SqlDbType.Int;
                case "money": return SqlDbType.Money;
                case "nchar": return SqlDbType.NChar;
                case "ntext": return SqlDbType.NText;
                case "numeric": return SqlDbType.Decimal;
                case "nvarchar": return SqlDbType.NVarChar;
                case "real": return SqlDbType.Real;
                case "smalldatetime": return SqlDbType.SmallDateTime;
                case "smallint": return SqlDbType.SmallInt;
                case "smallmoney": return SqlDbType.SmallMoney;
                //case "sql_variant": m_SqlDbType = SqlDbType; 
                //case "sysname": m_SqlDbType = SqlDbType; 
                case "text": return SqlDbType.Text;
                case "time": return SqlDbType.Time;
                case "timestamp": return SqlDbType.Timestamp;
                case "tinyint": return SqlDbType.TinyInt;
                case "uniqueidentifier": return SqlDbType.UniqueIdentifier;
                case "varbinary": return SqlDbType.VarBinary;
                case "varchar": return SqlDbType.VarChar;
                case "xml": return SqlDbType.Xml;
            }

            return null;
        }

    }
}
