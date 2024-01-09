using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BT_KimMex.Class;
using BT_KimMex.Models;
using BT_KimMex.Entities;

namespace BT_KimMex.Models
{
    public class MRRelatedStatusModel
    {
        public static void SaveMRRelatedStatus(tb_mr_related_status entity)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_mr_related_status status = new tb_mr_related_status();
                status.mr_id = entity.mr_id;
                status.st_status = entity.st_status;
                status.po_status = entity.po_status;
                status.tw_status = entity.tw_status;
                status.active = entity.active;
                status.created_at = entity.created_at;
                status.created_by = entity.created_by;
                db.tb_mr_related_status.Add(entity);
                db.SaveChanges();
            }
        }
    }
}