using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Manager
{
    public class DataManager : Singleton<DataManager>
    {
        public Dictionary<int, RouteModel> RouteData;

        private void Awake()
        {
            MakeDatabase("Route", ref RouteData);
        }

        private void MakeDatabase<T>(string dataname, ref Dictionary<int, T> database)
        {
            database = new Dictionary<int, T>();
            var json = JObject.Parse(File.ReadAllText($"{Application.dataPath}/Data/{dataname}.json"));
            var routeArray = json[dataname].ToArray();
            for (int i = 0; i < routeArray.Length; i++)
            {
                var route = routeArray[i].ToObject<T>();
                database.Add((route as IDataModel).Id, route);
            }
        }
    }
}
