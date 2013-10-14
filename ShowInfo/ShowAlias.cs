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
        [DataMember]
        public string Show { get; set; }
        
        [DataMember]
        public string Alias { get; set; }
    }
}
