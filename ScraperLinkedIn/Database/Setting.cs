//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ScraperLinkedIn.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Setting
    {
        public int Id { get; set; }
        public System.TimeSpan TimeStart { get; set; }
        public int IntervalType { get; set; }
        public int IntervalValue { get; set; }
        public string Token { get; set; }
        public int CompanyBatchSize { get; set; }
        public int ProfileBatchSize { get; set; }
        public string Password { get; set; }
        public int AccountID { get; set; }
        public System.DateTime DateUpdate { get; set; }
        public string Login { get; set; }
        public string TechnologiesSearch { get; set; }
        public string RolesSearch { get; set; }
    }
}
