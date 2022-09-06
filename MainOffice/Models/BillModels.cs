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
    [DisplayClass(GroupName = "BillList", Name = "BillShortName", ResourceType = typeof(GlobalRes), ControllerName = "Bills")]
    public class Bill
    {
        public Bill() { }
        public Bill(OperDayBill opBill, DateTime date, int salon, int employee)
        {
            BillNumber = opBill.BillNumber;
            IssueDate = date;
            SalonId = salon;
            EmployeeId = employee;
            StartTime = opBill.StartTime;
            EndTime = opBill.EndTime;
            BO = opBill.BO;
            ClientId = opBill.ClientId;
            BillLines = new List<BillLine>();
            foreach (OperDayBillLine operDayBillLine in opBill.BillLines )
            {
                BillLine line = new BillLine(operDayBillLine);
                BillLines.Add(line);
            }
            PrintCount = opBill.PrintOperDayBill.Count;

        }
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "BillNumber", Prompt = "PromptEnter")]
        public int BillNumber { get; set; }
        [Column(TypeName = "Date")] //Колонка в базе содержит только дату
        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "BillIssueDate", Prompt = "PromptEnter")]
        public DateTime IssueDate { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonName", ShortName = "SalonShortName")]
        public int SalonId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonName", ShortName = "SalonShortName")]
        public Salon Salon { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        public int EmployeeId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        public Employee Employee { get; set; }
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
        public virtual List<BillLine> BillLines { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "PrintCountName")]
        public int PrintCount { get; set; }
    }
    public class BillFilter
    {
        public BillFilter() { }
        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? IssueDateFrom { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? IssueDateTo { get; set; }

        public int[] SalonIdSelected { get; set; }

        public int[] EmployeeIdSelected { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Time)]
        public DateTime? StartTimeFrom { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Time)]
        public DateTime? StartTimeTo { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Time)]
        public DateTime? EndTimeFrom { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Time)]
        public DateTime? EndTimeTo { get; set; }
        public bool BO { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        public int? PrintCountFrom { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        public int? PrintCountTo { get; set; }
    }
    public class BillJsonViewModel
    {
        public BillJsonViewModel(Bill bill)
        {
            id = bill.Id;
            BillNumber = bill.BillNumber;
            IssueDate = bill.IssueDate.ToShortDateString();
            Salon = new InternalName
            {
                Name = bill.Salon.Name
            };
            Employee = new InternalName
            {
                Name = bill.Employee.FamilyName + " " + bill.Employee.Name + " " + bill.Employee.StaffNumber 
            };
            if (bill.StartTime.HasValue)
            { StartTime = bill.StartTime.Value.ToString(@"hh\:mm"); }
            if (bill.EndTime.HasValue)
            { EndTime = bill.EndTime.Value.ToString(@"hh\:mm"); }
            BO = bill.BO;
            if (bill.Client != null)
            {
                Client = new InternalName
                {
                    Name = bill.Client.PhoneNumber + (bill.Client.FamilyName != null ? " " + bill.Client.FamilyName : "") + " " + bill.Client.Name
                };
            }
            if (bill.BillLines != null)
            { 
                BillLines = new List<BillLineJsonViewModel>();
                foreach (BillLine billLine in bill.BillLines)
                {
                    BillLineJsonViewModel newLine = new BillLineJsonViewModel(billLine);
                    if (billLine.Cancel)
                    {
                        Corrections = true;
                    }
                    else
                    {
                        Summ = Summ + newLine.Summ;
                    }
                    BillLines.Add(new BillLineJsonViewModel(billLine));
                }
            }
            PrintCount = bill.PrintCount;
        }
        public int id { get; set; }
        public int BillNumber { get; set; }
        public string IssueDate { get; set; }
        public InternalName Salon { get; set; }
        public InternalName Employee { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool BO { get; set; }
        public InternalName Client { get; set; }
        public List<BillLineJsonViewModel> BillLines { get; set; }
        public decimal Summ { get; set; }
        public bool Corrections { get; set; } = false;
        public class InternalName
        {
            public string Name { get; set; }
        }
        public int PrintCount { get; set; }

    }
    public class BillLine
    {
        public BillLine() { }
        public BillLine(OperDayBillLine operDayBillLine)
        {
            CashRegCode = operDayBillLine.CashRegCode;
            ProductOrServiceName = operDayBillLine.ProductOrServiceName;
            Quantity = operDayBillLine.Quantity;
            Promotion = operDayBillLine.Promotion;
            SellPrice = operDayBillLine.SellPrice;
            InsertDateTime = operDayBillLine.InsertDateTime;
            Cancel = operDayBillLine.Cancel;
            CancelDateTime = operDayBillLine.CancelDateTime;
            AdminVisaId = operDayBillLine.AdminVisaId;
        }
        public int Id { get; set; }
        public int BillId { get; set; }
        public Bill Bill { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodeName")]
        public int CashRegCode { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "ProductServiceName")]
        public string ProductOrServiceName { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "Quantity")]
        public int Quantity { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Promotion")]
        public string Promotion { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "SellPrice")]
        public decimal SellPrice { get; set; }
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
    public class BillLineJsonViewModel
    {
        public BillLineJsonViewModel(BillLine billLine)
        {
            id = billLine.Id;
            CashRegCode = billLine.CashRegCode;
            ProductOrServiceName = billLine.ProductOrServiceName;
            Quantity = billLine.Quantity;
            Promotion = billLine.Promotion;
            SellPrice = billLine.SellPrice;
            Summ = Quantity * SellPrice;
            InsertDateTime = billLine.InsertDateTime.ToShortDateString() + " " + billLine.InsertDateTime.ToShortTimeString();
            Cancel = billLine.Cancel;
            if (billLine.CancelDateTime.HasValue)
            { CancelDateTime = billLine.CancelDateTime.Value.ToShortDateString() + " " + billLine.CancelDateTime.Value.ToShortTimeString(); }
            else
            { CancelDateTime = ""; }
            if (billLine.AdminVisa != null)
            {
                AdminVisa = new InternalName
                {
                    Name = billLine.AdminVisa.StaffNumber + " " + billLine.AdminVisa.FamilyName + " " + billLine.AdminVisa.Name
                };
            }
        }
        public int id { get; set; }
        public int CashRegCode { get; set; }
        public string ProductOrServiceName { get; set; }
        public int Quantity { get; set; }
        public string Promotion { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Summ { get; set; }
        public string InsertDateTime { get; set; }
        public bool Cancel { get; set; }
        public string CancelDateTime { get; set; }
        public InternalName AdminVisa { get; set; }
        public class InternalName
        {
            public string Name { get; set; }
        }
    }
    
    public class TemporaryBillModel
    {
        public int id { get; set; }
        [Display(Name = "Номер счета")]
        [RegularExpression(@"^([0-9]{5})$")]
        public int BillNumber { get; set; }
        [Display(Name = "Дата")]
        [Required]
        [RegularExpression(@"^20([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|[1-2][0-9]|3[0-1])$",ErrorMessage ="Дата должна быть в формате ГГГГММДД")]
        public string Date { get; set; }
        [Display(Name = "Время печати")]
        [RegularExpression(@"^([0-1][0-9]|2[0-3])(0[0-9]|[1-5][0-9])$", ErrorMessage = "Время должно быть в формате ЧЧММ")]
        [Required]
        public string StartTime { get; set; }
        [Display(Name = "Салон")]
        public string Salon { get; set; }
        [Display(Name = "Сотрудник")]
        public string Worker { get; set; }
        [Display(Name = "Сумма счета")]
        public decimal SellPrice { get; set; }
        [Display(Name = "Ошибка")]
        public int Actual { get; set; }
        [Display(Name = "БО")]
        public string BO { get; set; }
        public List<TemporaryBillLine> BillLines { get; set; }
    }

    public class TemporaryBillLine
    {
        public int id { get; set; }
        [Display(Name = "Номер счета")]
        public int Bill { get; set; }
        [Display(Name = "Код кассы")]
        [Required]
        public int CashRegCode { get; set; }
        [Display(Name = "Наименование услуги/продукции")]
        [Required]
        public string ServiceProductName { get; set; }
        [Display(Name = "Колличество")]
        [Required]
        [Range(1,999999,ErrorMessage ="Кол-во должно быть от 1 до 999999")]
        public int Quantity { get; set; }
        [DataType(DataType.Currency)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodePriceName", ShortName = "CashRegCodePriceShortName")]
        [Range(0.1, 999999, ErrorMessageResourceType = typeof(GlobalRes), ErrorMessageResourceName = "ErrPriceRange")]
        public decimal SellPrice { get; set; }
        [Display(Name = "Скидка")]
        public string Discount { get; set; }
        public decimal Summ { get; set; }
    }

    public class AnySelectList
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class TemporaryBillReport
    {
        public int id { get; set; }
        public string Worker { get; set; }
        public string Salon { get; set; }
        public decimal Summ { get; set; }
        public decimal SummDisc10 { get; set; }
        public decimal SummDiscStaff { get; set; }
        public decimal SummBO { get; set; }
        public int BillQuantity { get; set; }
        public int BillErrorQuantity { get; set; }

    }

    public class TemporaryBillReportView
    {
        public string start { get; set; }
        public string end { get; set; }
        public List<TemporaryBillReportSalon> reportsSalon { get; set; }
    }

    public class TemporaryBillReportSalon
    {
        public string Salon { get; set; }
        public decimal Summ { get; set; }
        public decimal SummDisc10 { get; set; }
        public decimal SummDiscStaff { get; set; }
        public decimal SummBO { get; set; }
        public int BillQuantity { get; set; }
        public int BillErrorQuantity { get; set; }
        public List<TemporaryBillReport> reports { get; set; }
    }
}