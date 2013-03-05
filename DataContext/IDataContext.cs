using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAvail.MVC.Bootstrap.DataContext
{
    public interface IDataContext<TDom>
    {
        TDom GetNew(int? ParentId);

        int Create(TDom Order);

        int Update(TDom Order);

        TDom GetOne(int Id);

        TDom[] GetAll(int? Page, dynamic Filter);
       
        void Delete(int Id);

        void InitializeCrumbs(TDom Dom, int? ParentId);
    }
}