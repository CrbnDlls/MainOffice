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
    public class OperationDay
    {
        public int Id { get; set; }
        public int SalonId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonShortName")]
        public Salon Salon { get; set; }
        [Column(TypeName = "Date")] //Колонка в базе содержит только дату
        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(GlobalRes), Name = "OperationDate")]
        public DateTime? OperationDate { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        public DateTime? OpenOperationPoint { get; set; }
        public int? OpenEmployeeId { get; set; }
        [ForeignKey("OpenEmployeeId")]
        public Employee OpenEmployee { get; set; }
        public string OpenGeoLocation { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        public DateTime? CloseOperationPoint { get; set; }
        public int? CloseEmployeeId { get; set; }
        [ForeignKey("CloseEmployeeId")]
        public Employee CloseEmployee { get; set; }
        public string CloseGeoLocation { get; set; }
        public virtual List<OperationDayEmployee> OperationDayEmployees { get; set; }
        public bool Alarm { get; set; }
    }
    [DisplayClass(GroupName = "OperationDayArchiveShortNameList", Name = "OperationDayArchiveShortName", ResourceType = typeof(GlobalRes), ControllerName = "OperationDayArchives")]
    public class OperationDayArchive
    {
        public OperationDayArchive() { }
        public OperationDayArchive(OperationDay operationDay)
        {
            SalonId = operationDay.SalonId;
            OperationDate = operationDay.OperationDate.Value;
            OpenOperationPoint = operationDay.OpenOperationPoint.Value;
            OpenEmployeeId = operationDay.OpenEmployeeId.Value;
            OpenGeoLocation = operationDay.OpenGeoLocation;
            CloseOperationPoint = operationDay.CloseOperationPoint.Value;
            CloseEmployeeId = operationDay.CloseEmployeeId.Value;
            CloseGeoLocation = operationDay.CloseGeoLocation;
        }
        public int Id { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonShortName")]
        public int SalonId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonShortName")]
        public Salon Salon { get; set; }
        [Column(TypeName = "Date")] //Колонка в базе содержит только дату
        [DataType(DataType.Date)]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [Display(ResourceType = typeof(GlobalRes), Name = "OperationDate")]
        public DateTime OperationDate { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:g}")]
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenOperationPoint")]
        public DateTime OpenOperationPoint { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenEmployee")]
        public int OpenEmployeeId { get; set; }
        [ForeignKey("OpenEmployeeId")]
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenEmployee")]
        public Employee OpenEmployee { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenGeoLocation")]
        public string OpenGeoLocation { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:g}")]
        [Display(ResourceType = typeof(GlobalRes), Name = "CloseOperationPoint")]
        public DateTime CloseOperationPoint { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CloseEmployee")]
        public int CloseEmployeeId { get; set; }
        [ForeignKey("CloseEmployeeId")]
        [Display(ResourceType = typeof(GlobalRes), Name = "CloseEmployee")]
        public Employee CloseEmployee { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "CloseGeoLocation")]
        public string CloseGeoLocation { get; set; }
        public virtual List<OperationDayEmployeeArchive> OperationDayEmployeeArchives { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class OperationDayArchiveFilter
    {
        public OperationDayArchiveFilter() { }

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? OperationDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? OperationDateTo { get; set; }

        public int[] SalonIdSelected { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.DateTime)]
        public DateTime? OpenOperationPointFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.DateTime)]
        public DateTime? OpenOperationPointTo { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.DateTime)]
        public DateTime? CloseOperationPointFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.DateTime)]
        public DateTime? CloseOperationPointTo { get; set; }

    }
    public class OperationDayArchiveJsonViewModel
    {
        public OperationDayArchiveJsonViewModel(OperationDayArchive operDay)
        {

            id = operDay.Id;
            CloseEmployee = new InternalName { Name = operDay.CloseEmployee != null ? operDay.CloseEmployee.FamilyName + " " + operDay.CloseEmployee.Name.Substring(0, 1) + "." + operDay.CloseEmployee.FathersName.Substring(0, 1) + "." : "" };
            CloseGeoLocation = operDay.CloseGeoLocation;
            CloseOperationPoint = operDay.CloseOperationPoint.ToString();
            OpenEmployee = new InternalName { Name = operDay.OpenEmployee != null ? operDay.OpenEmployee.FamilyName + " " + operDay.OpenEmployee.Name.Substring(0, 1) + "." + operDay.OpenEmployee.FathersName.Substring(0, 1) + "." : "" };
            OpenGeoLocation = operDay.OpenGeoLocation;
            OpenOperationPoint = operDay.OpenOperationPoint.ToString();
            OperationDate = operDay.OperationDate.ToShortDateString();
            Salon = new InternalName { Name = operDay.Salon.Name };
        }
        public int id { get; set; }

        public string CloseGeoLocation { get; set; }

        public string CloseOperationPoint { get; set; }

        public string OpenGeoLocation { get; set; }

        public string OpenOperationPoint { get; set; }

        public string OperationDate { get; set; }

        public InternalName CloseEmployee { get; set; }

        public InternalName OpenEmployee { get; set; }

        public InternalName Salon { get; set; }

        public class InternalName
        {
            public string Name { get; set; }
        }

    }

    public class OperationDayEmployee
    {
        public int Id { get; set; }
        public int OperationDayId { get; set; }
        public OperationDay OperationDay { get; set; }
        public int EmployeeId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        public Employee Employee { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        [Display(ResourceType = typeof(GlobalRes), Name = "StartTime")]
        public DateTime StartPoint { get; set; }
        public int StartEmployeeId { get; set; }
        [ForeignKey("StartEmployeeId")]
        public Employee StartEmployee { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        [Display(ResourceType = typeof(GlobalRes), Name = "EndTime")]
        public DateTime? EndPoint { get; set; }
        public int? EndEmployeeId { get; set; }
        [ForeignKey("EndEmployeeId")]
        public Employee CloseEmployee { get; set; }
        public virtual List<OperDayBill> OperDayBills { get; set; }
        public int? pin { get; set; }
    }

    [DisplayClass(GroupName = "WorkSheets", Name = "WorkSheet", ResourceType = typeof(GlobalRes), ControllerName = "OperationDayEmployeeArchives")]
    public class OperationDayEmployeeArchive
    {
        public OperationDayEmployeeArchive() { }
        public OperationDayEmployeeArchive(OperationDayEmployee operationDayEmployee)
        {
            EmployeeId = operationDayEmployee.EmployeeId;
            StartPoint = operationDayEmployee.StartPoint;
            StartEmployeeId = operationDayEmployee.StartEmployeeId;
            EndPoint = operationDayEmployee.EndPoint.Value;
            EndEmployeeId = operationDayEmployee.EndEmployeeId.Value;
        }
        public int Id { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "OperationDayArchiveShortName")]
        public int OperationDayArchiveId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "OperationDayArchiveShortName")]
        public OperationDayArchive OperationDayArchive { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        public int EmployeeId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        public Employee Employee { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:g}")]
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenOperationPoint")]
        public DateTime StartPoint { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenEmployee")]
        public int StartEmployeeId { get; set; }
        [ForeignKey("StartEmployeeId")]
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenEmployee")]
        public Employee StartEmployee { get; set; }
        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:g}")]
        [Display(ResourceType = typeof(GlobalRes), Name = "CloseOperationPoint")]
        public DateTime EndPoint { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CloseEmployee")]
        public int EndEmployeeId { get; set; }
        [ForeignKey("EndEmployeeId")]
        [Display(ResourceType = typeof(GlobalRes), Name = "CloseEmployee")]
        public Employee EndEmployee { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }
    
    public class OperationDayEmployeeArchiveJsonViewModel
    {
        public OperationDayEmployeeArchiveJsonViewModel(OperationDayEmployeeArchive operDayEmployee)
        {
            id = operDayEmployee.Id;
            EndEmployee = new InternalName { Name = operDayEmployee.EndEmployee != null ? operDayEmployee.EndEmployee.FamilyName + " " + operDayEmployee.EndEmployee.Name.Substring(0, 1) + "." + operDayEmployee.EndEmployee.FathersName.Substring(0, 1) + "." : "" };
            Employee = new InternalName { Name = operDayEmployee.Employee != null ? operDayEmployee.Employee.FamilyName + " " + operDayEmployee.Employee.Name + " " + operDayEmployee.Employee.StaffNumber : "" };
            EndPoint = operDayEmployee.EndPoint.ToString();
            StartEmployee = new InternalName { Name = operDayEmployee.StartEmployee != null ? operDayEmployee.StartEmployee.FamilyName + " " + operDayEmployee.StartEmployee.Name.Substring(0, 1) + "." + operDayEmployee.StartEmployee.FathersName.Substring(0, 1) + "." : "" };
            StartPoint = operDayEmployee.StartPoint.ToString();
            OperationDayArchive = new InternalName { Name = operDayEmployee.OperationDayArchive.Salon.Name + " " + operDayEmployee.OperationDayArchive.OperationDate.ToShortDateString()};
        }
        public int id { get; set; }

        

        public string EndPoint { get; set; }

        

        public string StartPoint { get; set; }

        public InternalName OperationDayArchive { get; set; }

        public InternalName EndEmployee { get; set; }

        public InternalName StartEmployee { get; set; }

        public InternalName Employee { get; set; }

        public class InternalName
        {
            public string Name { get; set; }
        }

    }

    [DisplayClass(GroupName = "WorkSheets", Name = "WorkSheet", ResourceType = typeof(GlobalRes), ControllerName = "OperationDays")]
    public class RegisterEmployeeViewModel
    {
        public RegisterEmployeeViewModel(Employee employee)
        {
            Id = employee.Id;
            StaffNumber = employee.StaffNumber.Value;
            FamilyName = employee.FamilyName;
            Name = employee.Name;
            Profession = employee.Profession.Name;
            if (employee.BarberLevel != null)
            BarberLevel = employee.BarberLevel.Name;
            foreach (PriceListUnit unit in employee.PriceListUnits)
            {
                PriceListUnits = PriceListUnits + " - " + unit.Name;
            }

        }
        public int Id { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "StaffNumberShortName")]
        public int StaffNumber { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "FamilyName")]
        public string FamilyName { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Name")]
        public string Name { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "ProfessionShortName")]
        public string Profession { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "BarberLevelShortName")]
        public string BarberLevel { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "PriceListUnitShortName")]
        public string PriceListUnits { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "WorkSheet")]
        public string Salon { get; set; }
    }

    public class StaffStatusOperDayViewModel
    {
        public string Profession { get; set; }
        public string Color { get; set; }
        public List<StatusMasterCardViewModel> Staff { get; set; }
    }

    public class StatusMasterCardViewModel
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public bool IsBusy { get; set; }
        public bool CallForVisa { get; set; }
        public string timeBusy { get; set; }
        public OperDayStatisticsViewModel.Employee.Bill billBusy { get; set; }
        public bool ShouldPay { get; set; }
        public OperDayStatisticsViewModel.Employee.Bill billToPay { get; set; }
        public bool HasPreReg { get; set; }
        public string TimePreReg { get; set; }

        //public PreRegestration PreReg {get;set;}
        
    }

    public class UnlockEditViewModel
    {
        public int Id { get; set; }
        public string WhoLocked { get; set; }
        public bool MayUnlock { get; set; }
        public string Version { get; set; }
    }

    
}