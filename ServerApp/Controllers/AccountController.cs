﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signManager;

        public AccountController(UserManager<IdentityUser> userMgr,
                                 SignInManager<IdentityUser> signMgr)
        {
            userManager = userMgr;
            signManager = signMgr;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel creds, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if(await DoLogin(creds))
                {
                    return Redirect(returnUrl ?? "/");
                }else{
                    ModelState.AddModelError("", "Invalid username or password");
                }
                return View(creds);
            }
            return View(creds);
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string redirectUrl)
        {
            await signManager.SignOutAsync();
            return Redirect(redirectUrl ?? "/");

        }

        private async Task<bool> DoLogin(LoginViewModel creds)
        {
            IdentityUser user = await userManager.FindByNameAsync(creds.Name);

            if (user != null)
            {
                await signManager.SignOutAsync();
                Microsoft.AspNetCore.Identity.SignInResult result 
                    = await signManager.PasswordSignInAsync(user, creds.Password, false, false);

                return result.Succeeded;
            }
            return false;
        }

        [HttpPost("/api/account/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel creds)
        {
            if(ModelState.IsValid && await DoLogin(creds))
            {
                return Ok("true");
            }

            return BadRequest();
        }

        [HttpPost("/api/account/logout")]
        public async Task<IActionResult> Logout()
        {
            await signManager.SignOutAsync();
            return Ok();
        }
    }

    public class LoginViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

