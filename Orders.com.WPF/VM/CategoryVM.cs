﻿using Orders.com.BLL.Domain;
using Orders.com.BLL.Services;

namespace Orders.com.WPF.VM
{
    public class CategoryVM : OrdersDotComVMBase<Category>
    {
        public CategoryVM (CategoryService service) : base(service)
        {
        }

        public CategoryVM(Category customer, CategoryService service) : base(customer, service)
        {
        }

        public long ID
        {
            get { return CurrentEntity.ID; }
        }

        public string Name
        {
            get { return CurrentEntity.Name; }
            set
            {
                CurrentEntity.Name = value;
                IsDirty = true;
                OnPropertyChanged("Name");
            }
        }

        protected override void OnInsertSuccess(Category result)
        {
            OnPropertyChanged("ID");
        }
    }
}
