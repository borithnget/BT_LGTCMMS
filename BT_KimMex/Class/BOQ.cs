using BT_KimMex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Entities;

namespace BT_KimMex.Class
{
    public class BOQ
    {
        private kim_mexEntities db = new kim_mexEntities();
        public List<AttachmentViewModel> GetBOQAttachments(string id)
        {
            List<AttachmentViewModel> attachments = new List<AttachmentViewModel>();
            try
            {
                attachments = db.tb_attachment.OrderBy(m => m.attachment_name).Where(m => m.attachment_ref_id == id).Select(m => new AttachmentViewModel() {attachment_id=m.attachment_id,attachment_name=m.attachment_name,attachment_extension=m.attachment_extension,attachment_path=m.attachment_path,attachment_ref_id=m.attachment_ref_id,attachment_ref_type=m.attachment_ref_type }).ToList();
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "BOQ.cs", "GetBOQAttachments", ex.StackTrace, ex.Message);
            }
            return attachments;
        }
        public List<ProjectViewModel> GetBOQProjects()
        {
            List<ProjectViewModel> projects = new List<ProjectViewModel>();
            try
            {
                projects = (from boq in db.tb_build_of_quantity
                            join proj in db.tb_project on boq.project_id equals proj.project_id
                            orderby proj.project_full_name
                            where boq.status == true && boq.boq_status == "Completed"
                            select new ProjectViewModel() { project_id = boq.project_id, project_short_name = proj.project_short_name, project_full_name = proj.project_full_name }).ToList();
            }catch(Exception ex)
            {
                ErrorLog.ErrorLogger.LogEntry(EnumConstants.ErrorType.Error, "BOQ.cs", "GetBOQProjects", ex.StackTrace, ex.Message);
            }
            return projects;
        }
    }
}