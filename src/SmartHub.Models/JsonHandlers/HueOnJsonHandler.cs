using SmartHub.Models.Hue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHub.Models.JsonHandlers
{
    public class HueOnJsonHandler : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            bool result = false;
            while (reader.Read())
            {
                if (!result
                    && reader.TokenType == JsonTokenType.PropertyName
                    && reader.GetString() == "on")
                {
                    reader.Read();
                    result = reader.TokenType == JsonTokenType.True;
                    while(reader.Read() && reader.TokenType != JsonTokenType.EndObject);
                    break;
                }

                //try
                //{
                //    var typ = reader.TokenType;
                //}
                //catch (Exception ex)
                //{

                //}
                //try
                //{
                //    var str = reader.GetString();
                //}
                //catch (Exception ex)
                //{

                //}
            }
            return result;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public class DictionaryTKeyEnumTValueConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
            {
                return false;
            }

            return typeToConvert.GetGenericArguments()[0].IsEnum;
        }

        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {
            //Type keyType = type.GetGenericArguments()[0];
            //Type valueType = type.GetGenericArguments()[1];

            //JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            //    typeof(DictionaryEnumConverterInner<,>).MakeGenericType(
            //        new Type[] { keyType, valueType }),
            //    BindingFlags.Instance | BindingFlags.Public,
            //    binder: null,
            //    args: new object[] { options },
            //    culture: null)!;

            //return converter;
            return null;
        }

        private class DictionaryEnumConverterInner<TKey, TValue> :
            JsonConverter<Dictionary<TKey, TValue>> where TKey : struct, Enum
        {
            private readonly JsonConverter<TValue> _valueConverter;
            private readonly Type _keyType;
            private readonly Type _valueType;

            public DictionaryEnumConverterInner(JsonSerializerOptions options)
            {
                // For performance, use the existing converter if available.
                _valueConverter = (JsonConverter<TValue>)options
                    .GetConverter(typeof(TValue));

                // Cache the key and value types.
                _keyType = typeof(TKey);
                _valueType = typeof(TValue);
            }

            public override Dictionary<TKey, TValue> Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var dictionary = new Dictionary<TKey, TValue>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return dictionary;
                    }

                    // Get the key.
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string? propertyName = reader.GetString();

                    // For performance, parse with ignoreCase:false first.
                    if (!Enum.TryParse(propertyName, ignoreCase: false, out TKey key) &&
                        !Enum.TryParse(propertyName, ignoreCase: true, out key))
                    {
                        throw new JsonException(
                            $"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\".");
                    }

                    // Get the value.
                    TValue value;
                    if (_valueConverter != null)
                    {
                        reader.Read();
                        value = _valueConverter.Read(ref reader, _valueType, options)!;
                    }
                    else
                    {
                        value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;
                    }

                    // Add to dictionary.
                    dictionary.Add(key, value);
                }

                throw new JsonException();
            }

            public override void Write(
                Utf8JsonWriter writer,
                Dictionary<TKey, TValue> dictionary,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach ((TKey key, TValue value) in dictionary)
                {
                    var propertyName = key.ToString();
                    writer.WritePropertyName
                        (options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);

                    if (_valueConverter != null)
                    {
                        _valueConverter.Write(writer, value, options);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, value, options);
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}
