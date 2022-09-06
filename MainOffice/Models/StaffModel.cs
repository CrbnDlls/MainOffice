using MainOffice.Annotations;
using MainOffice.App_LocalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Linq;

namespace MainOffice.Models
{
    [DisplayClass(GroupName = "EmployeeList", Name = "EmployeeShortName",ResourceType = typeof(GlobalRes), ControllerName = "Employees")]
    public class Employee
    {
        [Required]
        [ScaffoldColumn(false)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "FamilyName", Prompt = "PromptEnter")]
        [Index("IX_EmployeeUnique",1,IsUnique = true)]
        public string FamilyName { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "Name", Prompt = "PromptEnter")]
        [Index("IX_EmployeeUnique", 2, IsUnique = true)]
        public string Name { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "FathersName", Prompt = "PromptEnter")]
        [Index("IX_EmployeeUnique", 3, IsUnique = true)]
        public string FathersName { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "BirthDay", Prompt = "PromptEnter")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Date")] //Колонка в базе содержит только дату
        [DataType(DataType.Date)]
        [Index("IX_EmployeeUnique", 4, IsUnique = true)]
        public DateTime BirthDay { get; set; }
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "OldFamilyName", Prompt = "PromptEnter", ShortName = "OldFamilyShortName")]
        [DisplayFormat(NullDisplayText ="-")]
        public string OldFamilyName { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ProfessionShortName")]
        public int? ProfessionId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "ProfessionShortName")]
        public Profession Profession { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "BarberLevelName", ShortName = "BarberLevelShortName")]
        public int? BarberLevelId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "BarberLevelName", ShortName = "BarberLevelShortName")]
        public BarberLevel BarberLevel { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeSalon", ShortName = "SalonShortName")]
        public int? SalonId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeSalon", ShortName = "SalonShortName")]
        public Salon Salon { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "HireDate", Prompt = "PromptEnter", ShortName = "HireDateShortName")]
        [Column(TypeName = "Date")]//Колонка в базе содержит только дату
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "StaffNumber", ShortName = "StaffNumberShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? StaffNumber { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(([0-9]{3})\)[ ]([0-9]{3})[-]([0-9]{2})[-]([0-9]{2})$", ErrorMessageResourceName = "TelFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(15, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 15)]
        [Display(ResourceType = typeof(GlobalRes), Name = "PhoneNumber1Name", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX", ShortName = "PhoneNumber1ShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        public string PhoneNumber1 { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(([0-9]{3})\)[ ]([0-9]{3})[-]([0-9]{2})[-]([0-9]{2})$", ErrorMessageResourceName = "TelFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(15, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 15)]
        [Display(ResourceType = typeof(GlobalRes), Name = "PhoneNumber2Name", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX", ShortName = "PhoneNumber2ShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        public string PhoneNumber2 { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "RegisterCompany", ShortName = "RegisterCompanyShortName")]
        public int? CompanyId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "RegisterCompany", ShortName = "RegisterCompanyShortName")]
        public Company Company { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "RegisterDate", ShortName = "RegisterDateShortName")]
        [Column(TypeName = "Date")]//Колонка в базе содержит только дату
                                   //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? RegisterDate { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "DismissalDate", ShortName = "DismissalDateShortName")]
        [Column(TypeName = "Date")]//Колонка в базе содержит только дату
                                   //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? DismissalDate { get; set; }
        [Include]
        public ICollection<DelayedUpdateEmployee> DelayedUpdateEmployees { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "PriceListUnitList")]
        public virtual ICollection<PriceListUnit> PriceListUnits { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class EmployeeFilter
    {
        public EmployeeFilter() { }

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? BirthDayFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? BirthDayTo { get; set; }

        public int OldFamilyNameButtons { get; set; } = 0;

        public int[] ProfessionIdSelected { get; set; }
        

        public int[] BarberLevelIdSelected { get; set; }
       

        public int[] SalonIdSelected { get; set; }
       

        public int HireDateButtons { get; set; } = 0;

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? HireDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? HireDateTo { get; set; }

        public int StaffNumberButtons { get; set; } = 0;

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        public int? StaffNumberFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        public int? StaffNumberTo { get; set; }

        public int PhoneNumber1Buttons { get; set; } = 0;

        public int PhoneNumber2Buttons { get; set; }
        public int[] CompanyIdSelected { get; set; }
        
        public int RegisterDateButtons { get; set; } = 0;

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? RegisterDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? RegisterDateTo { get; set; }

        public int DismissalDateButtons { get; set; } = 0;

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? DismissalDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? DismissalDateTo { get; set; }

    }
   
    //View model для вывода в json
    public class EmployeeJsonViewModel
    {
        public EmployeeJsonViewModel(Employee employee, bool includeEmployee)
        {
            
            id = employee.Id;
            FamilyName = employee.FamilyName;
            Name = employee.Name;
            FathersName = employee.FathersName;
            BirthDay = employee.BirthDay.ToShortDateString();
            OldFamilyName = employee.OldFamilyName;
            if (employee.Profession != null)
            {
                Profession = new InternalName
                {
                    Name = employee.Profession.Name
                };
            }
            if (employee.BarberLevel != null)
            {
                BarberLevel = new InternalName
                {
                    Name = employee.BarberLevel.Name
                };
            }
            if (employee.Salon != null)
            {
                Salon = new InternalName
                {
                    Name = employee.Salon.Name
                };
            }
            if (employee.HireDate.HasValue)
                HireDate = employee.HireDate.Value.ToShortDateString();
            StaffNumber = employee.StaffNumber;
            PhoneNumber1 = employee.PhoneNumber1;
            PhoneNumber2 = employee.PhoneNumber2;
            if (employee.Company != null)
            {
                Company = new InternalName
                {
                    Name = employee.Company.Name
                };
            }
            if (employee.RegisterDate.HasValue)
                RegisterDate = employee.RegisterDate.Value.ToShortDateString();
            if (employee.DismissalDate.HasValue)
                DismissalDate = employee.DismissalDate.Value.ToShortDateString();
            if (includeEmployee & employee.DelayedUpdateEmployees != null && employee.DelayedUpdateEmployees.Count > 0)
                DelayedUpdateEmployee = new DelayedUpdateEmployeeJsonViewModel(employee.DelayedUpdateEmployees.First(), false);
            foreach (PriceListUnit unit in employee.PriceListUnits)
            {
                PriceListUnits = PriceListUnits + " - " + unit.Name;
            }

        }

        public int id { get; set; }
        
        public string FamilyName { get; set; }
        
        public string Name { get; set; }
        
        public string FathersName { get; set; }
       
        public string BirthDay { get; set; }
        
        public string OldFamilyName { get; set; }
        
        public InternalName Profession { get; set; }
        
        public InternalName BarberLevel { get; set; }
       
        public InternalName Salon { get; set; }
        
        public string HireDate { get; set; }
        
        public int? StaffNumber { get; set; }
        
        public string PhoneNumber1 { get; set; }
        
        public string PhoneNumber2 { get; set; }
        
        public InternalName Company { get; set; }
        
        public string RegisterDate { get; set; }
       
        public string DismissalDate { get; set; }

        public DelayedUpdateEmployeeJsonViewModel DelayedUpdateEmployee { get; set; }

        public string PriceListUnits { get; set; }

        public class InternalName
        {
            public string Name { get; set; }
        }

    }


    [DisplayClass(GroupName = "ProfessionList", Name = "ProfessionShortName", ResourceType = typeof(GlobalRes), ControllerName = "Professions")]
    public class Profession
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ProfessionName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "OrderNumber")]
        public int? OrderNumber { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Color")]
        public string Color { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class ProfessionJsonViewModel
    {
        public ProfessionJsonViewModel(Profession profession)
        {

            id = profession.Id;
            Name = profession.Name;
            OrderNumber = profession.OrderNumber;
            Color = profession.Color;
        }
        public int id { get; set; }
        public string Name { get; set; }
        public int? OrderNumber { get; set; }
        public string Color { get; set; }
    }

    [DisplayClass(GroupName = "BarberLevelList", Name = "BarberLevelShortName", ResourceType = typeof(GlobalRes), ControllerName = "BarberLevels")]
    public class BarberLevel
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "BarberLevelName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "OrderNumber")]
        public int? OrderNumber { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Color")]
        public string Color { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class BarberLevelJsonViewModel
    {
        public BarberLevelJsonViewModel(BarberLevel barberLevel)
        {

            id = barberLevel.Id;
            Name = barberLevel.Name;
            OrderNumber = barberLevel.OrderNumber;
            Color = barberLevel.Color;
        }
        public int id { get; set; }
        public string Name { get; set; }
        public int? OrderNumber { get; set; }
        public string Color { get; set; }
    }

    [DisplayClass(GroupName = "EmployeeList", Name = "EmployeeShortName", ResourceType = typeof(GlobalRes), ControllerName = "DelayedUpdateEmployees")]
    public class DelayedUpdateEmployee
    {
        public DelayedUpdateEmployee()
        { }
        public DelayedUpdateEmployee(Employee employee)
        {
            this.BarberLevelId = employee.BarberLevelId;
            this.BirthDay = employee.BirthDay;
            this.CompanyId = employee.CompanyId;
            this.DismissalDate = employee.DismissalDate;
            this.EmployeeId = employee.Id;
            this.FamilyName = employee.FamilyName;
            this.FathersName = employee.FathersName;
            this.HireDate = employee.HireDate;
            this.Name = employee.Name;
            this.OldFamilyName = employee.OldFamilyName;
            this.PhoneNumber1 = employee.PhoneNumber1;
            this.PhoneNumber2 = employee.PhoneNumber2;
            this.ProfessionId = employee.ProfessionId;
            this.RegisterDate = employee.RegisterDate;
            this.SalonId = employee.SalonId;
            this.StaffNumber = employee.StaffNumber;
            this.PriceListUnits = employee.PriceListUnits;
        }
        [Required]
        [ScaffoldColumn(false)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "FamilyName", Prompt = "PromptEnter")]
        [Index("IX_EmployeeUnique", 1, IsUnique = true)]
        public string FamilyName { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "Name", Prompt = "PromptEnter")]
        [Index("IX_EmployeeUnique", 2, IsUnique = true)]
        public string Name { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "FathersName", Prompt = "PromptEnter")]
        [Index("IX_EmployeeUnique", 3, IsUnique = true)]
        public string FathersName { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "BirthDay", Prompt = "PromptEnter")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Date")] //Колонка в базе содержит только дату
        [DataType(DataType.Date)]
        [Index("IX_EmployeeUnique", 4, IsUnique = true)]
        public DateTime BirthDay { get; set; }
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "OldFamilyName", Prompt = "PromptEnter", ShortName = "OldFamilyShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        public string OldFamilyName { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ProfessionShortName")]
        public int? ProfessionId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "ProfessionShortName")]
        public Profession Profession { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "BarberLevelShortName")]
        public int? BarberLevelId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "BarberLevelShortName")]
        public BarberLevel BarberLevel { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeSalon", ShortName = "SalonShortName")]
        public int? SalonId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeSalon", ShortName = "SalonShortName")]
        public Salon Salon { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "HireDate", Prompt = "PromptEnter", ShortName = "HireDateShortName")]
        [Column(TypeName = "Date")]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "StaffNumber", ShortName = "StaffNumberShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        public int? StaffNumber { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(([0-9]{3})\)[ ]([0-9]{3})[-]([0-9]{2})[-]([0-9]{2})$", ErrorMessageResourceName = "TelFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(15, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 15)]
        [Display(ResourceType = typeof(GlobalRes), Name = "PhoneNumber1Name", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX", ShortName = "PhoneNumber1ShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        public string PhoneNumber1 { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(([0-9]{3})\)[ ]([0-9]{3})[-]([0-9]{2})[-]([0-9]{2})$", ErrorMessageResourceName = "TelFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(15, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 15)]
        [Display(ResourceType = typeof(GlobalRes), Name = "PhoneNumber2Name", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX", ShortName = "PhoneNumber2ShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        public string PhoneNumber2 { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "RegisterCompany", ShortName = "RegisterCompanyShortName")]
        public int? CompanyId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "RegisterCompany", ShortName = "RegisterCompanyShortName")]
        public Company Company { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "RegisterDate", ShortName = "RegisterDateShortName")]
        [Column(TypeName = "Date")]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? RegisterDate { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "DismissalDate", ShortName = "DismissalDateShortName")]
        [Column(TypeName = "Date")]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? DismissalDate { get; set; }
        [HiddenInput(DisplayValue = false)]
        [AdditionalMetadata("ForeignKey", true)]
        public int? EmployeeId { get; set; }
        [ScaffoldColumn(false)]
        public Employee Employee { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "UpdateDate", ShortName = "UpdateDateShortName")]
        [Column(TypeName = "Date")]
        [DisplayFormat(NullDisplayText = "-", DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDate { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "PriceListUnitList")]
        public virtual ICollection<PriceListUnit> PriceListUnits { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }

    }

    public class DelayedUpdateEmployeeFilter
    {
        public DelayedUpdateEmployeeFilter() { }

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? BirthDayFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? BirthDayTo { get; set; }

        public int OldFamilyNameButtons { get; set; } = 0;

        public int[] ProfessionIdSelected { get; set; }


        public int[] BarberLevelIdSelected { get; set; }


        public int[] SalonIdSelected { get; set; }


        public int HireDateButtons { get; set; } = 0;

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? HireDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? HireDateTo { get; set; }

        public int StaffNumberButtons { get; set; } = 0;

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        public int? StaffNumberFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        public int? StaffNumberTo { get; set; }

        public int PhoneNumber1Buttons { get; set; } = 0;

        public int PhoneNumber2Buttons { get; set; }
        public int[] CompanyIdSelected { get; set; }

        public int RegisterDateButtons { get; set; } = 0;

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? RegisterDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? RegisterDateTo { get; set; }

        public int DismissalDateButtons { get; set; } = 0;

        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? DismissalDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? DismissalDateTo { get; set; }
        public int UpdateDateButtons { get; set; } = 0;
        [Display(ResourceType = typeof(GlobalRes), Name = "From")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDateFrom { get; set; }

        [Display(ResourceType = typeof(GlobalRes), Name = "To")]
        [DataType(DataType.Date)]
        public DateTime? UpdateDateTo { get; set; }

    }

    //View model для вывода в json
    public class DelayedUpdateEmployeeJsonViewModel
    {
        public DelayedUpdateEmployeeJsonViewModel(DelayedUpdateEmployee employee, bool includeEmployee)
        {
            id = employee.Id;
            FamilyName = employee.FamilyName;
            Name = employee.Name;
            FathersName = employee.FathersName;
            BirthDay = employee.BirthDay.ToShortDateString();
            OldFamilyName = employee.OldFamilyName;
            if (employee.Profession != null)
            {
                Profession = new InternalName
                {
                    Name = employee.Profession.Name
                };
            }
            if (employee.BarberLevel != null)
            {
                BarberLevel = new InternalName
                {
                    Name = employee.BarberLevel.Name
                };
            }
            if (employee.Salon != null)
            {
                Salon = new InternalName
                {
                    Name = employee.Salon.Name
                };
            }
            if (employee.HireDate.HasValue)
                HireDate = employee.HireDate.Value.ToShortDateString();
            StaffNumber = employee.StaffNumber;
            PhoneNumber1 = employee.PhoneNumber1;
            PhoneNumber2 = employee.PhoneNumber2;
            if (employee.Company != null)
            {
                Company = new InternalName() { Name = employee.Company.Name };
            }
            if (employee.RegisterDate.HasValue)
                RegisterDate = employee.RegisterDate.Value.ToShortDateString();
            if (employee.DismissalDate.HasValue)
                DismissalDate = employee.DismissalDate.Value.ToShortDateString();
            if (employee.UpdateDate.HasValue)
                UpdateDate = employee.UpdateDate.Value.ToShortDateString();
            if (includeEmployee && employee.Employee != null)
            Employee = new EmployeeJsonViewModel(employee.Employee, false);
            foreach (PriceListUnit unit in employee.PriceListUnits)
            {
                PriceListUnits = PriceListUnits + " - " + unit.Name;
            }

        }
        
        public int id { get; set; }

        public string FamilyName { get; set; }

        public string Name { get; set; }

        public string FathersName { get; set; }

        public string BirthDay { get; set; }

        public string OldFamilyName { get; set; }

        public InternalName Profession { get; set; }

        public InternalName BarberLevel { get; set; }

        public InternalName Salon { get; set; }

        public string HireDate { get; set; }

        public int? StaffNumber { get; set; }

        public string PhoneNumber1 { get; set; }

        public string PhoneNumber2 { get; set; }

        public InternalName Company { get; set; }

        public string RegisterDate { get; set; }

        public string DismissalDate { get; set; }

        public string UpdateDate { get; set; }

        public EmployeeJsonViewModel Employee { get; set; }
        public string PriceListUnits { get; set; }
        public class InternalName
        {
            public string Name { get; set; }
        }

    }
}