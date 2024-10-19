using System;
using System.Collections.Generic;
using System.Text;

namespace Framework
{
    /// <summary>
    /// Utility class for parsing and creating CSV files.
    /// </summary>
    public static class CSV
    {

        /// <summary>
        /// Parses a line from a CSV file.
        /// </summary>
        /// <param name="line">The line</param>
        /// <returns>The parsed values</returns>
        public static List<String> ParseLine(string line)
        {
            int fieldStartIndex = 0;
            bool evenNumberOfQuotes = true;
            List<string> fields = new List<string>();

            for (int i = 0; i < line.Length; i++)
            {

                //We've encountered a quote
                if (line[i] == '"')
                {
                    evenNumberOfQuotes = !evenNumberOfQuotes;
                }

                //We've encountered a comma that is not inside quotes
                if (evenNumberOfQuotes && line[i] == ',')
                {
                    fields.Add(ParseField(line.Substring(fieldStartIndex, i - fieldStartIndex)));
                    fieldStartIndex = i + 1;
                }
                else if (i == line.Length - 1)//We've reached the end of the line
                {
                    fields.Add(ParseField(line.Substring(fieldStartIndex)));
                }

            }

            return fields;
        }

        /// <summary>
        /// Parses a line from a CSV file.
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="numColumns">The expected number of columns in the line</param>
        /// <returns>The parsed values</returns>
        public static string[] ParseLine(string line, int numColumns)
        {
            int fieldStartIndex = 0, currentField = 0;
            bool evenNumberOfQuotes = true;
            string[] fields = new string[numColumns];

            for (int i = 0; i < line.Length; i++)
            {
                //Check to see if we've already filled the number of fields we were expecting
                if (currentField == numColumns)
                {
                    UnityEngine.Debug.LogError("More fields than expected in CSV line.");
                    break;
                }


                //We've encountered a quote
                if (line[i] == '"')
                {
                    evenNumberOfQuotes = !evenNumberOfQuotes;
                }

                //We've encountered a comma that is not inside quotes
                if (evenNumberOfQuotes && line[i] == ',')
                {
                    fields[currentField] = ParseField(line.Substring(fieldStartIndex, i - fieldStartIndex));
                    currentField++;
                    fieldStartIndex = i + 1;
                }
                else if (i == line.Length - 1)//We've reached the end of the line
                {
                    fields[currentField] = ParseField(line.Substring(fieldStartIndex));
                }

            }

            return fields;
        }


        /// <summary>
        /// Creates CSV formatted string for an array of objects, uses the ToString() method for each object.
        /// </summary>
        /// <param name="fields">The fields that will be separated by commas</param>
        /// <returns>The formatted CSV line</returns>
        public static string CreateLine(params object[] fields)
        {

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < fields.Length; i++)
            {

                string field = fields[i] == null ? "" : fields[i].ToString();

                if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
                {
                    builder.Append('"');
                    builder.Append(field.Replace("\"", "\"\""));
                    builder.Append('"');
                }
                else
                {
                    builder.Append(field.Trim());
                }

                if (i < fields.Length - 1)
                {
                    builder.Append(',');
                }

            }

            return builder.ToString();
        }

        private static string ParseField(string field)
        {
            //Trim whitespace
            field = field.Trim();

            //Return null for empty fields
            if (field.Length < 1)
            {
                return null;
            }

            //Remove fully encapsulating quotes
            if (field[0] == '"')
            {
                field = field.Substring(1, field.Length - 2);
            }

            //Remove double quotes (using minimal allocations)
            if (field.Length > 1)
            {
                for (int j = 0; j < field.Length - 1; j++)
                {
                    if (field[j] == '"' && field[j + 1] == '"')
                    {
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < field.Length; i++)
                        {
                            if (i + 1 < field.Length && field[i] == '"' && field[i + 1] == '"') i++;
                            builder.Append(field[i]);
                        }

                        return builder.ToString();
                    }
                }
            }

            return field;
        }

    }
}
