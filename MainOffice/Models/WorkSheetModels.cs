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
    [DisplayClass(GroupName = "AttendanceList", Name = "AttendanceShortName", ResourceType = typeof(GlobalRes), ControllerName = "Attendances")]
    public class Attendance
    {
        [Required]
        [ScaffoldColumn(false)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "AttendanceDate", Prompt = "PromptEnter")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Date")] //Колонка в базе содержит только дату
        [DataType(DataType.Date)]
        [Index("IX_AttendanceUnique", 1, IsUnique = true)]
        public DateTime Date { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "AttendanceDate", Prompt = "PromptEnter")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Time(0)")] //Колонка в базе содержит только время
        [DataType(DataType.Time)]
        public DateTime Start { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Display(ResourceType = typeof(GlobalRes), Name = "AttendanceDate", Prompt = "PromptEnter")]
        [Column(TypeName = "Time(0)")] //Колонка в базе содержит только время
        [DataType(DataType.Time)]
        public DateTime End { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "AttendanceSalon", ShortName = "SalonShortName")]
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Index("IX_AttendanceUnique", 2, IsUnique = true)]
        public int SalonId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "AttendanceSalon", ShortName = "SalonShortName")]
        public Salon Salon { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [Index("IX_AttendanceUnique", 3, IsUnique = true)]
        public int EmployeeId { get; set; }
        [Display(ResourceType = typeof(GlobalRes), Name = "EmployeeShortName")]
        public Employee Employee { get; set; }
    }
}