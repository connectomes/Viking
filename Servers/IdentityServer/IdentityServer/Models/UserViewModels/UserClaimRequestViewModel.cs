﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Models.UserViewModels
{
    public class UserClaimRequestViewModel
    {
        [Required]
        public string UserId { get; set; }

        public List<OrganizationSelectedViewModel> AvailableOrganizations { get; set; }

        public List<RoleSelectedViewModel> AvailableRoles { get; set; }
    }
}
