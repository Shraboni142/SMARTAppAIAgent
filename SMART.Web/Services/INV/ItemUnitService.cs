using PagedList;
using SMART.Web.Models.INV;
using SMART.Web.Repositories;
using SMART.Web.Repositories.INV;
using SMART.Web.TOs;
using System;
using System.Collections.Generic;

namespace SMART.Web.Services.INV
{
    public interface IItemUnitService
    {
        IPagedList<ItemUnit> GetModelPageList(string searchStr, string sCode, int page, int size, string flag);
        IEnumerable<ItemUnit> GetAllItemUnits();
        ItemUnit GetItemUnitDetails(int itemUnitID);

        bool CheckIfExist(int id, string name);

        bool AddItemUnit(ItemUnit itemUnit);
        bool UpdateItemUnit(ItemUnit itemUnit);

        void SaveItemUnit();

        bool DeleteItemUnit(int ItemUnitId);
        IEnumerable<IdNameListTO> ItemUnitNameIdList();
        ItemUnit GetUnitByName(string name);
    }

    public class ItemUnitService : IItemUnitService
    {
        private readonly IItemUnitRepository _itemUnitReporitory;


        private readonly IUnitOfWork _unitOfWork;
        public ItemUnitService(IItemUnitRepository itemUnitReporitory, IUnitOfWork unitOfWork)
        {
            _itemUnitReporitory = itemUnitReporitory;

            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ItemUnit> GetAllItemUnits()
        {
            var getAllItemUnits = _itemUnitReporitory.GetMany(p => p.IsDeleted != true);
            return getAllItemUnits;
        }


        public ItemUnit GetItemUnitDetails(int ID)
        {
            var getItemUnitDetails = _itemUnitReporitory.Get(u => u.Id == ID);
            return getItemUnitDetails;
        }

        public bool AddItemUnit(ItemUnit model)
        {
            try
            {
                _itemUnitReporitory.Add(model);
                SaveItemUnit();
                return true;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }

        }

        public bool CheckIfExist(int id, string name)
        {
            name = !string.IsNullOrEmpty(name) ? name.Trim().ToLower() : "";

            int countUnit = _itemUnitReporitory.GetCount(r => r.Id != id && r.UnitName.Trim().ToLower() == name);

            if (countUnit > 0)
                return true;
            return false;
        }

        public bool UpdateItemUnit(ItemUnit model)
        {
            try
            {
                model.UpdatedOn = DateTime.Now;
                _itemUnitReporitory.Update(model);
                SaveItemUnit();
                return true;
            }

            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }
        }

        public void SaveItemUnit()
        {
            _unitOfWork.Commit();
        }

        public bool DeleteItemUnit(int UnitId)
        {
            var UnitDetails = _itemUnitReporitory.Get(u => u.Id == UnitId);
            try
            {
                _itemUnitReporitory.Delete(UnitDetails);
                SaveItemUnit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<IdNameListTO> ItemUnitNameIdList()
        {
            return _itemUnitReporitory.GetBy(r => true, r => new IdNameListTO { Id = r.Id, Name = r.UnitName });
        }
        public ItemUnit GetUnitByName(string name)
        {
            return _itemUnitReporitory.Get(r => r.UnitName.ToUpper() == name.ToUpper());
        }

        public IPagedList<ItemUnit> GetModelPageList(string searchStr, string sCode, int page, int size, string flag)
        {
            var pagedModels = _itemUnitReporitory.GetPage(
                new Page { PageNumber = page, PageSize = size },
                d => d.IsDeleted != true
                && (!string.IsNullOrEmpty(searchStr) ? d.UnitName.ToLower().Contains(searchStr.Trim().ToLower()) : true)
                && (!string.IsNullOrEmpty(sCode) ? d.Remark.ToLower().Contains(sCode.Trim().ToLower()) : true),
                order => order.UnitName);
            return pagedModels;
        }
    }
}