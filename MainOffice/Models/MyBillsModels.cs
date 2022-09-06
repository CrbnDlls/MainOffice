using MainOffice.Annotations;
using MainOffice.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MainOffice.Models
{
    [DisplayClass(GroupName = "BillList", Name = "BillShortName", ResourceType = typeof(GlobalRes), ControllerName = "MyBills")]
    public class MyBillsViewModel
    {

        public MyBillsViewModel(OperDayBill bill)
        {
            Id = bill.Id;
            BillNumber = bill.BillNumber;
            StartTime = bill.StartTime.HasValue ? bill.StartTime.Value.ToString("t") : "";
            EndTime = bill.EndTime.HasValue ? bill.EndTime.Value.ToString("t") : "";
            BO = bill.BO;/* ? GlobalRes.Yes : GlobalRes.No;*/
            ClientName = bill.Client != null ? bill.Client.Name : "";
            InitialPromo = bill.InitialPromo;
            Locked = bill.Locked;
            if (Locked)
            {
                WhoLocked = bill.WhoLocked.StaffNumber + " " + bill.WhoLocked.FamilyName + " " + bill.WhoLocked.Name.Substring(0, 1) + ". " + bill.WhoLocked.FathersName.Substring(0, 1) + ".";
            }
            RowVersion = Convert.ToBase64String(bill.RowVersion);
            if (bill.BillLines != null)
            {
                BillLines = new List<MyBillLineViewModel>();
                foreach (OperDayBillLine line in bill.BillLines)
                {
                    if (!line.Cancel)
                    {
                        Summ = Summ + (line.SellPrice * line.Quantity);
                        BillLines.Add(new MyBillLineViewModel(line));
                    }
                }
            }
            if (bill.PrintOperDayBill != null)
            {
                PrintCount = bill.PrintOperDayBill.Count;
            }
        }
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "BillNumber", Prompt = "PromptEnter")]
        public int BillNumber { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "BillStartTime", ShortName = "Start")]
        public string StartTime { get; set; }
        [DataType(DataType.Time)]
        [Display(ResourceType = typeof(GlobalRes), Name = "BillEndTime", ShortName = "End")]
        public string EndTime { get; set; }
        [Display(Name = "БО")]
        public bool BO { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "ClientShortName")]
        public string ClientName { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Summ")]
        public decimal Summ { get; set; }
        public List<MyBillLineViewModel> BillLines { get; set; }
        public string InitialPromo { get; set; }
        public bool Locked { get; set; } = false;
        public string WhoLocked { get; set; }
        public string RowVersion { get; set; }
        public int PrintCount { get; set; }
    }

    public class MyBillLineViewModel
    {
        public MyBillLineViewModel(OperDayBillLine line)
        {
            Id = line.Id;
            CashRegCode = line.CashRegCode;
            ProductOrServiceName = line.ProductOrServiceName;
            Quantity = line.Quantity;
            Promotion = line.Promotion;
            SellPrice = line.SellPrice;
        }
        public int Id { get; set; }
        public int CashRegCode { get; set; }
        public string ProductOrServiceName { get; set; }
        public int Quantity { get; set; }
        public string Promotion { get; set; }
        public decimal SellPrice { get; set; }
    }

    [DisplayClass(GroupName = "BillList", Name = "BillShortName", ResourceType = typeof(GlobalRes), ControllerName = "Bills")]
    public class OperDayBill
    {
        public bool RequireAdminVisa()
        {
            if (BillLines.Any(x => x.Cancel & !x.AdminVisaId.HasValue))
                return true;
            if (BillLines.Any(x => !x.Cancel & x.Promotion != InitialPromo) & !VisaPromoId.HasValue)
                return true;

            return false;
        }
        public bool RequirePayment()
        {
            if (EndTime.HasValue & !PStatusId.HasValue & BillLines != null && BillLines.Any(x => !x.Cancel))
                return true;
            return false;
        }

        public bool IsHidden()
        {
            if (EndTime.HasValue & PStatusId.HasValue && PStatusId.Value == 3)
                return true;
            return false;
        }

        [Required]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "BillNumber", Prompt = "PromptEnter")]
        public int BillNumber { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        public int OperationDayEmployeeId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        public virtual OperationDayEmployee OperationDayEmployee { get; set; }
        [Column(TypeName = "DateTime2")]
        //[DataType(DataType.DateTime)] //Колонка в базе содержит только дату
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [DataType(DataType.Time)]
        [Display(ResourceType = typeof(GlobalRes), Name = "BillStartTime", ShortName = "Start")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "DateTime2")]
        //[DataType(DataType.DateTime)] //Колонка в базе содержит только дату
        [DataType(DataType.Time)]
        [Display(ResourceType = typeof(GlobalRes), Name = "BillEndTime", ShortName = "End")]
        public DateTime? EndTime { get; set; }
        [Display(Name = "БО")]
        public bool BO { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ClientShortName")]
        public int? ClientId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "ClientShortName")]
        public Client Client { get; set; }
        [Include]
        public virtual List<OperDayBillLine> BillLines { get; set; }
        public string InitialPromo { get; set; }
        public string ClientPromo { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "Visa")]
        public int? VisaPromoId { get; set; }
        [ForeignKey("VisaPromoId")]
        [Display(ResourceType = typeof(GlobalRes), Name = "Visa")]
        public Employee VisaPromo { get; set; }
        //concurrency dealing
        public bool Locked { get; set; } = false;
        public int? WhoLockedId { get; set; }
        [ForeignKey("WhoLockedId")]
        [Display(ResourceType = typeof(GlobalRes), Name = "EditLock")]
        public Employee WhoLocked { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
        public virtual OperDayBillPrint PrintOperDayBill { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "PStatus")]
        public int? PStatusId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "PStatus")]
        public virtual PStatus PStatus { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "Visa")]
        public int? PayVisaId { get; set; }
        [ForeignKey("PayVisaId")]
        [Display(ResourceType = typeof(GlobalRes), Name = "Visa")]
        public Employee PayVisa { get; set; }
    }

    public class OperDayBillLine
    {
        public int Id { get; set; }
        public int OperDayBillId { get; set; }
        public OperDayBill Bill { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodeName")]
        public int CashRegCode { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "ProductServiceName")]
        public string ProductOrServiceName { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "Quantity", ShortName = "QuantityShort")]
        public int Quantity { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Promotion")]
        public string Promotion { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "SellPrice", ShortName = "CashRegCodePriceShortName")]
        public decimal SellPrice { get; set; }
        public decimal MaxPrice { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "InsertDateTime")]
        public DateTime InsertDateTime { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Cancel")]
        public bool Cancel { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CancelDateTime")]
        public DateTime? CancelDateTime { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "Visa")]
        public int? AdminVisaId { get; set; }
        [ForeignKey("AdminVisaId")]
        [Display(ResourceType = typeof(GlobalRes), Name = "Visa")]
        public Employee AdminVisa { get; set; }
    }

    public class OperDayBillPrint
    {
        [ForeignKey("OperDayBill")]
        public int Id { get; set; }
        public int Count { get; set; }
        public virtual OperDayBill OperDayBill { get; set; }
    }
    public class PrintBillContent
    {
        public string action { get; set; }
        public string bill { get; set; }
        public string printerName { get; set; }
    }

    public class PStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public bool IsHidden { get; set; }
    }
}