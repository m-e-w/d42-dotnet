using System.Linq;
using System.Data;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace D42_POC_Demo
{
    public class CsvReader
    {
        public static DataTable GenerateDataTable(string csvString, bool firstRowContainsFieldNames = true)
        {
            DataTable result = new DataTable();

            if (csvString == "")
            {
                return result;
            }

            string delimiters = ",";
            using (TextFieldParser tfp = new TextFieldParser(new StringReader(csvString)))
            {
                tfp.SetDelimiters(delimiters);

                // Get The Column Names
                if (!tfp.EndOfData)
                {
                    string[] fields = tfp.ReadFields();

                    for (int i = 0; i < fields.Count(); i++)
                    {
                        if (firstRowContainsFieldNames)
                            result.Columns.Add(fields[i]);
                        else
                            result.Columns.Add("Col" + i);
                    }

                    // If first line is data then add it
                    if (!firstRowContainsFieldNames)
                        result.Rows.Add(fields);
                }

                // Get Remaining Rows from the CSV
                while (!tfp.EndOfData)
                    result.Rows.Add(tfp.ReadFields());
            }
            return result;
        }
    }
}