using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using LiteDB;
using System.Management.Automation;

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
                    if (kvp.Key.ToLower().Contains("time") || kvp.Key.ToLower().Contains("date"))
                    {
                        if (kvp.Value.AsString.ToLower().Contains("date"))
                        {
                            // microsoft date format
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, MSJsonDateConverter.Convert(kvp)));
                        }
                        else
                        {
                            //standard json iso date conversion
                            Obj.Properties.Add(new PSNoteProperty(kvp.Key, MSJsonDateConverter.Convert(kvp)));
                        }

                    }
                    else
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, kvp.Value.RawValue));
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
                    if(li.Count > 0)
                    {
                        Obj.Properties.Add(new PSNoteProperty(kvp.Key, li));
                    }
                }
            }
            return Obj;
        }
    }


    
}
