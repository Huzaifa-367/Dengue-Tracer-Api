//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FYP_Api.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class NOTIFICATION
    {
        public int notif_id { get; set; }
        public Nullable<bool> type { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<short> status { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> sec_id { get; set; }
        public Nullable<int> percnt { get; set; }
    }
}
