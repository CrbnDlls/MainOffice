using MainOffice.Annotations;
using MainOffice.App_LocalResources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace MainOffice.Models
{
    [DisplayClass(GroupName = "CompanyList", Name = "CompanyShortName", ResourceType = typeof(GlobalRes), ControllerName = "Companies")]
    public class Company
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CompanyName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        //[Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(100, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 5)]
        [Display(ResourceType = typeof(GlobalRes), Name = "CompanyAddress")]
        [DisplayFormat(NullDisplayText = "-")]
        public string Address { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeList")]
        public ICollection<Employee> Employees { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }

    }

    public class CompanyJsonViewModel
    {
        public CompanyJsonViewModel(Company company)
        {

            id = company.Id;
            Name = company.Name;
            Address = company.Address;
        }
        public int id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}