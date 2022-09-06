using MainOffice.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MainOffice.Models
{
    public class OperDayStatisticsViewModel
    {
        public OperDayStatisticsViewModel(OperationDay operDay, bool fullStat, bool overrideAlarm)
        {
            Id = operDay.Id;
            OperationDate = operDay.OperationDate.HasValue ? operDay.OperationDate.Value.ToShortDateString() : " - ";
            Salon = operDay.Salon.Name + " " + operDay.Salon.Address;
            OpenTime = operDay.OpenOperationPoint.HasValue ? operDay.OpenOperationPoint.Value.ToString() : " - ";
            OpenEmployee = operDay.OpenEmployee.FamilyName + " " + operDay.OpenEmployee.Name + " " + operDay.OpenEmployee.StaffNumber;
            StaffQuantity = operDay.OperationDayEmployees.Count(x => !x.EndPoint.HasValue).ToString();
            StaffQuantityBuisy = operDay.OperationDayEmployees.Count(x => !x.EndPoint.HasValue & x.OperDayBills.Any(y=>y.StartTime.HasValue & !y.EndTime.HasValue)).ToString();
            decimal SummClosedDec = 0, SummInWorkDec = 0, TerminalSummDec = 0, KasaSummDec = 0, DepositSummDec = 0, UnpaidSummDec = 0;
            int boQuantity = 0;
            Alarm = operDay.Alarm;
            if (fullStat)
            {
                Employees = new List<Employee>();
                StatTime = DateTime.Now.ToString("g");
                operDay.OperationDayEmployees = operDay.OperationDayEmployees != null ? operDay.OperationDayEmployees.OrderBy(x => x.Employee.Profession.OrderNumber).ThenBy(x => x.Employee.BarberLevel != null ? x.Employee.BarberLevel.OrderNumber : 0).ThenBy(x => x.Employee.FamilyName).ToList() : null;
            }

            foreach (OperationDayEmployee employee in operDay.OperationDayEmployees)
            {
                decimal workSheetSumm = 0,workSheetBoSumm = 0;
                
                if (employee.OperDayBills == null)
                    continue;
                foreach (OperDayBill bill in employee.OperDayBills)
                {
                    if (operDay.Alarm & bill.IsHidden() && !overrideAlarm)
                    {
                        continue;
                    }

                    if (bill.BO)
                        boQuantity = boQuantity + 1;
                    if (bill.BillLines == null)
                        continue;
                    if (bill.BillLines.Count == 0)
                        continue;
                    foreach (OperDayBillLine line in bill.BillLines)
                    {
                        if (line.Cancel)
                            continue;
                        if (bill.BO)
                        {
                            workSheetBoSumm = workSheetBoSumm + line.SellPrice * line.Quantity;
                        }
                        else
                        {
                            if (bill.EndTime.HasValue)
                            {
                                SummClosedDec = SummClosedDec + line.SellPrice * line.Quantity;
                                workSheetSumm = workSheetSumm + line.SellPrice * line.Quantity;
                                switch (bill.PStatusId)
                                {
                                    case 1:
                                        KasaSummDec = KasaSummDec + +line.SellPrice * line.Quantity;
                                        break;
                                    case 2:
                                        TerminalSummDec = TerminalSummDec + +line.SellPrice * line.Quantity;
                                        break;
                                    case 3:
                                        DepositSummDec = DepositSummDec + +line.SellPrice * line.Quantity;
                                        break;
                                    default:
                                        UnpaidSummDec = UnpaidSummDec + +line.SellPrice * line.Quantity;
                                        break;
                                }
                            }
                            else
                            {
                                SummInWorkDec = SummInWorkDec + line.SellPrice * line.Quantity;
                            }
                        }
                    }
                }
                if (fullStat)
                {
                    Employee worksheet = new Employee(employee, overrideAlarm ? false : operDay.Alarm);
                    worksheet.Summ = workSheetSumm.ToString();
                    worksheet.BoSumm = workSheetBoSumm.ToString();
                    Employees.Add(worksheet);
                }

            }
            BOQuantity = boQuantity.ToString();
            SummClosed = SummClosedDec.ToString();
            SummInWork = SummInWorkDec.ToString();
            KasaSumm = KasaSummDec.ToString();
            TerminalSumm = TerminalSummDec.ToString();
            DepositSumm = DepositSummDec.ToString();
            UnpaidSumm = UnpaidSummDec.ToString();
        }
        public int Id { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "OperationDate")]
        public string OperationDate { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonShortName")]
        public string Salon { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenOperationPoint")]
        public string OpenTime { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "OpenEmployee")]
        public string OpenEmployee { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "StaffQuantity")]
        public string StaffQuantity { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "StaffQuantityBuisy")]
        public string StaffQuantityBuisy { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SummClosed")]
        public string SummClosed { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SummInWork")]
        public string SummInWork { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "BoQuantity")]
        public string BOQuantity { get; set; }
        public List<Employee> Employees { get; set; }
        public string StatTime { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Terminal")]
        public string TerminalSumm { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Kasa")]
        public string KasaSumm { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Deposit")]
        public string DepositSumm { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Unpaid")]
        public string UnpaidSumm { get; set; }
        public bool Alarm { get; set; }

        public class Employee
        {
            public Employee(OperationDayEmployee worksheet, bool Alarm)
            {
                Id = worksheet.Id;
                IsClosed = worksheet.EndPoint.HasValue;
                EmployeeName = worksheet.Employee.FamilyName + " " + worksheet.Employee.Name + " " + worksheet.Employee.StaffNumber;
                Profession = worksheet.Employee.Profession.Name;
                if (worksheet.Employee.BarberLevelId.HasValue)
                {
                    BarberLevel = worksheet.Employee.BarberLevel.Name;
                    Color = worksheet.Employee.BarberLevel.Color;
                }
                else
                {
                    BarberLevel = "-";
                    Color = worksheet.Employee.Profession.Color;
                }
                BarberLevel = worksheet.Employee.BarberLevelId.HasValue ? worksheet.Employee.BarberLevel.Name : "-";
                StartPoint = worksheet.StartPoint.ToString();
                StartEmployee = worksheet.StartEmployee.FamilyName + " " + worksheet.StartEmployee.Name + " " + worksheet.StartEmployee.StaffNumber;
                EndPoint = worksheet.EndPoint.HasValue ? worksheet.EndPoint.Value.ToString() : "-";
                CloseEmployee = worksheet.EndEmployeeId.HasValue ? worksheet.CloseEmployee.FamilyName + " " + worksheet.CloseEmployee.Name + " " + worksheet.CloseEmployee.StaffNumber : "-";
                if (worksheet.OperDayBills != null & worksheet.OperDayBills.Count > 0)
                {

                    ClosedBills = new List<Bill>();
                    BillsInWork = new List<Bill>();
                    BillsToPay = new List<Bill>();
                    IsWorking = false;
                    TimeSpan startWork = new TimeSpan(0, 0, 0);
                    TimeSpan startConsult = new TimeSpan(0, 0, 0);
                    foreach (OperDayBill operDayBill in worksheet.OperDayBills)
                    {
                        if (Alarm & operDayBill.IsHidden())
                        {
                            continue;
                        }
                        if (operDayBill.EndTime.HasValue)
                        {
                            if (operDayBill.PStatusId.HasValue)
                            {
                                ClosedBills.Add(new Bill(operDayBill));
                            }
                            else
                            {
                                BillsToPay.Add(new Bill(operDayBill));
                            }
                        }
                        else
                        {
                            if (operDayBill.BillLines != null & operDayBill.BillLines.Any(x => !x.Cancel))
                            {
                                IsWorking = true;
                                startWork = startWork < DateTime.Now - operDayBill.BillLines.First().InsertDateTime ? DateTime.Now - operDayBill.BillLines.First().InsertDateTime : startWork;
                            }
                            else
                            {
                                startConsult = (DateTime.Now - operDayBill.StartTime).Value > startConsult ? (DateTime.Now - operDayBill.StartTime).Value : startConsult;
                            }
                            BillsInWork.Add(new Bill(operDayBill));
                        }
                    }
                    if (BillsInWork.Count > 0)
                    {
                        if (IsWorking)
                        {
                            Status = "Занят " + startWork.ToString(@"hh\:mm");
                            TimeWorking = startWork.ToString(@"hh\:mm");
                        }
                        else
                        {
                            IsConsulting = true;
                            Status = "Консультирует клиента " + startConsult.ToString(@"hh\:mm");
                            TimeWorking = startConsult.ToString(@"hh\:mm");
                        }
                    }
                    else
                    { Status = "Свободный сотрудник"; }
                }
                else
                {
                    Status = "Свободный сотрудник";
                }


            }
            public int Id { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
            public string EmployeeName { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "ProfessionShortName")]
            public string Profession { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "BarberLevelShortName")]
            public string BarberLevel { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "StartTime")]
            public string StartPoint { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "OpenEmployee")]
            public string StartEmployee { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "EndTime")]
            public string EndPoint { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "CloseEmployee")]
            public string CloseEmployee { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "SalonStateShortName")]
            public string Status { get; set; }
            public List<Bill> ClosedBills { get; set; }
            public List<Bill> BillsInWork { get; set; }
            public List<Bill> BillsToPay { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "SummClosed")]
            public string Summ { get; set; }
            [Display(ResourceType = typeof(GlobalRes), Name = "BoSumm")]
            public string BoSumm { get; set; }
            public string Color { get; set; }
            public bool IsWorking { get; set; } = false;
            public bool IsConsulting { get; set; } = false;
            public string TimeWorking { get; set; }
            public bool IsClosed { get; set; }

            public class Bill
            {
                public Bill(OperDayBill operDayBill)
                {
                    Id = operDayBill.Id;
                    BillNumber = operDayBill.BillNumber.ToString();
                    StartTime = operDayBill.StartTime.Value.ToString("t");
                    EndTime = operDayBill.EndTime.HasValue ? operDayBill.EndTime.Value.ToString("t") : "-";
                    BO = operDayBill.BO;
                    ClientId = operDayBill.ClientId;
                    Client = ClientId.HasValue ? operDayBill.Client.FamilyName + " " + operDayBill.Client.Name : "-";
                    InitialPromo = operDayBill.InitialPromo;
                    ClientPromo = operDayBill.ClientPromo;
                    VisaPromo = operDayBill.VisaPromoId.HasValue ? operDayBill.VisaPromo.FamilyName + " " + operDayBill.VisaPromo.Name + " " + operDayBill.VisaPromo.StaffNumber : "-";

                    if (operDayBill.BillLines != null)
                    {
                        decimal SummDec = 0;
                        BillLines = new List<BillLine>();
                        foreach (OperDayBillLine line in operDayBill.BillLines)
                        {
                            if (!line.Cancel)
                            {
                                SummDec = SummDec + (line.SellPrice * line.Quantity);
                            }
                            BillLines.Add(new BillLine(line));
                        }
                        Summ = SummDec.ToString();
                    }
                    CallForVisa = operDayBill.RequireAdminVisa();
                    RowVersion = operDayBill.RowVersion;
                }
                public int Id { get; set; }
                [Display(ResourceType = typeof(GlobalRes), Name = "BillNumber", Prompt = "PromptEnter")]
                public string BillNumber { get; set; }
                [Display(ResourceType = typeof(GlobalRes), Name = "BillStartTime", ShortName = "Start")]
                public string StartTime { get; set; }
                [Display(ResourceType = typeof(GlobalRes), Name = "BillEndTime", ShortName = "End")]
                public string EndTime { get; set; }
                [Display(Name = "БО")]
                public bool BO { get; set; }
                [Display(ResourceType = typeof(GlobalRes), Name = "ClientShortName")]
                public int? ClientId { get; set; }
                [Display(ResourceType = typeof(GlobalRes), Name = "ClientShortName")]
                public string Client { get; set; }
                public List<BillLine> BillLines { get; set; }
                [Display(ResourceType = typeof(GlobalRes), Name = "InitialPromo")]
                public string InitialPromo { get; set; }
                public string ClientPromo { get; set; }
                [Display(ResourceType = typeof(GlobalRes), Name = "VisaPromo")]
                public string VisaPromo { get; set; }
                public string Summ { get; set; }
                //concurrency dealing
                public bool CallForVisa { get; set; }
                public byte[] RowVersion { get; set; }

                public class BillLine
                {
                    public BillLine(OperDayBillLine operDayBillLine)
                    {
                        Id = operDayBillLine.Id;
                        CashRegCode = operDayBillLine.CashRegCode.ToString();
                        ProductOrServiceName = operDayBillLine.ProductOrServiceName;
                        Quantity = operDayBillLine.Quantity.ToString();
                        Promotion = operDayBillLine.Promotion;
                        SellPrice = operDayBillLine.SellPrice.ToString();
                        InsertDateTime = operDayBillLine.InsertDateTime.ToString();
                        Cancel = operDayBillLine.Cancel;
                        CancelDateTime = operDayBillLine.CancelDateTime.ToString();
                        AdminVisa = operDayBillLine.AdminVisaId.HasValue ? operDayBillLine.AdminVisa.FamilyName + " " + operDayBillLine.AdminVisa.Name + " " + operDayBillLine.AdminVisa.StaffNumber : "-";
                        Summ = (operDayBillLine.SellPrice * operDayBillLine.Quantity).ToString();

                    }
                    public int Id { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "CashRegCodeName")]
                    public string CashRegCode { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "ProductServiceName")]
                    public string ProductOrServiceName { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "QuantityShort")]
                    public string Quantity { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "Promotion")]
                    public string Promotion { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "SellPrice")]
                    public string SellPrice { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "InsertDateTime")]
                    public string InsertDateTime { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "Cancel")]
                    public bool Cancel { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "CancelDateTime")]
                    public string CancelDateTime { get; set; }
                    [Display(ResourceType = typeof(GlobalRes), Name = "VisaShort")]
                    public string AdminVisa { get; set; }
                    public string Summ { get; set; }
                }
            }

        }
    }
}