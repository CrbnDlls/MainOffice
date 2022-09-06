using MainOffice.Annotations;
using MainOffice.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace MainOffice.Models
{
    [DisplayClass(GroupName = "CashRegCodeList", Name = "CashRegCodeShortName", ResourceType = typeof(GlobalRes), ControllerName = "CashRegCodes")]
    public class CashRegCode
    {
        public string GetCodeName()
        {
            return Service != null ? Service.Name + (Service.ServiceVolume != null ? " | " + Service.ServiceVolume.Name : "") : Product.Name + (Product.ProductVolume != null ? " | " + Product.ProductVolume.Name : "") + (Product.Trademark != null ? " | " + Product.Trademark.Name : "") + (Product.Productline != null ? " | " + Product.Productline.Name : "");
        }
        public decimal GetPriceAccordingToDiscount(string discount)
        {
            return String.IsNullOrEmpty(discount) ? Price : discount == "10%" ? Price10.HasValue ? Price10.Value : Price : discount == "50%" ? Price50.HasValue ? Price50.Value : Price : discount == "Сотрудник" ? PriceStaff.HasValue ? PriceStaff.Value : Price : Price;
        }

        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        [Index("IX_CashRegCodeUnique", IsUnique = true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodeName", ShortName = "CashRegCodeShortName")]
        [RegularExpression(@"^([0-9]{1,6})$", ErrorMessageResourceName = "CashRegCodeFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        public int Code { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        public int? PriceListUnitId { get; set; }
        public PriceListUnit PriceListUnit { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        public int? ServiceId { get; set; }
        public Service Service { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePriceName", ShortName = "CashRegCodePriceShortName")]
        [Range(0,999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal Price { get; set; }
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePrice10Name", ShortName = "CashRegCodePrice10ShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Range(0, 999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal? Price10 { get; set; }
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePrice50Name", ShortName = "CashRegCodePrice50ShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Range(0, 999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal? Price50 { get; set; }
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePriceStaffName", ShortName = "CashRegCodePriceStaffShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Range(0, 999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal? PriceStaff { get; set; }
        [Include]
        public ICollection<DelayedUpdateCashRegCode> DelayedUpdateCashRegCodes { get; set; }
        [NotMapped]
        public string RadiosSwitch { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }
    public class CashRegCodeJsonViewModel
    {
        public CashRegCodeJsonViewModel(CashRegCode item, bool include)
        {

            id = item.Id;
            Code = item.Code;
            if (item.PriceListUnit != null)
            {
                PriceListUnit = new InternalName() { Name = item.PriceListUnit.Name };
            }
            if (item.Product != null)
            {
                Product = new InternalName() { Name = item.Product.Name + (item.Product.ProductVolume != null ? " | " + item.Product.ProductVolume.Name : "") + (item.Product.Trademark != null ? " | " + item.Product.Trademark.Name : "") + (item.Product.Productline != null ? " | " + item.Product.Productline.Name : "") };
            }
            if (item.Service != null)
            {
                Service = new InternalName() { Name = item.Service.Name + (item.Service.ServiceVolume != null ? " | " + item.Service.ServiceVolume.Name : "") };
            }
            Price = item.Price;
            Price10 = item.Price10;
            Price50 = item.Price50;
            PriceStaff = item.PriceStaff;
            if (include & item.DelayedUpdateCashRegCodes != null && item.DelayedUpdateCashRegCodes.Count > 0)
                DelayedUpdateCashRegCode = new DelayedUpdateCashRegCodeJsonViewModel(item.DelayedUpdateCashRegCodes.First(), false);
        }

        public int id { get; set; }

        public int Code { get; set; }

        public InternalName PriceListUnit { get; set; }

        public InternalName Product { get; set; }

        public InternalName Service { get; set; }

        public decimal Price { get; set; }

        public decimal? Price10 { get; set; }

        public decimal? Price50 { get; set; }

        public decimal? PriceStaff { get; set; }
        public DelayedUpdateCashRegCodeJsonViewModel DelayedUpdateCashRegCode { get; set; }

        public class InternalName
        {
            public string Name { get; set; }
        }

    }

    public class CashRegCodeFilter
    {
        public CashRegCodeFilter() { }
        public int[] PriceListUnitIdSelected { get; set; }
        
        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        public int? PriceFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        public int? PriceTo { get; set; }
        public int Price10Buttons { get; set; } = 0;
        public int Price50Buttons { get; set; } = 0;
        public int PriceStaffButtons { get; set; } = 0;
    }

    [DisplayClass(GroupName = "PriceListUnitList", Name = "PriceListUnitShortName", ResourceType = typeof(GlobalRes), ControllerName = "PriceListUnits")]
    public class PriceListUnit
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(100, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "PriceListUnitName", ShortName = "PriceListUnitShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<DelayedUpdateEmployee> DelayedUpdateEmployees { get; set; }
        public virtual ICollection<CashRegCode> CashRegCodes { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class PriceListUnitJsonViewModel
    {
        public PriceListUnitJsonViewModel(PriceListUnit item)
        {
            id = item.Id;
            Name = item.Name;
        }
        public int id { get; set; }
        public string Name { get; set; }
    }

    [DisplayClass(GroupName = "CashRegCodeList", Name = "CashRegCodeShortName", ResourceType = typeof(GlobalRes), ControllerName = "DelayedUpdateCashRegCodes")]
    public class DelayedUpdateCashRegCode
    {
        public DelayedUpdateCashRegCode() { }

        public DelayedUpdateCashRegCode(CashRegCode code)
        {
            this.CashRegCodeId = code.Id;
            this.Code = code.Code;
            this.Price = code.Price;
            this.Price10 = code.Price10;
            this.Price50 = code.Price50;
            this.PriceListUnitId = code.PriceListUnitId;
            this.PriceStaff = code.PriceStaff;
            this.ProductId = code.ProductId;
            this.ServiceId = code.ServiceId;
        }
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required]
        [Index("IX_CashRegCodeUnique", IsUnique = true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodeName", ShortName = "CashRegCodeShortName")]
        [RegularExpression(@"^([0-9]{1,6})$", ErrorMessageResourceName = "CashRegCodeFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        public int Code { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        public int? PriceListUnitId { get; set; }
        public PriceListUnit PriceListUnit { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        public int? ServiceId { get; set; }
        public Service Service { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePriceName", ShortName = "CashRegCodePriceShortName")]
        [Range(0, 999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal Price { get; set; }
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePrice10Name", ShortName = "CashRegCodePrice10ShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Range(0, 999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal? Price10 { get; set; }
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePrice50Name", ShortName = "CashRegCodePrice50ShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Range(0, 999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal? Price50 { get; set; }
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePriceStaffName", ShortName = "CashRegCodePriceStaffShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Range(0, 999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal? PriceStaff { get; set; }
        [HiddenInput(DisplayValue = false)]
        [AdditionalMetadata("ForeignKey", true)]
        public int? CashRegCodeId { get; set; }
        [ScaffoldColumn(false)]
        public CashRegCode CashRegCode { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "UpdateDate", ShortName = "UpdateDateShortName")]
        [Column(TypeName = "Date")]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDate { get; set; }
        [NotMapped]
        public string RadiosSwitch { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }
    public class DelayedUpdateCashRegCodeJsonViewModel
    {
        public DelayedUpdateCashRegCodeJsonViewModel(DelayedUpdateCashRegCode item, bool include)
        {

            id = item.Id;
            Code = item.Code;
            if (item.PriceListUnit != null)
            {
                PriceListUnit = new InternalName() { Name = item.PriceListUnit.Name };
            }
            if (item.Product != null)
            {
                Product = new InternalName() { Name = item.Product.Name + (item.Product.ProductVolume != null ? " | " + item.Product.ProductVolume.Name : "") + (item.Product.Trademark != null ? " | " + item.Product.Trademark.Name : "")  + (item.Product.Productline != null ? " | " + item.Product.Productline.Name : "") };
            }
            if (item.Service != null)
            {
                Service = new InternalName() { Name = item.Service.Name + (item.Service.ServiceVolume != null ? " | " + item.Service.ServiceVolume.Name : "") };
            }
            Price = item.Price;
            Price10 = item.Price10;
            Price50 = item.Price50;
            PriceStaff = item.PriceStaff;
            if (item.UpdateDate.HasValue)
                UpdateDate = item.UpdateDate.Value.ToShortDateString();
            if (include && item.CashRegCode != null)
                CashRegCode = new CashRegCodeJsonViewModel(item.CashRegCode, false);
        }

        public int id { get; set; }

        public int Code { get; set; }

        public InternalName PriceListUnit { get; set; }

        public InternalName Product { get; set; }

        public InternalName Service { get; set; }

        public decimal Price { get; set; }

        public decimal? Price10 { get; set; }

        public decimal? Price50 { get; set; }

        public decimal? PriceStaff { get; set; }
        public string UpdateDate { get; set; }

        public CashRegCodeJsonViewModel CashRegCode { get; set; }

        public class InternalName
        {
            public string Name { get; set; }
        }

    }
    public class DelayedUpdateCashRegCodeFilter
    {
        public DelayedUpdateCashRegCodeFilter() { }
        public int[] PriceListUnitIdSelected { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        public int? PriceFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        public int? PriceTo { get; set; }
        public int Price10Buttons { get; set; } = 0;
        public int Price50Buttons { get; set; } = 0;
        public int PriceStaffButtons { get; set; } = 0;
        public int UpdateDateButtons { get; set; } = 0;
        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDateTo { get; set; }
    }
    public class PriceListSelectViewModel
    {
        public int Identity { get; set; }
        public List<PriceListUnit> PriceListUnits { get; set; }
        public List<CashRegCode> CashRegCodes { get; set; }
        public bool ShowFull { get; set; } = false;
    }

    public class Promotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}