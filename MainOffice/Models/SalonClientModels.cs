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
    [DisplayClass(GroupName = "ClientList", Name = "ClientShortName", ResourceType = typeof(GlobalRes), ControllerName = "Clients")]
    public class Client
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "Name", Prompt = "PromptEnter")]
        public string Name { get; set; }
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "FamilyName", Prompt = "PromptEnter")]
        public string FamilyName { get; set; }
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "FathersName", Prompt = "PromptEnter")]
        public string FathersName { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "BirthDay", Prompt = "PromptEnter")]
        [Column(TypeName = "Date")] //Колонка в базе содержит только дату
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(([0-9]{3})\)[ ]([0-9]{3})[-]([0-9]{2})[-]([0-9]{2})$", ErrorMessageResourceName = "TelFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(15, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 15)]
        [Display(ResourceType = typeof(GlobalRes), Name = "MainPhone", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX", ShortName = "MainPhone")]
        [DisplayFormat(NullDisplayText = "-")]
        [Required]
        [Index("IX_ClientUnique",1,IsUnique = true)]
        public string PhoneNumber { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "AdditionalPhone", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX", ShortName = "AdditionalPhone")]
        public virtual IList<ClientPhone> AdditionalPhones { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "Email", ShortName = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

     public class ClientJsonViewModel
    {
        public ClientJsonViewModel(Client client)
        {

            id = client.Id;
            FamilyName = client.FamilyName;
            Name = client.Name;
            FathersName = client.FathersName;
            if (client.BirthDay.HasValue)
                BirthDay = client.BirthDay.Value.ToShortDateString();
            Email = client.Email;
            PhoneNumber = client.PhoneNumber;
            foreach (ClientPhone phone in client.AdditionalPhones)
            {
                AdditionalPhones = AdditionalPhones + " | " + phone.PhoneNumber;
            }

        }

        public int id { get; set; }
        public string FamilyName { get; set; }
        public string Name { get; set; }
        public string FathersName { get; set; }
        public string BirthDay { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AdditionalPhones { get; set; }
    }
    
    public class ClientPhone
    {
        public int Id { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(([0-9]{3})\)[ ]([0-9]{3})[-]([0-9]{2})[-]([0-9]{2})$", ErrorMessageResourceName = "TelFormat", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(15, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 15)]
        [Display(ResourceType = typeof(GlobalRes), Name = "AdditionalPhone", GroupName = "PhoneNumberGroupName", Prompt = "(0XX) XXX-XX-XX", ShortName = "AdditionalPhone")]
        [DisplayFormat(NullDisplayText = "-")]
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        public string PhoneNumber { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }
}