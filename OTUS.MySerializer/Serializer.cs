using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace OTUS.MySerializer
{
    /// <summary> Serializer </summary>
    public static partial class Serializer
    {
        private static string GetDelimeter()
        {
            return ";";
        }

        /// <summary> Serialize from object to CSV </summary>
        /// <param name="obj">any object</param>
        /// <returns>CSV</returns>
        public static string SerializeFromObjectToCSV(object obj)
        {
            if (obj == null)
            {
                return "";
            }

            var csvResult = new StringBuilder();
            var delimeter = GetDelimeter();
            Type type = obj.GetType();

            // Info about the type
            csvResult.AppendLine(type.Name);

            // Fields
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                csvResult.AppendLine($"{(int)ClassMembers.Field}" + delimeter
                                   + $"{field.Name}" + delimeter
                                   + $"{field.GetValue(obj)}");
            }

            // Props
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                csvResult.AppendLine($"{(int)ClassMembers.Property}" + delimeter
                                   + $"{prop.Name}" + delimeter
                                   + $"{prop.GetValue(obj)}");
            }

            return csvResult.ToString();
        }

        /// <summary> Deserialize from CSV to object</summary>
        /// <param name="csv">string in CSV format</param>
        /// <returns>object</returns>
        public static object DeserializeFromCSVToObject(Type type, string csv)
        {
            if (string.IsNullOrWhiteSpace(csv) ||
                type == null)
            {
                return null;
            }

            var delimeter = GetDelimeter();
            object instance = null;

            using (var reader = new StringReader(csv))
            {
                // create an instance of the type
                instance = Activator.CreateInstance(type);

                var lineNo = 1;
                for (string lineData = reader.ReadLine(); lineData != null; lineData = reader.ReadLine())
                {
                    // Header
                    if (lineNo == 1)
                    {
                        if (type.Name != lineData)
                        {
                            return null;
                        }

                        lineNo++;
                        continue;
                    }

                    var fields = lineData.Split(delimeter);
                    var idx = (int)CsvFieldsOrder.ClassMember;
                    ClassMembers classMember = (ClassMembers)int.Parse(fields[idx]);
                    switch (classMember)
                    {
                        case ClassMembers.Field:
                            var fieldName = fields[(int)CsvFieldsOrder.Name];
                            var field = type.GetField(fieldName);

                            var fieldValue = Convert.ChangeType(fields[(int)CsvFieldsOrder.Value], field.FieldType);
                            field.SetValue(instance, fieldValue);
                            break;

                        case ClassMembers.Property:
                            var propName = fields[(int)CsvFieldsOrder.Name];
                            var prop = type.GetProperty(propName);

                            var propValue = Convert.ChangeType(fields[(int)CsvFieldsOrder.Value], prop.PropertyType);
                            prop.SetValue(instance, propValue, null);
                            break;

                        default:
                            break;
                    }

                    lineNo++;
                }
            }

            return instance;
        }
    }
}