using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace MoviesApi;

public class CustomYearSerializer : IBsonSerializer<int>
{
    public Type ValueType => typeof(int);

    public int Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var type = context.Reader.GetCurrentBsonType();

        try
        {
            switch (type)
            {
                case BsonType.String:
                    var strValue = context.Reader.ReadString();
                    // Extract only numeric characters from the string
                    var numericOnly = new string(strValue.Where(c => char.IsDigit(c)).ToArray());
                    if (int.TryParse(numericOnly, out int yearFromString))
                    {
                        return yearFromString;
                    }
                    return 0;

                case BsonType.Int32:
                    return context.Reader.ReadInt32();

                case BsonType.Int64:
                    return (int)context.Reader.ReadInt64();

                case BsonType.Double:
                    return (int)context.Reader.ReadDouble();

                default:
                    context.Reader.SkipValue();
                    return 0;
            }
        }
        catch
        {
            return 0;
        }
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, int value)
    {
        context.Writer.WriteInt32(value);
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        return Deserialize(context, args);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        if (value is int intValue)
        {
            Serialize(context, args, intValue);
        }
    }
}
