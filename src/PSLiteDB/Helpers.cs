using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using LiteDB;
using System.Management.Automation;
using System.Collections;
using System.Text.RegularExpressions;

namespace PSLiteDB.Helpers
{

    class MSJsonDate
    {
        public string BsonKey { get; set; }
        public string BsonDate { get; set; }
    }

    public class MSJsonDateConverter
    {
        public string BsonKey { get; set; }
        public DateTime BsonDate { get; set; }



        public static DateTime Convert(KeyValuePair<string, BsonValue> kvp)
        {
            MSJsonDate Obj1;
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                Formatting = Formatting.Indented
            };

            if (kvp.Value.IsDocument)
            {
                Obj1 = new MSJsonDate { BsonKey = kvp.Key, BsonDate = kvp.Value.AsDocument["value"].AsString };
            }
            else
            {
                Obj1 = new MSJsonDate { BsonKey = kvp.Key, BsonDate = kvp.Value.AsString };
            }


            string json = JsonConvert.SerializeObject(Obj1, settings);
            var foo = JsonConvert.DeserializeObject<MSJsonDateConverter>(json, settings);
            return foo.BsonDate;
        }

        public static PSObject BSONtoPSObjectConverter(BsonDocument bsonobj, string Collection)
        {
            PSObject Obj = new PSObject();
            Obj.Properties.Add(new PSNoteProperty("Collection", Collection));

            foreach (KeyValuePair<string, BsonValue> kvp in bsonobj)
            {
                if (kvp.Value.GetType() == typeof(BsonValue))
                {
                    Regex rgx = new Regex(@"^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*))(?:Z|(\+|-)([\d|:]*))?$");
                    Regex rgx2 = new Regex(@"^\\/Date\(\d+");

                    if (kvp.Value.IsString)
                    {
 
                        if (rgx.IsMatch(kvp.Value.AsString) || rgx2.IsMatch(kvp.Value.AsString))
                        {
                            //standard json iso date conversion
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, Convert(kvp)));
                        }
                        else
                        {
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                        }
                    }
                    else
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                    }
                }
                else if (kvp.Value.IsDocument)
                {
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, BSONtoPSObjectConverter(kvp.Value.AsDocument, Collection)));
                }
                else if (kvp.Value.IsArray)
                {

                    var bsontype = kvp.Value.AsArray[0].Type;
                    var li = new List<string>();
                    foreach (var i in kvp.Value.AsArray)
                    {

                        if (i.IsDocument)
                        {
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, BSONtoPSObjectConverter(i.AsDocument, Collection)));
                        }
                        else
                        {
                            li.Add(i.AsString);

                        }
                    }
                    if (li.Count > 0)
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, li));
                    }
                }
                else
                {
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                }
            }
            return Obj;
        }

        public static PSObject BSONtoPSObjectConverter(BsonDocument bsonobj)
        {
            PSObject Obj = new PSObject();
          

            foreach (KeyValuePair<string, BsonValue> kvp in bsonobj)
            {
                if (kvp.Value.GetType() == typeof(BsonValue))
                {
                    Regex rgx = new Regex(@"^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*))(?:Z|(\+|-)([\d|:]*))?$");
                    Regex rgx2 = new Regex(@"^\\/Date\(\d+");

                    if (kvp.Value.IsString)
                    {

                        if (rgx.IsMatch(kvp.Value.AsString) || rgx2.IsMatch(kvp.Value.AsString))
                        {
                            //standard json iso date conversion
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, Convert(kvp)));
                        }
                        else
                        {
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                        }
                    }
                    else
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                    }
                }
                else if (kvp.Value.IsDocument)
                {
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, BSONtoPSObjectConverter(kvp.Value.AsDocument)));
                }
                else if (kvp.Value.IsArray)
                {

                    var bsontype = kvp.Value.AsArray[0].Type;
                    var li = new List<string>();
                    foreach (var i in kvp.Value.AsArray)
                    {

                        if (i.IsDocument)
                        {
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, BSONtoPSObjectConverter(i.AsDocument)));
                        }
                        else
                        {
                            li.Add(i.AsString);

                        }
                    }
                    if (li.Count > 0)
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, li));
                    }
                }
                else
                {
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                }
            }
            return Obj;
        }


        public static object ReadObject(BsonValue value)
        {
            switch (value.Type)
            {
                case BsonType.Array: return ReadArray(value.AsArray); // replacement
                case BsonType.Binary: return value.AsBinary;
                case BsonType.Boolean: return value.AsBoolean;
                case BsonType.DateTime: return value.AsDateTime;
                case BsonType.Decimal: return value.AsDecimal;
                case BsonType.Document: return ReadCustomObject(value.AsDocument); // replacement
                case BsonType.Double: return value.AsDouble;
                case BsonType.Guid: return value.AsGuid;
                case BsonType.Int32: return value.AsInt32;
                case BsonType.Int64: return value.AsInt64;
                case BsonType.Null: return null;
                case BsonType.ObjectId: return value.AsObjectId;
                case BsonType.String: return value.AsString;
                default: return value;
            }
        }
        internal static IList ReadArray(BsonArray value)
        {
            var list = new List<object>(value.Count);
            for (int i = 0; i < value.Count; ++i)
                list.Add(ReadObject(value[i]));
            return list;
        }
        internal static PSObject ReadCustomObject(BsonDocument value)
        {
            var ps = new PSObject();
            var properties = ps.Properties;

            foreach (var kv in value)
            {
                var value2 = ReadObject(kv.Value);
                properties.Add(new PSNoteProperty(kv.Key, value2), true); //! true is faster
            }

            return ps;
        }



    }



}