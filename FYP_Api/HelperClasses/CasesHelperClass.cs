using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_Api.HelperClasses
{
    public class CasesHelperClass
    {
        public DateTime date { get; set; }
        public int count { get; set; }
        /// <summary>
        /// //////////////
        /// </summary>
        public List<UserInfo> users { get; set; }  // Add this property
    }

    public class UserInfo
    {
        public int user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public string role { get; set; }
        public string home_location { get; set; }
        public string office_location { get; set; }
        public int sec_id { get; set; }
        public string sec_name { get; set; }
        public string description { get; set; }
        public DateTime startdate { get; set; }
        public bool status { get; set; }
        public DateTime? enddate { get; set; }
    }
}