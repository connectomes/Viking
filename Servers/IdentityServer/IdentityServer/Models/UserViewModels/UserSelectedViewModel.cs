﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.Models.UserViewModels
{
    public class UserSelectedViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        
        public bool Selected { get; set; }
    }

    /// <summary>
    /// Update the organization assignments to match the UserSelectedViewModel
    /// </summary>
    public static class UserSelectedViewModelExtensions
    {
        public static void UpdateUserOrganizations(this Organization organization, IEnumerable<UserSelectedViewModel> Users)
        {
            foreach (UserSelectedViewModel user in Users)
            {
                var ExistingMapping = organization.OrganizationAssignments.FirstOrDefault(u => u.UserId == user.Id);

                if (user.Selected)
                {
                    if (ExistingMapping == null)
                    {
                        //Create the mapping
                        OrganizationAssignment oa = new OrganizationAssignment() { OrganizationId = organization.Id, UserId = user.Id };
                        organization.OrganizationAssignments.Add(oa);
                    }
                }
                else
                {
                    if (ExistingMapping != null)
                    {
                        //Remove the mapping
                        organization.OrganizationAssignments.Remove(ExistingMapping);
                    }
                }
            }
        }
    }
}
