//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TradeSharp.Linq
{
    using System;
    using System.Collections.Generic;
    
    public partial class FILE
    {
        public FILE()
        {
            this.USER_INFO = new HashSet<USER_INFO>();
            this.USER_INFO1 = new HashSet<USER_INFO>();
            this.USER_INFO2 = new HashSet<USER_INFO>();
        }
    
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public string HashCode { get; set; }
    
        public virtual ICollection<USER_INFO> USER_INFO { get; set; }
        public virtual ICollection<USER_INFO> USER_INFO1 { get; set; }
        public virtual ICollection<USER_INFO> USER_INFO2 { get; set; }
    }
}
