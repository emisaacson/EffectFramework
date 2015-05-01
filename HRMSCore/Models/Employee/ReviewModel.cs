using HRMS.Modules.DBModel;
using HRMS.Modules.DBModel.Local;
using Local.Classes.Attributes;
using Local.Classes.Helpers;
using Local.Classes.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
/*
namespace Local.Models.Review
{
    using System;
	[DisplayName("Performance Review")]
	[MasterTable("ReviewMasters", typeof(ReviewMaster))]
	[DataContract]
	public class ReviewModel : Model
	{
		[MasterProperty]
		[DisplayName("Review ID")]
		[ReadOnly(true)]
		[DataMember]
		public int ReviewId { get; set; }

		[MasterProperty]
		[DisplayName("Employee ID")]
		[ReadOnly(true)]
		[DataMember]
		public int EmpMasterId { get; set; }

		[MasterProperty]
		[DisplayName("Review Year")]
		[ReadOnly(true)]
		[DataMember]
		public int ReviewYear { get; set; }

		[MasterProperty]
		[DisplayName("Is Mid Year?")]
		[ReadOnly(true)]
		[DataMember]
		public bool IsMidYear { get; set; }

		[MasterProperty]
		[DisplayName("Status")]
		[ReadOnly(true)]
		[DataMember]
		public string Status { get; set; }

		[MasterProperty]
		[DisplayName("Rating")]
		[ReadOnly(true)]
		[DataMember]
		public string EmpRating { get; set; }

		[MasterProperty]
		[DisplayName("Potential")]
		[ReadOnly(true)]
		[DataMember]
		public string EmpPotential { get; set; }
	}
}
*/