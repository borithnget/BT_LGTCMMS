//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BT_KimMex.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class tb_project_pm
    {
        public string project_pm_id { get; set; }
        public string project_id { get; set; }
        public string project_manager_id { get; set; }
    
        public virtual tb_project tb_project { get; set; }
    }
}
