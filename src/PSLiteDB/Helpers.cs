using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using LiteDB;
using System.Management.Automation;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;

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
        private static int i;


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


        public static PSObject BSONtoPSObjectConverter1(BsonDocument bsonobj, string Collection)
        {
            PSObject Obj = new PSObject();
            Obj.Properties.Add(new PSNoteProperty("Collection", Collection));
            i = 1;
            foreach (KeyValuePair<string, BsonValue> kvp in bsonobj)
            {
                //Console.WriteLine($"current iteration {i}");
                if (kvp.Value.GetType() == typeof(BsonValue))
                {
                    //Console.WriteLine($"detect bsonvalue {kvp.Key} {kvp.Value}");
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                }
                else if (kvp.Value.IsDocument)
                {                  
                    var json = LiteDB.JsonSerializer.Serialize(kvp.Value);
                    using (PowerShell PowerShellInstance = PowerShell.Create())
                    {
                        PowerShellInstance.Commands.AddCommand("ConvertFrom-Json").AddParameter("inputobject", json);
                        var PSOutput = PowerShellInstance.Invoke();
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, PSOutput));
                    }
                }
                else if (kvp.Value.IsArray)
                {
                    //Console.WriteLine($" detect array {kvp.Key} {kvp.Value}");
                    //var bsontype = kvp.Value.AsArray[0].Type;
                    var li = new List<string>();
                    var objlist = new List<PSObject>();
                    foreach (var i in kvp.Value.AsArray)
                    {

                        if (i.IsDocument)
                        {
                            //Console.WriteLine($"detect array doc {i.AsDocument} {i.RawValue}");
                            //objlist.Add(BSONtoPSObjectConverter2(i.AsDocument));

                            var json = LiteDB.JsonSerializer.Serialize(i);
                            using (PowerShell PowerShellInstance = PowerShell.Create())
                            {
                                PowerShellInstance.Commands.AddCommand("ConvertFrom-Json").AddParameter("inputobject", json);
                                var PSOutput = PowerShellInstance.Invoke();
                                foreach (var item in PSOutput)
                                {
                                    objlist.Add(item);
                                }
                                
                            }
                        }
                        else
                        {
                            //Console.WriteLine($"detect array string {i.AsString} {i.RawValue}");
                            li.Add(i.AsString);
                        }
                    }
                    if (li.Count > 0)
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, li));
                    }
                    if (objlist.Count > 0)
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, objlist));
                    }

                }
                else
                {
                    //Console.WriteLine(" detect all else");
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                }
                i++;
            }
            return Obj;
        }



        public static PSObject BSONtoPSObjectConverter2(BsonDocument bsonobj)
        {
            PSObject Obj = new PSObject();
            i = 1;
            foreach (KeyValuePair<string, BsonValue> kvp in bsonobj)
            {
                //Console.WriteLine($"current iteration {i}");
                if (kvp.Value.GetType() == typeof(BsonValue))
                {
                    //Console.WriteLine($" detect bsonvalue {kvp.Key} {kvp.Value}");
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                }
                else if (kvp.Value.IsDocument)
                {
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, BSONtoPSObjectConverter2(kvp.Value.AsDocument)));
                }
                else if (kvp.Value.IsArray)
                {
                    //Console.WriteLine($" detect array {kvp.Key} {kvp.Value}");
                    //var bsontype = kvp.Value.AsArray[0].Type;
                    var li = new List<string>();
                    var objlist = new List<PSObject>();
                    foreach (var i in kvp.Value.AsArray)
                    {

                        if (i.IsDocument)
                        {
                            //Console.WriteLine($" array doc {i.AsDocument} {i.RawValue}");
                            objlist.Add(BSONtoPSObjectConverter2(i.AsDocument));
                        }
                        else
                        {
                            //Console.WriteLine($" array string {i.AsString} {i.RawValue}");
                            li.Add(i.AsString);

                        }
                    }
                    if (li.Count > 0)
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, li));
                    }
                    if (objlist.Count > 0)
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, objlist));
                    }

                }
                else
                {
                    //Console.WriteLine(" detect all else");
                    Obj.Properties.Add(new PSNoteProperty(kvp.Key, ReadObject(kvp.Value)));
                }
                i++;
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