using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Class;
using BT_KimMex.Entities;
using BT_KimMex.Models;

namespace BT_KimMex.Class
{
    public class ClsMRCutOff
    {
        public static List<MRCutOffViewModel> GetMRCutOffListItems(bool isAdmin,bool isPurchaser,bool isSiteStockKeeper,string userid)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                if (isAdmin)
                {
                    return (from co in db.tb_mr_cut_off
                            join mr in db.tb_item_request on co.material_request_id equals mr.ir_id
                            join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                            orderby co.updated_at descending
                            where co.active == true
                            select new MRCutOffViewModel()
                            {
                                mr_cut_off_id = co.mr_cut_off_id,
                                mr_cut_off_number = co.mr_cut_off_number,
                                material_request_id = co.material_request_id,
                                material_request_number = mr.ir_no,
                                project_id = proj.project_id,
                                project_name = proj.project_full_name,
                                mr_cut_off_status = co.mr_cut_off_status,
                                created_by = co.created_by,
                                created_at = co.created_at
                            }).ToList();
                }else
                {
                    List<MRCutOffViewModel> models = new List<MRCutOffViewModel>();
                    if(isPurchaser && isSiteStockKeeper)
                    {

                    }else if (isPurchaser)
                    {
                        models= (from co in db.tb_mr_cut_off
                                 join mr in db.tb_item_request on co.material_request_id equals mr.ir_id
                                 join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                 orderby co.updated_at descending
                                 where co.active == true && string.Compare(co.created_by,userid)==0
                                 select new MRCutOffViewModel()
                                 {
                                     mr_cut_off_id = co.mr_cut_off_id,
                                     mr_cut_off_number = co.mr_cut_off_number,
                                     material_request_id = co.material_request_id,
                                     material_request_number = mr.ir_no,
                                     project_id = proj.project_id,
                                     project_name = proj.project_full_name,
                                     mr_cut_off_status = co.mr_cut_off_status,
                                     created_by = co.created_by,
                                     created_at = co.created_at
                                 }).ToList();
                    }
                    else if (isSiteStockKeeper)
                    {
                        models = (from co in db.tb_mr_cut_off
                                  join mr in db.tb_item_request on co.material_request_id equals mr.ir_id
                                  join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                                  join sitesup in db.tbSiteSiteSupervisors on proj.project_id equals sitesup.site_id
                                  orderby co.updated_at descending
                                  where co.active == true && ((string.Compare(sitesup.site_supervisor_id, userid) == 0 && string.Compare(co.mr_cut_off_status,Status.Pending)==0) || string.Compare(co.approved_by,userid)==0)
                                  select new MRCutOffViewModel()
                                  {
                                      mr_cut_off_id = co.mr_cut_off_id,
                                      mr_cut_off_number = co.mr_cut_off_number,
                                      material_request_id = co.material_request_id,
                                      material_request_number = mr.ir_no,
                                      project_id = proj.project_id,
                                      project_name = proj.project_full_name,
                                      mr_cut_off_status = co.mr_cut_off_status,
                                      created_by = co.created_by,
                                      created_at = co.created_at
                                  }).ToList();
                    }
                    return models;
                }
                
            }
        }
        public static List<Models.ItemRequestViewModel> GetAllItemRequestDropdownList(string id = null)
        {
            List<Models.ItemRequestViewModel> models = new List<ItemRequestViewModel>();
            using (kim_mexEntities context = new kim_mexEntities())
            {
                models = context.tb_item_request.OrderByDescending(x => x.created_date).Where(x => x.status == true && x.is_mr == false && (string.Compare(x.ir_status, "Approved") == 0 || (string.Compare(x.ir_status, "Completed") == 0 && x.is_completed == false && x.is_cut_off==false))).Select(x => new Models.ItemRequestViewModel() { ir_id = x.ir_id, ir_no = x.ir_no, created_date = x.created_date, ir_project_id = x.ir_project_id }).ToList();
                if (!string.IsNullOrEmpty(id))
                {
                    var isExits = models.Where(m => string.Compare(m.ir_id, id) == 0).FirstOrDefault() != null ? true : false;
                    if (!isExits)
                    {
                        var item = context.tb_item_request.Find(id);
                        models.Add(new ItemRequestViewModel() { ir_id = item.ir_id, ir_no = item.ir_no, created_date = item.created_date, ir_project_id = item.ir_project_id });
                        models = models.OrderByDescending(x => x.created_date).ToList();
                    }
                }
            }
            return models;
        }
        public static MRCutOffViewModel GetMRCutOffItem(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                MRCutOffViewModel model = new MRCutOffViewModel();
                model = (from co in db.tb_mr_cut_off
                         join mr in db.tb_item_request on co.material_request_id equals mr.ir_id
                         join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                         where string.Compare(co.mr_cut_off_id, id) == 0
                         select new MRCutOffViewModel()
                         {
                             mr_cut_off_id = co.mr_cut_off_id,
                             mr_cut_off_number = co.mr_cut_off_number,
                             material_request_id = co.material_request_id,
                             material_request_number = mr.ir_no,
                             project_id = proj.project_id,
                             project_name = proj.project_full_name,
                             mr_cut_off_status = co.mr_cut_off_status,
                             created_by = co.created_by,
                             created_at = co.created_at
                         }).FirstOrDefault();
                model.mrCutOffDetail = (from cod in db.tb_mr_cut_off_detail
                                        join prod in db.tb_product on cod.item_id equals prod.product_id
                                        join unit in db.tb_unit on cod.item_unit_id equals unit.Id
                                        where string.Compare(cod.cut_off_id, model.mr_cut_off_id) == 0
                                        select new MRCutOffDetailViewModel()
                                        {
                                            cut_off_detail_id=cod.cut_off_detail_id,
                                            item_id=cod.item_id,
                                            item_code=prod.product_code,
                                            item_name=prod.product_name,
                                            item_unit_id=cod.item_unit_id,
                                            item_unit_name=unit.Name,
                                            material_request_qty=cod.material_request_qty,
                                            cut_off_qty=cod.cut_off_qty,
                                            cut_off_reason=cod.cut_off_reason,
                                            item_status=cod.item_status,
                                            approval_comment=cod.approval_comment,
                                        }).ToList();

                return model;
            }
        }
        public static bool IsMRCutOffSiteSupervisor(string id,string supervisorID)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                return (from co in db.tb_mr_cut_off
                 join mr in db.tb_item_request on co.material_request_id equals mr.ir_id
                 join proj in db.tb_project on mr.ir_project_id equals proj.project_id
                 join sitesup in db.tbSiteSiteSupervisors on proj.project_id equals sitesup.site_id
                 where string.Compare(co.mr_cut_off_id,id)==0 && string.Compare(sitesup.site_supervisor_id, supervisorID) == 0 
                 select new
                 {
                     co
                 }).ToList().Any()?true:false;
            }
        }
    }
}