//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GlobalSolusindo.DataAccess
{
    using System;
    
    public partial class GetTimesheetMonthlyV2_Result
    {
        public Nullable<long> Row { get; set; }
        public int SOWAssign_FK { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> FirstCheckIn { get; set; }
        public string FirstLocation { get; set; }
        public Nullable<System.DateTime> LastCheckOut { get; set; }
        public string LastLocation { get; set; }
        public Nullable<bool> IsDiffDay { get; set; }
        public Nullable<int> TotalTask { get; set; }
    }
}
