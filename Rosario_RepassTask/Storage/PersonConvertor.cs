using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Rosario_repassTask
{
    public class PersonConvertor : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Person));
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            JObject jObject = JObject.Load(reader);
            Role role = jObject["UserRole"].ToObject<Role>();

            Person result = null;

            switch (role)
            {
                case Role.Student:
                    result = jObject.ToObject<Student>();
                    ((Student)result).Grades = jObject["Grades"].ToObject<List<Mark>>();
                    break;

                case Role.Teacher:
                    result = new Professor();
                    break;
            }

            if (result != null)
            {
                serializer.Populate(jObject.CreateReader(), result);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var person = value as Person;
            if (person != null)
            {
                JObject jObject = new JObject();
                Type type = person.GetType();
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    if (prop.CanRead)
                    {
                        object propVal = prop.GetValue(person, null);
                        if (propVal != null)
                        {
                            if (prop.Name == "Grades" && person is Student)
                            {
                                // Handle Grades list differently
                                jObject.Add(prop.Name, JToken.FromObject(((Student)person).Grades, serializer));
                            }
                            else
                            {
                                jObject.Add(prop.Name, JToken.FromObject(propVal, serializer));
                            }
                        }
                    }
                }
                jObject.WriteTo(writer);
            }
            else
            {
                throw new Exception("Expected Person object value");
            }
        }






    }
}
