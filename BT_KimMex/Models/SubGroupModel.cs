using BT_KimMex.Class;
using BT_KimMex.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BT_KimMex.Models
{
    public class SubGroupModel
    {
        [Key]
        public string sub_group_id { get; set; }
        public string sub_group_code { get; set; }
        public string sub_group_name { get; set; }
        public string class_id { get; set; }
        public string class_name { get; set; }
        public Nullable<bool> is_active { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public string updated_by { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

        public ClassViewModel group { get; set; }

        public SubGroupModel()
        {
            group=new ClassViewModel();
        }

        public static SubGroupModel ConvertEntityToModel(tb_sub_group entity)
        {
            return new SubGroupModel()
            {
                sub_group_id= entity.sub_group_id,
                sub_group_code=entity.sub_group_code,   
                sub_group_name=entity.sub_group_name,
                class_id=entity.class_id,
                is_active  = entity.is_active,
                created_by=entity.created_by,
                updated_by=entity.updated_by,
                updated_at=entity.updated_at,
                created_at=entity.created_at,
            };
        }

        public static SubGroupModel GetSubGroupItem(string id)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                SubGroupModel model = new SubGroupModel();
                var result = db.tb_sub_group.Find(id);
                model=SubGroupModel.ConvertEntityToModel(result);
                model.group = CommonFunctions.GetClassItem(model.class_id);
                return model;
            }
        }
        public static List<SubGroupModel> GetSubGroupList(string group_id="")
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                List<SubGroupModel> models=new List<SubGroupModel>();
                List<tb_sub_group> results = new List<tb_sub_group>();
                if(string.IsNullOrEmpty(group_id))
                    results=db.tb_sub_group.OrderBy(s=>s.sub_group_name).Where(s=>s.is_active==true).ToList();
                else
                    results = db.tb_sub_group.OrderBy(s => s.sub_group_name).Where(s => s.is_active == true && string.Compare(s.class_id,group_id)==0).ToList();

                foreach (var rs in results)
                {
                    models.Add(SubGroupModel.GetSubGroupItem(rs.sub_group_id));
                }
                return models;
            }
        }
        public static string CreateUpdateSubGroup(SubGroupModel model)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_sub_group subGroup = new tb_sub_group();
                if (string.IsNullOrEmpty(model.sub_group_id))
                {
                    subGroup = new tb_sub_group();
                    subGroup.sub_group_id = Guid.NewGuid().ToString();
                    subGroup.sub_group_code =string.IsNullOrEmpty(model.sub_group_code)? generateSubGroupCode(): model.sub_group_code;
                    subGroup.sub_group_name = model.sub_group_name;
                    subGroup.class_id = model.class_id;
                    subGroup.is_active = true;
                    subGroup.created_at = CommonClass.ToLocalTime(DateTime.Now);
                    subGroup.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                    subGroup.created_by = model.created_by;
                    subGroup.updated_by = model.created_by;
                    db.tb_sub_group.Add(subGroup);
                    db.SaveChanges();

                }
                else
                {
                    subGroup = db.tb_sub_group.Find(model.sub_group_id);
                    if (subGroup != null)
                    {
                        subGroup.sub_group_code = model.sub_group_code;
                        subGroup.sub_group_name = model.sub_group_name;
                        subGroup.class_id = model.class_id;
                        subGroup.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                        subGroup.updated_by = model.created_by;
                        db.SaveChanges();

                        updateProductCodebySubGroupId(subGroup.sub_group_id);
                    }
                }
                return subGroup.sub_group_id;
            }
        }

        public static void DeleteSubGroup(string id,string userId)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                tb_sub_group subGroup = db.tb_sub_group.Find(id);
                if (subGroup != null)
                {
                    subGroup.is_active = false;
                    subGroup.updated_at = CommonClass.ToLocalTime(DateTime.Now);
                    subGroup.updated_by = userId;
                    db.SaveChanges();
                }
            }
        }

        public static bool isSubGroupExist(string groupId,string subGroupCode,string subGroupId="")
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                bool isExist = false;
                tb_sub_group subGroup = new tb_sub_group();
                if (string.IsNullOrEmpty(subGroupId))
                {
                    subGroup = db.tb_sub_group.Where(s => s.is_active == true && string.Compare(s.class_id, groupId) == 0 && string.Compare(s.sub_group_code.ToLower(), subGroupCode.ToLower()) == 0).FirstOrDefault();

                }
                else
                {
                    subGroup = db.tb_sub_group.Where(s => s.is_active == true && string.Compare(s.class_id, groupId) == 0 && string.Compare(s.sub_group_code.ToLower(), subGroupCode.ToLower()) == 0 && string.Compare(s.sub_group_id,subGroupId)!=0)
                        .FirstOrDefault();
                }
                if (subGroup != null)
                    isExist = true;


                return isExist;
            }
        }
        

        public static bool isSubGroupCodeExist(string subGroupCode)
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                bool isExist = false;
                var obj= db.tb_sub_group.Where(s=>s.is_active==true && string.Compare(s.sub_group_code,subGroupCode)==0).FirstOrDefault();
                if(obj!=null)
                    isExist= true;
                return isExist;
            }
        }
        public static string generateSubGroupCode()
        {
            using (kim_mexEntities db = new kim_mexEntities()) {
                string newSubGroupCode = "1";
                var obj = db.tb_sub_group.Where(s => s.is_active == true).Count();
                int codeNumber = obj + 1;
                var isExist = isSubGroupCodeExist(codeNumber.ToString());
                if (isExist)
                    codeNumber = codeNumber + 1;
                newSubGroupCode=codeNumber.ToString();
                return newSubGroupCode;
            }
        }

        public static bool isSubGroupNameExist(string groupId,string subGroupName,string subGroupId="")
        {
            using(kim_mexEntities db=new kim_mexEntities())
            {
                bool isExist=false;
                int result = 0;
                if (string.IsNullOrEmpty(subGroupId))
                {
                    result = db.tb_sub_group.Where(s => s.is_active == true && string.Compare(s.class_id, groupId) == 0 && string.Compare(s.sub_group_name.ToLower().Trim(), subGroupName.ToLower().Trim()) == 0).Count();
                }
                else
                {
                    result = db.tb_sub_group
                        .Where(s => s.is_active == true && string.Compare(s.class_id, groupId) == 0 && string.Compare(s.sub_group_name.ToLower().Trim(), subGroupName.ToLower().Trim()) == 0 && string.Compare(s.sub_group_id,subGroupId)!=0 )
                        .Count();
                }
                
                if(result>0)
                    isExist=true;
                return isExist;
            }
        }

        public static void updateProductCodebySubGroupId(string subGroupId)
        {
            try
            {
                kim_mexEntities db = new kim_mexEntities();
                var products = db.tb_product.Where(s => s.status == true && string.Compare(s.sub_group_id, subGroupId) == 0).ToList();
                foreach(var product in products)
                {
                    if (!string.IsNullOrEmpty(product.product_code))
                    {
                        string productCode = CommonFunctions.ReGenerateProductCode(product.group_id, subGroupId, product);
                        tb_product newProduct = db.tb_product.Find(product.product_id);
                        newProduct.product_code = productCode;
                        newProduct.updated_date = CommonClass.ToLocalTime(DateTime.Now);
                        db.SaveChanges();
                    }
                }
            }catch(Exception ex)
            {

            }
        }

    }
}