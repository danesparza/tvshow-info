using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ShowInfo
{
    [DataContract]
    public class ShowAlias
    {
        [DataMember(Name="Show")]
        public string Show { get; set; }
        
        [DataMember(Name="Alias")]
        public string Alias { get; set; }
    }
}
