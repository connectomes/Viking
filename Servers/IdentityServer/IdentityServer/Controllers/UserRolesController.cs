﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Data;
using IdentityServer.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityServer.Models.AccountViewModels;
using IdentityServer.Services;
using IdentityServer.Models.UserViewModels;

namespace IdentityServer.Controllers
{
 
    [Route("[controller]/[action]")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserRolesController(ApplicationDbContext context)
        {
            _context = context;
        }
         
        // GET: UserRoles 
        public async Task<IActionResult> Index()
        {
            List<ApplicationRole> AvailableRoles = _context.ApplicationRole.ToList();
            List<UserRolesViewModel> UserRolesModels = _context.ApplicationUser.Select(
                user => new UserRolesViewModel
                {
                    Username = user.UserName,
                    Roles = _context.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList(),
                    AvailableRoles = AvailableRoles,
                }).ToList();

            return View(UserRolesModels);
            /*
            return View(await _context.ApplicationUser.Select(
                user => new UserRolesViewModel
                {
                    Username = user.UserName,
                    Roles = _context.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList(),
                    AvailableRoles = AvailableRoles,
                })..ToListAsync());
                */
        }

        // GET: UserRoles/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        /*
        // GET: UserRoles/Create
        public ActionResult Create()
        {
            return View();
        }
        */
        /*
        // POST: UserRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        */
        /*
        [HttpPost]
        // GET: UserRoles/Edit/5
        public ActionResult Edit(string id)
        {
            return View();
        }*/

        private bool IsChecked(Microsoft.Extensions.Primitives.StringValues val)
        {
            return val[0] != "false";
        }

        // POST: UserRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Access Manager")]
        public ActionResult Edit(UserRolesViewModel id, IFormCollection collection)
        {
            try
            {
                var User = _context.ApplicationUser.FirstOrDefault(u => u.UserName == id.Username);
                List<ApplicationRole> AvailableRoles = _context.ApplicationRole.ToList();
                // TODO: Add update logic here
                var listUserRoles = _context.UserRoles.Where(ur => ur.UserId == User.Id).ToList();
                
                foreach(var userRole in AvailableRoles)
                {
                    var form = collection[userRole.Id];
                    bool check = IsChecked(form);
                    if(check && !listUserRoles.Any(ur => ur.RoleId == userRole.Id))
                    {
                        _context.UserRoles.Add(new IdentityUserRole<string>() { UserId = User.Id, RoleId = userRole.Id });
                    }
                    else if(!check && listUserRoles.Any(ur => ur.RoleId == userRole.Id))
                    {
                        var urToRemove = listUserRoles.First(ur => ur.RoleId == userRole.Id && ur.UserId == User.Id);
                        _context.UserRoles.Remove(urToRemove);
                    }
                }

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        /*
        [HttpPost]
        public JsonResult UpdatePublicPostStatus(string id, bool isPublicPost)
        { 
            var result = _context.UserRoles.FirstOrDefault(user => user.UserId == id);
           
            return Json(null);
        }
        */
        /*
        // GET: UserRoles/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserRoles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
    }
}