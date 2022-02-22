using IdentityBasicDotNetCore.Models;
using IdentityBasicDotNetCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityBasicDotNetCore.Controllers
{
    public class IdentityController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityController(RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            var signupviewModel = new SignUpViewModel(){ Roles = "Member"};

            return View(signupviewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (!await _roleManager.RoleExistsAsync(model.Roles))
                {
                    var roleresult = await _roleManager.CreateAsync(new IdentityRole() { Name = model.Roles });
                    if (!roleresult.Succeeded)
                    {
                        ModelState.AddModelError("RoleManager", string.Join("", roleresult.Errors.Select(x => x.Description)));
                    }
                }
                

                if (await _userManager.FindByEmailAsync(model.Email) == null)
                {
                    var user = new IdentityUser()
                    {
                        UserName = model.Email,
                        Email = model.Email
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        user = await _userManager.FindByIdAsync(user.Id);
                        var claims = new Claim("Department", model.Department);
                        await _userManager.AddClaimAsync(user, claims);
                        await _userManager.AddToRoleAsync(user, model.Roles);
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationlink = Url.ActionLink("ConfirmMail", "Identity", new { userId = user.Id, @tokenId = token });

                        await _emailSender.SendEmailAsync("saransaravana@gmail.com", model.Email, "Confirm Email", confirmationlink);

                        return RedirectToAction("SignIn");
                    }
                    ModelState.AddModelError("SignUp",string.Join("", result.Errors.Select(x => x.Description)));
                }
                return View();
            }
            else
            {
                return View(model);
            }

           
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmMail(string userId,string tokenId)
        {
            var user = await  _userManager.FindByIdAsync(userId);

            var result = await _userManager.ConfirmEmailAsync(user, tokenId);

            if (result.Succeeded)
            {
                return RedirectToAction("SignIn");
            }

            return new OkResult();
        }


        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            var signinviewModel = new SignInViewModel();
            return View(signinviewModel);
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                SignInResult twofasetup;
                bool isTwoFactor = false;
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.RequiresTwoFactor)
                {
                    twofasetup = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.Code, false, false);
                    if (twofasetup.Succeeded) isTwoFactor = true;
                }
 
                if (result.Succeeded || isTwoFactor)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    var claims = await _userManager.GetClaimsAsync(user);

                    if (!claims.ToList().Any(x => x.Type == "Department"))
                    {
                        ModelState.AddModelError("Claim", "User not have in tech department");
                        return View(model);
                        
                    }
                    if(await _userManager.IsInRoleAsync(user, "Member"))
                    {
                        return RedirectToAction("Member", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Admin", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("SignIn", "Login Failed");
                }
                return View(model);
            }
            else
            {
                return View(model);
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MFASetup()
        {
            var model = new MFAViewModel();
            var user = await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var token =await  _userManager.GetAuthenticatorKeyAsync(user);
            if(token != null)
            model.Token = token;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MFASetup(MFAViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.Code);
                if (result)
                {
                    await _userManager.SetTwoFactorEnabledAsync(user, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("MfaSetup", "Code is not valid");
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }


        [HttpGet]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    }
}
