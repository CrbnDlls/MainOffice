using MainOffice.Annotations;
using MainOffice.App_LocalResources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace MainOffice.Models
{
    [DisplayClass(GroupName = "SalonList", Name = "SalonShortName", ResourceType = typeof(GlobalRes), ControllerName ="Salons")]
    public class Salon
    {
        [Required]
        [ScaffoldColumn(false)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 5)]
        [Display(ResourceType = typeof(GlobalRes),Name = "SalonName", ShortName = "SalonShortName", GroupName = "SalonGroupName", Prompt = "EnterName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(100, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 5)]
        [Display(ResourceType = typeof(GlobalRes), Name = "AddressName", GroupName = "AddressGroupName", Prompt = "EnterName")]
        [DisplayFormat(NullDisplayText = "-")]
        public string Address { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(([0-9]{3})\)[ ]([0-9]{3})[-]([0-9]{2})[-]([0-9]{2})$", ErrorMessageResourceName = "TelFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(15, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 15)]
        [Display(ResourceType = typeof(GlobalRes), Name = "PhoneNumber1Name", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_Phone1Unique", IsUnique = true)]
        public string PhoneNumber1 { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(([0-9]{3})\)[ ]([0-9]{3})[-]([0-9]{2})[-]([0-9]{2})$", ErrorMessageResourceName = "TelFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(15, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 15)]
        [Display(ResourceType = typeof(GlobalRes), Name = "PhoneNumber2Name", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_Phone2Unique", IsUnique = true)]
        public string PhoneNumber2 { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonStateShortName")]
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        public int SalonStateId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonStateShortName")]
        public SalonState SalonState { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonTypeShortName")]
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        public int SalonTypeId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonTypeShortName")]
        public SalonType SalonType { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Longitude")]
        public double Longitude { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Latitude")]
        public double Latitude { get; set; }
        [RegularExpression(@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", ErrorMessageResourceName = "WrongIpFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "IPAdress")]
        public string IP { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
        public virtual List<SalonPrinter> SalonPrinters { get; set; }
    }

    public class SalonJsonViewModel
    {
        public SalonJsonViewModel(Salon salon)
        {

            id = salon.Id;
            Name = salon.Name;
            Address = salon.Address;
            PhoneNumber1 = salon.PhoneNumber1;
            PhoneNumber2 = salon.PhoneNumber2;
            if (salon.SalonState != null)
            {
                SalonState = new InternalName();
                SalonState.Name = salon.SalonState.Name;
            }
            if (salon.SalonType != null)
            {
                SalonType = new InternalName();
                SalonType.Name = salon.SalonType.Name;
            }
            Longitude = salon.Longitude;
            Latitude = salon.Latitude;
            IP = salon.IP;
        }
        public int id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public InternalName SalonState { get; set; }
        public InternalName SalonType { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string IP { get; set; }
        public class InternalName
        {
            public string Name { get; set; }
        }
    }

    [DisplayClass(GroupName = "SalonStateList", Name = "SalonStateShortName", ResourceType = typeof(GlobalRes), ControllerName ="SalonStates")]
    public class SalonState
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 5)]
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonStateName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }


    }
    public class SalonStateJsonViewModel
    {
        public SalonStateJsonViewModel(SalonState salonState)
        {

            id = salonState.Id;
            Name = salonState.Name;
        }
        public int id { get; set; }
        public string Name { get; set; }
    }

    [DisplayClass(GroupName = "SalonTypeList", Name = "SalonTypeShortName", ResourceType = typeof(GlobalRes), ControllerName ="SalonTypes")]
    public class SalonType
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 5)]
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonTypeName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class SalonTypeJsonViewModel
    {
        public SalonTypeJsonViewModel(SalonType salonType)
        {

            id = salonType.Id;
            Name = salonType.Name;
        }
        public int id { get; set; }
        public string Name { get; set; }
    }


    [DisplayClass(GroupName = "SalonPrinterList", Name = "SalonPrinterShortName", ResourceType = typeof(GlobalRes), ControllerName = "SalonPrinters")]
    public class SalonPrinter
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 5)]
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonPrinterName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_PrinterUnique",1, IsUnique = true)]
        public string Name { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonPrinterSysName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_PrinterSysNameUnique", 1, IsUnique = true)]
        public string SystemPrinterName { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonShortName")]
        [Index("IX_PrinterUnique", 2, IsUnique = true)]
        [Index("IX_PrinterSysNameUnique", 2, IsUnique = true)]
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        public int SalonId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "SalonShortName")]
        public Salon Salon { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }


    }
    public class SalonPrinterJsonViewModel
    {
        public SalonPrinterJsonViewModel(SalonPrinter salonPrinter)
        {

            id = salonPrinter.Id;
            Name = salonPrinter.Name;
            SystemPrinterName = salonPrinter.SystemPrinterName;
            Salon = new InternalName
            {
                Name = salonPrinter.Salon.Name
            };
        }
        public int id { get; set; }
        public string Name { get; set; }
        public string SystemPrinterName { get; set; }
        public InternalName Salon { get; set; }
        public class InternalName
        {
            public string Name { get; set; }
        }
    }
}