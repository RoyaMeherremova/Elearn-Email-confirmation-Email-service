using Elearn.Models;
using Elearn.Services.Interfaces;
using Elearn.ViewModel.Account;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using NuGet.Protocol.Plugins;

namespace Elearn.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;  //UserManager-Identity tablarla islemek ucun -databazaynan(Create elemey user uzerinde isler gormek)
        private readonly SignInManager<AppUser> _signInManager; //SignInManager-sayta giris elemey ucun (sayta giris etmek log out olmaq)
        private readonly IEmailService _emailService;
        public AccountController(UserManager<AppUser> userManager, 
                                  SignInManager<AppUser> signInManager,
                                  IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //----------SAYTA REGISTER----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)  //bos gelen input olarsa viewa qayit dolduruqun datalarla
            {
                return View(model);
            }

            AppUser newUser = new()  //bizim databazadaki user modelmize assayn edirik Viwnodelmizden gelenlere
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                //Paswordumuz heslenecek deye onu ayrica gonderik
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);     //CreateAsync -user yaratmaq ucun (databazaya save edir)

            if (!result.Succeeded)  //eyer giris ugursuz olarsa(qoyduqumuz sertlere uyqun olmazsa)
            {
                foreach (var item in result.Errors)  //errorlar List seklindedi
                {
                    ModelState.AddModelError(string.Empty, item.Description);   //string.Empty-her hansisa filt altinda yazilmasin deye,item.Description-errorlari icliyi
                }
                return View(model);
            }


            string token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);  //tesdiq ucun token aracidi(bag yaradir gedir emaile ve qayidir cavab tokenle gelir)-heslanmis data

            string link = Url.Action(nameof(ConfirmEmail), "Account", new {userId = newUser.Id,token},Request.Scheme,Request.Host.ToString()); //Url duzeldirik (klikleyende emailedeki tesdiqde bu urle gelsin deye)
                                                                                                                                               //new {userId = newUser.Id,token} --methoda argument gonderik
                                                                                                                                               //Request.Scheme-linkde http yaradir
                                                                                                                                               //Request.Host.ToString()-domainvin adni yazdirir

            #region Without SERVICE
            //  // create email message
            // var email = new MimeMessage();
            // email.From.Add(MailboxAddress.Parse("royaam@code.edu.az"));  //hansi mailden gedecek tesdiq ucun mesaj hamiya
            // email.To.Add(MailboxAddress.Parse(newUser.Email));    //gedecek register olan userin mailine
            // email.Subject = "Register confirmation"; //Emailini basliqi ne olacaq
            // email.Body = new TextPart(TextFormat.Html) { Text = $"<a href=`{link}`>Go to fiorello</a>" };  //mailin icliyi ne olacaq

            // // send email
            // using var smtp = new SmtpClient();
            // smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);  //gmailden gelecek tesdiq
            // smtp.Authenticate("royaam@code.edu.az", "ncymuvekwaomaubc");
            ////royaam@code.edu.az-hansi mailden gedecek tesdiq uucn mesaj   //"ncymuvekwaomaubc"-pasword mailmizin icindeki
            // smtp.Send(email);
            // smtp.Disconnect(true);
            #endregion
            

            string subject = "Register confirmation";
            string html = string.Empty;
        
            using (StreamReader reader = new StreamReader("wwwroot/templates/verify.html"))  //fayli oxu yaz beraberlesdir html-vareybla
            {
                html = reader.ReadToEnd();
            }

            html = html.Replace("{{link}}", link);  //htmldeki atributun deyerin deyisdire bilerik bucur-Replace
            html = html.Replace("{{headerText}}","Hello P135");
            _emailService.Send(newUser.Email, subject, html); //send mesaj with SERVICE

            return RedirectToAction(nameof(VerifyEmail));
        }




        //-------METHOD FOR CONFIRM EMAIL(ConfirmEmail propertisini true eden)
        public async Task<IActionResult> ConfirmEmail(string userId,string token)  //user id gelirki o Id-de useri tapib ConfirmEmail propertisisni true edek
        {
            if (userId == null || token == null) return BadRequest(); //bize gelen token veya id silinerse

            AppUser user = await _userManager.FindByIdAsync(userId);  //bu id-de useri tap

            if (user == null) return NotFound();   

            await _userManager.ConfirmEmailAsync(user, token);   //get hemin userin ConfirmEmailini true ele
            await _signInManager.SignInAsync(user, false); //login olmamis daxil olsun sayta
            return RedirectToAction("Index","Home");
        }





        //---------View for alert CHEKC-EMAIL------
        public IActionResult VerifyEmail()
        {
            return View();
        }








        //----------SAYTA LOGIN OLUB DAXIL OLMAQ----------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            AppUser user = await _userManager.FindByEmailAsync(model.EmaiOrUsername);  //tapaq bize gelen email varmi databazada(saytdan ya usernam gelir ya email)

            if (user is null)  //eyer o adda email yoxdusa
            {
                user = await _userManager.FindByNameAsync(model.EmaiOrUsername); //tapaq bize gelen adda varmi databazada(saytdan ya usernam gelir ya email)
            }
            if (user is null) //o adda hem email hem username yoxdursa
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");

                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded) //eyer pasword sehvdirse
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");

                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }



        //----------SAYTDAN CIXIS----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  //kim girmis vezyetdedise cixsin ordan(sessiondan tokeni silir)
            return RedirectToAction("Index", "Home");
        }


    }
}
