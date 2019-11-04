using System;

namespace Overlay
{
    [Serializable]
    public class DataObject
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Identifier { get; set; }

        public override string ToString()
        {
            return " Identifier: "+Identifier+"\n Key: "+Key+"\n Value: "+Value.ToString()+"\n";
        }

        
    }
}
