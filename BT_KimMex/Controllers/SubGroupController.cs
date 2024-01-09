using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BT_KimMex.Entities;
using BT_KimMex.Models;
using BT_KimMex.Class;
using Microsoft.AspNet.Identity;

namespace BT_KimMex.Controllers
{
    [Authorize]
    public class SubGroupController : Controller
    {
        // GET: SubGroup
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateUpdateAJAX(SubGroupModel model)
        {
            AJAXResultModel response = new AJAXResultModel();
            try
            {
                if (SubGroupModel.isSubGroupExist(model.class_id, model.sub_group_code,model.sub_group_id))
                {
                    response = new AJAXResultModel(false, "Sub Group Code is already exist.");
                }
                else if (SubGroupModel.isSubGroupNameExist(model.class_id, model.sub_group_name,model.sub_group_id))
                {
                    response = new AJAXResultModel(false, "Sub Group Name is already exist.");
                }
                else
                {
                    model.created_by = User.Identity.GetUserId();
                    string subGroupId = SubGroupModel.CreateUpdateSubGroup(model);
                }
                
                
            }catch(Exception ex)
            {
                response = new AJAXResultModel(false, ex.InnerException.Message);
            }
            return Json(new { response }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubGroupDataTable(string group_id="")
        {
            return Json(new { data = SubGroupModel.GetSubGroupList(group_id) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            AJAXResultModel response = new AJAXResultModel();
            try
            {
                SubGroupModel.DeleteSubGroup(id, User.Identity.GetUserId());
            }catch(Exception ex)
            {
                response = new AJAXResultModel(false, ex.InnerException.Message);
            }
            return Json(new { response }, JsonRequestBehavior.AllowGet);
        }
    }
}