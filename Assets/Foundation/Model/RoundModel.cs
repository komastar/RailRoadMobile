using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Foundation.Model
{
    public class RoundModel
    {
        public int Round { get; set; }
        public List<int> Dices { get; set; }

        public static RoundModel Parse(string jsonString)
        {
            return JObject.Parse(jsonString).ToObject<RoundModel>();
        }
    }
}
