using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Veracode.OSS.Declare.Language
{
    public interface ILocalizationRepository
    {
        string GetText(string code, params string[] entries);
    }
    public class LocalizationRepository : ILocalizationRepository
    {
        private MessageContainer _messages;
        public LocalizationRepository(string localization)
        {
            using StreamReader r = 
                new StreamReader(Path.Combine(
                $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}",
                $"{localization}.json"));

            string json = r.ReadToEnd();
            _messages = JsonConvert.DeserializeObject<MessageContainer>(json);

            if (!_messages.messages.Any())
                throw new ArgumentException($"{localization}.json does not contain any log messages!");
        }

        public class MessageContainer
        {
            public List<Message> messages { get; set; }
        }
        public class Message
        {
            public string Code { get; set; }
            public string Value { get; set; }
        }
        public string GetText(string code, params string[] entries)
        {
            foreach (var message in _messages.messages)
                if (message.Code.Equals(code))
                    return String.Format(message.Value, entries);

            throw new ArgumentException($"{code} is not in the resources dictionary.");
        }
    }
}
