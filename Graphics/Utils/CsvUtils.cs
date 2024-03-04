using Microsoft.VisualBasic.FileIO;

namespace Charts.Utils
{
    public class CsvUtils
    {
        public static List<string[]> ReadCsvFile(string filePath, int startingColumnIndex)
        {
            List<string[]> data = [];

            using (var parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                if (parser.EndOfData == false)
                    parser.ReadLine();

                while (parser.EndOfData == false)
                {
                    var fields = parser.ReadFields();

                    if (fields.Length > startingColumnIndex)
                    {
                        var selectedData = new string[fields.Length - startingColumnIndex];
                        Array.Copy(fields, startingColumnIndex, selectedData, 0, selectedData.Length);
                        data.Add(selectedData);
                    }
                }
            }

            return data;
        }
    }
}
