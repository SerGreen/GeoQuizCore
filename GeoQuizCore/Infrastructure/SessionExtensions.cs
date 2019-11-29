using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace GeoQuizCore.Infrastructure
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            session.SetString(key, json);
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });
        }
    }
}
