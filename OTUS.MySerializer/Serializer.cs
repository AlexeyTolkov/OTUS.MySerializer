using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace OTUS.MySerializer
{
    /// <summary> Serializer </summary>
    public static partial class Serializer
    {
        private const string delimeter = ";";

        /// <summary> Serialize from object to CSV </summary>
        /// <param name="obj">any object</param>
        /// <returns>CSV</returns>
        public static string SerializeFromObjectToCSV(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }

            var csvResult = new StringBuilder();
            Type type = obj.GetType();

            // Info about the type
            csvResult.AppendLine(type.Name);

            // Fields
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                csvResult.AppendLine($"{(int)ClassMembers.Field}" + Serializer.delimeter
                                   + $"{field.Name}" + Serializer.delimeter
                                   + $"{field.GetValue(obj)}");
            }

            // Props
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                csvResult.AppendLine($"{(int)ClassMembers.Property}" + Serializer.delimeter
                                   + $"{prop.Name}" + Serializer.delimeter
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

            var classValidation = false;
            var cons = type.GetConstructors();
            foreach (var con in cons)
            {
                if (con.IsPublic && con.GetParameters().Length == 0)
                {
                    classValidation = true;
                    break;
                }
            }

            if (!classValidation)
            { 
                return null;
            }

            object instance = null;

            using (var reader = new StringReader(csv))
            {
                // create an instance of the type
                instance = Activator.CreateInstance(type);
                // TODO:
                // Часто сериализаторы ограничивают дженерик так, чтобы принимал только классы с публичным конструктором без параметров where T: new()
                // Так будет гарантия, что класс будет создан. Если же такого конструктора нет, то activator.CreateInstance ругнется,
                // что не может создать инстанс класса именно из - за того, что нет нужных параметров для конструктора


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

                    var fields = lineData.Split(Serializer.delimeter);
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