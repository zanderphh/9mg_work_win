using _9M.Work.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api.Domain;

namespace _9M.Work.WPF_Common.WpfBind
{
    public  class TreeViewDataConverter
    {
        public static List<TreeModel> ConverterTreeFromSellerCat(List<SellerCat> list)
        {
            List<TreeModel> treelist = new List<TreeModel>();
            list.ForEach(x =>
            {
                if (x.ParentCid == 0)
                {
                    TreeModel model = new TreeModel() { Id = x.Cid.ToString(), Name = x.Name,ParentId = x.ParentCid};
                    List<SellerCat> slist =  list.FindAll(y => {
                        return x.Cid == y.ParentCid;
                    });
                    List<TreeModel> childlist = new List<TreeModel>();
                    slist.ForEach(z => {
                        TreeModel childmodel = new TreeModel() { Id = z.Cid.ToString(), Name = z.Name, ParentId = z.ParentCid };
                        childlist.Add(childmodel);
                    });
                    model.Children = childlist;
                    treelist.Add(model);
                }
            });
            return treelist;
        }

        public static List<TreeModel> ConverterTreeFromPermission(List<PermissionModel> list)
        {
            List<TreeModel> treelist = new List<TreeModel>();
            list.ForEach(x =>
            {
                if (x.ParentId == 0)
                {
                    TreeModel model = new TreeModel() { Id = x.Id.ToString(), Name = x.Pname, ParentId = x.ParentId };
                    List<PermissionModel> slist = list.FindAll(y =>
                    {
                        return x.Id == y.ParentId;
                    });
                    List<TreeModel> childlist = new List<TreeModel>();
                    slist.ForEach(z =>
                    {
                        TreeModel childmodel = new TreeModel() { Id = z.Id.ToString(), Name = z.Pname, ParentId = z.ParentId };
                        childlist.Add(childmodel);
                    });
                    model.Children = childlist;
                    treelist.Add(model);
                }
            });
            return treelist;
        }

        public static List<TreeModel> ConverterTreeFromPermission(List<UserPermissionModel> list)
        {
            List<TreeModel> treelist = new List<TreeModel>();
            list.ForEach(x =>
            {
                if (x.ParentId == 0)
                {
                    TreeModel model = new TreeModel() { Id = x.Id.ToString(), Name = x.Pname, ParentId = x.ParentId };
                    List<UserPermissionModel> slist = list.FindAll(y =>
                    {
                        return x.Id == y.ParentId;
                    });
                    List<TreeModel> childlist = new List<TreeModel>();
                    int flag = 0;
                    slist.ForEach(z =>
                    {
                        if(z.CanDo==1)
                        {
                            flag++;
                        }
                        TreeModel childmodel = new TreeModel() { Id = z.Id.ToString(), Name = z.Pname, ParentId = z.ParentId ,IsChecked = z.CanDo==1};
                        childlist.Add(childmodel);
                    });
                    if(flag>0)
                    {
                        model.IsChecked = true;
                    }
                    model.Children = childlist;
                    treelist.Add(model);
                }
            });
            return treelist;
        }
    }
}
