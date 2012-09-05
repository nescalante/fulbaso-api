using System.IO;
using System.Text;
using Fulbaso.Common.Security;
using Newtonsoft.Json;
using SignalR;

namespace Fulbaso.Web.Console
{
    public static class ConsoleUtil
    {
        public static void Call(string type, string action)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;

                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("type");
                jsonWriter.WriteValue(type);
                jsonWriter.WritePropertyName("action");
                jsonWriter.WriteValue(action);

                if (UserAuthentication.User != null)
                {
                    jsonWriter.WritePropertyName("user");
                    jsonWriter.WriteValue(UserAuthentication.User.UserName);
                }

                jsonWriter.WriteEndObject();
            }

            var context = GlobalHost.ConnectionManager.GetConnectionContext<SignalConnection>();
            context.Connection.Broadcast(sb.ToString());
        }
    }
}