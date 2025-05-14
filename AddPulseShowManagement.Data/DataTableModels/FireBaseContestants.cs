using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Google.Cloud.Firestore;

namespace AddPulseShowManagement.Data.DataTableModels
{
    public class FireBaseContestants
    {
       
        public string? PlayerID { get; set; }
       // public long PlayerID { get; set; }
        public string Name { get; set; }
        
        public string PolarID { get; set; }
        public long AveragePulse { get; set; }
        public DateTime? TimeStamp { get; set; }
       
    }

    public class FireBaseContestantsHistory
    {
        public long AveragePulse { get; set; }
        public long LivePulse { get; set; }
        //public long PlayerID { get; set; }
        public string PlayerID { get; set; }
        public string? PolarID { get; set; }      
        public DateTime? TimeStamp { get; set; }

    }

    public static class CovertFunctionClass
    {

    public static T GetObject<T>(this Dictionary<string, object> dict)
    {
        return (T)GetObject(dict, typeof(T));
    }

    public static Object GetObject(this Dictionary<string, object> dict, Type type)
    {
        var obj = Activator.CreateInstance(type);

        foreach (var kv in dict)
        {
            var prop = type.GetProperty(kv.Key);
            if (prop == null) continue;
          
            object value = kv.Value;

            if (kv.Key.ToLower().Equals("timestamp"))
            {
                var timestamp = (Timestamp)kv.Value;
                value = timestamp.ToDateTime();
                //value = DateTime.UtcNow;
            }

            if (value is Dictionary<string, object>)
            {
                value = GetObject((Dictionary<string, object>)value, prop.PropertyType); // <= This line
            }

            prop.SetValue(obj, value, null);
        }
        return obj;
    }

}
}
