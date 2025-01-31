﻿namespace Nerdbank.GitVersioning
{
    using System;
    using System.Reflection;
    using Newtonsoft.Json;

    internal class FilterPathJsonConverter : JsonConverter
    {
        private readonly string repoRelativeBaseDirectory;

        public FilterPathJsonConverter(string repoRelativeBaseDirectory)
        {
            this.repoRelativeBaseDirectory = repoRelativeBaseDirectory;
        }

        public override bool CanConvert(Type objectType) => typeof(FilterPath).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(FilterPath) || !(reader.Value is string value))
            {
                throw new NotSupportedException();
            }

            if (this.repoRelativeBaseDirectory is null)
            {
                throw new ArgumentNullException(nameof(this.repoRelativeBaseDirectory), $"Base directory must not be null to be able to deserialize filter paths. Ensure that one was passed to {nameof(VersionOptions.GetJsonSettings)}, and that the version.json file is being written to a Git repository.");
            }

            return new FilterPath(value, this.repoRelativeBaseDirectory);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is FilterPath filterPath))
            {
                throw new NotSupportedException();
            }

            if (this.repoRelativeBaseDirectory is null)
            {
                throw new ArgumentNullException(nameof(this.repoRelativeBaseDirectory), $"Base directory must not be null to be able to serialize filter paths. Ensure that one was passed to {nameof(VersionOptions.GetJsonSettings)}, and that the version.json file is being written to a Git repository.");
            }

            writer.WriteValue(filterPath.ToPathSpec(this.repoRelativeBaseDirectory));
        }
    }
}
